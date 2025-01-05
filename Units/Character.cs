using Telegram.Bot;
using Telegram.Bot.Types;
using static GlobalData;
using static StringCollection;

public class Character
{
    private int _baseATK;
    private int _baseHP;
    private int _baseHPRegen = 100;
    private int _baseLUCK = 5;
    private int _baseArmor = 0;

    private List<Buff> _activeBuffs;
    public long? UserID { get; set; }
    public ClassType ClassType { get; set; }
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
    public int SP { get; set; } = 100;
    public int CurrentSP { get; set; } = 100;
    public List<string> Achievements { get; set; } = new List<string>();

    public Item?[] EquippedItems { get; set; } = new Item[3];
    public List<Item> Inventory { get; set; } = new List<Item>();

    public Skill? UniqueSkill { get; set; }

    public int REGEN_HP
    {
        get
        {
            return _baseHPRegen
                + GetActiveBuffsHPRegen()
                + GetEquipBonus(item => item.HPRegenBonus);
        }
    }
    public int ARMOR
    {
        get
        {
            return _baseArmor
                + GetActiveBuffs(buff => buff.ArmorModifier)
                + GetEquipBonus(item => item.ArmorBonus);
        }
    }
    public int ATK
    {
        get 
        { 
            return _baseATK 
                + GetActiveBuffs(buff => buff.ATKModifier) 
                + GetEquipBonus(item => item.AttackBonus); 
        }
        set 
        { 
            _baseATK = value 
                - GetActiveBuffs(buff => buff.ATKModifier) 
                - GetEquipBonus(item => item.AttackBonus); 
        }
    }
    
    public int HP
    {
        get 
        { 
            return _baseHP 
                + GetActiveBuffs(buff => buff.HPModifier) 
                + GetEquipBonus(item => item.HealthBonus); 
        }
        set 
        { 
            _baseHP = value 
                - GetActiveBuffs(buff => buff.HPModifier) 
                - GetEquipBonus(item => item.HealthBonus); 
        }
    }

    public int BaseATK
    {
        get { return _baseATK; }
        set { _baseATK = value; }
    }

    public int BaseHP
    {
        get { return _baseHP; }
        set { _baseHP = value; }
    }
    public int BaseHPRegen
    {
        get { return _baseHPRegen; }
        set { _baseHPRegen = value; }
    }
    public int BaseLUCK
    {
        get { return _baseLUCK; }
        set { _baseLUCK = value; }
    }
    public int BaseArmor
    {
        get { return _baseArmor; }
        set { _baseArmor = value; }
    }

    public Character()
    {
        _activeBuffs = new List<Buff>();
        CurrentSP = SP;
        SetClassString();
    }

    public void SetClassString()
    {
        long Id = UserID ?? 0;

        switch (ClassType)
        {
            case ClassType.Solar_Flare:
                Class = $"Шаман Солнечного Огня ☼\n";
                UniqueSkill = new Skill("Воздаяние", "Высвобождает выброс солнечной энергии нанося урон врагу и заживляя раны союзников.", Id, initCol.Solar_FlareInit, null, null, 0);
                break;
            case ClassType.Dust:
                Class = $"Песчаный странник ⊕\n";
                UniqueSkill = new Skill("Песчаная буря", "Создает песчаную бурю, которая снижает точность противника.", Id, initCol.DustInit, TakeDamage.DodgeEffect, null, 5);
                break;
            case ClassType.Æther:
                Class = $"Эфирный чародей ⚷\n";
                UniqueSkill = new Skill("Всплеск духа", "Наполняет духовной силой союзников и создает вокруг Вас барьер.", Id, initCol.SpiritInit, TakeDamage.SpiritShieldEffect, null, 1);
                break;
            case ClassType.Infinity:
                Class = $"Призыватель Бездны ⊗\n";
                UniqueSkill = new Skill("Коррозия плоти", "Пагубно влияет на живительные силы врага и его способность сопротивляться.", Id, initCol.VoidInit, TakeDamage.CorruptionEffect, null, 5);
                break;
            case ClassType.Nexus:
                Class = $"Квантовый ткач ☿\n";
                UniqueSkill = new Skill("Собирательная сила", "Ваш следующий удар наполнен коллективной силой Ваших союзников.", Id, initCol.MonadaInit, null, null, 0);
                break;
            case ClassType.Deprived:
                Class = $"Лишенный ∅\n";
                UniqueSkill = null;
                break;
        }
    }



