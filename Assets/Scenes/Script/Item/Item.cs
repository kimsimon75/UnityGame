using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public delegate void Skill(Actor actor, Item item);
public enum ItemRank
{
    All,
    흔함,
    안흔함,
    특별함,
    희귀함,
    전설적인,
    히든,
    변화된,
    상위
}

public class Item
{
    public string Name { get; private set; }
    public ItemIngredient[] NecessaryItem { get; private set; }
    public ItemRank Rank { get; private set; }
    public byte Id;

    public Sprite Resource;
    public int count = 0;
    //확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위,단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
    public int AttackPower { get; private set; }
    public int AdditionalAttackPower { get; private set; }
    public int NeutralizeDefense { get; private set; }
    public int MagicalBuffer { get; private set; }
    public int MagicalDebuffer { get; private set; }
    public int TrueDamage { get; private set; }
    public float HealthRegen { get; private set; }
    public float ManaRegen { get; private set; }
    public int MoveSpeed { get; private set; }
    public int AttackSpeed { get; private set; }
    public int TowerDamage { get; private set; }
    public int TowerAttackSpeed { get; private set; }

    public int Percentage { get; private set; }
    public int MonoPhysics { get; private set; }
    public int MultiPhysics { get; private set; }
    public int MonoMagic { get; private set; }
    public int MultiMagic { get; private set; }
    public float MonoStun { get; private set; }
    public float MultiStun { get; private set; }
    public float Range { get; private set; }
    public int MonoPercent { get; private set; }
    public int EndPercent { get; private set; }
    public int MaxPercent { get; private set; }
    public int CurrPercent { get; private set; }
    public int Max_CurrPercent { get; private set; }
    public List<Item> parents { get; private set; }
    public Skill skill { get; private set; }


    public Item(string name, ItemIngredient[] neccesaryItem, ItemRank rank, byte id, Sprite resource,
    int attackPower, int additionalAttackPower, int neutralizeDefense, int magicalBuffer, int magicalDebuffer, int trueDamage, float healthRegen, float manaRegen, int moveSpeed, int attackSpeed, int towerDamage, int towerAttackSpeed,
    int percentage, int monoPhysics, int multiPhysics, int monoMagic, int multiMagic, float monoStun, float multiStun, float range,
    int monoPercent, int endPercent, int maxPercent, int currPercent, int max_CurrPercent
    , Skill _skill)
    {
        Name = name;
        NecessaryItem = neccesaryItem;
        Rank = rank;
        Id = id;
        Resource = resource;

        AttackPower = attackPower;
        AdditionalAttackPower = additionalAttackPower;
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

        Percentage = percentage;
        MonoPhysics = monoPhysics;
        MultiPhysics = multiPhysics;
        MonoMagic = monoMagic;
        MultiMagic = multiMagic;
        MonoStun = monoStun;
        MultiStun = multiStun;
        Range = range;
        MonoPercent = monoPercent;
        EndPercent = endPercent;
        MaxPercent = maxPercent;
        CurrPercent = CurrPercent;
        Max_CurrPercent = max_CurrPercent;

        skill = _skill;

        parents = new List<Item>();
    }

    public void SetParent(Item parent)
    {
        parents.Add(parent);
    }
    public List<Item> GetParent() { return parents; }

    public static implicit operator int(Item item) => (int)item.Rank;
    public static implicit operator string(Item item) => item.Name;
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
    private Dictionary<(string,ItemRank), Item> dict;
    private Image[] images;
    private Button[] buttons;
    private byte rankOn = 0b00000000;

    public List<Item> currentItem = new List<Item>(30);
    

    public int[] number;

    object[,] all = { { "만물석", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
    { "기억 조각", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},};

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
        dict = new Dictionary<(string, ItemRank), Item>();
        Stats = stats;
        Cannon = cannon;
        ItemManager = itemManager;

        images = ItemManager.GetImages();
        buttons = ItemManager.GetButtons();


