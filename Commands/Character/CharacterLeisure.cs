using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
using Telegram.Bot.Types.ReplyMarkups;
using System.Numerics;
public class CharacterLeisure
{
    async public Task Leisure(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        var user = userCharacterData[wrapper.UserId];
        string items =
        $"{user.Name}\n\n" +
        $"Здесь Вы можете:\n\n" +
        $" - Хранить свои вещи. \n" +
        $" - Выходить на сборы. \n" +
        $" - Участвовать на аукционе. \n" +
        $" - Вставать на стражу.\n\n";


        var media = new InputMediaPhoto
        {
            Media = inventoryImage,
            Caption = items,
            ParseMode = ParseMode.Html
        };

        await BotServices.Instance.Bot.EditMessageMedia(
            chatId: wrapper.ChatId,
            messageId: wrapper.MessageId,
            media: media,
            replyMarkup: LeisureKeyboard
        );
    }

    async public Task StoreItem(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        if (wrapper.Text == "back" && wrapper.CallbackQuery != null && wrapper.CallbackQueryId != null)
        {
            itemListToRemove.Remove(wrapper.UserId);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            return;
        }

        var user = userCharacterData[wrapper.UserId];

        var inlineKeyboard = CreateInlineKeyboardList(user.Inventory);

        string equippedItemsString = user.GetEquippedItemsSimple();
        string inventoryString = user.GetInventoryItemsSimple();
        string storageString = GetStorageItemsString(wrapper.UserId) + "</blockquote>";

        const int maxMessageLength = 4096;
        if (storageString.Length > maxMessageLength)
            storageString = storageString.Substring(0, maxMessageLength - 16) + "...</blockquote>";
        

        string items =
        $"{wrapper.UserId} {user.Name}\n\n" +
        $"🧥 Экипированные предметы:\n{equippedItemsString}\n\n" +
        $"💼 Инвентарь: [{user.Inventory.Count}/40] \n\n<blockquote expandable>{inventoryString}</blockquote>\n\n" +
        $"Что бы забрать предмет напишите - \n'@Забрать [Номер предмета по списку]'.\n\n" +
        $"🏛️ Хранилище:\n\n<blockquote expandable>{storageString}";

        _ = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            messageThreadId: wrapper.MessageThreadId,
            text: items,
            replyMarkup: inlineKeyboard,
            parseMode: ParseMode.Html
        );
        _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
    }

    async public Task WaitingForItem(Wrapper wrapper)
    {

            long originalID = long.Parse(wrapper.OriginalMessage.Text!.Split(" ")[0]);

            if (wrapper.UserId != originalID)
               return;

            var player = userCharacterData[wrapper.UserId];

            var buttonText = wrapper.Text.Split("_")[1];

            if (buttonText == "delStore" && wrapper.CallbackQuery != null && wrapper.CallbackQueryId != null)
            {
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                _ = MassageDeleter(wrapper.OriginalMessage, 1);
                return;
            }
            int itemIndex = int.Parse(buttonText);

            if (itemIndex != -1)
            {
                var itemToStore = player.Inventory[itemIndex];
                player.Inventory.RemoveAt(itemIndex);

                if (!userStorage.ContainsKey(wrapper.UserId))
                    userStorage[wrapper.UserId] = new List<Item>();

                userStorage[wrapper.UserId].Add(itemToStore);

                string message = $"Предмет '{itemToStore.Name} + {itemToStore.Quality}' успешно помещен в хранилище.";

                Message msgNew1 = await BotServices.Instance.Bot.SendMessage(
                    chatId: wrapper.ChatId,
                    text: message,
                    messageThreadId: wrapper.MessageThreadId
                );
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
                _ = MassageDeleter(msgNew1, 15);
                _ = MassageDeleter(wrapper.OriginalMessage, 1);
            }
            else
            {
                string message = $"Предмет не найден!";
                Message msgNew1 = await BotServices.Instance.Bot.SendMessage(
                    chatId: wrapper.ChatId,
                    text: message,
                    messageThreadId: wrapper.MessageThreadId
                );
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
                _ = MassageDeleter(msgNew1, 15);
                _ = MassageDeleter(wrapper.OriginalMessage, 1);
            }
    }

    async public Task RetrieveItem(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId)) return;
        var player = userCharacterData[wrapper.UserId];

        if (userStorage.ContainsKey(wrapper.UserId) && userStorage[wrapper.UserId].Count > 0)
        {
            var parts = wrapper.Text.Split(' ');
            var storageItems = userStorage[wrapper.UserId];

            if (int.TryParse(parts[1], out int index) && index > 0 && index <= storageItems.Count)
            {
                    var itemToRetrieve = storageItems[index - 1];

                    player.AddItemToInventory(itemToRetrieve);
                    storageItems.RemoveAt(index - 1);

                    string message = $"Предмет '{itemToRetrieve.Name} + {itemToRetrieve.Quality}' успешно забран из хранилища.";
                    Message msgNew = await BotServices.Instance.Bot.SendMessage(
                        chatId: wrapper.ChatId,
                        text: message,
                        messageThreadId: wrapper.MessageThreadId
                    );

                    _ = MassageDeleter(msgNew, 15);
            }
            else
            {
                string message = "Неверный индекс! Пожалуйста, укажите корректный номер предмета.";
                Message msgNew = await BotServices.Instance.Bot.SendMessage(
                    chatId: wrapper.ChatId,
                    text: message,
                    messageThreadId: wrapper.MessageThreadId
                );

                _ = MassageDeleter(msgNew, 15);
            }
        }
        else
        {
            string message = "Ваше хранилище пусто!";
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: message,
                messageThreadId: wrapper.MessageThreadId
            );

            _ = MassageDeleter(msgNew, 15);
        }
        _ = MassageDeleter(wrapper.OriginalMessage, 10);
    }

    private string GetStorageItemsString(long userId)
    {
        if (userStorage.ContainsKey(userId))
        {
            var items = userStorage[userId];
            return string.Join("\n", items.Select((item, index) => $"{index + 1}. {item.Name} + {item.Quality}"));
        }
        return "Пусто";
    }

    private InlineKeyboardMarkup CreateInlineKeyboardList(List<Item> items)
    {
        var buttons = items.Select((item, index) =>
            InlineKeyboardButton.WithCallbackData(
                item != null ? $"{item.Name} + {item.Quality}" : "Пусто",
                item != null ? $"@store_{index}" : "Пусто"))
            .ToArray();

        var rows = new List<InlineKeyboardButton[]>();
        for (int i = 0; i < buttons.Length; i += 2)
        {
            var row = buttons.Skip(i).Take(2).ToArray();
            rows.Add(row);
        }
        var backButtonRow = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Убрать сообщение", "@store_delStore") };
        rows.Add(backButtonRow);
        return new InlineKeyboardMarkup(rows);
    }

    public async Task SelectJob(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        var player = userCharacterData[wrapper.UserId];

        if (player.OccupationPlayer.CurrentJob == JobType.Free)
        {
            if (wrapper.Text == "back" && wrapper.CallbackQuery != null && wrapper.CallbackQueryId != null)
            {
                itemListToRemove.Remove(wrapper.UserId);
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
                _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
                return;
            }

            var media = new InputMediaPhoto
            {
                Media = inventoryImage,
                Caption = "Выберете работу на которую Вы бы хотели потратить время:",
                ParseMode = ParseMode.Html
            };

            await BotServices.Instance.Bot.EditMessageMedia(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                media: media,
                replyMarkup: SelectJobInlineKeyboard
            );
        }
        else if (DateTime.Now > player.OccupationPlayer.Duration)
        {
            var completion = player.OccupationPlayer.JobCompleted(player);

            var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: completion,
            messageThreadId: wrapper.MessageThreadId
        );
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            _ = MassageDeleter(msg, 15);
        }
        else 
        {
            string formattedWorkTime = player.OccupationPlayer.Duration.ToString("HH:mm");

            var completion = $"{player.Name}\n\n" +
            $"Вы все еще заняты работой.\n" +
            $"Работа будет завершена в '{formattedWorkTime}'";

            var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: completion,
            messageThreadId: wrapper.MessageThreadId
        );
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            _ = MassageDeleter(msg, 15);
        }
    }

    public async Task HandleJobSelection(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;


        var selectedJob = wrapper.Text.Split("_")[1]; 
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("2 часа", $"@jobDuration_{selectedJob}_2"),
                InlineKeyboardButton.WithCallbackData("4 часа", $"@jobDuration_{selectedJob}_4"),
                InlineKeyboardButton.WithCallbackData("6 часов", $"@jobDuration_{selectedJob}_6"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("8 часов", $"@jobDuration_{selectedJob}_8"),
                InlineKeyboardButton.WithCallbackData("Назад", "back"),
            },
        });

        await BotServices.Instance.Bot.EditMessageCaption(
            chatId: wrapper.ChatId,
            messageId: wrapper.MessageId,
            caption: "Выберете время которое Вы хотите потратить на работу:",
            replyMarkup: inlineKeyboard,
            parseMode: ParseMode.Html
        );
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
    }

    public async Task HandleDurationSelection(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        var parts = wrapper.Text.Split('_');
        var selectedJobString = parts[1];
        var durationInt = int.Parse(parts[2]);

        if (!Enum.TryParse(selectedJobString, out JobType selectedJob))
        { 
            Console.WriteLine("No job find!\n");
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            return;
        }

        var player = userCharacterData[wrapper.UserId];
        var workTime = DateTime.Now.AddHours(durationInt);
        string formattedWorkTime = workTime.ToString("HH:mm");

        player.OccupationPlayer.CurrentJob = selectedJob;
        player.OccupationPlayer.Duration = workTime;
        player.OccupationPlayer.Hours = durationInt;

        string message = 
            $"{player.Name}\n\n" +
            $"Вы занялись работой.\n" +
            $"Работа будет завершена в '{formattedWorkTime}'";

        var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: message,
            messageThreadId: wrapper.MessageThreadId
        );
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
        _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
        _ = MassageDeleter(msg, 15);
    }

    public async Task StopJob(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId)
            return;

        var player = userCharacterData[wrapper.UserId];
        var currentJob = player.OccupationPlayer.CurrentJob;
        var duration = player.OccupationPlayer.Duration;

        if (currentJob == JobType.Free)
        {
            var completion = 
                $"{player.Name}\n\n" +
                $"Вы не заняты никакой работой.\n";

            var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: completion,
            messageThreadId: wrapper.MessageThreadId
        );
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            _ = MassageDeleter(msg, 15);

        }
        else if (DateTime.Now > duration)
        {
            var completion = player.OccupationPlayer.JobCompleted(player);

            var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: completion,
            messageThreadId: wrapper.MessageThreadId
        );
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            _ = MassageDeleter(msg, 15);
        }
        else
        {
            player.OccupationPlayer.CurrentJob = JobType.Free;
            var completion =
                $"{player.Name}\n\n" +
                $"Вы бросили работу.\n" +
                $"Работа была бы завершена в '{duration.ToString("HH:mm")}'";

            var msg = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: completion,
            messageThreadId: wrapper.MessageThreadId
        );

            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId!);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery!);
            _ = MassageDeleter(msg, 15);
        }
    }
}

