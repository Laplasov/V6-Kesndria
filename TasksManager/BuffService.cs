using Telegram.Bot;
using Telegram.Bot.Types;
using static GlobalData;
public class BuffService : IBuffService
{
	private readonly CancellationToken _cancellationToken;

	public BuffService(CancellationToken cancellationToken)
	{
		_cancellationToken = cancellationToken;
	}

	public async Task DisbuffAfterDelay(long chatId, int? messageThreadId, Buff healthReductionBuff)
    {
        await Task.Delay(healthReductionBuff.Duration, _cancellationToken);
        if (!healthReductionBuff.IsReplaced)
        {
            var msg = await BotServices.Instance.Bot.SendMessage(
                chatId,
                healthReductionBuff.Message!,
                messageThreadId: messageThreadId
            );

            _ = MassageDeleter(msg, 30);
        }
        
    }
}