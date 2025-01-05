using static GlobalData;

public class HealthUpdateService : IHealthUpdateService
{
    private readonly CancellationToken _cancellationToken;

    public HealthUpdateService(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public async Task HealthUpdate()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), _cancellationToken);
                var characterEntries = userCharacterData.ToList();

                var regenerationTasks = characterEntries.Select(characterEntry =>
                    characterEntry.Value.RegenerateHealthAsync()).ToList();

                _ = Task.WhenAll(regenerationTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HealthUpdate: {ex.Message}");

            }
        }
    }
}