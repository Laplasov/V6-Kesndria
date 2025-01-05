﻿using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
public class CharacterItems
{

    async public Task UseItem(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;

        var user = userCharacterData[wrapper.UserId];

        if (wrapper.Type == UpdateType.Message)
        {
            var list = await user.GetInventoryItems();
            string items = $"Выберите предмет который Вы хотите использовать:\n{list}";

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: items,
                messageThreadId: wrapper.MessageThreadId
            );

            _ = MassageDeleter(msgNew, 60);
            itemListToUse.Add(wrapper.UserId);
            _ = MassageDeleter(wrapper.OriginalMessage, 30);
        }
        else if (wrapper.Type == UpdateType.CallbackQuery)
        {
            var inlineKeyboard = CreateInlineKeyboardList(user.Inventory);
            string items = $"\nВыберите предмет который Вы хотите использовать:\n";
            var media = new InputMediaPhoto
            {
                Media = inventoryImage,
                Caption = wrapper.OriginalMessage.Caption + items
            };

            await BotServices.Instance.Bot.EditMessageMedia(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                media: media,
                replyMarkup: inlineKeyboard
            );
            itemListToUse.Add(wrapper.UserId);
        }
    }
    async public Task WaitingForItem(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId)) return;

        var player = userCharacterData[wrapper.UserId];
        string itemName;

        if (wrapper.Type == UpdateType.Message)
        {
            itemName = wrapper.Text;
            _ = MassageDeleter(wrapper.OriginalMessage, 60);
        }
        else if (wrapper.Type == UpdateType.CallbackQuery)
        {
            if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
                return;
            if (wrapper.UserId != originalUserId)
                return;

            if (wrapper.Text == "back" && wrapper.CallbackQuery != null && wrapper.CallbackQueryId != null)
            {
                itemListToUse.Remove(wrapper.UserId);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
                return;
            }

            itemName = wrapper.Text;
        }
        else return;

        bool isUsed = player.EquipItem(itemName, wrapper.OriginalMessage);
        string message;

        if (isUsed)
            message = $"Предмет '{itemName}' удачно использован или надет.";
        else
            message = $"Предмет '{itemName}' не найден или нельзя использовать!";

        Message msgNew = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: message,
            messageThreadId: wrapper.MessageThreadId
        );

        _ = MassageDeleter(msgNew, 60);
        itemListToUse.Remove(wrapper.UserId);

        if (wrapper.CallbackQuery != null)
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
    }
    async public Task SellItem(Wrapper wrapper)
    {
        if (userCharacterData.ContainsKey(wrapper.UserId))
        {
            if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
                return;
            if (wrapper.UserId != originalUserId)
                return;

            var user = userCharacterData[wrapper.UserId];
            var inlineKeyboard = CreateInlineKeyboardList(user.Inventory);
            string items = $"\nВыберите предмет который Вы хотите продать:\n";
            var media = new InputMediaPhoto
            {
                Media = inventoryImage,
                Caption = wrapper.OriginalMessage.Caption + items
            };

            await BotServices.Instance.Bot.EditMessageMedia(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                media: media,
                replyMarkup: inlineKeyboard
                );

            itemListToSell.Add(wrapper.UserId);
        }
    }
    public async Task WaitingForItemToSell(Wrapper wrapper)
    {
        if (userCharacterData.ContainsKey(wrapper.UserId) && wrapper.OriginalMessage != null) 
            {
            if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
                return;
            if (wrapper.UserId != originalUserId || wrapper.CallbackQuery == null || wrapper.CallbackQueryId == null)
                return;

            if (wrapper.Text == "back" && wrapper.CallbackQueryId != null)
            {
                itemListToSell.Remove(wrapper.UserId);
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);

                return;
            }
            var player = userCharacterData[wrapper.UserId];

            string itemName = wrapper.Text;

            bool IsUsed = player.SellItem(itemName, 15);
            string message;

            if (IsUsed)
                message = $"Предмет {itemName} удачно продан.";
            else
                message = $"Предмет {itemName} не найден!";

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: message,
                messageThreadId: wrapper.MessageThreadId
            );
            _ = MassageDeleter(msgNew, 15);
            itemListToSell.Remove(wrapper.UserId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            if (wrapper.CallbackQueryId != null)
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
        }
    }
    async public Task RemoveItem(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;

        var user = userCharacterData[wrapper.UserId];

        if (wrapper.Type == UpdateType.Message)
        {
            var list = await user.GetEquippedItems();
            string items = $"Выберите предмет который Вы хотите убрать в инвентарь:\n{list}";

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: items,
                messageThreadId: wrapper.MessageThreadId
            );

            _ = MassageDeleter(msgNew, 60);
            itemListToRemove.Add(wrapper.UserId);
            _ = MassageDeleter(wrapper.OriginalMessage, 30);
        }
        else if (wrapper.Type == UpdateType.CallbackQuery)
        {
            var inlineKeyboard = ItemsKeyboardEqiped;
            string items = $"\nВыберите предмет который Вы хотите убрать в инвентарь:\n";
            var media = new InputMediaPhoto
            {
                Media = user.Image!,
                Caption = wrapper.OriginalMessage.Caption + items
            };

            await BotServices.Instance.Bot.EditMessageMedia(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                media: media,
                replyMarkup: inlineKeyboard
            );

            itemListToRemove.Add(wrapper.UserId);
        }
    }
    async public Task WaitingForItemRemove(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;

        var player = userCharacterData[wrapper.UserId];
        string message;

        if (wrapper.Type == UpdateType.Message)
        {
            bool isUsed = player.UnequipItemByName(wrapper.Text);
            message = isUsed ? $"Предмет {wrapper.Text} убран в инвентарь." : $"Предмет {wrapper.Text} не найден!";

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: message,
                messageThreadId: wrapper.MessageThreadId
            );

            _ = MassageDeleter(msgNew, 60);
            itemListToRemove.Remove(wrapper.UserId);
            _ = MassageDeleter(wrapper.OriginalMessage, 30);
        }
        else if (wrapper.Type == UpdateType.CallbackQuery)
        {
            if (wrapper.Text == "back" && wrapper.CallbackQuery != null && wrapper.CallbackQueryId != null)
            {
                itemListToRemove.Remove(wrapper.UserId);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
                return;
            }

            string itemName = wrapper.Text;
            bool isUsed = await player.UnequipItemByType(itemName);
            message = isUsed ? $"Предмет {itemName} убран в инвентарь." : $"Предмет {itemName} не найден!";

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: message,
                messageThreadId: wrapper.MessageThreadId
            );

            _ = MassageDeleter(msgNew, 15);
            itemListToRemove.Remove(wrapper.UserId);
            if (wrapper.CallbackQuery != null)
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
        }
    }

    public async Task SellAllTrash(Wrapper wrapper)
    {
        if (userCharacterData.ContainsKey(wrapper.UserId))
        {
            if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            {
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
                return;
            }
            if (wrapper.UserId != originalUserId)
                return;

            var player = userCharacterData[wrapper.UserId];
            var itemsToSell = player.Inventory.Where(item => item.Type == ItemType.ForSell).ToList();
            if (itemsToSell.Count == 0)
            {
                await BotServices.Instance.Bot.SendMessage(
                    chatId: wrapper.ChatId,
                    text: "У вас нет хлама для продажи.",
                    messageThreadId: wrapper.MessageThreadId
                );
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
                return;
            }
            foreach (var item in itemsToSell)
            {
                player.SellItem(item.Name, 15);
            }
            string soldItems = string.Join("\n", itemsToSell.Select(item => item.Name));
            await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: $"Вы успешно продали следующие предметы:\n{soldItems}",
                messageThreadId: wrapper.MessageThreadId
            );

            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
        }

    }

    private InlineKeyboardMarkup CreateInlineKeyboardList(List<Item> items)
    {
        var buttons = items.Select(item =>
            InlineKeyboardButton.WithCallbackData(item?.Name ?? "Пусто", item?.Name ?? "Пусто")).ToArray();
        var rows = new List<InlineKeyboardButton[]>();
        for (int i = 0; i < buttons.Length; i += 2)
        {
            var row = buttons.Skip(i).Take(2).ToArray();
            rows.Add(row);
        }
        var backButtonRow = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "back") };
        rows.Add(backButtonRow);
        return new InlineKeyboardMarkup(rows);
    }

}

