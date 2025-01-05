using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static GlobalData;
using static StringCollection;
public class CharacterDeletion 
{
    async public Task Delete(Message msg, UpdateType type)
    {
        if (msg.From == null) throw new ArgumentNullException(nameof(msg.From));
        if (!userCharacterData.ContainsKey(msg.From.Id)){
            Message msg1 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, "� ��� ��� ���������.", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msg1, 30);
        }
        else
        {
            userCharacterData.Remove(msg.From.Id);
            await SaveLoadManager.SaveCharacterDataAsync(userCharacterData, jsonFilePath);
            userCharacterData = await SaveLoadManager.LoadCharacterDataAsync(jsonFilePath);
            Message msg1 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, "��� �������� ��� ������� �����.", messageThreadId: msg.MessageThreadId);

            _ = MassageDeleter(msg, 30);
            _ = MassageDeleter(msg1, 30);
        }
    }

    async public Task GetGold(Message msg, UpdateType type)
    {
        if (msg.From == null) throw new ArgumentNullException(nameof(msg.From));
        if (userCharacterData.ContainsKey(msg.From.Id))
        {
            var player = userCharacterData[msg.From.Id];
            player.Gold += 4500;

            Message msg1 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, $"��� �������� {player.Name} ������� 4500 ������!!!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msg, 30);
            _ = MassageDeleter(msg1, 30);
        }
    }
}