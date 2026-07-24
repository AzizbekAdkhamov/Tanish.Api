using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Tanish.Api.Middleware;
using Tanish.Api.TelegramBot;
using Tanish.Application;
using Tanish.Infrastructure.DependencyInjection;
using Telegram.Bot;
using Hangfire;
using Tanish.Application.Matching.Jobs;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    new TelegramBotClient(builder.Configuration["Telegram:BotToken"]!));

builder.Services.AddSingleton<IConversationStateStore, InMemoryConversationStateStore>();
builder.Services.AddScoped<TelegramUpdateHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("telegram-webhook", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.QueueLimit = 0; // reject immediately over the limit, don't queue
    });

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests — slow down.", ct);
    };
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Default"))));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<CleanupStaleMatchesJob>();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire"); // visit locally to see job history

RecurringJob.AddOrUpdate<CleanupStaleMatchesJob>(
    "cleanup-stale-matches",
    job => job.RunAsync(CancellationToken.None),
    Cron.Hourly);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
