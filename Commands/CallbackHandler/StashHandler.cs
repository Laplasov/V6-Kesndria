using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
public class StashHandler
{
    private readonly Random _random;
    private readonly ItemDropCollection _itemDrop;

    public StashHandler()
    {
        _random = new Random();
        _itemDrop = new ItemDropCollection();
    }
    async public Task FindStash(Wrapper wrapper)
    {
        var callbackQuery = wrapper.CallbackQuery;
        string response = wrapper.Text;

        var parts = response.Split('_');

        var exp = int.Parse(parts[1]);
        var gold = int.Parse(parts[2]);
        var levelUp = int.Parse(parts[3]);

        bool IsPrize = false;
        if (parts[4].ToLower() == "true")
            IsPrize = true;
        else if (parts[4].ToLower() == "false")
            IsPrize = false;

        var levelDown = levelUp - 5;

        var userId = wrapper.UserId;

        if (userCharacterData.ContainsKey(userId) && !IsPrize)
        {
            var player = userCharacterData[userId];

            if (player.Level > levelUp)
            {
                Message msgNew = await BotServices.Instance.Bot.SendMessage(
                        wrapper.ChatId,
                        $"{player.Name}, это не достойно Вашего внимания."
                    );
                _ = MassageDeleter(msgNew, 30);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery!.Id);
                return;
            }

            if (!claimTracker.CanClaim(userId))
            {
                Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    wrapper.ChatId,
                    $"{player.Name}, Вы слишком устали что бы подбирать эти награды..."
                );
                _ = MassageDeleter(msgNew, 30);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery!.Id);
                return;
            }

            claimTracker.IncrementClaim(userId);
            int claimCount = claimTracker.GetClaimCount(userId);

            (exp, gold) = await player.GetLUCKAsync(new int[] { exp, gold }, luckEnemyLootImage, wrapper.ChatId, _random);
            await player.GetItemAsync(_itemDrop.CreateRandomItem(0, _random), wrapper.ChatId, _random);

            if (player.Level < MAX_LEVEL)
                player.EXP += exp;
            player.Gold += gold;

            string textCaption = "";
            if (callbackQuery?.Message?.Caption != null)
                textCaption = callbackQuery.Message.Caption;
            else if (callbackQuery?.Message?.Text != null)
                textCaption = callbackQuery.Message.Text;

            string newCaption = $"{textCaption}\n\n" +
                $"{player.Name} получил(a): \n" +
                $"{exp} Опыта\n" +
                $"{gold} Золота\n\n" +
                $"Остаток наград на сегодня: {claimTracker.ClaimCount - claimTracker.GetClaimCount(userId)}"
                ;

            _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQueryId: callbackQuery!.Id, text: RatASCII, cacheTime: 20, showAlert: true);
            await BotServices.Instance.Bot.EditMessageText(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                text: newCaption
                );

        }

        if (userCharacterData.ContainsKey(userId) && IsPrize)
        {
            var player = userCharacterData[userId];

            if (callbackQuery == null || callbackQuery.Message == null || callbackQuery.Message.Caption == null) throw
                    new ArgumentNullException(nameof(callbackQuery.Message.Caption));

            string originalCaption = callbackQuery.Message.Caption;
            if (!dangionList.Contains(userId))
            {
                Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    wrapper.ChatId,
                    "Вы не можете забрать эту награду, так как не участвовали в подземелье или уже получили награду."
                );
                await MassageDeleter(msgNew, 30);
                return;
            }

            if (dangionList.Contains(userId))
            {

                (exp, gold) = await player.GetLUCKAsync(new int[] { exp, gold }, luckEnemyLootImage, wrapper.ChatId, _random);
                await player.GetItemAsync(_itemDrop.CreateRandomItem(0, _random), wrapper.ChatId, _random);

                if (player.Level < MAX_LEVEL)
                    player.EXP += exp;
                player.Gold += gold;

                var newCaption = originalCaption + $"\n{player.Name} получил(а) {exp} опыта и {gold} золота.";
                dangionList.Remove(userId);

                InlineKeyboardMarkup? existingKeyboard = null;

                if (dangionList.Count > 0)
                {
                    existingKeyboard = callbackQuery?.Message?.ReplyMarkup;

                    await BotServices.Instance.Bot.EditMessageCaption(
                        chatId: wrapper.ChatId,
                        messageId: wrapper.MessageId,
                        caption: newCaption,
                        replyMarkup: existingKeyboard
                    );
                }
                else
                {
                    var newMedia = new InputMediaPhoto
                    {
                        Media = dangionChestOpenImage,
                        Caption = newCaption
                    };

                    await BotServices.Instance.Bot.EditMessageMedia(
                        chatId: wrapper.ChatId,
                        messageId: wrapper.MessageId,
                        media: newMedia,
                        replyMarkup: existingKeyboard
                    );
                }
            }
        }
        if (callbackQuery == null) throw new ArgumentNullException(nameof(callbackQuery));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);
    }
}

