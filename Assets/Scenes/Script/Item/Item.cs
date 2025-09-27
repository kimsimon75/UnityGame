using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static MyMathf;
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
    상위,
    획득
}

public sealed class Item : IComparable<Item>
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

    public float Probability { get; private set; }
    public int MonoPhysics { get; private set; }
    public int MultiPhysics { get; private set; }
    public int MonoMagic { get; private set; }
    public int MultiMagic { get; private set; }
    public float MonoStun { get; private set; }
    public float MultiStun { get; private set; }
    public float Range { get; private set; }
    public float Percent { get; private set; }
    public ArmorType AttackType { get; private set; }
    public bool BossPercentAttack { get; private set; }
    public int PercentCategory {get; private set;}
    public float DoublePhysics { get; private set; }
    public float DamageUp { get; private set; }
    public float AttackRange { get; private set; }
    public List<Item> parents { get; private set; }

    public Item(string name, ItemIngredient[] neccesaryItem, ItemRank rank, byte id, Sprite resource,
    int attackPower, int additionalAttackPower, int neutralizeDefense, int magicalBuffer, int magicalDebuffer, int trueDamage, float healthRegen, float manaRegen, int moveSpeed, int attackSpeed, int towerDamage, int towerAttackSpeed,
    float probability, int monoPhysics, int multiPhysics, int monoMagic, int multiMagic, float monoStun, float multiStun, float range,
    float percent, ArmorType attackType, bool bossPercentAttack, int percentCategory,
    float doublePhysics, float damageUp, float attackRange)
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

        Probability = probability;
        MonoPhysics = monoPhysics;
        MultiPhysics = multiPhysics;
        MonoMagic = monoMagic;
        MultiMagic = multiMagic;
        MonoStun = monoStun;
        MultiStun = multiStun;
        Range = range;
        Percent = percent;
        PercentCategory = percentCategory;

        AttackType = attackType;
        BossPercentAttack = bossPercentAttack;

        DoublePhysics = doublePhysics;
        DamageUp = damageUp;

        AttackRange = attackRange;

        parents = new List<Item>();
    }

    public void SetParent(Item parent)
    {
        parents.Add(parent);
    }
    public List<Item> GetParent() { return parents; }

    public int CompareTo(Item other)
    {
        int cmp = this.Rank.CompareTo(other.Rank);
        if (cmp != 0) return cmp;

        // Rank가 같으면 Name 기준 오름차순
        return other.Id.CompareTo(this.Id);
    }

    public static implicit operator int(Item item) => (int)item.Rank;
    public static implicit operator string(Item item) => item.Name;
}

public class ItemIngredient
{
    public string ItemName;
    public ItemRank Rank;
    public int Count { get; private set; }

    public ItemIngredient(string itemName, ItemRank rank, int count)
    {
        ItemName = itemName;
        Rank = rank;
        Count = count;
    }
    
}

public class List
{
    public List<Item>[] itemList = new List<Item>[(int)ItemRank.상위 + 1];
    public PlayerStats Stats;
    public CannonManager Cannon;
    public ItemManager ItemManager;
    private Dictionary<(string, ItemRank), Item> dict;
    private Image[] images;
    private UnityEngine.UI.Outline[] outlines;
    private int[] rankOn = new int[GameManager.Instance.Action.TargetNumberMax];
    private int cannonRankOn;

    public PriorityQueue<Item>[] currentItem;

    public const int gotItemCount = 15;
    public PriorityQueue<Item> GotItem = new PriorityQueue<Item>(gotItemCount);

    object[,] all = {
        { "만물석", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false ,0, 0f, 0f, 0f},
    { "기억 조각", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false ,0, 0f, 0f, 0f},
    { "영혼 파편", Array.Empty<ItemIngredient>(), 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f ,ArmorType.일반
        ,false ,0, 0f, 0f, 0f},};

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
        currentItem = new PriorityQueue<Item>[GameManager.Instance.Action.TargetNumberMax];
        for (int i = 0; i < currentItem.Length; i++)
        {
            currentItem[i] = new PriorityQueue<Item>(30);
        }

        for (int i = 0; i < itemList.Length; i++)
            {
                itemList[i] = new List<Item>();
            }
        dict = new Dictionary<(string, ItemRank), Item>();
        Stats = stats;
        Cannon = cannon;
        ItemManager = itemManager;

        images = ItemManager.GetImages();
        outlines = itemManager.GetOutlines();

        table = new object[(int)ItemRank.상위 + 1][,];
        table[(int)ItemRank.All] = all;
        SetItemSkeleton(ItemRank.All);


