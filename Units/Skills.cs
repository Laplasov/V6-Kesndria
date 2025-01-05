using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;
using static StringCollection;

public class Skill
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public long UserID { get; set; }
    public Func<Character, Enemy, long, Random, Task> InitSkill { get; set; }
    public OnTakeDamageEffectDelegate? OnTakeDamageEffect { get; set; }
    public OnDamagingEffectDelegate? OnDamagingEffect { get; set; }
    public Skill(
        string name,
        string description,
        long userID,
        Func<Character, Enemy, long, Random, Task> initSkill,
        OnTakeDamageEffectDelegate onTakeDamageEffect,
        OnDamagingEffectDelegate onDamagingEffect,
        int duration
        )
    {
        Name = name;
        Description = description;
        UserID = userID;
        InitSkill = initSkill;
        OnTakeDamageEffect = onTakeDamageEffect;
        OnDamagingEffect = onDamagingEffect;
        Duration = duration;
    }

}


public class InitSkillCollection 
{
    public async Task Solar_FlareInit(Character player, Enemy enemy, long id, Random random)
    {
        if (!await enemy.PlayerSPHandler(player, id)) 
            return;

        float CurrentPercentageEffect = enemy.ClassValue(player);

        int CurrentDamagePlayer = (int)(player.ATK + (player.ATK * CurrentPercentageEffect)) - enemy.Armor;
        int halveDamage = (CurrentDamagePlayer / 10);

        foreach (var playerId in enemy.PlayersID)
        {
            Character character = userCharacterData[playerId];
            character.CurrentHP = Math.Min(character.CurrentHP + halveDamage, character.HP);
        }
        int damage = Math.Max(CurrentDamagePlayer + halveDamage, 0);

        enemy.HP -= damage;

        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name} использует навык `{player.UniqueSkill!.Name}`! \nНаносит {enemy.Name} {damage} урона и восстанавливает {damage/10} команде."
        );
        _ = MassageDeleter(msg, 20);
    }

    public async Task DustInit(Character player, Enemy enemy, long id, Random random)
    {
        if (!await enemy.PlayerSPHandler(player, id))
            return;
        int duration;
        var existingSkill = enemy.ActiveSkills.FirstOrDefault(skill => skill.Name == player.UniqueSkill!.Name);
        if (existingSkill != null)
        {
            existingSkill.Duration += player.UniqueSkill!.Duration;
            duration = existingSkill.Duration;
        }
        else
        {
            var newSkill = new Skill(
            player.UniqueSkill!.Name,
            player.UniqueSkill.Description,
            player.UniqueSkill.UserID,
            player.UniqueSkill.InitSkill,
            player.UniqueSkill!.OnTakeDamageEffect!,
            player.UniqueSkill!.OnDamagingEffect!,
            player.UniqueSkill!.Duration
        );
            enemy.ActiveSkills.Add(newSkill);
            duration = newSkill.Duration;
        }

        player.Gold += enemy.Level;

        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name} использует навык `{player.UniqueSkill!.Name}` на {enemy.Name}!" +
            $"\nПолучено 30% шанса уклонения команды и {enemy.Level} золота." +
            $"\nПродолжительность способности: {duration}"
        );
        _ = MassageDeleter(msg, 20);
    }
    public async Task SpiritInit(Character player, Enemy enemy, long id, Random random)
    {
        if (!await enemy.PlayerSPHandler(player, id))
            return;
        int duration;
        var existingSkill = enemy.ActiveSkills.FirstOrDefault(skill => skill.Name == player.UniqueSkill!.Name);
        if (existingSkill != null)
        {
            existingSkill.Duration += player.UniqueSkill!.Duration;
            duration = existingSkill.Duration;
        }
        else
        {
            var newSkill = new Skill(
            player.UniqueSkill!.Name,
            player.UniqueSkill.Description,
            player.UniqueSkill.UserID,
            player.UniqueSkill.InitSkill,
            player.UniqueSkill!.OnTakeDamageEffect!,
            player.UniqueSkill!.OnDamagingEffect!,
            player.UniqueSkill!.Duration
        );
            enemy.ActiveSkills.Add(newSkill);
            duration = newSkill.Duration;
        }

        foreach (var playerId in enemy.PlayersID)
        {
            Character character = userCharacterData[playerId];
            character.CurrentSP = Math.Min(character.CurrentSP + 30, character.SP);
        }


        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name} использует навык `{player.UniqueSkill!.Name}` на {enemy.Name}!" +
            $"\n50% урона будет отражено и восстановлено 30% энергии союзникам." +
            $"\nПродолжительность способности: {duration}"
        );
        _ = MassageDeleter(msg, 20);
    }

    public async Task VoidInit(Character player, Enemy enemy, long id, Random random)
    {
       
        if (!await enemy.PlayerSPHandler(player, id))
        return;
        int duration;

        var existingSkill = enemy.ActiveSkills.FirstOrDefault(skill => skill.Name == player.UniqueSkill!.Name);
        if (existingSkill != null)
            {
                existingSkill.Duration += player.UniqueSkill!.Duration;
                duration = existingSkill.Duration;
            }
        else
        {
            var newSkill = new Skill(
            player.UniqueSkill!.Name,
            player.UniqueSkill.Description,
            player.UniqueSkill.UserID,
            player.UniqueSkill.InitSkill,
            player.UniqueSkill!.OnTakeDamageEffect!,
            player.UniqueSkill!.OnDamagingEffect!,
            player.UniqueSkill!.Duration
        );
            enemy.ActiveSkills.Add(newSkill);
            duration = newSkill.Duration;
        }

        float CurrentPercentageEffect = enemy.ClassValue(player);

        int CurrentDamagePlayer = (int)(player.ATK + (player.ATK * CurrentPercentageEffect)) - enemy.Armor;

        enemy.HP -= Math.Max(CurrentDamagePlayer, 0);

        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name} использует навык `{player.UniqueSkill!.Name}`! " +
            $"\nНаносит {enemy.Name} {CurrentDamagePlayer} урона и ослабляет врага на 20%." +
            $"\nПродолжительность способности: {duration}"
        );
            _ = MassageDeleter(msg, 20); 

    }

    public async Task MonadaInit(Character player, Enemy enemy, long id, Random random)
    {
        if (!await enemy.PlayerSPHandler(player, id))
            return;

        float CurrentPercentageEffect = enemy.ClassValue(player);

        int CurrentDamagePlayer = (int)(player.ATK + (player.ATK * CurrentPercentageEffect)) - enemy.Armor;
        int CommonDamage = 0;
        foreach (var playerId in enemy.PlayersID)
            CommonDamage += userCharacterData[playerId].ATK / 10;

        enemy.HP -= Math.Max(CurrentDamagePlayer + CommonDamage, 0);

        Message msg = await BotServices.Instance.Bot.SendMessage(
            chatId: id,
            text: $"{player.Name} использует навык `{player.UniqueSkill!.Name}`! \nНаносит {enemy.Name} {CurrentDamagePlayer} урона."
        );
        _ = MassageDeleter(msg, 20);
    }
}
public class OnTakeDamageEffectCollection
{
    public (string message, int currentDamageEnemy) DodgeEffect(Character player, Enemy enemy, int Damage, Random random)
    {
        if (Damage <= 0)
            return ("", Damage);

        var dodgeSkill = enemy.ActiveSkills.FirstOrDefault(skill => skill.OnTakeDamageEffect == DodgeEffect);
        string message = "";
        int currentDamageEnemy;

        int chance = random.Next(0, 100);

        dodgeSkill!.Duration--;
        if (chance <= 30)
        {
            currentDamageEnemy = 0;
            message = $"\n\n{player.Name} уклонился от атаки {enemy.Name}!\nОсталось уворотов: {dodgeSkill!.Duration}";
        }
        else
        {
            currentDamageEnemy = Damage;
            message = $"\n\n{player.Name} не удалось уклонился от атаки {enemy.Name}!\nОсталось уворотов: {dodgeSkill!.Duration}";
        }

        if (dodgeSkill!.Duration <= 0)
            enemy.ActiveSkills.Remove(dodgeSkill);

        return (message, currentDamageEnemy);
    }
    public (string message, int currentDamageEnemy) SpiritShieldEffect(Character player, Enemy enemy, int Damage, Random random)
    {
        var spiritShieldEffect = enemy.ActiveSkills.FirstOrDefault(skill => skill.OnTakeDamageEffect == SpiritShieldEffect);

        if (spiritShieldEffect?.UserID != player.UserID || Damage <= 0)
            return ("", Damage);

        Damage /= 2;

        spiritShieldEffect!.Duration--;
        var (message, currentDamageEnemy) = (
            $"\n\n{player.Name} отразил(а) 50% атаки {enemy.Name}!\nОсталось зашиты: {spiritShieldEffect!.Duration}",
            Damage
            );


        if (spiritShieldEffect!.Duration <= 0)
            enemy.ActiveSkills.Remove(spiritShieldEffect);

        return (message, currentDamageEnemy);
    }

    public (string message, int currentDamageEnemy) CorruptionEffect(Character player, Enemy enemy, int Damage, Random random)
    {
        if (Damage <= 0)
            return ("", Damage);

        var сorruptionEffect = enemy.ActiveSkills.FirstOrDefault(skill => skill.OnTakeDamageEffect == CorruptionEffect);
        Damage -= Damage / 4;
        сorruptionEffect!.Duration--;

        var (message, currentDamageEnemy) =
            (
            $"\n\n{player.Name} получил(а) на 20% меньше урона от {enemy.Name}!" +
            $"\nОсталось ослаблений: {сorruptionEffect!.Duration}", 
            Damage
            );

        if (сorruptionEffect!.Duration <= 0)
            enemy.ActiveSkills.Remove(сorruptionEffect);

        return (message, currentDamageEnemy);
    }
}

public class OnDamagingEffectCollection
{

}