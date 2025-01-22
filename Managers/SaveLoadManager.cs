using Newtonsoft.Json;
using static GlobalData;
using static StringCollection;

public static class SaveLoadManager
{
    public static async Task<Dictionary<long, Character>> LoadCharacterDataAsync(string jsonFilePath)
    {
        if (!System.IO.File.Exists(jsonFilePath))
            return new Dictionary<long, Character>();

        var json = await System.IO.File.ReadAllTextAsync(jsonFilePath);

        if (string.IsNullOrWhiteSpace(json) || json == "[]")
            return new Dictionary<long, Character>();

        var characterData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<long, Character>>(json);

        foreach (var kvp in characterData!) 
            kvp.Value.SetClassString();


        return characterData ?? new Dictionary<long, Character>();

    }

    public static async Task LoadTopTierAsync()
    {
        var json = await System.IO.File.ReadAllTextAsync(jsonFilePathTierList);

        if (!System.IO.File.Exists(jsonFilePathTierList) || string.IsNullOrWhiteSpace(json))
            userTopTier = new List<Character>();
        else
        {
            List<Character>? characters = await Task.Run(() => System.Text.Json.JsonSerializer.Deserialize<List<Character>>(json));
            if (characters != null)
                userTopTier = characters;
        }
    }

    public static async Task SaveTopTierAsync()
    {
        var json = System.Text.Json.JsonSerializer.Serialize(userTopTier);
        await System.IO.File.WriteAllTextAsync(jsonFilePathTierList, json);
    }

    public static async Task SaveCharacterDataAsync(Dictionary<long, Character> data, string jsonFilePath)
    {
        var clonedData = new Dictionary<long, CharacterDTO>();
        foreach (var entry in data)
        {
            clonedData[entry.Key] = entry.Value.CloneWithoutBuffs();
        }

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(clonedData, new JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
        });

        await System.IO.File.WriteAllTextAsync(jsonFilePath, json);
    }

    public static async Task SaveEnemiesToJson(Enemy newEnemy, string jsonPath)
    {
        var enemies = new List<Enemy>();

        var json = await System.IO.File.ReadAllTextAsync(jsonPath);

        enemies = await Task.Run(() => System.Text.Json.JsonSerializer.Deserialize<List<Enemy>>(json) ?? new List<Enemy>());

        enemies.Add(newEnemy);

        var updatedJson = System.Text.Json.JsonSerializer.Serialize(enemies);

        await System.IO.File.WriteAllTextAsync(jsonPath, updatedJson);
        enemies = null;
        json = null;
    }

    public static async Task LoadEnemiesPrests()
    {
        var json = await System.IO.File.ReadAllTextAsync(jsonFilePresetEnemis);
        var json2 = await System.IO.File.ReadAllTextAsync(jsonFilePresetEnemisDangion);

        List<Enemy>? enemies = await Task.Run(() => System.Text.Json.JsonSerializer.Deserialize<List<Enemy>>(json));
        List<Enemy>? enemies2 = await Task.Run(() => System.Text.Json.JsonSerializer.Deserialize<List<Enemy>>(json2));

        if (enemies != null && enemies2 != null)
        {
            enemyPreset = enemies;
            dangionEnemyList = enemies2;
        }

    }

    public static async Task SaveUserStorageAsync()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(userStorage, new JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto
        });

        await System.IO.File.WriteAllTextAsync(jsonUserStorage, json);
    }

    public static async Task LoadUserStorageAsync()
    {
        if (!System.IO.File.Exists(jsonUserStorage))
            userStorage = new Dictionary<long, List<Item>>();

        var json = await System.IO.File.ReadAllTextAsync(jsonUserStorage);

        var userStorageHolder = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<long, List<Item>>>(json);
        userStorage = userStorageHolder!;

    }
}