        number = new int[(int)ItemRank.상위 + 1];
        number[(int)ItemRank.All] = 0;

        table = new object[(int)ItemRank.상위 + 1][,];
        table[(int)ItemRank.All] = all;
        SetItem(ItemRank.All);


        common = new object[,]{
        {"단검",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 10, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0, 0, 0, 0, 0, 0f, 0f, 5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0, 0, 0, 0, 0},//단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
        {"마법봉",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 1, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"소울스톤",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0.01f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"리버스스톤",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"망토",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 5, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 1, 1
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"고기",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 0, 0, 0, 0.01f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"철퇴",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 20, 0, 0, 0, 0, 0, 0f, 0f, 0, -1, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"신발",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"장갑",new[]{new ItemIngredient(FindItem("만물석", ItemRank.All), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 1, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},};


        table[(int)ItemRank.흔함] = common;
        SetItem(ItemRank.흔함);

        uncommon = new object[,] {
        {"꿰뚫는 창",new []{new ItemIngredient(FindItem("단검", ItemRank.흔함), 1), new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1)}
        , 15, 0, 1, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0, 0, 0, 0, 0, 0f, 0f,5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0, 0, 0, 0, 0},//단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
        {"생명의 샘물",new []{ new ItemIngredient(FindItem("마법봉", ItemRank.흔함),1), new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 1)}
        , 0, 0, 0, 2, 0, 0, 0f, 0.02f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0    },
        {"끈끈이",new [] {new ItemIngredient(FindItem("신발", ItemRank.흔함), 1), new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 1, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0   },
        { "마법사",new [] {new ItemIngredient(FindItem("마법봉", ItemRank.흔함), 1), new ItemIngredient(FindItem("망토", ItemRank.흔함), 1)}
        , 0, 0, 0, 2, 2, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "로봇 팔",new [] {new ItemIngredient(FindItem("장갑", ItemRank.흔함), 1), new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 1)}
        , 50, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0    },
        { "도적",new [] {new ItemIngredient(FindItem("단검", ItemRank.흔함), 1), new ItemIngredient(FindItem("장갑", ItemRank.흔함), 1)}
        , 20, 0, 0, 0, 0, 0, 0f, 0f, 0, 3, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0   },
        { "인간",new [] {new ItemIngredient(FindItem("고기", ItemRank.흔함), 1), new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 1)}
        , 0, 0, 0, 0, 0, 0, 0.02f, 0.02f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0   },
        { "날개",new [] {new ItemIngredient(FindItem("신발", ItemRank.흔함), 1), new ItemIngredient(FindItem("망토", ItemRank.흔함), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "전사",new [] {new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 1), new ItemIngredient(FindItem("고기", ItemRank.흔함), 1)}
        , 100, 0, 0, 0, 0, 0, 0.03f, 0f, 0, -5, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "사신",new [] {new ItemIngredient(FindItem("망토", ItemRank.흔함), 1), new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 1)}
        , 70, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "파이어볼",new [] {new ItemIngredient(FindItem("마법봉", ItemRank.흔함), 1), new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 1)}
        , 0, 0, 0, 3, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0    },
        { "좀비",new [] {new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1), new ItemIngredient(FindItem("고기", ItemRank.흔함), 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 5, 0, 0, 100, 0, 0f, 0f,0f
        ,0, 0, 0, 0, 0   },
        { "갑옷",new [] {new ItemIngredient(FindItem("신발", ItemRank.흔함), 1), new ItemIngredient(FindItem("장갑", ItemRank.흔함), 1)}
        , 30, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        };
        table[(int)ItemRank.안흔함] = uncommon;
        SetItem(ItemRank.안흔함);

        special = new object[,] {
        {"롱소드",new []{new ItemIngredient(FindItem("단검", ItemRank.흔함), 3)},
        50, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0, 0, 0, 0, 0, 0f, 0f, 5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0, 0, 0, 0, 0},//단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
        {"블링크",new []{new ItemIngredient(FindItem("신발", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"만찬",new []{new ItemIngredient(FindItem("고기", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 0, 0.05f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0   },
        {"광선",new []{new ItemIngredient(FindItem("마법봉", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 5, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"아담의 영혼",new []{new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 0, 0f, 0.05f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"군단",new []{new ItemIngredient(FindItem("망토", ItemRank.흔함), 3)},
        10, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 10
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        {"대포알",new []{new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 20, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "빅뱅",new []{new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 3)},
        -1, 0, -1, 3, 3, 3, -1f, -1f, 0, -1, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "민첩함",new []{new ItemIngredient(FindItem("장갑", ItemRank.흔함), 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 5, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "메카닉",new []{new ItemIngredient(FindItem("로봇 팔", ItemRank.안흔함), 2),new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 1)},
        70, 0, 0, 0, 0, 0, 0f, 0.1f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "사이보그",new []{new ItemIngredient(FindItem("로봇 팔", ItemRank.안흔함), 1),new ItemIngredient(FindItem("인간", ItemRank.안흔함), 1)},
        50, 0, 0, 0, 0, 0, 0.1f, 0.1f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "헌터",new []{new ItemIngredient(FindItem("전사", ItemRank.안흔함), 1),new ItemIngredient(FindItem("인간", ItemRank.안흔함), 1),new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1)},
        200, 0, 0, 0, 0, 0, 0f, 0.3f, 5, -10, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "프로즌",new []{new ItemIngredient(FindItem("인간", ItemRank.안흔함), 1),new ItemIngredient(FindItem("마법사", ItemRank.안흔함), 1),new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 1)},
        -50, 0, 0, 5, 5, 0, 0.3f, 0f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "전염병",new []{new ItemIngredient(FindItem("좀비", ItemRank.안흔함), 1),new ItemIngredient(FindItem("끈끈이", ItemRank.안흔함), 1),new ItemIngredient(FindItem("고기", ItemRank.흔함), 1)},
        0, 0, 3, 0, 0, 0, 0f, 0f, 5, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "해독제",new []{new ItemIngredient(FindItem("좀비", ItemRank.안흔함), 1),new ItemIngredient(FindItem("생명의 샘물", ItemRank.안흔함), 1),new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 0, 0f, 0.5f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "앨리스",new []{new ItemIngredient(FindItem("사신", ItemRank.안흔함), 1),new ItemIngredient(FindItem("소울스톤", ItemRank.흔함), 1),new ItemIngredient(FindItem("리버스스톤", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "용기병",new []{new ItemIngredient(FindItem("날개", ItemRank.안흔함), 1),new ItemIngredient(FindItem("갑옷", ItemRank.안흔함), 1),new ItemIngredient(FindItem("철퇴", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "강철",new []{new ItemIngredient(FindItem("파이어볼", ItemRank.안흔함), 1),new ItemIngredient(FindItem("갑옷", ItemRank.안흔함), 1),new ItemIngredient(FindItem("장갑", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "영혼 낫",new []{new ItemIngredient(FindItem("사신", ItemRank.안흔함), 1),new ItemIngredient(FindItem("도적", ItemRank.안흔함), 1),new ItemIngredient(FindItem("단검", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0 },
        { "도끼",new []{new ItemIngredient(FindItem("꿰뚫는 창", ItemRank.안흔함), 1),new ItemIngredient(FindItem("마법사", ItemRank.안흔함), 1),new ItemIngredient(FindItem("마법봉", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "죽음",new []{new ItemIngredient(FindItem("사신", ItemRank.안흔함), 1),new ItemIngredient(FindItem("인간", ItemRank.안흔함), 1),new ItemIngredient(FindItem("고기", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "버서커",new []{new ItemIngredient(FindItem("날개", ItemRank.안흔함), 1),new ItemIngredient(FindItem("전사", ItemRank.안흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "레이저 포",new []{new ItemIngredient(FindItem("파이어볼", ItemRank.안흔함), 1),new ItemIngredient(FindItem("마법사", ItemRank.안흔함), 1),new ItemIngredient(FindItem("망토", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "관통",new []{new ItemIngredient(FindItem("꿰뚫는 창", ItemRank.안흔함), 1),new ItemIngredient(FindItem("갑옷", ItemRank.안흔함), 1),new ItemIngredient(FindItem("단검", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "미래",new []{new ItemIngredient(FindItem("생명의 샘물", ItemRank.안흔함), 1),new ItemIngredient(FindItem("로봇 팔", ItemRank.안흔함), 1),new ItemIngredient(FindItem("마법봉", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "용접",new []{new ItemIngredient(FindItem("끈끈이", ItemRank.안흔함), 2),new ItemIngredient(FindItem("장갑", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "마법 화살",new []{new ItemIngredient(FindItem("꿰뚫는 창", ItemRank.안흔함), 1),new ItemIngredient(FindItem("날개", ItemRank.안흔함), 1),new ItemIngredient(FindItem("망토", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "금화",new []{new ItemIngredient(FindItem("도적", ItemRank.안흔함), 2),new ItemIngredient(FindItem("망토", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "레이피어",new []{new ItemIngredient(FindItem("꿰뚫는 창", ItemRank.안흔함), 1),new ItemIngredient(FindItem("전사", ItemRank.안흔함), 1),new ItemIngredient(FindItem("신발", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "화산",new []{new ItemIngredient(FindItem("날개", ItemRank.안흔함), 1),new ItemIngredient(FindItem("파이어볼", ItemRank.안흔함), 1),new ItemIngredient(FindItem("신발", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "영생약",new []{new ItemIngredient(FindItem("생명의 샘물", ItemRank.안흔함), 1),new ItemIngredient(FindItem("좀비", ItemRank.안흔함), 1),new ItemIngredient(FindItem("단검", ItemRank.흔함), 1)},
        0, 0, 0, 0, 0, 10, 0.2f, 0.2f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        };
        table[(int)ItemRank.특별함] = special;
        SetItem(ItemRank.특별함);

        rare = new object[,] {
        { "행운의 토큰",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,0f
        ,0, 0, 0, 0, 0},
        { "전쟁",new []{new ItemIngredient(FindItem("헌터", ItemRank.특별함), 1),new ItemIngredient(FindItem("프로즌", ItemRank.특별함), 1),new ItemIngredient(FindItem("군단", ItemRank.특별함), 1)},
        1000, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0 // 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0, 0, 0, 0, 0, 0f, 0f,5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0, 0, 0, 0, 0},//단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
        { "차원 거울",new []{new ItemIngredient(FindItem("죽음", ItemRank.특별함), 1),new ItemIngredient(FindItem("아담의 영혼", ItemRank.특별함), 1),new ItemIngredient(FindItem("빅뱅", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 100, 0, 0, 0, 0, 0f, 10f,5f
        ,0, 0, 0, 0, 0},
        { "타이탄",new []{new ItemIngredient(FindItem("메카닉", ItemRank.특별함), 1),new ItemIngredient(FindItem("강철", ItemRank.특별함), 1),new ItemIngredient(FindItem("사이보그", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "전쟁 영웅",new []{new ItemIngredient(FindItem("롱소드", ItemRank.특별함), 1),new ItemIngredient(FindItem("버서커", ItemRank.특별함), 1),new ItemIngredient(FindItem("죽음", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "탱크",new []{new ItemIngredient(FindItem("레이저 포", ItemRank.특별함), 1),new ItemIngredient(FindItem("대포알", ItemRank.특별함), 1),new ItemIngredient(FindItem("강철", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "대마법사",new []{new ItemIngredient(FindItem("레이저 포", ItemRank.특별함), 1),new ItemIngredient(FindItem("마법 화살", ItemRank.특별함), 1),new ItemIngredient(FindItem("프로즌", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "웜홀",new []{new ItemIngredient(FindItem("블링크", ItemRank.특별함), 1),new ItemIngredient(FindItem("민첩함", ItemRank.특별함), 1),new ItemIngredient(FindItem("버서커", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0 },
        { "공돌이",new []{new ItemIngredient(FindItem("앨리스", ItemRank.특별함), 1),new ItemIngredient(FindItem("메카닉", ItemRank.특별함), 1),new ItemIngredient(FindItem("용기병", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        { "플라즈마 광선",new []{new ItemIngredient(FindItem("관통", ItemRank.특별함), 1),new ItemIngredient(FindItem("레이저 포", ItemRank.특별함), 1),new ItemIngredient(FindItem("광선", ItemRank.특별함), 1)},
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},
        };

        table[(int)ItemRank.희귀함] = rare;
        SetItem(ItemRank.희귀함);


        hidden = new object[,]{
        { "함선",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0, 0, 0, 0, 0, 0f, 0f,0f
        ,0, 0, 0, 0, 0},
        { "이브",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 10, 0
        , 0, 0, 0, 0, 0, 0f, 0f,5f
        ,0, 0, 0, 0, 0},

        };
        table[(int)ItemRank.히든] = hidden;
        SetItem(ItemRank.히든);


        legendary = new object[,] {
        { "이브",new []{new ItemIngredient(FindItem("이브", ItemRank.히든), 1), new ItemIngredient(FindItem("차원 거울", ItemRank.희귀함), 1),new ItemIngredient(FindItem("영혼 낫", ItemRank.특별함), 1),new ItemIngredient(FindItem("기억 조각", ItemRank.All), 5)},
        5000, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0, 0, 0, 0, 0, 0f, 0f,5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0, 0, 0, 0, 0},//단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
        };
        table[(int)ItemRank.전설적인] = legendary;
        SetItem(ItemRank.전설적인);


        changed = new object[,]{

        };
        table[(int)ItemRank.변화된] = changed;
        SetItem(ItemRank.변화된);

        upperRanked = new object[,]{

        };
        table[(int)ItemRank.상위] = upperRanked;
        SetItem(ItemRank.상위);

        for (int rank = 0; rank < table.Length; rank++)
        {
            int rows = table[rank].GetLength(0);         // 행 개수
            for (int r = 0; r < rows; r++)
            {
                string itemName = (string)table[rank][r, 0];

                string path = $"Image/Item/{(ItemRank)rank}/{itemName}";
                Sprite sprite = Resources.Load<Sprite>(path);
                if (sprite != null)
                {
                    if (!DataManager.Instance.imageDict.ContainsKey(sprite))
                        DataManager.Instance.imageDict.Add(sprite, (ItemRank)rank);
                }
                else
                {
                    Debug.Log(sprite);
                }
                    
            }
        }


    }

    public void SetItem(ItemRank rank)
    {
        if (rank != ItemRank.상위)
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
            int addAtkPower = (int)grade[j, 3];
            int nDefense = (int)grade[j, 4];
            int mAtkPower = (int)grade[j, 5];   // magicalBuffer
            int mDebuffer = (int)grade[j, 6];
            int tDamage = (int)grade[j, 7];
            float hpRegen = (float)grade[j, 8];
            float mpRegen = (float)grade[j, 9];
            int moveSpeed = (int)grade[j, 10];
            int attackSpeed = (int)grade[j, 11];
            int towerDamage = (int)grade[j, 12];
            int towerAttackSpd = (int)grade[j, 13];
            //확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위,단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
            int percentage = (int)grade[j, 14];
            int monoPhysics = (int)grade[j, 15];
            int multiPhysics = (int)grade[j, 16];
            int monoMagic = (int)grade[j, 17];
            int multiMagic = (int)grade[j, 18];
            float monoStun = (float)grade[j, 19];
            float multiStun = (float)grade[j, 20];
            float range = (float)grade[j, 21];
            int monoPercent = (int)grade[j, 22];
            int endPercent = (int)grade[j, 23];
            int maxPercent = (int)grade[j, 24];
            int currPercent = (int)grade[j, 25];
            int max_CurrPercent = (int)grade[j, 26];


            Sprite sprite = Resources.Load<Sprite>($"Image/Item/{rank}/{(string)grade[j, 0]}");

            Item newItem = new Item(
                name,
                nItem,
                rank,
                (byte)j,
                sprite,
                atkPower,
                addAtkPower,
                nDefense,
                mAtkPower,
                mDebuffer,
                tDamage,
                hpRegen,
                mpRegen,
                moveSpeed,
                attackSpeed,
                towerDamage,
                towerAttackSpd,
                percentage,
                monoPhysics,
                multiPhysics,
                monoMagic,
                multiMagic,
                monoStun,
                multiStun,
                range,
                monoPercent,
                endPercent,
                maxPercent,
                currPercent,
                max_CurrPercent,
                SetSkill);

            if (itemList == null)
                Debug.LogError("Error");

            itemList[(int)rank].Add(newItem);

            dict.Add((name, rank), itemList[(int)rank][itemList[(int)rank].Count - 1]);

            foreach (ItemIngredient item in nItem)
            {
                item.Item.SetParent(newItem);
            }
        }

    }

    public Item FindItem(string s, ItemRank rank)
    {
        return dict[(s, rank)];
    }

    public Item GetRandomItem(ItemRank rank, bool logOut = true)
    {
        int rand = UnityEngine.Random.Range(0, itemList[(int)rank].Count);

        Item item = itemList[(int)rank][rand];

        item.count++;
        if (item.count == 1)
        {
            StatsUp(item);
        }
        if (logOut)
        {
            string hex = ColorUtility.ToHtmlStringRGB(Color.black);
            switch (rank)
            {
                case ItemRank.흔함:
                    hex = ColorUtility.ToHtmlStringRGB(Color.green);
                    break;
                case ItemRank.안흔함:
                    hex = ColorUtility.ToHtmlStringRGB(Color.purple);
                    break;
                case ItemRank.특별함:
                    hex = ColorUtility.ToHtmlStringRGB(Color.yellow);
                    break;
                case ItemRank.희귀함:
                    hex = "FF00FF";
                    break;
            }
            ItemManager.chat.Push($"<color=#{hex}>{rank}</color> 등급의 {item.Name} 획득");
        }
        ItemManager.Clear(ItemManager.GetEditItem(), false);
        return item;
    }

    public void GetMemoriesParts(int count)
    {
        FindItem("기억 조각", ItemRank.All).count += count;
        ItemManager.chat.Push($"<color=#Yellow>기억 조각</color> {count}개 획득.");
    }

    public bool CombineItem(Item item)
    {
        if (item.NecessaryItem.Count() == 0) return false;
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
            ItemManager.Clear(null, false);
        }
        return enough;
    }

    public Dictionary<(string, ItemRank), int> CombineAllItem(Item item, bool combine)
    {
        Dictionary<(string, ItemRank), int> itemDict = new Dictionary<(string, ItemRank), int>();
        if (item.NecessaryItem.Count() == 0) return itemDict;
        bool isOkay = false;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            itemDict.Add((nItem.Item, nItem.Item.Rank), nItem.Count);
        }

        while (!isOkay)
        {
            isOkay = true;
            foreach (KeyValuePair<(string, ItemRank), int> kvp in itemDict.ToList())
            {

                if (dict[(kvp.Key.Item1, kvp.Key.Item2)].NecessaryItem.Count() == 0) continue;
                if (dict[(kvp.Key.Item1, kvp.Key.Item2)].count < kvp.Value)
                {
                    isOkay = false;
                    string Key = kvp.Key.Item1;
                    ItemRank Key2 = kvp.Key.Item2;
                    int necessaryCount = Mathf.Max(kvp.Value - dict[(Key,Key2)].count, 0);
                    foreach (ItemIngredient nItem in dict[(Key,Key2)].NecessaryItem)
                    {
                        if (itemDict.ContainsKey((nItem.Item.Name, nItem.Item.Rank)))
                        {
                            itemDict[(nItem.Item.Name, nItem.Item.Rank)] += necessaryCount * nItem.Count;
                        }
                        else
                            itemDict.Add((nItem.Item.Name, nItem.Item.Rank), necessaryCount * nItem.Count);
                    }
                    itemDict[kvp.Key] -= necessaryCount;
                    if (itemDict[kvp.Key] <= 0)
                        itemDict.Remove(kvp.Key);
                }
            }
        }
        if (combine)
        {
            foreach (KeyValuePair<(string, ItemRank), int> nItem in itemDict)
            {
                if (dict[(nItem.Key.Item1, nItem.Key.Item2)].count < nItem.Value) return itemDict;
            }
            if ((itemDict.ContainsKey(("만물석", ItemRank.All)) && itemDict[("만물석", ItemRank.All)] <= dict[("만물석", ItemRank.All)].count) || !itemDict.ContainsKey(("만물석", ItemRank.All)))
                {
                    foreach (KeyValuePair<(string, ItemRank), int> nItem in itemDict)
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
                    ItemManager.Clear(null, false);
                }
        }
        return itemDict;
    }

    public Dictionary<(string, ItemRank), int> DissolutionAll(Item item)
    {
        Dictionary<(string, ItemRank), int> itemDict = new Dictionary<(string, ItemRank), int>();
        if (item.NecessaryItem.Count() == 0) return itemDict;
        bool isOkay = false;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            if (nItem.Item.Name != "기억 조각")
            itemDict.Add((nItem.Item.Name,nItem.Item.Rank), nItem.Count);
        }

        while (!isOkay)
        {
            isOkay = true;
            foreach (KeyValuePair<(string, ItemRank), int> kvp in itemDict.ToList())
            {
                string Key = kvp.Key.Item1;
                ItemRank key2 = kvp.Key.Item2;
                if (dict[(Key, key2)].NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                if (dict[(Key, key2)].NecessaryItem[0].Item.NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                Item targetItem = dict[(Key, key2)].NecessaryItem[0].Item;
                if (dict[(Key, key2)].count < kvp.Value)
                {
                    isOkay = false;
                    int necessaryCount = kvp.Value;
                    foreach (ItemIngredient nItem in dict[(Key, key2)].NecessaryItem)
                    {
                        if (itemDict.ContainsKey((nItem.Item.Name, nItem.Item.Rank)))
                        {
                            itemDict[(nItem.Item.Name, nItem.Item.Rank)] += necessaryCount * nItem.Count;
                        }
                        else
                            itemDict.Add((nItem.Item.Name, nItem.Item.Rank), necessaryCount * nItem.Count);
                    }
                    itemDict[(Key, key2)] -= necessaryCount;
                    if (itemDict[(Key, key2)] <= 0)
                        itemDict.Remove((Key, key2));
                }
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

            if (item.Name == "행운의 토큰" || item.Name == "함선")
            {
                Stats.damage += 400;
                Stats.attackDelay = 0.9f;
                Stats.attackSpeedBonus += 25f;
                Cannon.SetCannon(80, 10);
                return;
            }
            else if (item.Name == "이브")
            {
                Stats.damage += 4500;
                Stats.attackDelay = 0.85f;
                Stats.attackSpeedBonus += 45f;
                Cannon.SetCannon(920, 20);
            }

            if (item.Rank == ItemRank.희귀함 && !GameManager.Instance.RareGet && GameManager.Instance.GetRound() <= 7)
                {
                    FindItem("만물석", ItemRank.All).count++;
                    GameManager.Instance.RareGet = true;
                }

            if (item.Rank >= ItemRank.안흔함)
                {
                    int shift = item.Rank - ItemRank.안흔함;
                    if (shift < 0 || shift >= 31)
                    {
                        Debug.LogError($"Rank shift 값이 비정상입니다: {shift}");
                        return;
                    }
                    int bit = 1 << shift;

                    for (int i = 1; i <= bit; i <<= 1)
                    {
                        if ((rankOn & i) == 0)
                        {
                            switch ((ItemRank)(Mathf.Log(i, 2) + (int)ItemRank.안흔함))
                            {
                                case ItemRank.안흔함:
                                    Stats.damage += 100;
                                    Stats.attackDelay = 0.95f;
                                    Cannon.SetCannon(20, 5);
                                    break;
                                case ItemRank.특별함:
                                    Stats.damage += 400;
                                    Stats.attackDelay = 0.9f;
                                    Stats.attackSpeedBonus += 25f;
                                    Cannon.SetCannon(80, 10);
                                    break;
                                case ItemRank.희귀함:
                                    Stats.damage += 4500;
                                    Stats.attackDelay = 0.85f;
                                    Stats.attackSpeedBonus += 45f;
                                    Cannon.SetCannon(920, 20);
                                    break;
                            }
                            rankOn |= (byte)i;
                        }
                    }

                }

            currentItem.Add(item);
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
        
        currentItem.Remove(item);
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

                image.transform.Find("number1").gameObject.SetActive(false);
                image.transform.Find("number2").gameObject.SetActive(false);

            }
        if (buttons != null)
        {
            foreach (Button button in buttons)
                button.GetComponent<UnityEngine.UI.Outline>().effectDistance = new Vector2(0, 0);

        }
    }

    private void SetSkill(Actor actor, Item item)//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위,단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
    {
        string name = item.Name;

        ItemRank rank = item.Rank;

        int Percentage = item.Percentage;

        int MonoPhysics = item.MonoPhysics;
        int MultiPhysics = item.MultiPhysics;
        int MonoMagic = item.MonoMagic;
        int MultiMagic = item.MultiMagic;
        float MonoStun = item.MonoStun;
        float MultiStun = item.MultiStun;
        float Range = item.Range;
        int MonoPercent = item.MonoPercent;
        int EndPercent = item.EndPercent;
        int MaxPercent = item.MaxPercent;
        int CurrPercent = item.CurrPercent;
        int Max_CurrPercent = item.Max_CurrPercent;

        int rand = UnityEngine.Random.Range(0, 10000);

        float MultiPercentage = (1f - Mathf.Pow(1f - Percentage / 100f, item.count)) * 100; // 다중 확률

        if (Percentage * 100 > rand)
        {
            actor.TakeDamageAll(MultiPhysics, MonoPhysics, Range
            , actor.armorType, true, Stats.neutralizeDefense, 0);

            actor.TakeDamageAll(MultiMagic, MonoMagic, Range
            , actor.armorType, false, 0, 0);

            actor.TakeStunAll(MultiStun, MonoStun, Range);

            actor.TakeDamageAll(0, MonoPercent, Range, actor.armorType, false, 0, 2);
            actor.TakeDamageAll(0, EndPercent, Range, actor.armorType, false, 0, 1);

            actor.TakeDamageAll(MaxPercent, 0, Range, actor.armorType, false, 0, 1);
            actor.TakeDamageAll(CurrPercent, 0, Range, actor.armorType, false, 0, 2);
            actor.TakeDamageAll(Max_CurrPercent, 0, Range, actor.armorType, false, 0, 3);
        }

        if (MultiPercentage * 100 >= rand)
        {
            switch (name)
            {
                case "좀비":
                    if (actor.isDead)
                        FindItem(name,rank).count++;
                    break;
            }
        }
    }
}