        common = new object[,]{
        {"단검",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 10, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0f, 0, 0, 0, 0, 0f, 0f, 5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0f, ArmorType.일반//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 0f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        {"마법봉",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 1, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"소울스톤",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0.01f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"리버스스톤",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"망토",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 5, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 1, 1
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"고기",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 0, 0, 0, 0.01f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"철퇴",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 20, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"신발",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"장갑",new[]{new ItemIngredient("만물석", ItemRank.All, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},};



        uncommon = new object[,] {
        {"창",new []{new ItemIngredient("단검", ItemRank.흔함, 1), new ItemIngredient("리버스스톤", ItemRank.흔함, 1)}
        , 15, 0, 1, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0f, 0, 0, 0, 0, 0f, 0f,5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0f, ArmorType.일반//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 0f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        {"생명의 샘물",new []{ new ItemIngredient("마법봉", ItemRank.흔함,1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)}
        , 0, 0, 0, 2, 0, 0, 0f, 0.02f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반 
        ,false , 0, 0f , 0f, 0f  },
        {"끈끈이",new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("리버스스톤", ItemRank.흔함, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 1, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f  , 0f , 0f},
        { "마법사",new [] {new ItemIngredient("마법봉", ItemRank.흔함, 1), new ItemIngredient("망토", ItemRank.흔함, 1)}
        , 0, 0, 0, 2, 2, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "로봇 팔",new [] {new ItemIngredient("장갑", ItemRank.흔함, 1), new ItemIngredient("철퇴", ItemRank.흔함, 1)}
        , 50, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f , 0f   },
        { "도적",new [] {new ItemIngredient("단검", ItemRank.흔함, 1), new ItemIngredient("장갑", ItemRank.흔함, 1)}
        , 20, 0, 0, 0, 0, 0, 0f, 0f, 0, 3, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f , 0f  },
        { "인간",new [] {new ItemIngredient("고기", ItemRank.흔함, 1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)}
        , 0, 0, 0, 0, 0, 0, 0.02f, 0.02f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f   },
        { "날개",new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("망토", ItemRank.흔함, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "전사",new [] {new ItemIngredient("철퇴", ItemRank.흔함, 1), new ItemIngredient("고기", ItemRank.흔함, 1)}
        , 100, 0, 0, 0, 0, 0, 0.03f, 0f, 0, -5, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "사신",new [] {new ItemIngredient("망토", ItemRank.흔함, 1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)}
        , 70, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "파이어볼",new [] {new ItemIngredient("마법봉", ItemRank.흔함, 1), new ItemIngredient("철퇴", ItemRank.흔함, 1)}
        , 0, 0, 0, 3, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f    },
        { "좀비",new [] {new ItemIngredient("리버스스톤", ItemRank.흔함, 1), new ItemIngredient("고기", ItemRank.흔함, 1)}
        , 0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 5f, 0, 0, 100, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f   },
        { "갑옷",new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("장갑", ItemRank.흔함, 1)}
        , 30, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        };

        special = new object[,] {
        {"롱소드",new []{new ItemIngredient("단검", ItemRank.흔함, 3)},
        50, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0f, 0, 0, 0, 0, 0f, 0f, 0f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0f, ArmorType.일반//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 5f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        {"블링크",new []{new ItemIngredient("신발", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 100, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false ,0, 0f, 0f, 0f},
        {"만찬",new []{new ItemIngredient("고기", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 0, 0.05f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f  },
        {"광선",new []{new ItemIngredient("마법봉", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 5, 0f, 0f, 0, 0, 0, 10
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"아담의 영혼",new []{new ItemIngredient("소울스톤", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0.05f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"군단",new []{new ItemIngredient("망토", ItemRank.흔함, 3)},
        10, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 10
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        {"대포알",new []{new ItemIngredient("철퇴", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 20, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "빅뱅",new []{new ItemIngredient("리버스스톤", ItemRank.흔함, 3)},
        -1, 0, -1, 3, 3, 3, -1f, -1f, 0, -1, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "민첩함",new []{new ItemIngredient("장갑", ItemRank.흔함, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 5, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "메카닉",new []{new ItemIngredient("로봇 팔", ItemRank.안흔함, 2),new ItemIngredient("단검", ItemRank.흔함, 1)},
        70, 0, 0, 0, 0, 0, 0f, 0.1f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "사이보그",new []{new ItemIngredient("로봇 팔", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1)},
        50, 0, 0, 0, 0, 0, 0.1f, 0.1f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "헌터",new []{new ItemIngredient("전사", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)},
        200, 0, 0, 0, 0, 0, 0f, 0.3f, 5, -10, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "프로즌",new []{new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("소울스톤", ItemRank.흔함, 1)},
        -50, 0, 0, 5, 5, 0, 0.3f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "전염병",new []{new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("끈끈이", ItemRank.안흔함, 1),new ItemIngredient("철퇴", ItemRank.흔함, 1)},
        0, 0, 3, 0, 0, 0, 0f, 0f, 5, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "해독제",new []{new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0.5f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "앨리스",new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("소울스톤", ItemRank.흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 100, 0.2f, 0.2f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "용기병",new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("철퇴", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 10f, 0, 0, 0, 0, 0f, 0f,5f
        ,0.1f, ArmorType.일반
        ,false , 2, 0f, 0f, 0f},
        { "강철",new []{new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("고기", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 10, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "영혼 낫",new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("도적", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)},
        0, 0, 0, 5, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "도끼",new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)},
        80, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 10f, 500, 0, 0, 0, 1f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "죽음",new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("고기", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 50f, 0, 0, 300, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "버서커",new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("전사", ItemRank.안흔함, 1)},
        200, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "레이저 포",new []{new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("망토", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 100, 10
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "관통",new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("단검", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 5, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "미래",new []{new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("로봇 팔", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0.1f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "용접",new []{new ItemIngredient("끈끈이", ItemRank.안흔함, 2),new ItemIngredient("장갑", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 50, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "마법 화살",new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("망토", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 10.1f, 0, 0, 1250, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "금화",new []{new ItemIngredient("도적", ItemRank.안흔함, 2),new ItemIngredient("망토", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 100, 10
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "레이피어",new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("전사", ItemRank.안흔함, 1),new ItemIngredient("신발", ItemRank.흔함, 1)},
        100, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 15f, 1000, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "화산",new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("신발", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0.1f, 0f, 5, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "영생약",new []{new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("단검", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0.1f, 0.1f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "표창",new []{new ItemIngredient("도적", ItemRank.안흔함, 1),new ItemIngredient("끈끈이", ItemRank.안흔함, 1),new ItemIngredient("장갑", ItemRank.흔함, 1)},
        100, 0, 0, 0, 0, 2, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        };

        rare = new object[,] {
        { "행운의 토큰",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "전쟁",new []{new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("군단", ItemRank.특별함, 1)},
        1000, 0, 0, 0, 0, 20, 0.3f, 0.3f, 0, 0, 0, 0 // 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 0f, 0, 0, 0, 0, 0f, 0f,0f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0f, ArmorType.일반//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 0f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        { "차원 거울",new []{new ItemIngredient("죽음", ItemRank.특별함, 1),new ItemIngredient("아담의 영혼", ItemRank.특별함, 1),new ItemIngredient("빅뱅", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 10f, 0, 0, 0, 0, 0f, 10f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "타이탄",new []{new ItemIngredient("메카닉", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 1000, 30
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "영웅",new []{new ItemIngredient("도끼", ItemRank.특별함, 1),new ItemIngredient("버서커", ItemRank.특별함, 1),new ItemIngredient("죽음", ItemRank.특별함, 1)},
        5000, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "탱크",new []{new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 1000, 30
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "대마법사",new []{new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("마법 화살", ItemRank.특별함, 1),new ItemIngredient("아담의 영혼", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 12.5f, 0, 0, 0, 10000, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "웜홀",new []{new ItemIngredient("블링크", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1),new ItemIngredient("버서커", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 15, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f },
        { "공돌이",new []{new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("메카닉", ItemRank.특별함, 1),new ItemIngredient("용기병", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "플라즈마 광선",new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("광선", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "무기의 달인",new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("도끼", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "과학자",new []{new ItemIngredient("해독제", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "되살아난 영웅",new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "저격수",new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("광선", ItemRank.특별함, 1),new ItemIngredient("블링크", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "도적",new []{new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1),new ItemIngredient("사신", ItemRank.안흔함, 1), new ItemIngredient("신발", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "군인",new []{new ItemIngredient("버서커", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "굶주림",new []{new ItemIngredient("만찬", ItemRank.특별함, 1),new ItemIngredient("영생약", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "엑스칼리버",new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("도끼", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "앨리스",new []{new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "캐논쉽",new []{new ItemIngredient("군단", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("메카닉", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "추적자",new []{new ItemIngredient("용기병", ItemRank.특별함, 1),new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "바이오 로봇",new []{new ItemIngredient("영생약", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1),new ItemIngredient("용접", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "행성",new []{new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "좀비",new []{new ItemIngredient("해독제", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1),new ItemIngredient("만찬", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "활",new []{new ItemIngredient("마법 화살", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("해독제", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "합금",new []{new ItemIngredient("광선", ItemRank.특별함, 1),new ItemIngredient("용접", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "부메랑",new []{new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("만찬", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "사신",new []{new ItemIngredient("죽음", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "꿰뚫는 창",new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("용기병", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "아담",new []{new ItemIngredient("아담의 영혼", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1),new ItemIngredient("영생약", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "헌터 국왕",new []{new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("빅뱅", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "프로즌 국왕",new []{new ItemIngredient("블링크", ItemRank.특별함, 1),new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("마법 화살", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "메카 군단",new []{new ItemIngredient("용접", ItemRank.특별함, 2),new ItemIngredient("군단", ItemRank.특별함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "우주",new []{new ItemIngredient("빅뱅", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        };

        legendary = new object[,] {
        { "이브",new []{new ItemIngredient("이브", ItemRank.히든, 1), new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("기억 조각", ItemRank.All, 5)},
        5000, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0// 공격력, 추가공격력, 방어력 감소, 마법증폭, 마법방어력 감소, 트루 데미지, 체젠, 마젠, 이동속도 감소, 공격속도, 타워 데미지, 타워 공속
        , 10f, 0, 0, 500000, 100000, 0f, 0f,5f//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위
        ,0f, ArmorType.보스//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 0f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        { "전쟁 영웅",new []{new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "도적",new []{new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "앨리스",new []{new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "헌터 국왕",new []{new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "프로즌 국왕",new []{new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "다차원",new []{new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "작은 거인",new []{new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "일그러진 영웅",new []{new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "공돌이",new []{new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("추적자", ItemRank.희귀함, 1),new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "메카 군단",new []{new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "과학자",new []{new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "발키리",new []{new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "시빌워",new []{new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "아담",new []{new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "저격수",new []{new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "시간",new []{new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "군인",new []{new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "타이탄",new []{new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "차원 문",new []{new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "좀비",new []{new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "태초",new []{new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "죽음",new []{new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "저격총",new []{new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "속력",new []{new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("추적자", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "탱크",new []{new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "캐논쉽",new []{new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "대마법사",new []{new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "우주",new []{new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "메카닉",new []{new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "워프",new []{new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "치유력",new []{new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "영웅의 딸",new []{new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},  
        { "사신",new []{new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},    
        };
        

        hidden = new object[,]{
        { "함선",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 0, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,0f
        ,0f, ArmorType.일반//퍼센트, 공격 타입
        ,false , 0, 0f, 0f, 0f},//보잡, 퍼뎀 유형, 짭플, 치명타, 공격 범위
        { "이브",Array.Empty<ItemIngredient>(),
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        { "해결사",new []{new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("공돌이", ItemRank.희귀함, 1)},
        0, 0, 0, 0, 0, 0, 0f, 0f, 0, 0, 10, 0
        , 0f, 0, 0, 0, 0, 0f, 0f,5f
        ,0f, ArmorType.일반
        ,false , 0, 0f, 0f, 0f},
        };

        changed = new object[,]{

        };

        upperRanked = new object[,]{

        };

        table[(int)ItemRank.흔함] = common;
        table[(int)ItemRank.안흔함] = uncommon;
        table[(int)ItemRank.특별함] = special;
        table[(int)ItemRank.희귀함] = rare;
        table[(int)ItemRank.히든] = hidden;
        table[(int)ItemRank.전설적인] = legendary;
        table[(int)ItemRank.변화된] = changed;
        table[(int)ItemRank.상위] = upperRanked;


        SetItemSkeleton(ItemRank.흔함);

        SetItemSkeleton(ItemRank.안흔함);

        SetItemSkeleton(ItemRank.특별함);
        SetItemSkeleton(ItemRank.희귀함);

        SetItemSkeleton(ItemRank.히든);


        SetItemSkeleton(ItemRank.전설적인);
        SetItemSkeleton(ItemRank.변화된);
        SetItemSkeleton(ItemRank.상위);

        for (int rank = 0; rank < table.Length; rank++)
        {
            SetItemParent(rank);
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
    public void SetItemSkeleton(ItemRank rank)
    {

        object[,] grade = table[(int)rank];
        int rowCount = grade.GetLength(0);            // 아이템 개수

        for (int j = 0; j < rowCount; j++)               // 아이템 반복
        {
            Debug.Log($"{j}, {rank}");
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
            float Probability = (float)grade[j, 14];
            int monoPhysics = (int)grade[j, 15];
            int multiPhysics = (int)grade[j, 16];
            int monoMagic = (int)grade[j, 17];
            int multiMagic = (int)grade[j, 18];
            float monoStun = (float)grade[j, 19];
            float multiStun = (float)grade[j, 20];
            float range = (float)grade[j, 21];
            float Percent = (float)grade[j, 22];

            ArmorType armorType = (ArmorType)grade[j, 23];
            bool boss = (bool)grade[j, 24];
            int percentageCategory = (int)grade[j, 25];
            float doublePhysics = (float)grade[j, 26];
            float damageUp = (float)grade[j, 27];
            float AttackRange = (float)grade[j, 28];


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
                Probability,
                monoPhysics,
                multiPhysics,
                monoMagic,
                multiMagic,
                monoStun,
                multiStun,
                range,
                Percent,
                armorType,
                boss,
                percentageCategory,
                doublePhysics,
                damageUp,
                AttackRange);

            if (itemList == null)
                Debug.LogError("Error");

            itemList[(int)rank].Add(newItem);

            dict.Add((name, rank), itemList[(int)rank][itemList[(int)rank].Count - 1]);
        }
    }

    public void SetItemParent(int itemRank)
    {
        object[,] grade = table[itemRank];
        int rowCount = grade.GetLength(0);
        for (int j = 0; j < rowCount; j++)
        {
            Item targetItem = FindItem((string)grade[j, 0], (ItemRank)itemRank);
            for (int i = 0; i < targetItem.NecessaryItem.Count(); i++)
            {
                Item NecessaryItem = FindItem(targetItem.NecessaryItem[i].ItemName, targetItem.NecessaryItem[i].Rank);
                NecessaryItem.SetParent(targetItem);
            }
        }
    }

    public Item FindItem(string s, ItemRank rank)
    {
        return dict[(s, rank)];
    }

    public Item ChangeItem(Item item)
    {
        int rand = UnityEngine.Random.Range(1, itemList[(int)item.Rank].Count);
        Item ChangedItem = itemList[(int)item.Rank][rand];
        while (rand == ChangedItem)
        {
            rand = UnityEngine.Random.Range(1, itemList[(int)item.Rank].Count);
            ChangedItem = itemList[(int)item.Rank][rand];
        }
        ChangedItem.count++;
        string hex = ColorUtility.ToHtmlStringRGB(ItemManager.GetColor(item));
        GameManager.Instance.chat.Push($"<color=#{hex}>{item.Name}</color> 희귀함이 <color=#{hex}>{ChangedItem.Name}</color> 희귀함으로 대체되었습니다. ");
        GameManager.Instance.chat.Push($"남은 횟수 : {--ItemManager.RerollCount}");
        return ChangedItem;
    }

    public Item GetRandomItem(ItemRank rank, bool logOut = true)
    {
        int rand = UnityEngine.Random.Range(0, itemList[(int)rank].Count);

        Item item = itemList[(int)rank][rand];

        item.count++;
        if (item.count == 1)
        {
            GotItem.Enqueue(item);
            SetUnity(item);
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
        GameManager.Instance.scrollView.ImageInit(currentItem[GameManager.Instance.Action.targetNumber]);
        ItemManager.Clear(ItemManager.GetEditItem(), false);
        return item;
    }
    public void GetSoulParts(int count)
    {
        if (count <= 0) return;
        FindItem("영혼 파편", ItemRank.All).count += count;
        if (ItemManager.isAllToggle) ChangeSouls();
        else
        {
            ItemManager.chat.Push($"<color=White>영혼 파편</color> {count}개 획득."); 
            ItemManager.Clear(ItemManager.GetEditItem(), false);
        }


    }

    public void GetAll(int count)
    {
        if (count <= 0) return;
        FindItem("만물석", ItemRank.All).count += count;
        ItemManager.Clear(ItemManager.GetEditItem(), false);
        ItemManager.chat.Push($"<color=White>만물석</color> {count}개 획득.");
    }
    public void GetMemoriesParts(int count)
    {
        if (count <= 0) return;
        FindItem("기억 조각", ItemRank.All).count += count;
        ItemManager.Clear(ItemManager.GetEditItem(), false);
        ItemManager.chat.Push($"<color=Yellow>기억 조각</color> {count}개 획득.");
    }

    public void GetSoulMana(int count)
    {
        if (count <= 0) return;
        int Mana = Mathf.Min((int)((GameManager.Instance.GetRound() * 1.5f) + 20) * count, 100);
        GameManager.Instance.energy.currentEnergy += Mana;
        ItemManager.Clear(ItemManager.GetEditItem(), false);
    }

    public void ChangeSouls()
    {
        Item item = FindItem("영혼 파편", ItemRank.All);
        int count = item.count;
        item.count = 0;
        switch (Log2(ItemManager.SetSoulParts))
        {
            case 0:
                for (int i = 0; i < count; i++)
                {
                    int rand = UnityEngine.Random.Range(0, 10000);
                    if (rand < 24)
                    {
                        ItemManager.SetUpState(FindItem("함선", ItemRank.히든));
                        string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.skyBlue);
                        ItemManager.chat.Push($"<color=#{hex}>히든</color> 등급의 함선 획득.");
                    }
                    else GetRandomItem(ItemRank.흔함);
                }
                break;
            case 1:
                for (int i = 0; i < count; i++)
                {
                    int rand = UnityEngine.Random.Range(0, 100);
                    if (rand < 66)
                        GetMemoriesParts(1);
                    else GameManager.Instance.chat.Push($"<color=Yellow>기억 조각</color> 획득에 실패했습니다.");
                }
                break;
            case 2:
                GetSoulMana(count);
                break;
        }
    }

    private void Token(Item token)
    {
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand < 80) ItemManager.list.GetRandomItem(ItemRank.특별함);
        else ItemManager.list.GetRandomItem(ItemRank.희귀함);
        token.count -= 3;
        ItemManager.Clear(ItemManager.editItem, false);
    }

    public bool CombineItem(Item item)
    {
        if (item.Name == "행운의 토큰" && item.Rank == ItemRank.희귀함)
        {
            if (item.count >= 3)
            {
                Token(item);
                return true;
            }
            else return false;
        }

        if (item.NecessaryItem.Count() == 0) return false;
        bool enough = true;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            if (nItem.Count > FindItem(nItem.ItemName, nItem.Rank).count)
            {
                enough = false;
                break;
            }
        }
        if (enough)
        {
            foreach (ItemIngredient nItem in item.NecessaryItem)
            {
                Item findItem = FindItem(nItem.ItemName, nItem.Rank);
                findItem.count -= nItem.Count;
                if (findItem.count == 0)
                {
                    GotItem.Remove(findItem);
                    DeleteUnrankedItem(findItem);
                }
            }

            item.count++;
            if (item.count == 1)
            {
                GotItem.Enqueue(item);
                SetUnity(item);
            }
            ItemManager.Clear(null, false);
            GameManager.Instance.scrollView.ImageInit(currentItem[GameManager.Instance.Action.targetNumber]);
        }
        return enough;
    }

    public Dictionary<(string, ItemRank), int> CombineAllItem(Item item, bool combine)
    {
        
        Dictionary<(string, ItemRank), int> itemDict = new Dictionary<(string, ItemRank), int>();
        if (item.Name == "행운의 토큰" && item.Rank == ItemRank.희귀함 && combine == true)
        {
            if (item.count >= 3)
            {
                Token(item);
            }
            return itemDict;
        }

        if (item.NecessaryItem.Count() == 0) return itemDict;
        bool isOkay = false;
        foreach (ItemIngredient nItem in item.NecessaryItem)
        {
            itemDict.Add((FindItem(nItem.ItemName, nItem.Rank), nItem.Rank), nItem.Count);
        }

        while (!isOkay)
        {
            isOkay = true;
            foreach (KeyValuePair<(string, ItemRank), int> kvp in itemDict.ToList())
            {

                string Key = kvp.Key.Item1;
                ItemRank Key2 = kvp.Key.Item2;
                ItemIngredient[] NecessaryItem = dict[(Key, Key2)].NecessaryItem;
                if (NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                if (FindItem(NecessaryItem[0].ItemName, NecessaryItem[0].Rank).NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                if (dict[(kvp.Key.Item1, kvp.Key.Item2)].count < kvp.Value)
                {
                    isOkay = false;
                    int necessaryCount = Mathf.Max(kvp.Value - dict[(Key, Key2)].count, 0);
                    foreach (ItemIngredient nItem in dict[(Key, Key2)].NecessaryItem)
                    {
                        if (itemDict.ContainsKey((nItem.ItemName, nItem.Rank)))
                        {
                            itemDict[(nItem.ItemName, nItem.Rank)] += necessaryCount * nItem.Count;
                        }
                        else
                            itemDict.Add((nItem.ItemName, nItem.Rank), necessaryCount * nItem.Count);
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
                    if (items.count == 0)
                    {
                        GotItem.Remove(items);
                        DeleteUnrankedItem(items);
                    }
                }

                item.count++;

                if (item.count == 1)
                {
                    GotItem.Enqueue(item);
                    SetUnity(item);
                }

                ItemManager.Clear(null, false);
                GameManager.Instance.scrollView.ImageInit(currentItem[GameManager.Instance.Action.targetNumber]);
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
            if (nItem.ItemName != "기억 조각")
                itemDict.Add((nItem.ItemName, nItem.Rank), nItem.Count);
        }

        while (!isOkay)
        {
            isOkay = true;
            foreach (KeyValuePair<(string, ItemRank), int> kvp in itemDict.ToList())
            {
                string Key = kvp.Key.Item1;
                ItemRank key2 = kvp.Key.Item2;
                Debug.Log($"{Key}, {key2}");
                ItemIngredient[] NecessaryItem = dict[(Key, key2)].NecessaryItem;
                if (NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                if (FindItem(NecessaryItem[0].ItemName, NecessaryItem[0].Rank).NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                Item targetItem = FindItem(NecessaryItem[0].ItemName, NecessaryItem[0].Rank);
                isOkay = false;
                int necessaryCount = kvp.Value;
                foreach (ItemIngredient nItem in dict[(Key, key2)].NecessaryItem)
                {
                    if (itemDict.ContainsKey((nItem.ItemName, nItem.Rank)))
                    {
                        itemDict[(nItem.ItemName, nItem.Rank)] += necessaryCount * nItem.Count;
                    }
                    else
                        itemDict.Add((nItem.ItemName, nItem.Rank), necessaryCount * nItem.Count);
                }
                itemDict[(Key, key2)] -= necessaryCount;
                if (itemDict[(Key, key2)] <= 0)
                    itemDict.Remove((Key, key2));
            }
        }
        return itemDict;
    }
    public void SetUnity(Item item)
    {
        if (item.Rank != ItemRank.상위 && item.BossPercentAttack == false && item.Percent != 0 && item.PercentCategory == 2)
        {
            UnitySet(0, item);
        }
        if (item.Rank != ItemRank.상위 && item.BossPercentAttack == true)
        {
            UnitySet(1, item);
        }
        if (item.Rank != ItemRank.상위 && item.MultiStun != 0)
        {
            UnitySet(2, item);
        }
        if (item.Rank != ItemRank.상위 && item.Percent != 0 && item.PercentCategory == 1)
        {
            UnitySet(3, item);
        }
        if (item.Rank == ItemRank.상위)
        {
            UnitySet(4, item);
        }
        if (item.Rank != ItemRank.상위 && (
            item.Percent == 0 &&
            item.BossPercentAttack == false &&
            item.MultiStun == 0))
        {
            UnitySet(5, item);
        }
    }

    private void UnitySet(int count, Item item)
    {
        currentItem[count].Enqueue(item);
        SetRankedItem(item, count);
        StatsUp(item, count);
    }

    public void SetRankedItem(Item item, int number)
    {
        if (item.Rank <= ItemRank.흔함) return;
        int shift = item.Rank - ItemRank.안흔함;
        if (shift < 0 || shift >= 31)
        {
            Debug.LogError($"Rank shift 값이 비정상입니다: {shift}");
            return;
        }
        int bit = 1 << shift;

        for (int i = 1; i <= bit; i <<= 1)
        {
            if ((rankOn[number] & i) == 0)
            {
                switch ((ItemRank)(Mathf.Log(i, 2) + (int)ItemRank.안흔함))
                {
                    case ItemRank.안흔함:
                        Stats.damage[number] += 100;
                        Stats.attackDelay[number] = 0.95f;
                        break;
                    case ItemRank.특별함:
                        Stats.damage[number] += 400;
                        Stats.attackDelay[number] = 0.9f;
                        Stats.attackSpeedBonus += 25f;
                        break;
                    case ItemRank.희귀함:
                        Stats.damage[number] += 4500;
                        Stats.attackDelay[number] = 0.85f;
                        Stats.attackSpeedBonus += 45f;
                        break;
                    case ItemRank.전설적인:
                        Stats.damage[number] += 9000;
                        Stats.attackDelay[number] = 0.70f;
                        Stats.attackSpeedBonus += 215f;
                        break;

                }
                rankOn[number] |= (byte)i;
            }

            if ((cannonRankOn & i) == 0)
            {
                switch ((ItemRank)(Mathf.Log(i, 2) + (int)ItemRank.안흔함))
                {
                    case ItemRank.안흔함:
                        Cannon.SetCannon(20, 5);
                        break;
                    case ItemRank.특별함:
                        Cannon.SetCannon(80, 10);
                        break;
                    case ItemRank.희귀함:
                        Cannon.SetCannon(920, 20);
                        break;
                    case ItemRank.전설적인:
                        Cannon.SetCannon(3000, 30);
                        break;

                }
                cannonRankOn |= (byte)i;
            }
        }
    }

    private void DeleteRankedItem(Item item, int number)
    {
        if (item.Rank <= ItemRank.흔함) return;
        int shift = item.Rank - ItemRank.안흔함;
        if (shift < 0 || shift >= 31)
        {
            Debug.LogError($"Rank shift 값이 비정상입니다: {shift}");
            return;
        }
        int bit = 1 << shift;

        for (int i = (int)ItemRank.상위; i >= bit; i >>= 1)
        {
            if ((rankOn[number] & i) != 0)
            {
                switch ((ItemRank)(Mathf.Log(i, 2) + (int)ItemRank.안흔함))
                {
                    case ItemRank.안흔함:
                        Stats.damage[number] -= 100;
                        Stats.attackDelay[number] = 1f;
                        break;
                    case ItemRank.특별함:
                        Stats.damage[number] -= 400;
                        Stats.attackSpeedBonus -= 25f;
                        break;
                    case ItemRank.희귀함:
                        Stats.damage[number] -= 4500;
                        Stats.attackSpeedBonus -= 45f;
                        break;
                    case ItemRank.전설적인:
                        Stats.damage[number] -= 9000;
                        Stats.attackSpeedBonus -= 215f;
                        break;
                    case ItemRank.상위:
                        break;

                }
                rankOn[number] &= (byte)~i;
            }

            if ((cannonRankOn & i) != 0)
            {
                switch ((ItemRank)(Mathf.Log(i, 2) + (int)ItemRank.안흔함))
                {
                    case ItemRank.안흔함:
                        Cannon.SetCannon(-20, -5);
                        break;
                    case ItemRank.특별함:
                        Cannon.SetCannon(-80, -10);
                        break;
                    case ItemRank.희귀함:
                        Cannon.SetCannon(-920, -20);
                        break;
                    case ItemRank.전설적인:
                        Cannon.SetCannon(-3000, -30);
                        break;

                }
                cannonRankOn &= (byte)~i;;
            }
        }
        
    }

    public void DeleteUnrankedItem(Item item)
    {
        for (int i = 0; i < GameManager.Instance.Action.TargetNumberMax; i++)
        {
            bool t = currentItem[i].Remove(item);
            if (t)
            {
                StatsDown(item, i);
                DeleteRankedItem(item, i);  
            }
        }
    }

    public void StatsUp(Item item, int number)
    {
        if (Stats != null)
        {
            Stats.damage[number] += item.AttackPower;
            Stats.attackSpeedBonus += item.AttackSpeed;
            Stats.HealthRegen[number] += DataManager.Instance.RoundX(item.HealthRegen, 3);
            Stats.manaRegen[number] += DataManager.Instance.RoundX(item.ManaRegen, 3);
            Stats.doublePhysics[number] += item.DoublePhysics;
            Stats.TrueDamage[number] += item.TrueDamage;
            Stats.neutralizeDefense += item.NeutralizeDefense;
            Stats.Radius[number] = Mathf.Max(Stats.Radius[number], item.AttackRange);
            Cannon.SetCannon(item.TowerDamage, item.TowerAttackSpeed);
            if (item.NecessaryItem.Count() == 0)
                return;

            if (item.Rank == ItemRank.희귀함 && !GameManager.Instance.RareGet && GameManager.Instance.GetRound() <= 7)
            {
                FindItem("만물석", ItemRank.All).count++;
                GameManager.Instance.RareGet = true;
            }
        }
    }
    public void StatsDown(Item item, int number)
    {
        Stats.damage[number] -= item.AttackPower;
        Stats.attackSpeedBonus -= item.AttackSpeed;
        Stats.HealthRegen[number] -= item.HealthRegen;
        Stats.manaRegen[number] -= item.ManaRegen;
        Stats.TrueDamage[number] -= item.TrueDamage;
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

                image.transform.Find("number1").gameObject.SetActive(false);
                image.transform.Find("number2").gameObject.SetActive(false);

            }
        if (outlines != null)
        {
            foreach (UnityEngine.UI.Outline outline in outlines)
                outline.effectDistance = new Vector2(0, 0);

        }
    }

    public float SetSkill(Actor actor, Item item)//확률, 단일 물리 데미지, 범위 물리 데미지, 단일 마법데미지, 범위 마법 데미지, 단일 스턴, 범위 스턴, 스킬 범위,단일 퍼센트 데미지, 끝딜 퍼센트 데미지, 전퍼, 현퍼, 잃퍼
    {
        string name = item.Name;

        ItemRank rank = item.Rank;

        float Probability = item.Probability;

        int MonoPhysics = item.MonoPhysics;
        int MultiPhysics = item.MultiPhysics;
        int MonoMagic = item.MonoMagic;
        int MultiMagic = item.MultiMagic;
        float MonoStun = item.MonoStun;
        float MultiStun = item.MultiStun;
        float Range = item.Range;
        float Percent = item.Percent;
        float DamageUp = item.DamageUp;
        int PercentageCategory = item.PercentCategory;
        bool BossPercentAttack = item.BossPercentAttack;

        float DamagePercentage = 0f;

        int rand = UnityEngine.Random.Range(0, 10000);
                Debug.Log(Probability * 100);

        for(int i=0;i<item.count;i++)
        if (rand < Mathf.Ceil(Probability * 100))
        {
            actor.TakeDamageAll(MultiPhysics, MonoPhysics, Range
            , ArmorType.고정, true, 0, Stats.neutralizeDefense, 0);

            actor.TakeDamageAll(MultiMagic, MonoMagic, Range
            , ArmorType.마법, false, 0, 0, 0);

            actor.TakeStunAll(MultiStun, MonoStun, Range);

            if (Range > 0)
                actor.TakeDamageAll(Percent, 0, Range, ArmorType.마법, false, 0, 0, PercentageCategory);
            else
            {
                actor.TakeDamageAll(0, Percent, 0, item.AttackType, false, 0, 0, PercentageCategory);
            }

            if (actor.GetComponent<EnemyStats>() != null && BossPercentAttack == true)
            {
                actor.GetComponent<EnemyStats>().TakeDamage(Percent, ArmorType.마법, false, 0, PercentageCategory, BossPercentAttack);
            }

            DamagePercentage += DamageUp;
        }

        rand = UnityEngine.Random.Range(0, 10000);

        float MultiPercentage = (1f - Mathf.Pow(1f - Probability / 100f, item.count)) * 100; // 다중 확률
        
        if (MultiPercentage * 100 >= rand)
        {
            switch (name)
            {
                case "좀비":
                    if (actor.isDead)
                        FindItem(name, rank).count++;
                    ItemManager.Clear(ItemManager.editItem, false);
                    GameManager.Instance.scrollView.GetComponent<ItemScrollView>().ImageInit(currentItem[GameManager.Instance.Action.targetNumber]);
                    break;
            }
        }
        return DamagePercentage;
    }
}