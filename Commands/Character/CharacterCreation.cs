using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using static GlobalData;
using static StringCollection;
public class CharacterCreation
{
    async public Task CreatClass(Message msg, UpdateType type)
    {
        if (msg.From == null) throw new ArgumentNullException(nameof(msg));
        if (userCharacterData.ContainsKey(msg.From.Id))
            return;

        userCharacterData[msg.From.Id] = new Character();
        userCharacterData[msg.From.Id].Name = $"{msg.From.FirstName} {msg.From.LastName}";
        userCharacterData[msg.From.Id].Rank = characterRanks[1];
        userCharacterData[msg.From.Id].Level = 0;
        userCharacterData[msg.From.Id].Image = m_holderImage;
        userCharacterData[msg.From.Id].UserID = msg.From.Id;

        Message msgNew1 = await BotServices.Instance.Bot.SendPhoto(chatId: msg.Chat.Id,
                photo: userCharacterData[msg.From.Id].Image!,
                caption:
                $"Поздравляю, {userCharacterData[msg.From.Id].Name}! \n" +
                $"Вы теперь {userCharacterData[msg.From.Id].Rank} \n" +
                $"Теперь ваш уровень {userCharacterData[msg.From.Id].Level} \n",
                messageThreadId: m_creatCharTopic
                );

        Message msgNew2 = await BotServices.Instance.Bot.SendPhoto(
            chatId: msg.Chat.Id,
            photo: m_classImage,
            caption: $"{msg.From.FirstName} \nВыбери пол (Мужской/Женский).",
            messageThreadId: m_creatCharTopic
        );
        _ = MassageDeleter(msgNew1, 30);
        _ = MassageDeleter(msgNew2, 30);
        _ = MassageDeleter(msg, 30);
    }
    public async Task HandleGenderSelection(Message msg, UpdateType type)
    {
        if (msg.From == null) throw new ArgumentNullException(nameof(msg));
        var character = userCharacterData[msg.From.Id];
        if (msg.Text != "Мужской" && msg.Text != "Женский")
        {
            MassageManager.nextCommand.Remove(msg.From.Id);
            userCharacterData.Remove(msg.From.Id);
            Message msgNew1 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, "Извините, но гендера только два. Начните заново", messageThreadId: m_creatCharTopic);
            _ = MassageDeleter(msgNew1, 30);
            return;
        }
        character.Gender = msg.Text;
        Message msgNew2 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, 
            $"{character.Name} \nТеперь выбери класс: \n" +
            $"Шаман Солнечного Огня ☼\n" +
            $"Песчаный странник ⊕\n" +
            $"Эфирный чародей ⚷\n" +
            $"Призыватель Бездны ⊗\n" +
            $"Квантовый ткач ☿\n", 
            messageThreadId: m_creatCharTopic);
        _ = MassageDeleter(msgNew2, 30);
    }

    public async Task HandleClassSelection(Message msg, UpdateType type)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));
        var character = userCharacterData[msg.From.Id];

        var validClasses = new Dictionary<string, ClassType>
    {
        { "Шаман Солнечного Огня", ClassType.Solar_Flare }, 
        { "Песчаный странник", ClassType.Dust },
        { "Эфирный чародей", ClassType.Æther }, 
        { "Призыватель Бездны", ClassType.Infinity}, 
        { "Квантовый ткач", ClassType.Nexus } 
    };
        if (!validClasses.ContainsKey(msg.Text))
        {
            MassageManager.nextCommand.Remove(msg.From.Id);
            userCharacterData.Remove(msg.From.Id);
            Message msgNew2 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, "Выбрана неизведанная сущность. Начните заново", messageThreadId: m_creatCharTopic);
            _ = MassageDeleter(msgNew2, 30);
            return;
        }
        character.ClassType = validClasses[msg.Text];
        character.SetClassString();
        Message msgNew1 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, $"{character.Name} \nТеперь расскажите о себе:", messageThreadId: m_creatCharTopic);
        _ = MassageDeleter(msgNew1, 30);
    }
    public async Task HandleCharacterDetails(Message msg, UpdateType type)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));
        var character = userCharacterData[msg.From.Id];

        character.About = msg.Text;
        character.Level = 1;
        character.EXP = 0;
        character.EXPToNextLevel = 160;
        character.ATK = 50;
        character.HP = 120;
        character.Gold = 0;
        character.CurrentHP = 120;
        character.StoryProgression = 0;

        if (character.Gender == "Мужской")
            character.Image = MalePhotos[1];
        if (character.Gender == "Женский")
            character.Image = FemalePhotos[1];

        List<string> achievements = AchievementsFinder(msg);

        character.Achievements = achievements;
        string achievementsString = achievements.Count > 0 ? string.Join("\n", achievements) : "";

        Message msgNew2 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, $"{Awaking}",
            messageThreadId: m_creatCharTopic);

        Message msgNew3 = await BotServices.Instance.Bot.SendPhoto(
            chatId: msg.Chat.Id,
            photo: character.Image!,
            caption:
            $"{character.Name} \n" +
            $"Персонаж создан:\n" +
            $"⚜️ Ваш ранг: {character.Rank} \n" +
            $"🎭 Пол: {character.Gender}\n" +
            $"🔰 Класс: {character.Class}\n" +
            $"🔎 О себе: {character.About}\n\n" +
            $"🧠 Левел: {character.Level}\n" +
            $"✨ Опыт: {character.EXP}/{character.EXPToNextLevel}\n" +
            $"⚔️ Атака: {character.ATK}\n" +
            $"🪖 Зашита: {character.ARMOR}\n" +
            $"☄️ Восстановление: {character.REGEN_HP}\n" +
            $"❤️ Здоровье: {character.HP}/{character.HP}\n" +
            $"💰 Золото: {character.Gold}\n" +
            $"📚 Уровень сюжета: {character.StoryProgression}\n\n" +
            $"🏆 Достижения:\n {achievementsString}",
            messageThreadId: m_creatCharTopic
            );

        await SaveLoadManager.SaveCharacterDataAsync(userCharacterData, jsonFilePath);

        userCharacterData = await SaveLoadManager.LoadCharacterDataAsync(jsonFilePath);
        Message msgNew4 = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id,
            "ТЫ ГОТОВ ОТПРАВИТЬСЯ В ПУТЕШЕСТВИЕ?",
            replyMarkup: m_inlineKeyboardAct1YesAndNo,
            messageThreadId: m_creatCharTopic);
        _ = MassageDeleter(msgNew2, 180);
        _ = MassageDeleter(msgNew3, 180);
        _ = MassageDeleter(msgNew4, 180);
        _ = MassageDeleter(msg, 30);
        
    }

    public async Task RefreshName(Wrapper wrapper)
    {
        if (wrapper.MessageThreadId != m_creatCharTopic || !userCharacterData.ContainsKey(wrapper.UserId))
            return;
        string newName = $"{wrapper.FirstName} {wrapper.LastName}";
        string oldName = userCharacterData[wrapper.UserId].Name!;

        Message msgNew1 = await BotServices.Instance.Bot.SendMessage(
            wrapper.ChatId,
            $"Старое имя:\n{oldName}\nНовое имя:\n{newName}",
            messageThreadId: m_creatCharTopic
        );

        userCharacterData[wrapper.UserId].Name = newName;
        _ = MassageDeleter(msgNew1, 180);
        if (wrapper.CallbackQuery != null)
        {
            if (!userMessgaeMap.TryGetValue(wrapper.MessageId, out long originalUserId))
                return;
            if (wrapper.UserId != originalUserId)
            return;
        }
        if (wrapper.Type == UpdateType.Message)  
            _ = MassageDeleter(wrapper.OriginalMessage, 180);
        if (wrapper.CallbackQueryId != null)
            _ = BotServices.Instance.Bot.AnswerCallbackQuery(wrapper.CallbackQueryId);

    }
    public static List<string> AchievementsFinder(Message msg)
    {
        if (msg.From == null || msg.Text == null) throw new ArgumentNullException(nameof(msg));
        var Achievements = new List<string>();

        if (JsonOldDocument == null)
            throw new InvalidOperationException("JSON data has not been loaded.");

        string userId = msg.From.Id.ToString();
        if (JsonOldDocument.RootElement.TryGetProperty(userId, out JsonElement characterElement))
        {
            if (characterElement.TryGetProperty("Level", out JsonElement levelElement))
            {
                int level = levelElement.GetInt32();
                if (level > 10) Achievements.Add("✨ Траилблейзер.");
                if (level > 5) Achievements.Add("🚩 Пионер.");
                if (level > 0) Achievements.Add("🛡️ В первой сотне.");

                return Achievements;
            }
        }
        else
        {
            return Achievements;
        }

        return Achievements;
    }
}