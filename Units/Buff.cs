
public class Buff
{
    public required string Name { get; set; }
    public int ATKModifier { get; set; }
    public int HPModifier { get; set; }
    public int ArmorModifier { get; set; } = 0;
    public int LuckModifier { get; set; } = 0;
    public int HPRegenModifier { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public string? Message { get; set; }
    public bool IsReplaced { get; set; } = false;

    public bool IsActive()
    {
        return DateTime.Now - StartTime < Duration;
    }
}