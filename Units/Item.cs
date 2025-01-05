
public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Accessory,
    ForSell
}
public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }
    public Buff? Buff { get; set; }
    public string MessageOnUse { get; set; }
    public int AttackBonus { get; set; }
    public int HealthBonus { get; set; }
    public int HPRegenBonus { get; set; }
    public int LuckBonus { get; set; } = 0;
    public int ArmorBonus { get; set; } = 0;
    public int? HPRestor { get; set; } = 0;
    public int? SPRestor { get; set; } = 0;

    public Item(string name, string description, ItemType type, Buff? buff, string messageOnUse, int attackBonus, int healthBonus, int hpRegenBonus, int luckBonus, int armorBonus)
    {
        Name = name;
        Description = description;
        Type = type;
        Buff = buff;
        MessageOnUse = messageOnUse;
        AttackBonus = attackBonus;
        HealthBonus = healthBonus;
        HPRegenBonus = hpRegenBonus;
        LuckBonus = luckBonus;
        ArmorBonus = armorBonus;
    }
}