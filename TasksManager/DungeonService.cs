using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static GlobalData;
using static StringCollection;
public class DungeonService : IDungeonService
{
    private readonly CancellationToken _cancellationToken;

    public DungeonService(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public async Task DungeonSpawn(int delayBetween)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            _ = Task.Run(() => DangionUpdate("1-5", 1));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);
            dangionList.Clear();
            _ = Task.Run(() => DangionUpdate("6-10", 2));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);
            dangionList.Clear();
        }
    }
    public async Task DangionUpdate(string level, int imageNum)
    {
        try
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            InlineKeyboardButton.WithCallbackData("Подписаться на подземелье", $"@dangion_{level}")
            });

        string dangionEnterTextLocal = $"Подземелье для уровней {level}\n\n" +
           $"{dangionEnterText}";

        string? dangionImage = "";
        if (imageNum == 1)
            dangionImage = dangionEnterImageOne;
        if (imageNum == 2)
            dangionImage = dangionEnterImageSecound;

            var sentMessage = await BotServices.Instance.Bot.SendPhoto(
            chatId: m_mainChat,
            photo: dangionImage,
            caption: dangionEnterTextLocal,
            replyMarkup: inlineKeyboard
        );


        _ = MassageDeleter(sentMessage, 300);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DangionUpdate for level {level}: {ex.Message}\n");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

}