using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static GlobalData;



public class ShopKeeper
{
    private CharacterDisplay characterDisplay; 
    public ShopKeeper()
    {
        characterDisplay = new CharacterDisplay(); 
    }

    private List<IShopItem> shopItems = new List<IShopItem>
    {
        new HealthPotionInstant(),
        new SPPotionInstant(),
        new AttackPotion(),
        //new ArmorPotion(),
        new WeaponShiv(),
        new ArmorVeil(),
        new AccessoryEarring(),
        new AccessoryCatcher(),
    };

    #region MonsterShop
    async public Task MonsterShop(Message msg, UpdateType type)
    {
        if (userCharacterData.ContainsKey(msg.From!.Id))
        {
            string enemys = "Выберите имя врага которого хотите призвать:\n";

            foreach (var enemy in enemyPreset)
            {
                int cost = 10000;
                if (enemy.Level <= 5)
                    cost = 750;
                else if (enemy.Level <= 10)
                    cost = 1500;
                else if (enemy.Level <= 15)
                    cost = 3000;

                enemys += $"{enemy.Name} - {cost}\n";
            }

            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: enemys,
                messageThreadId: msg.MessageThreadId
            );
            _ = MassageDeleter(msgNew, 60);

            tradeListEnemy.Add(msg.From.Id);
            _ = MassageDeleter(msg, 30);
        }
    }

    async public Task ChoseMonster(Message msg, UpdateType type)
    {
        Enemy selectedEnemy = null;
        foreach (var enemy in enemyPreset)
        {
            if (enemy.Name.Equals(msg.Text, StringComparison.OrdinalIgnoreCase))
            {
                selectedEnemy = enemy;
                break;
            }
        }
        if(selectedEnemy == null)
        {
            string response = "Враг с таким именем не найден. Пожалуйста, попробуйте еще раз.";
            Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id, 
                text: response,
                messageThreadId: msg.MessageThreadId);
            tradeListEnemy.Remove(msg.From!.Id);
            _ = MassageDeleter(msgNew2, 30);
            return;
        }
        if (EnemyPool.ContainsKey(selectedEnemy.EnemyId))
        {
            string response = "Этот враг уже существует в игре. Пожалуйста, выберите другого врага.";
            Message msgNew3 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: response,
                messageThreadId: msg.MessageThreadId);
            tradeListEnemy.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew3, 30);
            return;

        }

        Enemy enemyEntity = new Enemy(
            selectedEnemy.EnemyId,
            selectedEnemy.Level,
            selectedEnemy.Name,
            selectedEnemy.MaxHP,
            selectedEnemy.ATK,
            selectedEnemy.Image,
            selectedEnemy.EXP,
            selectedEnemy.Gold,
            selectedEnemy.Caption,
            selectedEnemy.Armor
        );

        var player = userCharacterData[msg.From!.Id];


        string levelScoup = "";
        int cost = 10000;

        if (enemyEntity.Level <= 5)
        {
            levelScoup = "1 - 5";
            cost = 0;
        }
        else if (enemyEntity.Level <= 10)
        {
            levelScoup = "6 - 10";
            cost = 0;
        }
        else if (enemyEntity.Level <= 15)
        {
            levelScoup = "11 - 15";
            cost = 0;
        }

        if (player.Gold < cost)
            {
            Message msgNew3 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "Недостаточно золота!",
                messageThreadId: msg.MessageThreadId
                );
            tradeListEnemy.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew3, 30);
            return;
        }

        player.Gold -= cost;

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
        InlineKeyboardButton.WithCallbackData("Attack", $"@attack_{0}_{enemyEntity.EnemyId}_{DateTime.Now.Ticks}")
        });
        var message = $"{enemyEntity.Name}\nЗдоровье: {enemyEntity.HP} / {enemyEntity.MaxHP}\n" +
            $"Атака: {enemyEntity.ATK}\n" +
            $"Уровень: {enemyEntity.Level}\n\n" +
            $"{enemyEntity.Caption}\n\n" +
            $"Награда за победу: {enemyEntity.EXP} опыта и {enemyEntity.Gold} золота!\n\n" +
            $"Враг для левела: {levelScoup}.";

        var sentMessage = await BotServices.Instance.Bot.SendPhoto(
            chatId: msg.Chat.Id,
            photo: enemyEntity.Image,
            caption: message,
            replyMarkup: inlineKeyboard
        );

        enemyEntity.MassageId = sentMessage.MessageId;
        EnemyPool[enemyEntity.EnemyId] = enemyEntity;

        tradeListEnemy.Remove(msg.From.Id);
        _ = MassageDeleter(msg, 30);
    }
    #endregion

    public async Task ItemShop(Message msg)
    {
        if (msg.From == null) return;
        if (userCharacterData.ContainsKey(msg.From.Id))
        {
            string items = "Выберите предмет который Вы хотите купить:\n";
            foreach (var item in shopItems)
            {
                items += $"{item.Name} - {item.Cost} золота.\n ({item.Discription})\n";
            }
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: items,
                messageThreadId: msg.MessageThreadId
            );
            _ = MassageDeleter(msgNew, 60);
            tradeListItem.Add(msg.From.Id);
            _ = MassageDeleter(msg, 30);
        }
    }

    public async Task ItemShop(Wrapper wrapper)
    {
        if(!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId || wrapper.CallbackQueryId == null)
            return;

        if (userCharacterData.ContainsKey(wrapper.UserId))
        {
            var inlineKeyboard = CreateInlineKeyboard(shopItems);
            var player = userCharacterData[wrapper.UserId];

            string items = "\nВыберите предмет который Вы хотите купить:\n";
            var media = new InputMediaPhoto
            {
                Media = player.Image!,
                Caption = wrapper.CallbackQuery?.Message?.Caption + items
            };

            await BotServices.Instance.Bot.EditMessageMedia(
                chatId: wrapper.ChatId,
                messageId: wrapper.MessageId,
                media: media,
                replyMarkup: inlineKeyboard
                );
            
            tradeListItem.Add(wrapper.UserId);
        }
        _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }

    async public Task ChoseItem(Message msg, UpdateType type)
    {
        if (msg.From == null) return;
        var player = userCharacterData[msg.From.Id];
        var selectedItem = shopItems.FirstOrDefault(item => item.Name == msg.Text);

        if (selectedItem != null)
        {
            await selectedItem.Purchase(player, msg);
            tradeListItem.Remove(msg.From.Id);
            _ = MassageDeleter(msg, 30);
        }
        else
        {
            Message msgNew3 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "Предмет не найден.",
                messageThreadId: msg.MessageThreadId
            );
            tradeListItem.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew3, 30);
        }
    }

    public async Task ChoseItem(Wrapper wrapper)
    {
        if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
            return;
        if (wrapper.UserId != originalUserId || wrapper.CallbackQueryId == null || wrapper.CallbackQuery?.Message == null)
            return;

        if (wrapper.Text == "back")
        {
            tradeListItem.Remove(wrapper.UserId);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            return;
        }
        var player = userCharacterData[wrapper.UserId];
        string selectedItemName = wrapper.Text; 

        var selectedItem = shopItems.FirstOrDefault(item => item.Name == selectedItemName);

        if (selectedItem != null)
        {
            _ = selectedItem.Purchase(player, wrapper.CallbackQuery.Message);
            tradeListItem.Remove(wrapper.UserId);
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
        }
        else
        {
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: wrapper.ChatId,
                text: "Предмет не найден.",
                messageThreadId: wrapper.MessageThreadId
            );
            tradeListItem.Remove(wrapper.UserId);
            _ = CharacterDisplay.ShowCharacterRefresh(wrapper.CallbackQuery);
            _ = MassageDeleter(msgNew, 30);
        }
    }
    private InlineKeyboardMarkup CreateInlineKeyboard(List<IShopItem> items)
    {
        var buttons = items.Select(item =>
            InlineKeyboardButton.WithCallbackData($"{item.Name} - {item.Cost} золота", item.Name)).ToArray();

        var rows = new List<InlineKeyboardButton[]>();
        for (int i = 0; i < buttons.Length; i += 1)
        {
            var row = buttons.Skip(i).Take(1).ToArray();
            rows.Add(row);
        }

        var backButtonRow = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "back") };
        rows.Add(backButtonRow);

        return new InlineKeyboardMarkup(rows);
    }
}
