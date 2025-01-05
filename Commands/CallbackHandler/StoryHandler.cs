using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
public class StoryHandler
{
    async public Task yes_ACT_1_CHOSE_1(Wrapper wrapper)
    {
        var callbackQuery = wrapper.CallbackQuery;
        if (userCharacterData[wrapper.UserId].StoryProgression > 0) 
        {
            await UntilNextTime(wrapper);
            return;
        }
        Message msg1 = await BotServices.Instance.Bot.SendPhoto(chatId: wrapper.ChatId, photo: m_ACT_1_PIC_1, messageThreadId: m_creatCharTopic);
        Message msg2 = await BotServices.Instance.Bot.SendMessage(wrapper.ChatId, m_ACT_1_TEXT_1, messageThreadId: m_creatCharTopic);
        Message msg3 = await BotServices.Instance.Bot.SendMessage(wrapper.ChatId,
            "УЗНАТЬ О БОГИНЕ БОЛЬШЕ?",
            replyMarkup: m_inlineKeyboardAct1Yes2,
            messageThreadId: m_creatCharTopic
            );
        _= MassageDeleter(msg1, 30);
        _= MassageDeleter(msg2, 30);
        _= MassageDeleter(msg3, 30);
        if (wrapper.CallbackQueryId == null) throw new ArgumentNullException(nameof(wrapper.CallbackQueryId));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);

    }
    async public Task no_ACT_1_CHOSE_1(Wrapper wrapper)
    {
        Message msg1 = await BotServices.Instance.Bot.SendMessage(wrapper.ChatId, "Хорошо, когда будете готовы, дайте знать!", messageThreadId: m_creatCharTopic);
        _ = MassageDeleter(msg1, 30);
        if (wrapper.CallbackQueryId == null) throw new ArgumentNullException(nameof(wrapper.CallbackQueryId));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }
    async public Task yes_ACT_1_CHOSE_2(Wrapper wrapper)
    {
        Message msg1 = await BotServices.Instance.Bot.SendPhoto(chatId: wrapper.ChatId, photo: m_ACT_1_PIC_2, messageThreadId: m_creatCharTopic);
        Message msg2 = await BotServices.Instance.Bot.SendMessage(wrapper.ChatId, m_ACT_1_TEXT_2, messageThreadId: m_creatCharTopic);
        userCharacterData[wrapper.UserId].StoryProgression += 1;
        _ = MassageDeleter(msg1, 30);
        _ = MassageDeleter(msg2, 30);
        if (wrapper.CallbackQueryId == null) throw new ArgumentNullException(nameof(wrapper.CallbackQueryId));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }
    async public Task UntilNextTime(Wrapper wrapper)
    {
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: $"Продолжение следует...",
                messageThreadId: wrapper.MessageThreadId
            );
        _ = MassageDeleter(msgNew, 30);
        if (wrapper.CallbackQueryId == null) throw new ArgumentNullException(nameof(wrapper.CallbackQueryId));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }
}