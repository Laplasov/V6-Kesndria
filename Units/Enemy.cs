using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using System;
using static StringCollection;
using System.Security.Claims;
using System.Numerics;

public class Enemy
{
    public int EnemyId { get; set; }
    public int Level { get; set; }
    public int MassageId { get; set; }
    public string Name { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int Armor { get; set; }
    public string Image { get; set; }
    public List<long> PlayersID { get; set; }
    public int EXP { get; set; }
    public int Gold { get; set; }
    public string Caption { get; set; }
    public int LastRemainder { get; set; }
    public ClassType? ClassTypeEnum { get; set; }
    public string ClassName { get; set; }

    public List<Skill> ActiveSkills { get; set; } = new List<Skill>();
    private List<Skill> clonedSkills { get; set; } = new List<Skill>();

    public Enemy() { }

    public Enemy(int enemyId, int level, string name, int maxHP, int atk, string imageID, int exp, int gold, string caption, int armor)
    {
        EnemyId = enemyId;
        Level = level;
        Name = name;
        MaxHP = maxHP;
        HP = maxHP;
        ATK = atk;
        Image = imageID;
        PlayersID = new List<long>();
        EXP = exp;
        Gold = gold;
        Caption = caption;
        Armor = armor;
    }

    async public Task<bool> CalculateDamage(Character player, long id, Random random)
    {
        string AddString = "";
        float CurrentPercentageEffect = ClassValue(player);

        int CurrentDamageEnemy = (int)(ATK + (ATK * -CurrentPercentageEffect)) - player.ARMOR;

        clonedSkills = new List<Skill>(ActiveSkills);

        foreach (var skill in clonedSkills)
            if (skill.OnTakeDamageEffect != null)
            {
                var (message, currentDamage) = skill.OnTakeDamageEffect.Invoke(player, this, CurrentDamageEnemy, random);
                CurrentDamageEnemy = currentDamage;
                AddString += message;
            }

        player.CurrentHP -= Math.Max(CurrentDamageEnemy, 0);

        if (player.CurrentHP <= 0)
            return false;

        int CurrentDamagePlayer = (int)(player.ATK + (player.ATK * CurrentPercentageEffect)) - Armor;

        int crit = await player.GetLUCKAsync(CurrentDamagePlayer, luckEnemyAttackImage, id, random);

        //foreach (var skill in clonedSkills)
        //    if (skill.OnDamagingEffect != null)
        //        await skill.OnDamagingEffect.Invoke(player, this, id, random);

        HP -= Math.Max(crit, 0);

        string messageBase = $"Герой {player.Name} \nКласс: {player.Class}\n" +
            $"Противник {Name} \nКласс: {ClassName}\n\n" +
            $"Разница класса {(int)(CurrentPercentageEffect * 100)}%\n" +
            $"Нанесено урона - {crit}\n" +
            $"Получено урона - {CurrentDamageEnemy}\n\n" +
            $"Ваше здоровье: \n{player.CurrentHP} / {player.HP}";


        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: messageBase + AddString);

        _ = MassageDeleter(msg, 10);
        return true;
    }

    public void GetClassStringEnemy()
    {
        switch (ClassTypeEnum)
        {
            case ClassType.Solar_Flare:
                ClassName = $"Шаман Солнечного Огня ☼";
                break;
            case ClassType.Dust:
                ClassName = $"Песчаный странник ⊕";
                break;
            case ClassType.Æther:
                ClassName = $"Эфирный чародей ⚷";
                break;
            case ClassType.Infinity:
                ClassName = $"Призыватель Бездны ⊗";
                break;
            case ClassType.Nexus:
                ClassName = $"Квантовый ткач ☿";
                break;
            case ClassType.Deprived:
                ClassName = $"Лишенный ∅";
                break;
        }
    }

    public float ClassValue(Character player)
    {
        if ((int)player.ClassType == 5)
            return -0.25f;
        else
        {
            int diff = ((int)ClassTypeEnum! - (int)player.ClassType + 5) % 5;
            if (diff > 2) diff -= 5;
            return diff / 4.0f;
        }
    }

    async public Task<bool> PlayerSPHandler(Character player, long id)
    {
        if (player.CurrentSP < 50)
        {
            Message msg1 = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name}, Вам не хватает энергии что бы использовать `{player.UniqueSkill!.Name}`!"
        );
            _ = MassageDeleter(msg1, 20);
            return false;
        }
        player.CurrentSP -= 50;
        return true;
    }
}
