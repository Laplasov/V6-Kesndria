
public class ItemDropCollection
{
    private Dictionary<int, Item> _itemDropTable = new Dictionary<int, Item>
        {
            // Level 0 Items for stash
        { -2, new Item("Плазма", "Светиться в темноте.", ItemType.ForSell, null, "Вы не нашли ему применение.", 0, 0, 0, 0, 0) },
        { -1, new Item("Кусок метеорита", "Радужный блеск.", ItemType.ForSell, null, "Вы не нашли ему применение.", 0, 0, 0, 0, 0) },
        { 0, new Item("Бездарная душа", "Излучает недопонимание.", ItemType.ForSell, null, "Вы не нашли ему применение.", 0, 0, 0, 0, 0) },
        // Level 1-5 Items
        { 1, new Item("Солнечный Кристалл", "Кристалл, излучающий солнечный свет.", ItemType.Weapon, null, "Вы чувствуете прилив энергии.", 30, 0, 5, 1, 0) },
        { 2, new Item("Песчаная Броня", "Легкая броня, сделанная из песка и магии.", ItemType.Armor, null, "Вы облачены в песчаную броню.", 0, 50, 0, 0, 30) },
        { 3, new Item("Звездная Серьга", "Увеличивает удачу, связанная с космосом.", ItemType.Accessory, null, "Вы чувствуете себя удачливее.", 0, 30, 5, 3, 0) },
        // Level 6-10 Items
        { 6, new Item("Копье Пустоты", "Оружие, способное разрывать пространство.", ItemType.Weapon, null, "Вы вооружены копьем Пустоты.", 70, 0, 10, 0, 25) },
        { 7, new Item("Броня Звездного Путешественника", "Броня, сделанная из звездной материи.", ItemType.Armor, null, "Вы облачены в броню звездного путешественника.", 0, 180, 0, 0, 60) },
        { 8, new Item("Серьга Песчаного Ветра", "Увеличивает удачу и скорость восстановления", ItemType.Accessory, null, "Вы чувствуете себя удачливее.", 0, 0, 10, 3, 0) },
        // Level 11-15 Items
        { 11, new Item("Меч Солнечного Пламени", "Меч, который сжигает врагов солнечным светом.", ItemType.Weapon, null, "Вы вооружены мечом солнечного пламени.", 220, 330, 0, 0, 30) },
        { 12, new Item("Броня Межпространственного Духа", "Броня, сделанная из песка и магии пустыни.", ItemType.Armor, null, "Вы облачены в броню песчаного духа.", 0, 400, 5, 0, 100) },
        { 13, new Item("Серьга Астрального Пламени", "Увеличивает атаку и удачу.", ItemType.Accessory, null, "Вы чувствуете себя удачливее и сильнее.", 80, 0, 10, 5, 0) },
        //// Additional Items for Levels 16-20
        //{ 16, new Item("Копье Звездного Путешественника", "Копье, которое может разрывать пространство и время.", ItemType.Weapon, null, "Вы вооружены копьем звездного путешественника.", 60, 0, 0f, 0, 0) },
        //{ 19, new Item("Серьга Геометрической Удачи", "Увеличивает удачу на 7 и дает бонус к критическому урону.", ItemType.Accessory, null, "Вы чувствуете себя удачливее и сильнее.", 0, 0, 0f, 7, 0) },
        //// Additional Items for Levels 21-25
        //{ 21, new Item("Меч Пустоты", "Меч, который поглощает свет и душу врагов.", ItemType.Weapon, null, "Вы вооружены мечом Пустоты.", 75, 0, 0f, 0, 0) },
        //{ 22, new Item("Броня Космического Странника", "Броня, сделанная из космической материи.", ItemType.Armor, null, "Вы облачены в броню космического странника.", 0, 200, 0f, 0, 0) },
        };
    private static List<Item> minerItems = new List<Item>
            {
                new Item("Сияющий Рудник", "Редкая руда, сверкающая загадочным светом.", ItemType.Ore, null, "Вы можете продать эту руду за хорошую цену.", 0, 0, 0, 0, 0),
                new Item("Кровавый Камень", "Глубокий красный камень, излучающий тепло.", ItemType.Ore, null, "Вы можете продать этот камень за хорошую цену.", 0, 0, 0, 0, 0),
                new Item("Слеза Отздарвы", "Напоминает черную каплю, издает загадочные скрежет.", ItemType.Ore, null, "Вы можете продать этот камень за хорошую цену.", 0, 0, 0, 0, 0),
                new Item("Мох Голгофы", "Камень переливает зеленым и вызывает головокружение.", ItemType.Ore, null, "Вы можете продать этот камень за хорошую цену.", 0, 0, 0, 0, 0),
                new Item("Восковая память", "Белый мягкий металл, обладает магнитными свойствами.", ItemType.Ore, null, "Вы можете продать этот камень за хорошую цену.", 0, 0, 0, 0, 0),
            };
    private static List<Item> herbalistItems = new List<Item>
            {
                new Item("Мистическая Трава", "Редкая трава, известная своими целебными свойствами.", ItemType.Herb, null, "Используется для создания зелья, увеличивающего физическую силу.", 0, 0, 0, 0, 0),
                new Item("Лунный Цветок", "Цветок, который распускается только в полнолуние, излучая мягкий свет.", ItemType.Herb, null, "Используется для создания зелья, увеличивающего магическую силу.", 0, 0, 0, 0, 0),
                new Item("Трава Снов", "Трава, которая вызывает яркие сны, когда ее нюхают.", ItemType.Herb, null, "Используется для создания зелья, увеличивающего защиту.", 0, 0, 0, 0, 0),
                new Item("Кровавый Лист", "Лист, который кажется пропитанным кровью, но обладает целебными свойствами.", ItemType.Herb, null, "Используется для создания зелья, восстанавливающего здоровье.", 0, 0, 0, 0, 0),
                new Item("Звездная Пыльца", "Пыльца, собранная с цветков, растущих под звездами, придающая силе.", ItemType.Herb, null, "Используется для создания зелья, увеличивающего удачу.", 0, 0, 0, 0, 0),
            };
    private static List<Item> scavengerItems = new List<Item>
            {
                new Item("Сушильный шест", "Очень длинный меч, использовался учениками одной из древних школ боевых искусств.", ItemType.Weapon, null, "Вы можете использовать этот реликт как оружие.", 15, 0, 0, 0, 0),
                new Item("Изношенный Кинжал", "Простой кинжал, который видел лучшие времена, но все еще острый.", ItemType.Weapon, null, "Вы готовы к быстрой атаке с Изношенным Кинжалом.", 20, 0, 0, 0, 0),
                new Item("Броня Забытого", "Легкая броня, сделанная из собранных материалов, обеспечивающая хорошую защиту.", ItemType.Armor, null, "Вы облачены в Броню Забытого, чувствуя себя более защищенным.", 0, 0, 0, 0, 25),
           };


    public Item CreateRandomItem(int enemyLevel, Random random)
    {
        var availableItems = _itemDropTable
            .Where(minLevel => minLevel.Key < enemyLevel) 
            .Select(minLevel => minLevel.Value)
            .ToList();

        int randomIndex = random.Next(availableItems.Count);
        return availableItems[randomIndex];

    }

    public static string CreateRewardItem(Character player, int times)
    {
        List<Item> items = new List<Item>();
        var random = new Random();
        string holder = "";
        switch (player.OccupationPlayer.CurrentJob)
        {
            case JobType.Miner:
                items = minerItems;
                break;
            case JobType.Herbalist:
                items = herbalistItems;
                break;
            case JobType.Scavenger:
                items = scavengerItems;
                break;
            default:
                return "Нет предметов для сбора.";
        }
        for (int i = 0; i < times; i++)
        {
            var item = items[random.Next(items.Count)];
            item.Quality = 0;
            player.AddItemToInventory(item);
            holder += $"{item.Name}\n";
        }
        return holder;
    }
}