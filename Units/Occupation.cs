using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Occupation
{
    public JobType CurrentJob { get; set; }

    public DateTime Duration { get; set; }

    public int Hours { get; set; }

    public Occupation()
    {
        CurrentJob = JobType.Free; 
        Duration = default;
        Hours = 0;
    }

    public string JobCompleted(Character player)
    {
        string completion = "";
        var random = new Random();
        int times = (Hours / 2);

        switch (CurrentJob)
        {
            case JobType.Guard:
                var salary = (5 * player.Level) * times;
                player.Gold += salary;
                completion = $"Работа закончена.\n" +
                    $"Вы зарабатываете {salary} золота!";
                break;

            case JobType.Study:
                var exp = (5 * player.Level) * times;
                player.EXP += exp;
                completion = $"Работа закончена.\n" +
                    $"Вы зарабатываете {exp} опыта!";
                break;
            case JobType.Miner:
                var itemsMiner = ItemDropCollection.CreateRewardItem(player, times);
                completion = $"Работа закончена. Вы добыли: \n{itemsMiner}";
                break;
            case JobType.Herbalist:
                var itemsHerbalist = ItemDropCollection.CreateRewardItem(player, times);
                completion = $"Работа закончена. Вы собрали: \n{itemsHerbalist}";
                break;
            case JobType.Scavenger:
                var itemsScavenger = ItemDropCollection.CreateRewardItem(player, times);
                completion = $"Работа закончена. Вы нашли: \n{itemsScavenger}";
                break;
        } 
        player.OccupationPlayer.CurrentJob = JobType.Free;
        return completion;
    }
}
public enum JobType
{
    Free,
    Guard,
    Herbalist,
    Scavenger,
    Miner,
    Study,
}