using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;
public static class CharacterManager
{
    public static async Task AddCharacterToTopTier(Character newCharacter)
    {
        userTopTier.RemoveAll(c => c.Name.Equals(newCharacter.Name, StringComparison.OrdinalIgnoreCase));

        int index = userTopTier.FindIndex(c => c.Level < newCharacter.Level);

        if (index == -1)
            userTopTier.Add(newCharacter);

        else if (index == 0)
            userTopTier.Insert(0, newCharacter);

        else
        {
            int sameLevelStartIndex = index - 1;

            while (sameLevelStartIndex >= 0 && userTopTier[sameLevelStartIndex].Level == newCharacter.Level)
                sameLevelStartIndex--;

            userTopTier.Insert(sameLevelStartIndex + 1 + userTopTier.Count(c => c.Level == newCharacter.Level), newCharacter);
        }
        if (userTopTier.Count > 20)
            userTopTier.RemoveAt(userTopTier.Count - 1);

        await SaveLoadManager.SaveTopTierAsync();
        await SaveLoadManager.LoadTopTierAsync();
    }

    public static async Task UpdateEXP(Character character, Message msg)
    {
        if (character.Level == 0 || character.Level >= MAX_LEVEL || originalHPs.ContainsKey(msg.From.Id))
            return;

        Message msgNew;
        character.EXP++;

        if (character.EXP >= character.EXPToNextLevel)
        {
            character.Level++;
            character.EXP = 0;

            if (character.Level <= 5)
                character.EXPToNextLevel += 40;
            else if (character.Level == 6)
                character.EXPToNextLevel = 720;
            else if (character.Level > 6 && character.Level <= 10)
                character.EXPToNextLevel += 80;
            else if (character.Level == 11)
                character.EXPToNextLevel = 2240;
            else if (character.Level > 11)
                character.EXPToNextLevel += 160;

            if (character.Level <= 5)
                character.BaseATK += 10;
            else if (character.Level > 5 && character.Level <= 10)
                character.BaseATK += 30;
            else if (character.Level > 10)
                character.BaseATK += 90;

            if (character.Level <= 5)
                character.BaseHP += 20;
            else if (character.Level > 5 && character.Level <= 10)
                character.BaseHP += 50;
            else if (character.Level > 10)
                character.BaseHP += 125;

            character.CurrentHP = character.HP;

            if (character.Level < characterRanks.Length)
                character.Rank = characterRanks[character.Level];

            if (character.Gender == "Мужской")
            {
                if (character.Level < MalePhotos.Length)
                    character.Image = MalePhotos[character.Level];
            }
            if (character.Gender == "Женский")
            {
                if (character.Level < FemalePhotos.Length)
                    character.Image = FemalePhotos[character.Level];
            }

            if (character.Level != 1)
            {
                msgNew = await BotServices.Instance.Bot.SendPhoto(chatId: msg.Chat.Id,
                    photo: character.Image!,
                    caption:
                    $"Поздравляю, {character.Name}! \n" +
                    $"Вы теперь {character.Rank} \n" +
                    $"Теперь ваш уровень {character.Level} \n" +
                    $"Напишите \"Играть\" что бы узнать подробности.\n"
                    );
                await SaveLoadManager.SaveCharacterDataAsync(userCharacterData, jsonFilePath);
                userCharacterData = await SaveLoadManager.LoadCharacterDataAsync(jsonFilePath);

                _ = MassageDeleter(msgNew, 120);
            }
            _ = AddCharacterToTopTier(character);

        }
    }

}