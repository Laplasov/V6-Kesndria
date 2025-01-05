using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using static GlobalData;
using System.Security.Claims;

public class ClassShop
{
    public async Task ChangeClass(Wrapper wrapper)
    {
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        long userId = wrapper.UserId;

        if (userCharacterData.ContainsKey(userId))
        {
            var character = userCharacterData[userId];

            string ClassString = character.Class!;


            await BotServices.Instance.Bot.EditMessageCaption(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                caption:
                $" Ваш текущий класс:\n{ClassString}\n\n" +
                $" Смена класса стоит 1500 золота!\n" +
                $" Классы доступные для смены:\n",
                replyMarkup: ClassesKeyboardToChose
            );

        }
        else
        {
            await BotServices.Instance.Bot.SendMessage(userId, "У вас нет персонажа или это не ваш персонаж.");
        }

        if (wrapper.Type == UpdateType.Message)
            _ = MassageDeleter(wrapper.OriginalMessage, 30);
        if (wrapper.Type == UpdateType.CallbackQuery)
            if (wrapper.CallbackQueryId != null)
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }


    public async Task ChoseClass(Wrapper wrapper)
    {
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId || wrapper.CallbackQueryId == null || wrapper.CallbackQuery?.Message == null)
            return;
        if (wrapper.Text == "back")
        {
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            return;
        }

        var player = userCharacterData[wrapper.UserId];

        if (player.Gold < 1500) 
        {
            _ = BotServices.Instance.Bot.SendMessage(chatId: wrapper.ChatId, text: "Недостаточно золота!", messageThreadId: wrapper.MessageThreadId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            return;
        }
        if (Enum.TryParse<ClassType>(wrapper.Text, out var classType)) 
        {
            player.Gold -= 1500;
            player.ClassType = classType;
            if (player.ClassType != classType)
            {
                Console.WriteLine("Failed to set ClassType correctly.");
                return; 
            }
            player.SetClassString();
            _ = BotServices.Instance.Bot.SendMessage(chatId: wrapper.ChatId, text: $"Ваш новый класс:\n{wrapper.Text}\n Остаток золота:\n{player.Gold}", messageThreadId: wrapper.MessageThreadId);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            return;
        }
        _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);

        await Task.CompletedTask;
    }
}
