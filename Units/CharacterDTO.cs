public class CharacterDTO
{
    public int BaseATK { get; set; }
    public int BaseHP { get; set; }
    public int BaseHPRegen { get; set; }
    public int BaseLUCK { get; set; }
    public int BaseArmor { get; set; }
    public long? UserID { get; set; }
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public string? Rank { get; set; }
    public string? Class { get; set; }
    public string? About { get; set; }
    public int Level { get; set; }
    public int EXP { get; set; }
    public int EXPToNextLevel { get; set; }
    public string? Image { get; set; }
    public int Gold { get; set; }
    public int CurrentHP { get; set; }
    public int StoryProgression { get; set; }
    public List<string> Achievements { get; set; } = new List<string>();
    public ClassType ClassType { get; set; }
    public Item?[] EquippedItems { get; set; } = new Item[3];
    public List<Item> Inventory { get; set; } = new List<Item>();
    public Occupation OccupationPlayer { get; set; } = new Occupation();

}