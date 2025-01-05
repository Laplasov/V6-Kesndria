using Telegram.Bot;

public class BotServices
{
    private static BotServices? _instance;
    public TelegramBotClient Bot { get; }
    public MassageManager MassageManager { get; private set; }
    public UpdateManager UpdateManager { get; private set; }
    public ReminderCoordinator ReminderCoordinator { get; private set; }
    public BuffService BuffService { get; private set; }
    public EnemySpawnService EnemySpawnService { get; private set; }
    public StashService StashService { get; private set; }

    private BotServices(string token, CancellationToken cancellationToken)
    {
        Bot = new TelegramBotClient(token, cancellationToken: cancellationToken);
        MassageManager = new MassageManager();
        UpdateManager = new UpdateManager();
        BuffService = new BuffService(cancellationToken);
        EnemySpawnService = new EnemySpawnService(cancellationToken);
        StashService = new StashService(cancellationToken);
        ReminderCoordinator = new ReminderCoordinator(cancellationToken);

    }

    public static void Initialize(string token, CancellationToken cancellationToken)
    {
        if (_instance == null)
        {
            _instance = new BotServices(token, cancellationToken);
        }
    }

    public static BotServices Instance => _instance ?? throw new InvalidOperationException("BotServices not initialized. Call Initialize first.");
}