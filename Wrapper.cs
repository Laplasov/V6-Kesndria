using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using static GlobalData;
public class Wrapper
{
    public long UserId { get; set; }
    public long ChatId { get; set; }
    public int? MessageThreadId { get; set; }
    public int MessageId { get; set; }
    public string Text { get; set; }
    public UpdateType Type { get; set; }
    public CallbackQuery? CallbackQuery { get; set; }
    public string? CallbackQueryId { get; set; }
    public Message OriginalMessage { get; set; }
    public Update? OriginalUpdate { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public Wrapper(Message message, UpdateType type)
    {
        if (message == null) throw 
                new ArgumentNullException(nameof(message), "Message cannot be null.");
        if (message.From == null) throw 
                new ArgumentNullException(nameof(message.From), "Message.From cannot be null.");
        if (message.Chat == null) throw 
                new ArgumentNullException(nameof(message.Chat), "Message.Chat cannot be null.");
        if (message.Text == null) throw
                new ArgumentNullException(nameof(message.Text), "message.Text cannot be null.");

        UserId = message.From.Id;
        ChatId = message.Chat.Id;
        MessageThreadId = message.MessageThreadId;
        MessageId = message.MessageId;
        Text = message.Text;
        Type = type;
        OriginalMessage = message;
        FirstName = message.From.FirstName;
        LastName = message.From.LastName;

    }


    public Wrapper(Update update)
    {
        if (update == null) throw 
                new ArgumentNullException(nameof(update), "Update cannot be null.");
        if (update.CallbackQuery == null) return;
            //throw new ArgumentNullException(nameof(update.CallbackQuery), "CallbackQuery cannot be null.");
        if (update.CallbackQuery!.From == null) throw 
                new ArgumentNullException(nameof(update.CallbackQuery.From), "CallbackQuery.From cannot be null.");
        if (update.CallbackQuery.Message == null) throw 
                new ArgumentNullException(nameof(update.CallbackQuery.Message), "CallbackQuery.Message cannot be null.");
        if (update.CallbackQuery.Message.Chat == null) throw 
                new ArgumentNullException(nameof(update.CallbackQuery.Message.Chat), "CallbackQuery.Message.Chat cannot be null.");
        if (update.CallbackQuery.Data == null) throw
                new ArgumentNullException(nameof(update.CallbackQuery.Message.Chat), "CallbackQuery.Message.Chat cannot be null.");

        CallbackQuery = update.CallbackQuery;
        UserId = CallbackQuery.From.Id;
        ChatId = CallbackQuery.Message.Chat.Id;
        MessageThreadId = CallbackQuery?.Message?.MessageThreadId;
        MessageId = update.CallbackQuery.Message.MessageId;
        Text = update.CallbackQuery.Data;
        Type = update.Type;
        CallbackQueryId = update.CallbackQuery.Id;
        OriginalMessage = update.CallbackQuery.Message;
        OriginalUpdate = update;
        FirstName = update.CallbackQuery.From.FirstName;
        LastName = update.CallbackQuery.From.LastName;
    }


    public bool IsValid()
    {
        if (Type == UpdateType.CallbackQuery)
        {
            return userMessgaeMap.TryGetValue(MessageId, out long originalUserId) && UserId == originalUserId;
        }
        return true; 
    }
}