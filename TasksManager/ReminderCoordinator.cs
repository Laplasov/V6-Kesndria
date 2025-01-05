
using System.Threading;

public class ReminderCoordinator
{
    private readonly EnemyReminderService _enemyReminderService;
    private readonly HealthUpdateService _healthUpdateService;
    private readonly EnemySpawnService _enemySpawnService;
    private readonly StashService _stashService;
    private readonly DungeonService _dungeonService;
    private readonly CancellationToken _cancellationToken;

    public ReminderCoordinator(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        _enemyReminderService = new EnemyReminderService(cancellationToken);
        _healthUpdateService = new HealthUpdateService(cancellationToken);
        _enemySpawnService = new EnemySpawnService(cancellationToken);
        _stashService = new StashService(cancellationToken);
        _dungeonService = new DungeonService(cancellationToken);

    }

    public async Task StartReminders()
    {

        _ = Task.Run(() => _enemyReminderService.CheckEnemyPoolReminders(), _cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        Console.WriteLine($"CheckEnemyPoolReminders");

        _ = Task.Run(() => _healthUpdateService.HealthUpdate(), _cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        Console.WriteLine($"HealthUpdate");

        _ = Task.Run(() => _enemySpawnService.EnemySpawn(30, 30), _cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        Console.WriteLine($"EnemySpawn");

        _ = Task.Run(() => _stashService.StashSpawn(3, 3), _cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        Console.WriteLine($"StashSpawn");

        _ = Task.Run(() => _dungeonService.DungeonSpawn(15), _cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), _cancellationToken);
        Console.WriteLine($"DungeonSpawn");

    }
}
