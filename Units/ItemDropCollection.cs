
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


    public Item CreateRandomItem(int enemyLevel, Random random)
    {
        var availableItems = _itemDropTable
        .Where(minLevel => minLevel.Key < enemyLevel) 
        .Select(minLevel => minLevel.Value)
        .ToList();

        int randomIndex = random.Next(availableItems.Count);
        return availableItems[randomIndex];

    }
}