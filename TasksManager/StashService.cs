using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static StringCollection;

public class StashService : IStashService
{
    private readonly CancellationToken _cancellationToken;

    public StashService(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public async Task StashSpawn(int delayBetween, int delay)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            _ = Task.Run(() => StashUpdate(delay, 1, 5, "1 - 5", false));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);

            _ = Task.Run(() => StashUpdate(delay, 5, 10, "6 - 10", false));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);

            _ = Task.Run(() => StashUpdate(delay, 10, 15, "11 - 15", false));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);
        }
    }

    public async Task StashUpdate(int delay, int min, int max, string level, bool IsPrize)
    {
        Random random = new Random();

        int ranTime = random.Next(1, delay);

        if (!IsPrize)
            await Task.Delay(TimeSpan.FromMinutes(ranTime), _cancellationToken);

        int Exp = random.Next(min, max);
        int Gold = random.Next(min, max);

        int maxLevel = 0;

        if (level == "1 - 5")
            maxLevel = 5;
        if (level == "6 - 10")
            maxLevel = 10;
        if (level == "11 - 15")
            maxLevel = 15;

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
        InlineKeyboardButton.WithCallbackData("Забрать", $"@stash_{Exp}_{Gold}_{maxLevel}_{IsPrize}")
        });

        string stashCaption;
        Message? lastMassage;

        if (!IsPrize)
        {
            stashCaption = $"{StashText[random.Next(0, 4)]}\n Для уровней: {level}";

            lastMassage = await BotServices.Instance.Bot.SendMessage(
               chatId: m_mainChat,
               text: stashCaption,
               replyMarkup: inlineKeyboard
            );
        }
        else
        {
            stashCaption = "Перед Вами огромный сундук, Вы можете забрать награду.\nЗабрали награду:";

            lastMassage = await BotServices.Instance.Bot.SendPhoto(
               chatId: m_mainChat,
               photo: dangionChestImage,
               caption: stashCaption,
               replyMarkup: inlineKeyboard
            );
        }

        await Task.Delay((TimeSpan.FromMinutes(delay)), _cancellationToken);

        await BotServices.Instance.Bot.DeleteMessage(
            chatId: m_mainChat,
            messageId: lastMassage.MessageId
            );
    }
}