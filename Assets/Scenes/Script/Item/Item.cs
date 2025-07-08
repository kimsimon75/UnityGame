using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
public enum ItemRank
{
    All,
    Common,
    Uncommon,
    Special,
    Rare,
    Legendary,
    Hidden,
    Changed,
    UpperRanked
}

public class Item : IComparable<Item>
{
    public string Name { get;private set; }
    public ItemIngredient[] NecessaryItem { get;private set; }
    public ItemRank Rank { get;private set; }
    public byte Id;

    public Sprite Resource;
    public int count = 0;

    public int AttackPower { get; private set; }
    public int NeutralizeDefense { get;private set; }
    public int MagicalBuffer { get;private set; }
    public int MagicalDebuffer { get;private set; }
    public int TrueDamage { get;private set; }
    public float HealthRegen { get;private set; }
    public float ManaRegen { get;private set; }
    public int MoveSpeed { get;private set; }
    public int AttackSpeed { get;private set; }
    public int TowerDamage { get;private set; }
    public int TowerAttackSpeed { get;private set; }
    public List<Item> parents { get;private set; }


    public Item(string name, ItemIngredient[] neccesaryItem, ItemRank rank, byte id, Sprite resource,
    int attackPower, int neutralizeDefense, int magicalBuffer, int magicalDebuffer, int trueDamage, float healthRegen, float manaRegen, int moveSpeed, int attackSpeed, int towerDamage, int towerAttackSpeed)
    {
        Name = name;
        NecessaryItem = neccesaryItem;
        Rank = rank;
        Id = id;
        Resource = resource;

        AttackPower = attackPower;
        NeutralizeDefense = neutralizeDefense;
        MagicalBuffer = magicalBuffer;
        MagicalDebuffer = magicalDebuffer;
        TrueDamage = trueDamage;
        HealthRegen = healthRegen;
        ManaRegen = manaRegen;
        MoveSpeed = moveSpeed;
        AttackSpeed = attackSpeed;
        TowerDamage = towerDamage;
        TowerAttackSpeed = towerAttackSpeed;

        parents = new List<Item>();
    }

    public int CompareTo(Item obj)
    {
        if (Rank >= obj.Rank || obj == null)
            return 1;
        else if (Rank == obj.Rank)
        {
            if (Id >= obj.Id)
                return 1;
            else if (Id == obj.Id)
                return 0;
            else
                return -1;
        }
        else return -1;
    }

    public void SetParent(Item parent)
    {
        parents.Add(parent);
    }
    public List<Item> GetParent() { return parents; }


    public static implicit operator byte(Item item) => item.Id;

    public static implicit operator int(Item item) => (int)item.Rank;
}

public class ItemIngredient
{
    public Item Item;
    public int Count{ get; private set; }

    public ItemIngredient(Item item, int count)
    {
        Item = item;
        Count = count;
    }
    
}

public class List
{
    public List<Item>[] itemList = new List<Item>[9];
    public PlayerStats Stats;
    public CannonManager Cannon;
    public ItemManager ItemManager;
    private Dictionary<string, Item> dict;
    private Image[] images;
    private Button[] buttons;

    public int[] number;

    object[,] all = { { "만물석", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
    { "기억 조각", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },};

    object[,] common;
    object[,] uncommon;
    object[,] special;
    object[,] rare;
    object[,] legendary;
    object[,] hidden;
    object[,] changed;
    object[,] upperRanked;

    public object[][,] table;

    public List(PlayerStats stats, CannonManager cannon, ItemManager itemManager)
    {

        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i] = new List<Item>();
        }
        dict = new Dictionary<string, Item>();
        Stats = stats;
        Cannon = cannon;
        ItemManager = itemManager;

        images = ItemManager.GetImages();
        buttons = ItemManager.GetButtons();


        number = new int[(int)ItemRank.UpperRanked + 1];
        number[(int)ItemRank.All] = 0;

        table = new object[(int)ItemRank.UpperRanked + 1][,];
        table[(int)ItemRank.All] = all;
        SetItem(ItemRank.All);



