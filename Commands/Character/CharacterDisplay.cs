using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static GlobalData;

public class CharacterDisplay 
{
    async public Task ShowCharacter(long Id, int? MessageThreadId, long IdFrom, Message msg)
    {
        Message msg1;
        if (!userCharacterData.ContainsKey(IdFrom))
            msg1 = await BotServices.Instance.Bot.SendMessage(Id, "У вас нет персонажа.", messageThreadId: MessageThreadId);
        else
        {
            var character = userCharacterData[IdFrom];
            string cap = GetCharacterCaption(character, IdFrom);
            if (character.Image == null) throw new ArgumentNullException(nameof(character.Image), "No image character.");
            msg1 = await BotServices.Instance.Bot.SendPhoto(
                chatId: Id,
                photo: character.Image,
                caption: cap,
                replyMarkup: MenuKeyboard,
                messageThreadId: MessageThreadId,
                parseMode: ParseMode.Html
            );

            Message msg2 = await BotServices.Instance.Bot.SendMessage(Id,
                "ПРОДОЛЖИТЬ СЮЖЕТ?\n(Продолжение появится в игровом разделе.)",
                replyMarkup: m_StoryButton,
                messageThreadId: MessageThreadId);

            _ =MassageDeleter(msg2, 30);
        }
        userMessgaeMap[msg1.MessageId] = IdFrom;
        _ = MassageDeleter(msg, 5);
    }
    async public Task ShowCharacterRefreshProxy(Wrapper wrapper)
    {
        if (wrapper.CallbackQuery == null) throw new ArgumentNullException(nameof(wrapper.CallbackQuery));
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQuery.Id);
            await ShowCharacterRefresh(wrapper.CallbackQuery);
    }

    async public static Task ShowCharacterRefresh(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message == null) return;
        long chatId = callbackQuery.Message.Chat.Id;
        int messageId = callbackQuery.Message.MessageId;
        long IdFrom = callbackQuery.From.Id;

        if (!userMessgaeMap.TryGetValue(callbackQuery.Message.MessageId, out long originalUserId)){
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQueryId: callbackQuery!.Id, text: "Это не ваш персонаж!", cacheTime: 10);
            return;
        }
        if (callbackQuery.From.Id != originalUserId)
            return;

        Message msg1;
        if (!userCharacterData.ContainsKey(IdFrom))
            msg1 = await BotServices.Instance.Bot.SendMessage(chatId, "У вас нет персонажа.", messageThreadId: callbackQuery.Message.MessageThreadId);
        else
        {
            var character = userCharacterData[IdFrom];
            string cap = GetCharacterCaption(character, IdFrom);

            var media = new InputMediaPhoto
            {
                Media = character.Image!,
                Caption = cap,
                ParseMode = ParseMode.Html
            };

            bool captionChanged = callbackQuery.Message.Caption != cap;
            bool mediaChanged = callbackQuery.Message.Photo?.FirstOrDefault()?.FileId != character.Image!;

            if (mediaChanged)
              {
                msg1 = await BotServices.Instance.Bot.EditMessageMedia(
                chatId: chatId,
                messageId: messageId,
                media: media,
                replyMarkup: MenuKeyboard
            );
            } 
            else if (captionChanged) 
            {
                msg1 = await BotServices.Instance.Bot.EditMessageCaption(
                chatId: chatId,
                messageId: messageId,
                caption: cap,
                replyMarkup: MenuKeyboard,
                parseMode: ParseMode.Html
            );
            }
            else msg1 = callbackQuery.Message;

        }
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(callbackQuery.Id);
        userMessgaeMap.Remove(messageId);
        userMessgaeMap[msg1.MessageId] = IdFrom;
    }
    // SHORT SHOW MESSAGES
