using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;
using System.Security.Claims;

public static class GlobalData
{
    public delegate (string message, int currentDamageEnemy) OnTakeDamageEffectDelegate(Character player, Enemy enemy, int currentDamageEnemy, Random random);
    public delegate (string message, int currentDamageEnemy) OnDamagingEffectDelegate(Character player, Enemy enemy, int currentDamageEnemy, Random random);


    public static InlineKeyboardMarkup m_inlineKeyboardAct1YesAndNo = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Да", "ACT_1_yes_CHOSE_1"),
            InlineKeyboardButton.WithCallbackData("Нет", "ACT_1_no_CHOSE_1")
        }
    });
    public static InlineKeyboardMarkup m_inlineKeyboardAct1Yes2 = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Да", "ACT_1_yes_CHOSE_2")
        }
    });
    public static InlineKeyboardMarkup m_StoryButton = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Да", "ACT_1_yes_CHOSE_1")
        }
    });

    public static InitSkillCollection initCol = new InitSkillCollection();
    public static OnTakeDamageEffectCollection TakeDamage = new OnTakeDamageEffectCollection();
    public static OnDamagingEffectCollection OnDamaging = new OnDamagingEffectCollection();


    public static Dictionary<long, Character> userCharacterData = new Dictionary<long, Character>();
    public static List<Character> userTopTier = new List<Character>();
    public static List<Enemy> enemyPreset = new List<Enemy>();
    public static List<Enemy> dangionEnemyList = new List<Enemy>();

    public static Dictionary<long, long> tradeList = new Dictionary<long, long>();
    public static Dictionary<long, long> tradeListID = new Dictionary<long, long>();
    public static Dictionary<long, long> itemTradeListID = new Dictionary<long, long>();
    public static List<long> tradeListEnemy = new List<long>();
    public static List<long> tradeListItem = new List<long>();
    public static List<long> itemListToUse = new List<long>();
    public static List<long> itemListToRemove = new List<long>();
    public static List<long> dangionList = new List<long>();
    public static List<long> itemListToSell = new List<long>();

    public static CancellationToken СancellationTokenGlobal;
    public static JsonDocument? JsonOldDocument;
    public static Dictionary<long, int> originalHPs = new Dictionary<long, int>();
    public static Dictionary<long, DateTime> enemyTimers = new Dictionary<long, DateTime>();

    public static Dictionary<int, Enemy> EnemyPool = new Dictionary<int, Enemy>();
    public static ClaimTracker claimTracker = new ClaimTracker();

    public static int dangionEnemyIndex = 0;
    public static string[] currentDangionEnemy = new string[0];
    public static int[] currentDangionEnemyIds = new int[0];
    public static Dictionary<long, long> userMessgaeMap = new Dictionary<long, long>();

    public static InlineKeyboardMarkup MenuKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Передать игроку", "give_gold"),
            InlineKeyboardButton.WithCallbackData("Мой инвентарь", "show_inventory"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Показать мои ауры", "show_auras"),
            InlineKeyboardButton.WithCallbackData("Обновить имя", "refresh_name"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Закрыть", "close_hero"),
            InlineKeyboardButton.WithCallbackData("Обновить", "back"),
        }
    }
    );

    public static InlineKeyboardMarkup MenuKeyboardShop = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("⬆️ Использовать предмет", "use_item"),
            InlineKeyboardButton.WithCallbackData("⬇️ Снять предмет", "remove_equipment"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("🛒 Открыть магазин", "show_shop"),
            InlineKeyboardButton.WithCallbackData("🏷️ Продать предмет", "sell_shop"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("📖 Изменить класс", "change_class"),
            InlineKeyboardButton.WithCallbackData("🗑️ Продать весь хлам", "trash_sell"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("💥 Заточить предмет", "upgrade_item"),
            InlineKeyboardButton.WithCallbackData("↩️ Назад", "back"),
        }
    }
    );
    public static InlineKeyboardMarkup ItemsKeyboardEqiped = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Оружие", "Weapon"),
            InlineKeyboardButton.WithCallbackData("Броня", "Armor"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Бижутерия", "Accessory"),
            InlineKeyboardButton.WithCallbackData("Назад", "back"),
        }
    });
    public static InlineKeyboardMarkup ClassesKeyboardToChose = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Шаман Солнечного Огня (☼ - Солнечная вспышка)", "Solar_Flare"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Песчаный странник (⊕ - Пыль)", "Dust"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Эфирный чародей (⚷ - Эфир)", "Æther"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Призыватель Бездны (⊗ - Бесконечность)", "Infinity"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Квантовый ткач (☿ Нексус)", "Nexus"),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад", "back"),
        }
    }
    );
    public static async Task MassageDeleter(Message msg, int sec)
    {
  
        await Task.Delay(TimeSpan.FromSeconds(sec), СancellationTokenGlobal);
        try
        {
            await BotServices.Instance.Bot.DeleteMessage(
                chatId: msg.Chat.Id,
                messageId: msg.MessageId
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting message: {ex.Message}");
        }
    }
    public static async Task CallbackDeleter(Update update, int sec)
    {
        await Task.Delay(TimeSpan.FromSeconds(sec), СancellationTokenGlobal);
        var callbackQuery = update.CallbackQuery;
        if (callbackQuery?.Message == null ) throw 
                new ArgumentNullException(nameof(callbackQuery.Message), "CallbackDeleter - message.Text cannot be null.");
        try
        {
            await BotServices.Instance.Bot.DeleteMessage(
                chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting callback message: {ex.Message}");
        }
    }

}


