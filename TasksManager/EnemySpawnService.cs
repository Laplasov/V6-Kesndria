using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static GlobalData;
using static StringCollection;

public class EnemySpawnService : IEnemySpawnService
{
    private readonly CancellationToken _cancellationToken;
    private readonly Random _random;
    public EnemySpawnService(CancellationToken cancellationToken)
    {
        _random = new Random();
        _cancellationToken = cancellationToken;
    }

    public async Task EnemySpawn(int delayBetween, int delayDefeat)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {

            _ = Task.Run(() => EnemyUpdate(11, 15, TimeSpan.FromMinutes(delayDefeat), null, null));
            _ = Task.Run(() => EnemyUpdate(1, 5, TimeSpan.FromMinutes(delayDefeat), null, null));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);

            _ = Task.Run(() => EnemyUpdate(11, 15, TimeSpan.FromMinutes(delayDefeat), null, null));
            _ = Task.Run(() => EnemyUpdate(1, 5, TimeSpan.FromMinutes(delayDefeat), null, null));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);

            _ = Task.Run(() => EnemyUpdate(11, 15, TimeSpan.FromMinutes(delayDefeat), null, null));
            _ = Task.Run(() => EnemyUpdate(1, 5, TimeSpan.FromMinutes(delayDefeat), null, null));
            await Task.Delay(TimeSpan.FromMinutes(delayBetween), _cancellationToken);
        }
    }

    public async Task EnemyUpdate(int minLevel, int maxLevel, TimeSpan delay, Enemy? selectedEnemy, InlineKeyboardMarkup? Keyboard)
    {

        if (selectedEnemy == null)
        {
            var filteredEnemies = enemyPreset.Where(e => e.Level >= minLevel && e.Level <= maxLevel).ToList();
            int randomNumber = new Random().Next(filteredEnemies.Count);
            selectedEnemy = filteredEnemies[randomNumber];
                while (EnemyPool.ContainsKey(selectedEnemy.EnemyId))
                    selectedEnemy = filteredEnemies[++randomNumber];
        }
        if (selectedEnemy is null) return;

        Enemy enemy = new Enemy(
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

        enemyTimers[enemy.EnemyId] = DateTime.UtcNow;
        enemy.ClassTypeEnum = (ClassType)_random.Next( 0, 4 );
        enemy.GetClassStringEnemy();

        InlineKeyboardMarkup? inlineKeyboard;

        if (Keyboard == null)
        {
            inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            InlineKeyboardButton.WithCallbackData("Атака", $"@attack_{0}_{enemy.EnemyId}_{DateTime.Now.Ticks}_Simple"),
            InlineKeyboardButton.WithCallbackData("Умение", $"@attack_{0}_{enemy.EnemyId}_{DateTime.Now.Ticks}_Skill")
            });
        }
        else inlineKeyboard = Keyboard;

        string levelText = "";
        if (enemy.Level >= 1 && enemy.Level <= 5)
            levelText = "1 - 5";
        else if (enemy.Level >= 6 && enemy.Level <= 10)
            levelText = "6 - 10";
        else if (enemy.Level >= 11 && enemy.Level <= 15)
            levelText = "11 - 15";

        var message = $"{enemy.Name}\n" +
            $"Класс: {enemy.ClassName}\n" +
            $"Здоровье: {enemy.HP} / {enemy.MaxHP}\n" +
            $"Атака: {enemy.ATK}\n" +
            $"Зашита: {enemy.Armor}\n" +
            $"Уровень: {enemy.Level}\n\n" +
            $"{enemy.Caption}\n\n" +
            $"Награда за победу: \n{enemy.EXP} Опыта\n{enemy.Gold} Золота\n" +
            $"Враг для левела: {levelText}"; ;

        var sentMessage = await BotServices.Instance.Bot.SendPhoto(
            chatId: m_mainChat,
            photo: enemy.Image,
            caption: message,
            replyMarkup: inlineKeyboard
        );


        enemy.MassageId = sentMessage.MessageId;
        EnemyPool[enemy.EnemyId] = enemy;

        await Task.Delay(delay, _cancellationToken);

        if (enemy.HP > 0)
        {
            var defeatCaption = $"{enemy.Name}\nЗдоровье: {enemy.HP} / {enemy.MaxHP}\n" +
                $"Атака: {enemy.ATK}\n" +
                $"Уровень: {enemy.Level}\n" +
                $"{enemy.Caption}\n\n" +
                $"Враг Вас уничтожил, поражение!";

            _ = BotServices.Instance.Bot.EditMessageCaption(
                        chatId: m_mainChat,
                        messageId: enemy.MassageId,
                        caption: defeatCaption
                    );

            Message msg = await BotServices.Instance.Bot.SendMessage(
                    m_mainChat,
                    defeatCaption
                );

            _ = MassageDeleter(msg, 60);
            enemy.PlayersID?.Clear();
            EnemyPool.Remove(enemy.EnemyId);
            _ = BotServices.Instance.Bot.DeleteMessage(
                chatId: m_mainChat,
                messageId: enemy.LastRemainder
            );
        }
        _ = MassageDeleter(sentMessage, 120);
    }
}