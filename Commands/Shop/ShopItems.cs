
using Telegram.Bot.Types;
using Telegram.Bot;
using static GlobalData;

public interface IShopItem
{
    string Name { get; }
    string Discription { get; }
    int Cost { get; }
    Task<bool> Purchase(Character player, Message msg);
}

public class HealthPotionInstant : IShopItem
{
    public string Name => "Зелье восстановления здоровья";
    public string Discription => "Восстанавливает 400 здоровье.";
    public int Cost => 100;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 30);
            return false;
        }

        player.Gold -= Cost;
        player.CurrentHP = Math.Min(player.CurrentHP + 400, player.HP);

        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text:
            $"Вы купили {Name}. \n400 здоровья было восстановлено.\n" +
            $"Текущее здоровье: {player.CurrentHP} / {player.HP}\n" +
            $"Остаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 30);
        return true;
    }
}
public class SPPotionInstant : IShopItem
{
    public string Name => "Зелье восстановления ДС";
    public string Discription => "Восстанавливает 30 ДС.";
    public int Cost => 100;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;
        player.CurrentSP = Math.Min(player.CurrentSP + 30, player.SP);

        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text:
            $"Вы купили {Name}. \n30 духовной силы было восстановлено.\n" +
            $"Текущая духовная сила: {player.CurrentSP} / {player.SP}\n" +
            $"Остаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;

    }

}
public class HealthPotion : IShopItem
{
    public string Name => "Зелье восстановления здоровья";
    public string Discription => "Восстанавливает 400 здоровье.";
    public int Cost => 100;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        Item healthPotion = new Item(Name, Discription, ItemType.Potion, null, "Здоровье восстановлено.", 0, 0, 0, 0, 0);
        healthPotion.HPRestor = 400;
        player.AddItemToInventory(healthPotion);

        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}
public class SPPotion : IShopItem
{
    public string Name => "Зелье восстановления ДС";
    public string Discription => "Восстанавливает 30 ДС.";
    public int Cost => 100;
    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        Item SPPotion = new Item(Name, Discription, ItemType.Potion, null, "Духовная сила восстановлена.", 0, 0, 0, 0, 0);
        SPPotion.SPRestor = 30;
        player.AddItemToInventory(SPPotion);

        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class AttackPotion : IShopItem
{
    public string Name => "Зелье атаки";
    public string Discription => "Усиление атаки на х1.5 на 15 минут.";
    public int Cost => 150;
    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        Buff attackBuff = new Buff
        {
            Name = "Усиление атаки",
            ATKModifier = (int)(player.GetBaseATK() / 2),
            HPModifier = 0,
            HPRegenModifier = 0,
            Duration = TimeSpan.FromMinutes(15),
            StartTime = DateTime.Now,
            Message = $"{player.Name}, Ваше усиление атаки на х1.5 закончилось."
        };

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Potion, attackBuff, "Атака была усилена на х1.5.", 0, 0, 0, 0, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class ArmorPotion : IShopItem
{
    public string Name => "Зелье зашиты";
    public string Discription => "Усиление зашиты на 50 на 15 минут.";
    public int Cost => 150;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        Buff attackBuff = new Buff
        {
            Name = "Усиление зашиты",
            ATKModifier = 0,
            HPModifier = 0,
            HPRegenModifier = 0,
            Duration = TimeSpan.FromMinutes(15),
            StartTime = DateTime.Now,
            Message = $"{player.Name}, Ваше усиление зашиты закончилось.",
            ArmorModifier = 50
        };

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Potion, attackBuff, "Зашита была усилена на 50.", 0, 0, 0, 0, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class WeaponShiv : IShopItem
{
    public string Name => "Жало мрака";
    public string Discription => "Вырвано из пасти змеи. Добавляет +30 к атаке.";
    public int Cost => 200;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Weapon, null, "У Вас у руках жало мрака (атака +30).", 30, 0, 0, 0, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class ArmorVeil : IShopItem
{
    public string Name => "Вуаль";
    public string Discription => "Сплетена из заблудившихся теней. Добавляет +100 к максимальному здоровью.";
    public int Cost => 200;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Armor, null, "Вы чувствуете, как вуаль наполняет Вас легкостью (+100 к максимальному здоровью). ", 0, 100, 0, 0, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class AccessoryEarring : IShopItem
{
    public string Name => "Серьга ветра";
    public string Discription => "Говорят в ней заточена фея ветра. Добавляет %10 к восстановлению здоровья в минуту.";
    public int Cost => 200;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Accessory, null, "Вас окружает зеленая аура (% 10 к восстановлению здоровья в минуту). ", 0, 0, 10, 0, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}

public class AccessoryCatcher : IShopItem
{
    public string Name => "Ловец снов";
    public string Discription => "Этот голубой глаз наполняет Вас уверенностью. Удача повышена.";
    public int Cost => 50;

    public async Task<bool> Purchase(Character player, Message msg)
    {
        if (player.Gold < Cost)
        {
            var msgNew2 = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: "Недостаточно золота!", messageThreadId: msg.MessageThreadId);
            _ = MassageDeleter(msgNew2, 10);
            return false;
        }

        player.Gold -= Cost;

        player.AddItemToInventory(new Item(Name, Discription, ItemType.Accessory, null, "Вы чувствуете, что вы более удачливый сегодня. ", 0, 0, 0, 3, 0));
        var msgNew = await BotServices.Instance.Bot.SendMessage(chatId: msg.Chat.Id, text: $"Вы купили: {Name}.\nОстаток золота {player.Gold}.", messageThreadId: msg.MessageThreadId);
        _ = MassageDeleter(msgNew, 10);
        return true;
    }
}
