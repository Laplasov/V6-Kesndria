using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static GlobalData;

public class CharacterTrade
{
    async public Task GiveGold(Wrapper wrapper)
    {
        if (!wrapper.IsValid() || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
            tradeListID[wrapper.UserId] = 0;

        string text = "������� ID ������������, �������� �� ������ �������� ������� ��� ������.";

        Message msgNew = await BotServices.Instance.Bot.SendMessage(
            chatId: wrapper.ChatId,
            text: text,
            messageThreadId: wrapper.MessageThreadId
        );

        _ = MassageDeleter(msgNew, 30);

        if (wrapper.Type == UpdateType.Message)
            _ = MassageDeleter(wrapper.OriginalMessage, 30);
        if (wrapper.Type == UpdateType.CallbackQuery)
            if (wrapper.CallbackQueryId != null) 
                _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);
    }
    async public Task WaitingForRecipient(Message msg, UpdateType type)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));
        long playerId = -1;

        try
        {
            playerId = long.Parse(msg.Text);
            if (!userCharacterData.ContainsKey(playerId))
            {
                MassageManager.SetNextCommand(msg.From.Id, MassageManager.EmptyCommand);
                Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                    chatId: msg.Chat.Id,
                    text: "�������� �� ������! ����������, ������� ���������� ID.",
                    messageThreadId: msg.MessageThreadId
                );
                tradeListID.Remove(msg.From.Id);
                _ = MassageDeleter(msgNew2, 30);
                _ = MassageDeleter(msg, 30);
                return;
            }
        }
        catch (FormatException)
        {
            MassageManager.SetNextCommand(msg.From.Id, MassageManager.EmptyCommand);
            Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "����������, ������� ���������� ID.",
                messageThreadId: msg.MessageThreadId
            );
            tradeListID.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew2, 30);
            _ = MassageDeleter(msg, 30);
            return;
        }
        tradeListID[msg.From.Id] = playerId;
        Message msgNew3 = await BotServices.Instance.Bot.SendMessage(
            chatId: msg.Chat.Id,
            text: "������� ������ ��� ������� �� ������ ��������.",
            messageThreadId: msg.MessageThreadId
        );
        _ = MassageDeleter(msgNew3, 30);
        _ = MassageDeleter(msg, 30);
    }
    async public Task WaitingForAmount(Message msg, UpdateType type)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));


        var playerFrom = userCharacterData[msg.From.Id];
        long playerToId;

        if (!tradeListID.TryGetValue(msg.From.Id, out playerToId))
        {
            Message msgNew2 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "������: ���������� �� ������.",
                messageThreadId: msg.MessageThreadId
            );
            tradeListID.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew2, 30);
            return;
        }
        var playerTo = userCharacterData[playerToId];

        if (!int.TryParse(msg.Text, out int amount))
        {
            await WaitingForItem(msg, playerFrom, playerTo);
            return;
        }
        

        if (playerFrom.Gold < amount && amount > 0)
        {
            Message msgNew3 = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "������������� ��� ���������� ���������� ������!",
                messageThreadId: msg.MessageThreadId
                );
            tradeListID.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew3, 30);
            return;

        }

        playerFrom.Gold -= amount;
        playerTo.Gold += amount;

        await BotServices.Instance.Bot.SendMessage(
            chatId: msg.Chat.Id,
            text: $"{playerFrom.Name} ������� �������(�) {amount} ������ {playerTo.Name}.",
            messageThreadId: msg.MessageThreadId
        );

        tradeListID.Remove(msg.From.Id);

    }

    async private Task WaitingForItem(Message msg, Character playerFrom, Character playerTo)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));


        var itemToTransfer = playerFrom.Inventory.FirstOrDefault(item => item.Name.Equals(msg.Text, StringComparison.OrdinalIgnoreCase));
        if (itemToTransfer == null)
        {
            Message msgNew = await BotServices.Instance.Bot.SendMessage(
                chatId: msg.Chat.Id,
                text: "����������, ������� ���������� ���������� ������ ��� �������� ��������. ",
                messageThreadId: msg.MessageThreadId
            );
            tradeListID.Remove(msg.From.Id);
            _ = MassageDeleter(msgNew, 30);
            return;
        }

        playerFrom.Inventory.Remove(itemToTransfer);
        playerTo.Inventory.Add(itemToTransfer);

        await BotServices.Instance.Bot.SendMessage(
            chatId: msg.Chat.Id,
            text: $"{playerFrom.Name} ������� �������(�) ������� '{itemToTransfer.Name}' ������ {playerTo.Name}.",
            messageThreadId: msg.MessageThreadId
        );
        tradeListID.Remove(msg.From.Id);
        return;
    }
}