        common = new object[,]{ // 공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        {"단검",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 10, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0},
        {"마법봉",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 1, 0, 0, 0f, 0f, 0, 0, 0, 0},
        {"소울스톤",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0.01f, 0, 0, 0, 0},
        {"리버스스톤",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0},
        {"망토",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 5, 0, 0, 0, 0, 0f, 0f, 0, 0, 1, 1},
        {"고기",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 0, 0, 0, 0.01f, 0f, 0, 0, 0, 0},
        {"철퇴",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 20, 0, 0, 0, 0, 0f, 0f, 0, -1, 0, 0},
        {"신발",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0},
        {"장갑",new[]{new ItemIngredient(FindItem("만물석"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 0, 1, 0, 0},};


        table[(int)ItemRank.Common] = common;
        SetItem(ItemRank.Common);

        uncommon = new object[,] {// 공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        {"꿰뚫는 창",new []{new ItemIngredient(FindItem("단검"), 1), new ItemIngredient(FindItem("리버스스톤"), 1)}
        , 15, 1, 0, 0, 0, 0f, 0f, 0, 0, 0, 0},
        {"생명의 샘물",new []{ new ItemIngredient(FindItem("마법봉"),1), new ItemIngredient(FindItem("소울스톤"), 1)}
        , 0, 0, 2, 0, 0, 0f, 0.02f, 0, 0, 0, 0 },
        {"끈끈이",new [] {new ItemIngredient(FindItem("신발"), 1), new ItemIngredient(FindItem("리버스스톤"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 1, 0, 0, 0 },
        { "마법사",new [] {new ItemIngredient(FindItem("마법봉"), 1), new ItemIngredient(FindItem("망토"), 1)}
        , 0, 0, 2, 2, 0, 0f, 0f, 0, 0, 0, 0 },
        { "로봇 팔",new [] {new ItemIngredient(FindItem("장갑"), 1), new ItemIngredient(FindItem("철퇴"), 1)}
        , 50, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        { "도적",new [] {new ItemIngredient(FindItem("단검"), 1), new ItemIngredient(FindItem("장갑"), 1)}
        , 20, 0, 0, 0, 0, 0f, 0f, 0, 3, 0, 0 },
        { "인간",new [] {new ItemIngredient(FindItem("고기"), 1), new ItemIngredient(FindItem("소울스톤"), 1)}
        , 0, 0, 0, 0, 0, 0.02f, 0.02f, 0, 0, 0, 0 },
        { "날개",new [] {new ItemIngredient(FindItem("신발"), 1), new ItemIngredient(FindItem("망토"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        { "전사",new [] {new ItemIngredient(FindItem("철퇴"), 1), new ItemIngredient(FindItem("고기"), 1)}
        , 100, 0, 0, 0, 0, 0.03f, 0f, 0, -5, 0, 0 },
        { "사신",new [] {new ItemIngredient(FindItem("망토"), 1), new ItemIngredient(FindItem("소울스톤"), 1)}
        , 70, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        { "파이어볼",new [] {new ItemIngredient(FindItem("마법봉"), 1), new ItemIngredient(FindItem("철퇴"), 1)}
        , 0, 0, 3, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        { "좀비",new [] {new ItemIngredient(FindItem("리버스스톤"), 1), new ItemIngredient(FindItem("고기"), 1)}
        , 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        { "갑옷",new [] {new ItemIngredient(FindItem("신발"), 1), new ItemIngredient(FindItem("장갑"), 1)}
        , 30, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        };
        table[(int)ItemRank.Uncommon] = uncommon;
        SetItem(ItemRank.Uncommon);

        special = new object[,] {// 공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        {"롱소드",new []{new ItemIngredient(FindItem("단검"), 3)},
        50, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        {"블링크",new []{new ItemIngredient(FindItem("신발"), 3)},
        0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0 },
        {"만찬",new []{new ItemIngredient(FindItem("고기"), 3)},
        0, 0, 0, 0, 0, 0.05f, 0f, 0, 0, 0, 0 },
        {"광선",new []{new ItemIngredient(FindItem("마법봉"), 3)},
        0, 0, 0, 0, 5, 0f, 0f, 0, 0, 0, 0 },
        {"아담의 영혼",new []{new ItemIngredient(FindItem("소울스톤"), 3)},
        0, 0, 0, 0, 0, 0f, 0.05f, 0, 0, 0, 0 },
        {"군단",new []{new ItemIngredient(FindItem("망토"), 3)},
        10, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 10 },
        {"대포알",new []{new ItemIngredient(FindItem("철퇴"), 3)},
        0, 0, 0, 0, 0, 0f, 0f, 0, 0, 20, 0 },
        { "빅뱅",new []{new ItemIngredient(FindItem("리버스스톤"), 3)},
        -1, -1, 3, 3, 3, -1f, -1f, 0, -1, 0, 0 },
        { "민첩함",new []{new ItemIngredient(FindItem("장갑"), 3)},
        0, 0, 0, 0, 0, 0f, 0f, 0, 5, 0, 0 },
        { "메카닉",new []{new ItemIngredient(FindItem("로봇 팔"), 2),new ItemIngredient(FindItem("철퇴"), 1)},
        70, 0, 0, 0, 0, 0f, 0.1f, 0, 0, 0, 0 },
        { "사이보그",new []{new ItemIngredient(FindItem("로봇 팔"), 1),new ItemIngredient(FindItem("인간"), 1)},
        50, 0, 0, 0, 0, 0.1f, 0.1f, 0, 0, 0, 0 },
        { "헌터",new []{new ItemIngredient(FindItem("전사"), 1),new ItemIngredient(FindItem("인간"), 1),new ItemIngredient(FindItem("리버스스톤"), 1)},
        200, 0, 0, 0, 0, 0f, 0.3f, 5, -10, 0, 0 },
        { "프로즌",new []{new ItemIngredient(FindItem("인간"), 1),new ItemIngredient(FindItem("마법사"), 1),new ItemIngredient(FindItem("소울스톤"), 1)},
        -50, 0, 5, 5, 0, 0.3f, 0f, 0, 0, 10, 0 },
        { "전염병",new []{new ItemIngredient(FindItem("좀비"), 1),new ItemIngredient(FindItem("끈끈이"), 1),new ItemIngredient(FindItem("고기"), 1)},
        0, 3, 0, 0, 0, 0f, 0f, 5, 0, 10, 0 },
        { "해독제",new []{new ItemIngredient(FindItem("좀비"), 1),new ItemIngredient(FindItem("생명의 샘물"), 1),new ItemIngredient(FindItem("리버스스톤"), 1)},
        0, 0, 0, 0, 0, 0f, 0.5f, 0, 0, 0, 0 },
        { "앨리스",new []{new ItemIngredient(FindItem("사신"), 1),new ItemIngredient(FindItem("소울스톤"), 1),new ItemIngredient(FindItem("리버스스톤"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "용기병",new []{new ItemIngredient(FindItem("날개"), 1),new ItemIngredient(FindItem("갑옷"), 1),new ItemIngredient(FindItem("철퇴"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "강철",new []{new ItemIngredient(FindItem("파이어볼"), 1),new ItemIngredient(FindItem("갑옷"), 1),new ItemIngredient(FindItem("장갑"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "영혼 낫",new []{new ItemIngredient(FindItem("사신"), 1),new ItemIngredient(FindItem("도적"), 1),new ItemIngredient(FindItem("소울스톤"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "마법 화살",new []{new ItemIngredient(FindItem("꿰뚫는 창"), 1),new ItemIngredient(FindItem("마법사"), 1),new ItemIngredient(FindItem("단검"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "죽음",new []{new ItemIngredient(FindItem("사신"), 1),new ItemIngredient(FindItem("인간"), 1),new ItemIngredient(FindItem("고기"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "버서커",new []{new ItemIngredient(FindItem("날개"), 1),new ItemIngredient(FindItem("전사"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "레이저 포",new []{new ItemIngredient(FindItem("파이어볼"), 1),new ItemIngredient(FindItem("마법사"), 1),new ItemIngredient(FindItem("망토"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        { "관통",new []{new ItemIngredient(FindItem("꿰뚫는 창"), 1),new ItemIngredient(FindItem("갑옷"), 1),new ItemIngredient(FindItem("단검"), 1)},
        0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0 },
        };
        table[(int)ItemRank.Special] = special;
        SetItem(ItemRank.Special);

        rare = new object[,] {// 공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        { "전쟁",new []{new ItemIngredient(FindItem("헌터"), 1),new ItemIngredient(FindItem("프로즌"), 1),new ItemIngredient(FindItem("군단"), 1)},
        1000, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "차원 거울",new []{new ItemIngredient(FindItem("죽음"), 1),new ItemIngredient(FindItem("아담의 영혼"), 1),new ItemIngredient(FindItem("빅뱅"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "타이탄",new []{new ItemIngredient(FindItem("메카닉"), 1),new ItemIngredient(FindItem("강철"), 1),new ItemIngredient(FindItem("사이보그"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "전쟁 영웅",new []{new ItemIngredient(FindItem("롱소드"), 1),new ItemIngredient(FindItem("버서커"), 1),new ItemIngredient(FindItem("죽음"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "이브",new []{new ItemIngredient(FindItem("앨리스"), 1),new ItemIngredient(FindItem("영혼 낫"), 1),new ItemIngredient(FindItem("빅뱅"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "탱크",new []{new ItemIngredient(FindItem("레이저 포"), 1),new ItemIngredient(FindItem("대포알"), 1),new ItemIngredient(FindItem("강철"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "대마법사",new []{new ItemIngredient(FindItem("레이저 포"), 1),new ItemIngredient(FindItem("마법 화살"), 1),new ItemIngredient(FindItem("프로즌"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "웜홀",new []{new ItemIngredient(FindItem("블링크"), 1),new ItemIngredient(FindItem("민첩함"), 1),new ItemIngredient(FindItem("버서커"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "공돌이",new []{new ItemIngredient(FindItem("앨리스"), 1),new ItemIngredient(FindItem("메카닉"), 1),new ItemIngredient(FindItem("용기병"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        { "플라즈마 광선",new []{new ItemIngredient(FindItem("관통"), 1),new ItemIngredient(FindItem("레이저 포"), 1),new ItemIngredient(FindItem("광선"), 1)},
        0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 },
        };
        table[(int)ItemRank.Rare] = rare;
        SetItem(ItemRank.Rare);

        legendary = new object[,] {

        };
        table[(int)ItemRank.Legendary] = legendary;
        SetItem(ItemRank.Legendary);

        hidden = new object[,]{

        };
        table[(int)ItemRank.Hidden] = hidden;
        SetItem(ItemRank.Hidden);

        changed = new object[,]{

        };
        table[(int)ItemRank.Changed] = changed;
        SetItem(ItemRank.Changed);

        upperRanked = new object[,]{

        };
        table[(int)ItemRank.UpperRanked] = upperRanked;
        SetItem(ItemRank.UpperRanked);


    }

    public void SetItem(ItemRank rank)
    {
        if (rank != ItemRank.UpperRanked)
            number[(int)rank + 1] = number[(int)rank] + table[(int)rank].GetLength(0);

        object[,] grade = table[(int)rank];
        int rowCount = grade.GetLength(0);            // 아이템 개수

        for (int j = 0; j < rowCount; j++)               // 아이템 반복
        {
            // 0번 열 = 이름
            string name = (string)grade[j, 0];

            // 숫자 필드는 타입 맞춰 캐스팅
            ItemIngredient[] nItem = (ItemIngredient[])grade[j, 1];
            int atkPower = (int)grade[j, 2];
            int nDefense = (int)grade[j, 3];
            int mAtkPower = (int)grade[j, 4];   // magicalBuffer
            int mDebuffer = (int)grade[j, 5];
            int tDamage = (int)grade[j, 6];
            float hpRegen = (float)grade[j, 7];
            float mpRegen = (float)grade[j, 8];
            int moveSpeed = (int)grade[j, 9];
            int attackSpeed = (int)grade[j, 10];
            int towerDamage = (int)grade[j, 11];
            int towerAttackSpd = (int)grade[j, 12];

            Sprite sprite = Resources.Load<Sprite>($"Image/Item/{rank}/{name}");

            Item newItem = new Item(
                name,
                nItem,
                rank,
                (byte)j,
                sprite,
                atkPower,
                nDefense,
                mAtkPower,
                mDebuffer,
                tDamage,
                hpRegen,
                mpRegen,
                moveSpeed,
                attackSpeed,
                towerDamage,
                towerAttackSpd);

            if (itemList == null)
                Debug.LogError("Error");

            itemList[(int)rank].Add(newItem);

            dict.Add(name, itemList[(int)rank][itemList[(int)rank].Count - 1]);

            foreach (ItemIngredient item in nItem)
            {
                item.Item.SetParent(newItem);
            }
        }

    }

    public Item FindItem(string s)
    {
        Item item = dict[s];
        return item;
    }

    public void GetRandomItem(ItemRank rank)
    {
        int rand = UnityEngine.Random.Range(0, itemList[(int)rank].Count);

        Item item = itemList[(int)rank][rand];

        item.count++;
        if (item.count == 1)
        {
            StatsUp(item);
        }
        ItemManager.Clear(ItemManager.GetEditItem());
    }

    public bool CombineItem(Item item)
    {
        if (item.Rank == ItemRank.All) return false;
        bool enough = true;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            if (nItem.Count > nItem.Item.count)
            {
                enough = false;
                break;
            }
        }
        if (enough)
        {
            foreach (ItemIngredient nItem in item.NecessaryItem)
            {
                nItem.Item.count -= nItem.Count;
                if (nItem.Item.count == 0)
                {
                    StatsDown(nItem.Item);
                }
            }
            item.count++;
            if (item.count == 1)
            {
                StatsUp(item);
            }
            ItemManager.Clear(null);
        }
        return enough;
    }

    public Dictionary<string, int> CombineAllItem(Item item, bool combine)
    {
        Dictionary<string, int> itemDict = new Dictionary<string, int>();
        bool isOkey = false;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            itemDict.Add(nItem.Item.Name, nItem.Count);
        }

        while (!isOkey)
        {
            isOkey = true;
            foreach (KeyValuePair<string, int> kvp in itemDict.ToList())
            {
                if (kvp.Key == "만물석")
                {
                    continue;
                }
                if (dict[kvp.Key].count < kvp.Value)
                    {
                        isOkey = false;
                        string Key = kvp.Key;
                        int neccesaryCount = kvp.Value - dict[Key].count;
                        foreach (ItemIngredient nItem in dict[Key].NecessaryItem)
                        {
                            if (itemDict.ContainsKey(nItem.Item.Name))
                            {
                                itemDict[nItem.Item.Name] += neccesaryCount;
                            }
                            else
                                itemDict.Add(nItem.Item.Name, neccesaryCount);
                        }
                        itemDict[Key] -= neccesaryCount;
                        if (itemDict[Key] <= 0)
                            itemDict.Remove(Key);
                    }
            }
        }
        if (combine)
        {
            if ((itemDict.ContainsKey("만물석") && itemDict["만물석"] <= dict["만물석"].count) || !itemDict.ContainsKey("만물석"))
            {
                foreach (KeyValuePair<string, int> nItem in itemDict)
                {
                    Item items = dict[nItem.Key];

                    items.count -= nItem.Value;
                    if (items.count <= 0)
                    {
                        StatsDown(items);
                    }
                }

                item.count++;
                if (item.count == 1)
                {
                    StatsUp(item);
                }
            ItemManager.Clear(null);
            }
        }
        return itemDict;
    }

    private void StatsUp(Item item)
    {
        if (Stats != null)
        {
            Stats.damage += item.AttackPower;
            Stats.attackSpeedBonus += item.AttackSpeed;
            Stats.HealthRegen += item.HealthRegen;
            Stats.manaRegen += item.ManaRegen;
            Stats.neutralizeDefense += item.NeutralizeDefense;
            Cannon.SetCannon(item.TowerDamage, item.TowerAttackSpeed);
        }
    }
    private void StatsDown(Item item)
    {
        Stats.damage -= item.AttackPower;
        Stats.attackSpeedBonus -= item.AttackSpeed;
        Stats.HealthRegen -= item.HealthRegen;
        Stats.manaRegen -= item.ManaRegen;
        Stats.neutralizeDefense -= item.NeutralizeDefense;
        Cannon.SetCannon(-item.TowerDamage, -item.TowerAttackSpeed);
    }

    public void Clear()
    {
        if (images != null)
            foreach (Image image in images)
            {
                image.sprite = null;
                Color c = image.color;
                c.a = 1f;
                image.color = c;

                image.transform.Find("number").gameObject.SetActive(false);

            }
        if (buttons != null)
        {
            foreach (Button button in buttons)
                button.GetComponent<Outline>().effectDistance = new Vector2(0, 0);

        }
    }

    public bool EnoughCombine(Dictionary<string, int> targetdict)
    {
        if (targetdict["만물석"] >= dict["만물석"].count) return false;
        else return true;
    }
}