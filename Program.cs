using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;
using static GlobalData;
using static SaveLoadManager;
using static StringCollection;

class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();

        string token = Env.GetString("DEV_BOT");

        InitializeDataStructures();

        using var cts = new CancellationTokenSource();

        BotServices.Initialize(token, cts.Token);
        var botServices = BotServices.Instance;
        var me = await botServices.Bot.GetMe(cts.Token);

        СancellationTokenGlobal = cts.Token;

        await LoadJSON();

        botServices.Bot.OnError += OnError;
        botServices.Bot.OnMessage +=  (msg, type) =>  botServices.MassageManager.OnMessage(msg, type);
        botServices.Bot.OnUpdate +=  (update) =>  botServices.UpdateManager.OnUpdate(update);

        _ = botServices.ReminderCoordinator.StartReminders();

        //foreach (var kvp in userCharacterData)
        //{
        //    var playerId = kvp.Key;
        //    var player = kvp.Value;
        //    player.SetClassString();
        //    player.UserID = playerId;
        //}

        Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();

        await SaveJSON();

        cts.Cancel();
    }


    static Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    public static Task BuffsHandler(long chatId, int? messageThreadId, Buff healthReductionBuff) =>
        Task.Run(() =>
        BotServices.Instance.BuffService?.DisbuffAfterDelay(chatId, messageThreadId, healthReductionBuff));

    public static Task SendEnemyCastom(int minLevel, int maxLevel, int delayDefeat, Enemy selectedEnemy, InlineKeyboardMarkup? Keyboard) =>
        Task.Run(() =>
        BotServices.Instance.EnemySpawnService?.EnemyUpdate(0, 0, TimeSpan.FromMinutes(delayDefeat), selectedEnemy, Keyboard));

    public static Task ChestHandler(int min, int max, string level) =>
        Task.Run(() =>
        BotServices.Instance.StashService?.StashUpdate(3, min, max, level, true));

    static void InitializeDataStructures()
    {
        userCharacterData = new Dictionary<long, Character>();
        userTopTier = new List<Character>();
        EnemyPool = new Dictionary<int, Enemy>();
        enemyPreset = new List<Enemy>();
        tradeList = new Dictionary<long, long>();
        tradeListID = new Dictionary<long, long>();
        tradeListEnemy = new List<long>();
        tradeListItem = new List<long>();
        originalHPs = new Dictionary<long, int>();
        enemyTimers = new Dictionary<long, DateTime>();
        claimTracker = new ClaimTracker();
        itemListToUse = new List<long>();
        itemListToRemove = new List<long>();
        dangionList = new List<long>();
        dangionEnemyList = new List<Enemy>();
        userMessgaeMap = new Dictionary<long, long>();
        itemListToSell = new List<long>();
    }

    static async Task LoadJSON()
    {
        var json = await File.ReadAllTextAsync(jsonUserCharacterData);
        JsonOldDocument = JsonDocument.Parse(json);
        userCharacterData = await LoadCharacterDataAsync(jsonFilePath);
        await LoadTopTierAsync();
        await LoadEnemiesPrests();
        await claimTracker.LoadClaimsFromJson();
    }

    static async Task SaveJSON()
    {
        await SaveCharacterDataAsync(userCharacterData, jsonFilePath);
        await SaveTopTierAsync();
        await claimTracker.SaveClaimsToJson();
    }
   

}
