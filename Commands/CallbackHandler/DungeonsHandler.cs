
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
using System;
public class DungeonsHandler
{
    public async Task DungeonsManage(Wrapper wrapper)
    {
        var callbackQuery = wrapper.CallbackQuery;
        if (callbackQuery == null || callbackQuery.Message == null || callbackQuery.Data == null || callbackQuery.Message.Caption == null) throw 
                new ArgumentNullException(nameof(callbackQuery));

        long userId = callbackQuery.From.Id;
        var character = userCharacterData[userId];
        int? messageThreadId = callbackQuery.Message.MessageThreadId;
        int messageId = callbackQuery.Message.MessageId;

        _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);
        string response = callbackQuery.Data;
        var parts = response.Split('_');
        bool IsEntering = false;
        string originalCaption = callbackQuery.Message.Caption;

        if (parts[1] == "1-5" && character.Level <= 5)
            IsEntering = true;
        if (parts[1] == "6-10" && character.Level <= 10 && character.Level > 5)
            IsEntering = true;
        if (parts[1] == "11-15" && character.Level > 10)
            IsEntering = true;



        if (!dangionList.Contains(userId) && IsEntering && userCharacterData.ContainsKey(userId) && dangionList.Count < 3)
        {
            dangionList.Add(userId);
        }
        else
        {
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name} \nВы уже подписались на подземелье или Ваш уровень не соответствует требованиям.",
                    messageThreadId: messageThreadId
                );
            _ = MassageDeleter(msgNew, 30);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);
            return;
        }

        if (dangionList.Count >= 3)
        {

            if (parts[1] == "1-5")
                currentDangionEnemyIds = new int[] { 1626, 1627, 1628 };
            if (parts[1] == "6-10")
                currentDangionEnemyIds = new int[] { 1654, 1655, 1662 };
            if (parts[1] == "11-15")
                currentDangionEnemy = dangionEnemyNamesPartOne;

            dangionEnemyIndex = 0;
            int enemyId = currentDangionEnemyIds[dangionEnemyIndex];
            Enemy? enemy = dangionEnemyList.FirstOrDefault(e => e.EnemyId == enemyId);
            if (enemy == null) throw new ArgumentNullException(nameof(enemy));

            string playerNames = string.Join("\n", dangionList
                .Where(id => userCharacterData.ContainsKey(id))
                .Select(id => userCharacterData[id].Name));

            await BotServices.Instance.Bot.EditMessageCaption(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: messageId,
                    caption: originalCaption + $"\n\nГруппа была собрана: \n{playerNames}"
                );

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"Группа была собрана: \n{playerNames}",
                    messageThreadId: messageThreadId
                );

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            InlineKeyboardButton.WithCallbackData("Attack", $"@attackDangion_{0}_{enemy.EnemyId}_{DateTime.Now.Ticks}")
            });

            _ = Program.SendEnemyCastom(0, 0, 5, enemy, inlineKeyboard);
            _ = MassageDeleter(msgNew, 30);
        }
        else
        {
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    callbackQuery.Message.Chat.Id,
                    $"{character.Name} \n Вы присоединились к группе. Осталось мест {3 - dangionList.Count} / 3",
                    messageThreadId: messageThreadId
                );
            _ = MassageDeleter(msgNew, 30);
        }
     _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);
    }
}

