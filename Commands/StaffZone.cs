using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using static StringCollection;

public class StaffZone
{
    async public Task MassageSender(Message msg, UpdateType type)
    {
        if (msg.Type == MessageType.Photo)
        {
            string? photo = msg?.Photo?[0].FileId;
            if (photo == null) throw new ArgumentNullException(nameof(photo));
            if (msg?.Caption == null) throw new ArgumentNullException(nameof(msg.Caption));

            if (msg.MessageThreadId == m_toGeneral)
            {
                await BotServices.Instance.Bot.SendPhoto(
                chatId: m_mainChat,
                photo: photo
                );
                await BotServices.Instance.Bot.SendMessage(m_mainChat, msg.Caption);
            }
            if (msg.MessageThreadId == m_toGameWorld)
            {
                await BotServices.Instance.Bot.SendPhoto(
                chatId: m_mainChat,
                photo: photo,
                messageThreadId: m_creatCharTopic
                );

                await BotServices.Instance.Bot.SendMessage(m_mainChat, msg.Caption, messageThreadId: m_creatCharTopic);
            }
        }
        else if (msg.Type == MessageType.Text)
        {
            if (msg.Text == null) throw new ArgumentNullException(nameof(msg.Text));
            if (msg.MessageThreadId == m_toGeneral)
                await BotServices.Instance.Bot.SendMessage(m_mainChat, msg.Text);

            if (msg.MessageThreadId == m_toGameWorld)
                await BotServices.Instance.Bot.SendMessage(m_mainChat, msg.Text, messageThreadId: m_creatCharTopic);
        }
    }
    async public Task PhotoSaver(Message msg, UpdateType type)
    {
        string? photo = msg?.Photo?[0].FileId;
        if (photo == null) throw new ArgumentNullException(nameof(photo));
        if (msg == null) throw new ArgumentNullException(nameof(msg));

        await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, photo, messageThreadId: msg.MessageThreadId);
    }

    public async Task SendEnemy(Message msg, UpdateType type)
    {
        if (msg.Type == MessageType.Photo)
        {
            if (msg.Caption == null) throw new ArgumentNullException(nameof(msg.Caption));

            EnemyData? enemyData = await Task.Run(() => JsonSerializer.Deserialize<EnemyData>(msg.Caption));

            if (enemyData == null) throw new ArgumentNullException(nameof(enemyData));
            if (msg?.Photo?[0] == null) throw new ArgumentNullException(nameof(msg.Photo));

            string enemyName = enemyData.Name;
            int level = enemyData.Level;
            int hp = enemyData.HP;
            int atk = enemyData.ATK;
            var enemyID = msg.MessageId;
            int exp = enemyData.EXP;
            int gold = enemyData.Gold;
            string caption = enemyData.Caption;
            int armor = enemyData.Armor;

            Enemy enemy = new Enemy(
                enemyID,
                level,
                enemyName,
                hp,
                atk,
                msg.Photo[0].FileId,
                exp,
                gold,
                caption,
                armor
            );
            enemy.MassageId = msg.MessageId;

            await SaveLoadManager.SaveEnemiesToJson(enemy, jsonFilePresetEnemisDangion);

            //var inlineKeyboard = new InlineKeyboardMarkup(new[]
            //{
            //InlineKeyboardButton.WithCallbackData("Attack", $"attack_{msg.From.Id}_{enemyID}")
            //});
            //var message = $"{enemy.Name}\nЗдоровье: {enemy.HP} / {enemy.MaxHP}\n" +
            //    $"Атака: {enemy.ATK}\n" +
            //    $"Уровень: {enemy.Level}\n\n" +
            //    $"{enemy.Caption}\n\n" +
            //    $"Награда за победу: {enemy.EXP} опыта и {enemy.Gold} золота!";

            //var sentMessage = await BotServices.Instance.Bot.SendPhoto(
            //    chatId: m_mainChat,
            //    photo: enemy.Image,
            //    caption: message,
            //    replyMarkup: inlineKeyboard
            //);

            //enemy.MassageId = sentMessage.MessageId;
            //EnemyPool[enemyID] = enemy;

            //for saving Enemis -
            // to dangion - jsonFilePresetEnemisDangion
            // to regular - jsonFilePresetEnemis
            //enemy.MassageId = msg.MessageId;

        }
    }
}
//    public async Task RefreshStats(Message msg, UpdateType type)
//    {
//        foreach (Character player in userCharacterData.Values)
//        {
//            if (player.HP == 5120)
//            {
//                player.HP = 2760;
//                player.ATK = 1124;
//                player.Level = 11;
//                player.EXPToNextLevel = 1200;
//            }
//        }
//    }
//    public async Task RefreshPhotos(Message msg, UpdateType type)
//    {
//        foreach (Character character in userCharacterData.Values)
//        {
//            if (character.Gender == "Мужской")
//            {
//                if (character.Level < MalePhotos.Length)
//                    character.Image = MalePhotos[character.Level];
//            }
//            if (character.Gender == "Женский")
//            {
//                if (character.Level < FemalePhotos.Length)
//                    character.Image = FemalePhotos[character.Level];
//            }
//        }
//    }
//}

public class EnemyData
{
    public required string Name { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int EXP { get; set; }
    public int Gold { get; set; }
    public int Armor { get; set; }
    public required string Caption { get; set; }
}