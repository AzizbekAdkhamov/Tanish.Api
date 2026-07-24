namespace Tanish.Api.TelegramBot;

public interface IConversationStateStore
{
    ConversationState Get(long telegramId);
    void Reset(long telegramId);
}

public class InMemoryConversationStateStore : IConversationStateStore
{
    private readonly Dictionary<long, ConversationState> _states = new();

    public ConversationState Get(long telegramId)
    {
        if (!_states.TryGetValue(telegramId, out var state))
        {
            state = new ConversationState();
            _states[telegramId] = state;
        }
        return state;
    }

    public void Reset(long telegramId) => _states[telegramId] = new ConversationState();
}