    public CharacterDTO CloneWithoutBuffs()
    {
        return new CharacterDTO
        {
            UserID = this.UserID,
            Name = this.Name,
            Gender = this.Gender,
            Rank = this.Rank,
            Class = this.Class,
            About = this.About,
            Level = this.Level,
            EXP = this.EXP,
            EXPToNextLevel = this.EXPToNextLevel,
            Image = this.Image,
            Gold = this.Gold,
            CurrentHP = this.CurrentHP,
            BaseATK = this.BaseATK,
            BaseHP = this.BaseHP,
            BaseHPRegen = this.BaseHPRegen,
            BaseLUCK = this.BaseLUCK,
            BaseArmor = this.BaseArmor,
            StoryProgression = this.StoryProgression,
            Achievements = new List<string>(this.Achievements),
            ClassType = this.ClassType,
            EquippedItems = this.EquippedItems,
            Inventory = this.Inventory,

        };

    }

    public int GetBaseATK() => _baseATK;
    public int GetBaseHP() => _baseHP;

    public async Task<int> GetLUCKAsync(int num, string image, long id, Random random)
    {
        int chance = random.Next(1, 101);
        int addluck = GetEquipBonus(item => item.LuckBonus) + GetActiveBuffs(buff => buff.LuckModifier);
        if (chance <= (_baseLUCK + addluck))
        {
            int modifiedNum = num + (int)(num * ((_baseLUCK + addluck) / 100.0));

            Message msg = await BotServices.Instance.Bot.SendPhoto(
                chatId: id,
                photo: image,
                caption: $"Герой {Name} сегодня испытал удачу на {modifiedNum}."
                );

            _ = MassageDeleter(msg, 20);

            return modifiedNum;
        }
        return num; 
    }

    public async Task<(int exp, int gold)> GetLUCKAsync(int[] nums, string image, long id, Random random)
    {
        int chance = random.Next(1, 101);
        int exp = nums[0];
        int gold = nums[1];
        int addluck = GetEquipBonus(item => item.LuckBonus);

        if (chance <= (_baseLUCK + addluck))
        {
            int[] modifiedNums = nums.Select(num => num + (int)(num * ((_baseLUCK + addluck) / 100))).ToArray();
            string Caption = $"Герой {Name} сегодня испытал удачу на: золота {modifiedNums[1]} и опыта {modifiedNums[0]}.";

            Message msg = await BotServices.Instance.Bot.SendPhoto(
                chatId: id,
                photo: image,
                caption: Caption
                );

            _ = MassageDeleter(msg, 20);
            exp = modifiedNums[0];
            gold = modifiedNums[1];
            return (exp, gold);

        }
        return (exp, gold);
    }

    public void AddItemToInventory(Item item) => Inventory.Add(item);

    public void ApplyBuff(Buff buff)
    {
        buff.StartTime = DateTime.Now;

        var existingBuffIndex = _activeBuffs.FindIndex(b => b.Name == buff.Name);

        if (existingBuffIndex != -1)
        {
            _activeBuffs[existingBuffIndex].IsReplaced = true;
            _activeBuffs[existingBuffIndex] = buff;
        }
        else
            _activeBuffs.Add(buff);

    }

    public List<Buff> GetActiveBuffs()
    {
        _activeBuffs.RemoveAll(buff => !buff.IsActive());
        return _activeBuffs;
    }

    private int GetActiveBuffs(Func<Buff, int> selector)
    {
        int totalModifier = 0;
        _activeBuffs.RemoveAll(buff => !buff.IsActive());
        foreach (var buff in _activeBuffs)
        {
            totalModifier += selector(buff);
        }
        return totalModifier;
    }

    private int GetActiveBuffsHPRegen()
    {
        int totalHPRegenModifier = 0;
        foreach (var buff in _activeBuffs)
        {
            if (buff.IsActive())
                totalHPRegenModifier += buff.HPRegenModifier;
            else _activeBuffs.Remove(buff);

        }
        return totalHPRegenModifier;
    }

    private int GetEquipBonus(Func<Item, int> selector)
    {
        int totalBonus = 0;
        foreach (Item? item in EquippedItems)
        {
            if (item != null)
            {
                totalBonus += selector(item);
            }
        }
        return totalBonus;
    }

    public bool EquipItem(string itemName, Message msg)
    {
        Item? itemToEquip = Inventory.FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        if (itemToEquip == null)
            return false;

        switch (itemToEquip.Type)
        {
            case ItemType.Weapon:
                if (EquippedItems[0] != null)
                    Inventory.Add(EquippedItems[0]!);
                EquippedItems[0] = itemToEquip;
                break;
            case ItemType.Armor:
                if (EquippedItems[1] != null)
                    Inventory.Add(EquippedItems[1]!);
                EquippedItems[1] = itemToEquip;
                break;
            case ItemType.Accessory:
                if (EquippedItems[2] != null)
                    Inventory.Add(EquippedItems[2]!);
                EquippedItems[2] = itemToEquip;
                break;
            case ItemType.Potion:
                _ = UsePotion(itemName, msg);
                break;
            default:
                return false;
        }
        Inventory.Remove(itemToEquip);
        return true;
    }

