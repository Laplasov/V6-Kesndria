using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using static GlobalData;
using static StringCollection;
public class EnemyReminderService : IEnemyReminderService 
{
    private readonly CancellationToken _cancellationToken;

    public EnemyReminderService(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public async Task CheckEnemyPoolReminders()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(3), _cancellationToken);
            if (EnemyPool.Count > 0)
            {

                foreach (var enemyEntry in EnemyPool)
                {
                    Enemy enemy = enemyEntry.Value;

                    TimeSpan timeElapsed = DateTime.UtcNow - enemyTimers[enemy.EnemyId];
                    string timeElapsedText = $"{(int)timeElapsed.TotalMinutes:D2}:{timeElapsed.Seconds:D2}";

                    string reminderMessage = $"{enemy.Name} еще жив(а)! \n" +
                        $"Здоровье: {enemy.HP} / {enemy.MaxHP}\n" +
                        $"Прошло времени в минутах: {timeElapsedText}";

                    try
                    {
                        if (enemy.LastRemainder != 0)
                        {
                            await BotServices.Instance.Bot.DeleteMessage(
                                chatId: m_mainChat,
                                messageId: enemy.LastRemainder
                            );
                        }

                        if (enemy.HP > 0) 
                        {
                            var sentMessage = await BotServices.Instance.Bot.SendMessage(
                                chatId: m_mainChat,
                                text: reminderMessage,
                                replyParameters: enemy.MassageId,
                                parseMode: ParseMode.Markdown
                            );

                            enemy.LastRemainder = sentMessage.MessageId; 
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"EnemyPool, reminder not send: {ex.Message}");
                    }

                }

            }
        }
    }
}
