
public class ItemDropCollection
{
    private Dictionary<int, Item> _itemDropTable = new Dictionary<int, Item>
        {
            // Level 0 Items for stash
        { -2, new Item("������", "��������� � �������.", ItemType.ForSell, null, "�� �� ����� ��� ����������.", 0, 0, 0, 0, 0) },
        { -1, new Item("����� ���������", "�������� �����.", ItemType.ForSell, null, "�� �� ����� ��� ����������.", 0, 0, 0, 0, 0) },
        { 0, new Item("��������� ����", "�������� �������������.", ItemType.ForSell, null, "�� �� ����� ��� ����������.", 0, 0, 0, 0, 0) },
        // Level 1-5 Items
        { 1, new Item("��������� ��������", "��������, ���������� ��������� ����.", ItemType.Weapon, null, "�� ���������� ������ �������.", 30, 0, 5, 1, 0) },
        { 2, new Item("�������� �����", "������ �����, ��������� �� ����� � �����.", ItemType.Armor, null, "�� �������� � �������� �����.", 0, 50, 0, 0, 30) },
        { 3, new Item("�������� ������", "����������� �����, ��������� � ��������.", ItemType.Accessory, null, "�� ���������� ���� ���������.", 0, 30, 5, 3, 0) },
        // Level 6-10 Items
        { 6, new Item("����� �������", "������, ��������� ��������� ������������.", ItemType.Weapon, null, "�� ��������� ������ �������.", 70, 0, 10, 0, 25) },
        { 7, new Item("����� ��������� ���������������", "�����, ��������� �� �������� �������.", ItemType.Armor, null, "�� �������� � ����� ��������� ���������������.", 0, 180, 0, 0, 60) },
        { 8, new Item("������ ��������� �����", "����������� ����� � �������� ��������������", ItemType.Accessory, null, "�� ���������� ���� ���������.", 0, 0, 10, 3, 0) },
        // Level 11-15 Items
        { 11, new Item("��� ���������� �������", "���, ������� ������� ������ ��������� ������.", ItemType.Weapon, null, "�� ��������� ����� ���������� �������.", 220, 330, 0, 0, 30) },
        { 12, new Item("����� �������������������� ����", "�����, ��������� �� ����� � ����� �������.", ItemType.Armor, null, "�� �������� � ����� ��������� ����.", 0, 400, 5, 0, 100) },
        { 13, new Item("������ ����������� �������", "����������� ����� � �����.", ItemType.Accessory, null, "�� ���������� ���� ��������� � �������.", 80, 0, 10, 5, 0) },
        //// Additional Items for Levels 16-20
        //{ 16, new Item("����� ��������� ���������������", "�����, ������� ����� ��������� ������������ � �����.", ItemType.Weapon, null, "�� ��������� ������ ��������� ���������������.", 60, 0, 0f, 0, 0) },
        //{ 19, new Item("������ �������������� �����", "����������� ����� �� 7 � ���� ����� � ������������ �����.", ItemType.Accessory, null, "�� ���������� ���� ��������� � �������.", 0, 0, 0f, 7, 0) },
        //// Additional Items for Levels 21-25
        //{ 21, new Item("��� �������", "���, ������� ��������� ���� � ���� ������.", ItemType.Weapon, null, "�� ��������� ����� �������.", 75, 0, 0f, 0, 0) },
        //{ 22, new Item("����� ������������ ���������", "�����, ��������� �� ����������� �������.", ItemType.Armor, null, "�� �������� � ����� ������������ ���������.", 0, 200, 0f, 0, 0) },
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