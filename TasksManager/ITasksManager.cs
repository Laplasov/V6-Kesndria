    public interface IEnemyReminderService
    {
        Task CheckEnemyPoolReminders();
    }

    public interface IHealthUpdateService
    {
        Task HealthUpdate();
    }

    public interface IEnemySpawnService
    {
        Task EnemySpawn(int delayBetween, int delayDefeat);
    }

    public interface IStashService
    {
        Task StashSpawn(int delayBetween, int delay);
    }

    public interface IBuffService
    {
        Task DisbuffAfterDelay(long chatId, int? messageThreadId, Buff healthReductionBuff);
    }

    public interface IDungeonService
    {
        Task DungeonSpawn(int delayBetween);
    }
