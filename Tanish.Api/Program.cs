using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Telegram.Bot;
using Tanish.Api.Middleware;
using Tanish.Api.TelegramBot;
using Tanish.Application;
using Tanish.Application.Matching.Jobs;
using Tanish.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5342");
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    new TelegramBotClient(builder.Configuration["Telegram:BotToken"]!));
builder.Services.AddSingleton<IConversationStateStore, InMemoryConversationStateStore>();
builder.Services.AddScoped<TelegramUpdateHandler>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("telegram-webhook", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests - slow down.", ct);
    };
});

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Default"))));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<CleanupStaleMatchesJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<CleanupStaleMatchesJob>(
    "cleanup-stale-matches",
    job => job.RunAsync(CancellationToken.None),
    Cron.Hourly);

app.Run();