#region Show Messages
    async public Task ShowTierList(Message msg, UpdateType type)
    {
        var topTierMessage = $"Число игроков: {userCharacterData.Count} \nСписок персонажей в топе:\n";
        foreach (var character in userTopTier)
        {
            topTierMessage += $"{character.Name} - Уровень: {character.Level}\n";
        }

        Message msgNew = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, topTierMessage, messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 120);
        _ = MassageDeleter(msg, 30);
    }
    async public Task ShowID(Message msg, UpdateType type)
    {
        if (msg?.From?.Id == null) return;
        var name = userCharacterData[msg.From.Id].Name;
        var text = $"{name}\nВаш ID: {msg.From.Id}";
        Message msgNew = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, text, messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 30);
        _ = MassageDeleter(msg, 30);
    }
    async public Task ShowHP(Message msg, UpdateType type)
    {
        if (msg?.From?.Id == null) return;
        var name = userCharacterData[msg.From.Id].Name;
        var curHp = userCharacterData[msg.From.Id].CurrentHP;
        var hp = userCharacterData[msg.From.Id].HP;
        var text = $"{name}\nВаше здоровье: {curHp} / {hp}";
        Message msgNew = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, text, messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        _ = MassageDeleter(msg, 10);
    }
    public async Task ShowInventory(Message msg, UpdateType type)
    {
        if (msg?.From?.Id == null) return;
        var user = userCharacterData[msg.From.Id];
        var items = user.GetInventoryItemsSimple();

        var text = $"{user.Name}\nВаш инвентарь: \n{items}";
        Message msgNew = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, text, messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        _ = MassageDeleter(msg, 10);
    }
    public async Task ShowEquippedItems(Message msg, UpdateType type)
    {
        if (msg?.From?.Id == null) return;
        var user = userCharacterData[msg.From.Id];
        var items = user.GetEquippedItemsSimple();

        var text = $"{user.Name}\nВаша экипировка: \n{items}";
        Message msgNew = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, text, messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        _ = MassageDeleter(msg, 10);
    }
    #endregion ??

    async public Task ShowBuffs(Wrapper wrapper)
    {
        if (!wrapper.IsValid()) return;

        var character = userCharacterData[wrapper.UserId];
        var name = character.Name;
        var activeBuffs = character.GetActiveBuffs();

        string buffsString = activeBuffs.Count > 0
            ? string.Join("\n", activeBuffs.Select(buff => buff.Name))
            : "Нет активной ауры.";

        var text = $"{name}\nВаши ауры: \n{buffsString}";

        Message msgNew = await BotServices.Instance.Bot.SendMessage(wrapper.ChatId, text, messageThreadId: wrapper.MessageThreadId);

        _ = MassageDeleter(msgNew, 10);
        if (wrapper.Type == UpdateType.Message)
            _ = MassageDeleter(wrapper.OriginalMessage, 10);
        if (wrapper.CallbackQueryId != null)
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }
    public async Task ShowInventoryEdit(Wrapper wrapper)
    {
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        long userId = wrapper.UserId;

        if (userCharacterData.ContainsKey(userId))
        {
            var character = userCharacterData[userId];

            string equippedItemsString = character.GetEquippedItemsSimple();
            string inventoryString = character.GetInventoryItemsSimple();
            //string storageString = GetStorageItemsString(wrapper.UserId);

            await BotServices.Instance.Bot.EditMessageCaption(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId, 
                caption:
                $"🧥 Экипированные предметы:\n{equippedItemsString}\n\n" +
                $"💼 Инвентарь: [{character.Inventory.Count}/40] \n\n<blockquote expandable>{inventoryString}</blockquote>\n\n",
                replyMarkup: MenuKeyboardShop,
                parseMode: ParseMode.Html
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
    public async Task CloseHero(Wrapper wrapper)
    {

        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        _ = BotServices.Instance.Bot.DeleteMessage(wrapper.ChatId, wrapper.MessageId);
        if (wrapper.CallbackQueryId != null)
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
        userMessgaeMap.Remove(wrapper.MessageId);
        await Task.CompletedTask;
    }
    private static string GetCharacterCaption(Character character, long IdFrom)
    {
        var achievements = character.Achievements;
        string achievementsString = achievements.Count > 0 ? string.Join("\n", achievements) : "";
        string equippedItemsString = character.GetEquippedItemsSimple();

        //string storageString = GetStorageItemsString(IdFrom);

        return $"🪪 {character.Name} \n" +
               $"Ваш ID: <tg-spoiler>{character.UserID}</tg-spoiler>\n\n" +
               $"👇 Ваш персонаж:\n" +
               $"⚜️ Ваш ранг: {character.Rank} \n" +
               $"🎭 Пол: {character.Gender}\n" +
               $"🔰 Класс: {character.Class}\n" +
               $"🔎 О себе: <tg-spoiler>{character.About}</tg-spoiler>\n\n" +
               $"🧠 Левел: {character.Level}\n" +
               $"✨ Опыт: {character.EXP} / {character.EXPToNextLevel}\n" +
               $"⚔️ Атака: {character.ATK}\n" +
               $"🪖 Зашита: {character.ARMOR}\n" +
               $"❤️ Здоровье: {character.CurrentHP} / {character.HP}\n" +
               $"💙 Духовные силы: {character.CurrentSP} / {character.SP}\n" +
               $"🧬 Восстановление: {character.REGEN_HP}\n" +
               $"💰 Золото: {character.Gold}\n" +
               $"📚 Уровень сюжета: {character.StoryProgression}\n\n" +
               $"🏆 Достижения:\n<blockquote expandable>{achievementsString}</blockquote>\n\n" +
               $"🪖 Экипированные предметы:\n{equippedItemsString}\n\n";
              // $"🏛️ Хранилище:\n\n<blockquote expandable>{storageString}</blockquote>\n\n"; 

    }

    private static string GetStorageItemsString(long userId)
    {
        if (userStorage.ContainsKey(userId))
        {
            var items = userStorage[userId];
            return string.Join("\n", items.Select((item, index) => $"{index + 1}. {item.Name} + {item.Quality}"));
        }
        return "Пусто";
    }


}