    public bool SellItem(string itemName, int salePrice)
    {
        Item? itemToSell = Inventory.FirstOrDefault(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (itemToSell == null)
            return false;

        Gold += salePrice;
        Inventory.Remove(itemToSell);
        return true;
    }

    public async Task UsePotion(string potionName, Message msg)
    {

        Item? potionToUse = Inventory
            .FirstOrDefault(item => item.Name.Equals(potionName, StringComparison.OrdinalIgnoreCase));

        if (potionToUse == null) return;

        if (potionToUse.HPRestor.HasValue || potionToUse.SPRestor.HasValue)
        {
            if(potionToUse.HPRestor != 0)
                CurrentHP = Math.Min(CurrentHP + potionToUse.HPRestor!.Value, HP);
            if (potionToUse.SPRestor != 0)
                CurrentSP = Math.Min(CurrentSP + potionToUse.SPRestor!.Value, SP);

            var Message = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, potionToUse.MessageOnUse);
            _ = MassageDeleter(Message, 30);
        } 
        else
        {
            potionToUse.Buff!.StartTime = DateTime.Now;
            ApplyBuff(potionToUse.Buff);

            _ = Program.BuffsHandler(msg.Chat.Id, msg.MessageThreadId, potionToUse.Buff);
            var Message = await BotServices.Instance.Bot.SendMessage(msg.Chat.Id, potionToUse.MessageOnUse);
            _ = MassageDeleter(Message, 30);
        }
    }

    public bool UnequipItemByName(string? itemName)
    {
        for (int i = 0; i < EquippedItems.Length; i++)
        {
            if (EquippedItems[i] != null && EquippedItems[i]!.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            {
                Item? itemToUnequip = EquippedItems[i];
                EquippedItems[i] = null;
                Inventory.Add(itemToUnequip!);
                return true;
            }
        }
        return false;
    }
    public Task<bool> UnequipItemByType(string? itemName)
    {
            if (!Enum.TryParse(itemName, true, out ItemType itemType))
                return Task.FromResult(false);

            int index;
        switch (itemType)
        {
            case ItemType.Weapon:
                index = 0;
                break;
            case ItemType.Armor:
                index = 1;
                break;
            case ItemType.Accessory:
                index = 2;
                break;
            default:
                index = - 1;
                break;
        }

        if (index < 0 || index >= EquippedItems.Length)
                return Task.FromResult(false);

            if (EquippedItems[index] != null)
            {
                Item? itemToUnequip = EquippedItems[index];
                if (itemToUnequip == null) return Task.FromResult(false);
                EquippedItems[index] = null;
                Inventory.Add(itemToUnequip);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
    }

    public Task<string> GetInventoryItems()
    {
        var sortedInventory = Inventory.OrderBy(item => item.Type).ToList();
        var result = string.Join("\n", sortedInventory.Select(item => $"{emojiMapItems[item.Type]} {item.Name}"));
        return Task.FromResult(result);
    }
    public Task<string> GetEquippedItems() => Task.FromResult(string.Join("\n", EquippedItems.Select(item => item?.Name ?? "Пусто")));
    public string GetEquippedItemsSimple() => string.Join("\n", EquippedItems.Select(item => item?.Name ?? "Пусто"));

    public Task RegenerateHealthAsync()
    {
        int healthToRegain = _baseHPRegen + GetActiveBuffsHPRegen() + GetEquipBonus(item => item.HPRegenBonus);
        if (healthToRegain < 1)
            Console.WriteLine($"Name: {Name} - healthToRegain: {healthToRegain}");

        CurrentSP = Math.Min(CurrentSP + 1, SP);
        CurrentHP = Math.Min(CurrentHP + healthToRegain, HP);
        return Task.CompletedTask;
    }

    public async Task GetItemAsync(Item item, long id, Random random)
    {
        int chance = random.Next(1, 121);
        int addluck = GetEquipBonus(item => item.LuckBonus);
        if (chance <= (_baseLUCK + addluck))
        {
            Inventory.Add(item);

            Message msg = await BotServices.Instance.Bot.SendMessage(
                chatId: id,
                text: $"Герой {Name} обнаружил предмет {item.Name}."
                );

            _ = MassageDeleter(msg, 20);

        }

    }
}


public enum ClassType
{
    Solar_Flare,
    Dust,
    Æther,
    Infinity,
    Nexus,
    Deprived,
}
