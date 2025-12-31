using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MyMathf;
using System.Text;

public enum StatI { atkPower, addAtkPower, nDefense, mAtkPower,mDebuffer,moveSpeed,attackSpeed,towerDamage,towerAttackSpd,monoPhysics,
multiPhysics,monoMagic,multiMagic,percentageCategory,percentKind, blink,regenRange,range, Count }   // int 스탯들
public enum StatF { tDamage, hpRegen, mpRegen,monoStun,multiStun,Probability,Percent,doublePhysics,damageUp,AttackRange,regenStun, Count } // float 스탯들
public enum StatB { boss,regen, Count } // bool 스탯들

public enum PercentageCategory{max, current, loss, count};
public enum PercentKind{physics, magics, trueDamage,  explosions, count};
public enum RegenKind{HealthRegen, manaRegen, count};

public enum DamageKind{physics, magics, count};
public enum DamageTarget{MonoDamage, MultiDamage, count};

static class ItemDefaults
{
    public static readonly int[] I = new int[(int)StatI.Count];
    public static readonly float[] F = new float[(int)StatF.Count];
    public static readonly bool[] B = new bool[(int)StatF.Count];
}

public sealed class ItemDef
{
    public string Name; 
    public ItemIngredient[] Ingredients;
    public int[] I;
    public float[] F;
    public bool[] B;
    public ArmorType Armor = ArmorType.일반;
    public bool Flag;
    public float[,,] regenPercent = new float[(int)RegenKind.count, (int)PercentageCategory.count,(int)PercentKind.count];
    public int[,,] regenDamage = new int[(int)RegenKind.count, (int)DamageKind.count, (int)DamageTarget.count];

    public ItemDef(string name, ItemIngredient[] ing)
    {
        Name = name;
        Ingredients = ing;
        I = (int[])ItemDefaults.I.Clone();
        F = (float[])ItemDefaults.F.Clone();
        B = (bool[])ItemDefaults.B.Clone();
    }

    public int Get(StatI s) => I[(int)s];
    public float Get(StatF s) => F[(int)s];
    public bool Get(StatB s) => B[(int)s];
    public float Get(RegenKind s1, PercentageCategory s2, PercentKind s3) => regenPercent[(int)s1, (int)s2, (int)s3];
    public int Get(RegenKind s1, DamageKind s2, DamageTarget s3) => regenDamage[(int)s1, (int)s2, (int)s3];
    public ItemDef Set(StatI stat, int v) { I[(int)stat] = v; return this; }
    public ItemDef Set(StatF stat, float v) { F[(int)stat] = v; return this; }
    public ItemDef Set(StatF stat, bool v) { B[(int)stat] = v; return this; }
    public ItemDef Set(RegenKind stat1, PercentageCategory stat2, PercentKind stat3, float v) {regenPercent[(int)stat1,(int)stat2, (int)stat3] = v; return this;}
    public ItemDef Set(RegenKind stat1,DamageKind stat2, DamageTarget stat3, int Damage) {regenDamage[(int)stat1,(int)stat2, (int)stat3] = Damage; return this;}

    public ItemDef SetArmor(ArmorType type) {Armor = type; return this;}
}
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
    public float TrueDamage { get; private set; }
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
    public PercentageCategory PercentCategory {get; private set;}
    public PercentKind PercentKind {get; private set;}
    public float DoublePhysics { get; private set; }
    public float DamageUp { get; private set; }
    public float AttackRange { get; private set; }

    public int Blink {get; private set;}
    public List<Item> parents { get; private set; }
    public Dictionary<(string, ItemRank), int> ItemIngredientDict;

    public float[,,] RegenPercent = new float[(int)RegenKind.count, (int)PercentageCategory.count,(int)PercentKind.count];
    public int[,,] RegenDamage = new int[(int)RegenKind.count, (int)DamageKind.count, (int)DamageTarget.count];

    public bool HaveRegenSkill = false;

    public float RegenStun{get; private set;}

    public float RegenRange{get; private set;}

    public Item(string name, ItemIngredient[] neccesaryItem, ItemRank rank, byte id, Sprite resource,
    int attackPower, int additionalAttackPower, int neutralizeDefense, int magicalBuffer, int magicalDebuffer, float trueDamage, float healthRegen, float manaRegen, int moveSpeed, int attackSpeed, int towerDamage, int towerAttackSpeed,
    float probability, int monoPhysics, int multiPhysics, int monoMagic, int multiMagic, float monoStun, float multiStun, float range,
    float percent, ArmorType attackType, bool bossPercentAttack, PercentageCategory percentCategory, PercentKind percentKind,
    float[,,] regenPercentAll, int[,,] regenDamage,
    float doublePhysics, float damageUp, float attackRange, int blink, float regenStun, float regenRange)
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
        PercentKind = percentKind;
        RegenPercent = regenPercentAll;
        RegenDamage = regenDamage;

        AttackType = attackType;
        BossPercentAttack = bossPercentAttack;

        DoublePhysics = doublePhysics;
        DamageUp = damageUp;

        AttackRange = attackRange;

        Blink = blink;
        RegenStun = regenStun;

        RegenRange = regenRange;

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
    private int[] rankOn;
    private int cannonRankOn;

    public PriorityQueue<Item>[] currentItem;

    public const int gotItemCount = 15;
    public PriorityQueue<Item> GotItem = new PriorityQueue<Item>(gotItemCount);
    public PriorityQueue<Item> DebuffItem = new();

    List<ItemDef> all = new List<ItemDef>{
        new ItemDef("만물석", Array.Empty<ItemIngredient>()),
        new ItemDef("기억 조각", Array.Empty<ItemIngredient>()),
        new ItemDef("영혼 파편", Array.Empty<ItemIngredient>()),
    };

    List<ItemDef> common;
    List<ItemDef> uncommon;
    List<ItemDef> special;
    List<ItemDef> rare;
    List<ItemDef>  legendary;
    List<ItemDef> hidden;
    List<ItemDef> changed;
    List<ItemDef> upperRanked;

    public List<ItemDef>[] table;

    public List(PlayerStats stats, CannonManager cannon, ItemManager itemManager)
    {
        currentItem = new PriorityQueue<Item>[DataManager.targetNumberMax];
        rankOn = new int[DataManager.targetNumberMax];
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
        outlines = ItemManager.GetOutlines();

        table = new List<ItemDef>[(int)ItemRank.상위 + 1];
        table[(int)ItemRank.All] = all;
        SetItemSkeleton(ItemRank.All);


        common = new List<ItemDef>{
            new ItemDef("단검", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }).Set(StatI.atkPower, 10),
            
            new ItemDef("마법봉", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }),
            
            new ItemDef("소울스톤", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }).Set(StatF.mpRegen, 0.01f),
            
            
            new ItemDef("리버스스톤", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }),
            
            new ItemDef("망토", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }).Set(StatI.mAtkPower, 5)
            .Set(StatI.towerDamage, 1)
            .Set(StatI.towerAttackSpd, 1),
            
            new ItemDef("고기", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }).Set(StatF.hpRegen, 0.01f),
            
            new ItemDef("철퇴", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }).Set(StatI.atkPower, 20),
            
            new ItemDef("신발", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }),
            
            new ItemDef("장갑", new []{
                new ItemIngredient("만물석", ItemRank.All, 1),
            }),
        };


        uncommon = new List<ItemDef>{
            new ItemDef("창", new []{new ItemIngredient("단검", ItemRank.흔함, 1), new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 15)
                .Set(StatI.nDefense, 1),

            new ItemDef("생명의 샘물", new []{ new ItemIngredient("마법봉", ItemRank.흔함,1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)})
                .Set(StatI.mAtkPower, 1)
                .Set(StatF.mpRegen, 0.02f),

            new ItemDef("끈끈이", new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
                .Set(StatI.moveSpeed, 1),

            new ItemDef("마법사", new [] {new ItemIngredient("마법봉", ItemRank.흔함, 1), new ItemIngredient("망토", ItemRank.흔함, 1)})
                .Set(StatI.mAtkPower, 1)
                .Set(StatI.mDebuffer, 1),

            new ItemDef("로봇 팔", new [] {new ItemIngredient("장갑", ItemRank.흔함, 1), new ItemIngredient("철퇴", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 50),

            new ItemDef("도적", new [] {new ItemIngredient("단검", ItemRank.흔함, 1), new ItemIngredient("장갑", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 20)
                .Set(StatI.attackSpeed, 1),

            new ItemDef("인간", new [] {new ItemIngredient("고기", ItemRank.흔함, 1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)})
                .Set(StatF.hpRegen, 0.02f)
                .Set(StatF.mpRegen, 0.02f),

            new ItemDef("날개", new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("망토", ItemRank.흔함, 1)}),

            new ItemDef("전사", new [] {new ItemIngredient("철퇴", ItemRank.흔함, 1), new ItemIngredient("고기", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 100)
                .Set(StatI.attackSpeed, -5)
                .Set(StatF.hpRegen, 0.03f),

            new ItemDef("사신", new [] {new ItemIngredient("망토", ItemRank.흔함, 1), new ItemIngredient("소울스톤", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 70),

            new ItemDef("파이어볼", new [] {new ItemIngredient("마법봉", ItemRank.흔함, 1), new ItemIngredient("철퇴", ItemRank.흔함, 1)})
                .Set(StatI.mAtkPower, 3),

            new ItemDef("좀비", new [] {new ItemIngredient("리버스스톤", ItemRank.흔함, 1), new ItemIngredient("고기", ItemRank.흔함, 1)})
                .Set(StatI.monoMagic, 100)
                .Set(StatF.Probability, 20f),

            new ItemDef("갑옷", new [] {new ItemIngredient("신발", ItemRank.흔함, 1), new ItemIngredient("장갑", ItemRank.흔함, 1)})
                .Set(StatI.atkPower, 30),
        };

// ===== special (32 items) =====
    special = new List<ItemDef>{
        new ItemDef("롱소드", new []{new ItemIngredient("단검", ItemRank.흔함, 3)})
            .Set(StatI.atkPower, 50)
            .Set(StatF.AttackRange, 100f),

        new ItemDef("블링크", new []{new ItemIngredient("신발", ItemRank.흔함, 3)})
            .Set(StatI.towerDamage, 100)
            .Set(StatI.blink, 200),

        new ItemDef("만찬", new []{new ItemIngredient("고기", ItemRank.흔함, 3)})
            .Set(StatF.hpRegen, 0.05f)
            .Set(RegenKind.HealthRegen,DamageKind.physics, DamageTarget.MonoDamage, 80000),

        new ItemDef("광선", new []{new ItemIngredient("마법봉", ItemRank.흔함, 3)})
            .Set(StatI.towerAttackSpd, 10)
            .Set(StatF.tDamage, 0.05f),

        new ItemDef("아담의 영혼", new []{new ItemIngredient("소울스톤", ItemRank.흔함, 3)})
            .Set(StatF.mpRegen, 0.05f)
            .Set(RegenKind.manaRegen, DamageKind.magics, DamageTarget.MonoDamage, 50000),

        new ItemDef("군단", new []{new ItemIngredient("망토", ItemRank.흔함, 3)})
            .Set(StatI.towerDamage, 10)
            .Set(StatI.towerAttackSpd, 10),

        new ItemDef("대포알", new []{new ItemIngredient("철퇴", ItemRank.흔함, 3)})
            .Set(StatI.towerDamage, 20),

        new ItemDef("빅뱅", new []{new ItemIngredient("리버스스톤", ItemRank.흔함, 3)})
            .Set(StatI.atkPower, -1)
            .Set(StatI.nDefense, -1)
            .Set(StatI.mAtkPower, 2)
            .Set(StatI.mDebuffer, 2)
            .Set(StatI.attackSpeed, -1)
            .Set(StatF.tDamage, 0.03f)
            .Set(StatF.hpRegen, -1f)
            .Set(StatF.mpRegen, -1f),

        new ItemDef("민첩함", new []{new ItemIngredient("장갑", ItemRank.흔함, 3)})
            .Set(StatI.attackSpeed, 3),

        new ItemDef("메카닉", new []{new ItemIngredient("로봇 팔", ItemRank.안흔함, 2),new ItemIngredient("단검", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, 70)
            .Set(StatF.mpRegen, 0.1f),

        new ItemDef("사이보그", new []{new ItemIngredient("로봇 팔", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1)})
            .Set(StatI.atkPower, 50)
            .Set(StatF.hpRegen, 0.1f)
            .Set(StatF.mpRegen, 0.1f),

        new ItemDef("헌터", new []{new ItemIngredient("전사", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, 200)
            .Set(StatI.moveSpeed, 5)
            .Set(StatI.attackSpeed, -10)
            .Set(StatF.mpRegen, 0.3f),

        new ItemDef("프로즌", new []{new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("소울스톤", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, -50)
            .Set(StatI.mAtkPower, 3)
            .Set(StatI.mDebuffer, 3)
            .Set(StatI.towerDamage, 10)
            .Set(StatF.hpRegen, 0.3f),

        new ItemDef("전염병", new []{new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("끈끈이", ItemRank.안흔함, 1),new ItemIngredient("철퇴", ItemRank.흔함, 1)})
            .Set(StatI.nDefense, 3)
            .Set(StatI.moveSpeed, 5)
            .Set(StatI.towerDamage, 10),

        new ItemDef("해독제", new []{new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
            .Set(StatF.mpRegen, 0.5f),

        new ItemDef("앨리스", new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("소울스톤", ItemRank.흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
            .Set(StatF.tDamage, 0.1f)
            .Set(StatF.hpRegen, 0.2f)
            .Set(StatF.mpRegen, 0.2f),

        new ItemDef("용기병", new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("철퇴", ItemRank.흔함, 1)})
            .Set(StatI.percentageCategory, (int)PercentageCategory.current)
            .Set(StatI.percentKind, 3)
            .Set(StatF.Percent, 0.1f)
            .Set(StatF.Probability, 10f),

        new ItemDef("강철", new []{new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("고기", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 10)
            .Set(StatF.tDamage, 0.1f),

        new ItemDef("영혼 낫", new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("도적", ItemRank.안흔함, 1),new ItemIngredient("리버스스톤", ItemRank.흔함, 1)})
            .Set(StatI.mAtkPower, 5),

        new ItemDef("도끼", new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, 80)
            .Set(StatI.monoPhysics, 500)
            .Set(StatF.monoStun, 1f)
            .Set(StatF.Probability, 10f),

        new ItemDef("죽음", new []{new ItemIngredient("사신", ItemRank.안흔함, 1),new ItemIngredient("인간", ItemRank.안흔함, 1),new ItemIngredient("고기", ItemRank.흔함, 1)})
            .Set(StatI.monoMagic, 300)
            .Set(StatF.Probability, 50f),

        new ItemDef("버서커", new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("전사", ItemRank.안흔함, 1)})
            .Set(StatI.atkPower, 200),

        new ItemDef("레이저 포", new []{new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("마법사", ItemRank.안흔함, 1),new ItemIngredient("망토", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 100)
            .Set(StatI.towerAttackSpd, 10),

        new ItemDef("관통", new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("갑옷", ItemRank.안흔함, 1),new ItemIngredient("단검", ItemRank.흔함, 1)})
            .Set(StatF.tDamage, 0.05f),

        new ItemDef("미래", new []{new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("로봇 팔", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)})
            .Set(StatF.mpRegen, 0.1f),

        new ItemDef("용접", new []{new ItemIngredient("끈끈이", ItemRank.안흔함, 2),new ItemIngredient("장갑", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 50),

        new ItemDef("마법 화살", new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("망토", ItemRank.흔함, 1)})
            .Set(StatI.monoMagic, 1250)
            .Set(StatF.Probability, 10f),

        new ItemDef("금화", new []{new ItemIngredient("도적", ItemRank.안흔함, 2),new ItemIngredient("망토", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 100)
            .Set(StatI.towerAttackSpd, 10),

        new ItemDef("레이피어", new []{new ItemIngredient("창", ItemRank.안흔함, 1),new ItemIngredient("전사", ItemRank.안흔함, 1),new ItemIngredient("신발", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, 100)
            .Set(StatI.monoPhysics, 1000)
            .Set(StatF.Probability, 15f),

        new ItemDef("화산", new []{new ItemIngredient("날개", ItemRank.안흔함, 1),new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("신발", ItemRank.흔함, 1)})
            .Set(StatI.moveSpeed, 5)
            .Set(StatF.hpRegen, 0.1f),

        new ItemDef("영생약", new []{new ItemIngredient("생명의 샘물", ItemRank.안흔함, 1),new ItemIngredient("좀비", ItemRank.안흔함, 1),new ItemIngredient("단검", ItemRank.흔함, 1)})
            .Set(StatF.hpRegen, 0.1f)
            .Set(StatF.mpRegen, 0.1f),

        new ItemDef("표창", new []{new ItemIngredient("도적", ItemRank.안흔함, 1),new ItemIngredient("끈끈이", ItemRank.안흔함, 1),new ItemIngredient("장갑", ItemRank.흔함, 1)})
            .Set(StatI.atkPower, 100)
            .Set(StatF.tDamage, 0.02f),
    };


// ===== rare (34 items) =====

    rare = new List<ItemDef>{
        new ItemDef("행운의 토큰", Array.Empty<ItemIngredient>()),

        new ItemDef("전쟁", new []{new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("군단", ItemRank.특별함, 1)})
            .Set(StatI.atkPower, 1000)
            .Set(StatF.tDamage, 0.2f)
            .Set(StatF.hpRegen, 0.3f)
            .Set(StatF.mpRegen, 0.3f),

        new ItemDef("차원 거울", new []{new ItemIngredient("죽음", ItemRank.특별함, 1),new ItemIngredient("아담의 영혼", ItemRank.특별함, 1),new ItemIngredient("빅뱅", ItemRank.특별함, 1)})
            .Set(StatF.multiStun, 10f)
            .Set(StatI.range, 500)
            .Set(StatF.Probability, 10f),

        new ItemDef("타이탄", new []{new ItemIngredient("메카닉", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 1000)
            .Set(StatI.towerAttackSpd, 30),

        new ItemDef("영웅", new []{new ItemIngredient("도끼", ItemRank.특별함, 1),new ItemIngredient("버서커", ItemRank.특별함, 1),new ItemIngredient("죽음", ItemRank.특별함, 1)})
            .Set(StatI.atkPower, 5000),

        new ItemDef("탱크", new []{new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 1000)
            .Set(StatI.towerAttackSpd, 30),

        new ItemDef("대마법사", new []{new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("마법 화살", ItemRank.특별함, 1),new ItemIngredient("아담의 영혼", ItemRank.특별함, 1)})
            .Set(StatI.multiMagic, 10000)
            .Set(StatI.range, 500)
            .Set(StatF.Probability, 12.5f),

        new ItemDef("웜홀", new []{new ItemIngredient("블링크", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1),new ItemIngredient("버서커", ItemRank.특별함, 1)})
            .Set(StatF.tDamage, 0.15f),

        new ItemDef("공돌이", new []{new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("메카닉", ItemRank.특별함, 1),new ItemIngredient("용기병", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("플라즈마 광선", new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("레이저 포", ItemRank.특별함, 1),new ItemIngredient("광선", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("무기의 달인", new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("도끼", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("과학자", new []{new ItemIngredient("해독제", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("되살아난 영웅", new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("저격수", new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("광선", ItemRank.특별함, 1),new ItemIngredient("블링크", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("도적", new []{new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1),new ItemIngredient("사신", ItemRank.안흔함, 1), new ItemIngredient("신발", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("군인", new []{new ItemIngredient("버서커", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1),new ItemIngredient("강철", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("굶주림", new []{new ItemIngredient("만찬", ItemRank.특별함, 1),new ItemIngredient("영생약", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("엑스칼리버", new []{new ItemIngredient("롱소드", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("도끼", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("앨리스", new []{new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("캐논쉽", new []{new ItemIngredient("군단", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("메카닉", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("추적자", new []{new ItemIngredient("용기병", ItemRank.특별함, 1),new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("바이오 로봇", new []{new ItemIngredient("영생약", ItemRank.특별함, 1),new ItemIngredient("사이보그", ItemRank.특별함, 1),new ItemIngredient("용접", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("행성", new []{new ItemIngredient("앨리스", ItemRank.특별함, 1),new ItemIngredient("대포알", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("좀비", new []{new ItemIngredient("해독제", ItemRank.특별함, 1),new ItemIngredient("민첩함", ItemRank.특별함, 1),new ItemIngredient("만찬", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("활", new []{new ItemIngredient("마법 화살", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("해독제", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("합금", new []{new ItemIngredient("광선", ItemRank.특별함, 1),new ItemIngredient("용접", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("부메랑", new []{new ItemIngredient("표창", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("만찬", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("사신", new []{new ItemIngredient("죽음", ItemRank.특별함, 1),new ItemIngredient("금화", ItemRank.특별함, 1),new ItemIngredient("전염병", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("꿰뚫는 창", new []{new ItemIngredient("관통", ItemRank.특별함, 1),new ItemIngredient("용기병", ItemRank.특별함, 1),new ItemIngredient("화산", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("아담", new []{new ItemIngredient("아담의 영혼", ItemRank.특별함, 1),new ItemIngredient("미래", ItemRank.특별함, 1),new ItemIngredient("영생약", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("헌터 국왕", new []{new ItemIngredient("헌터", ItemRank.특별함, 1),new ItemIngredient("레이피어", ItemRank.특별함, 1),new ItemIngredient("빅뱅", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("프로즌 국왕", new []{new ItemIngredient("블링크", ItemRank.특별함, 1),new ItemIngredient("프로즌", ItemRank.특별함, 1),new ItemIngredient("마법 화살", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("메카 군단", new []{new ItemIngredient("용접", ItemRank.특별함, 2),new ItemIngredient("군단", ItemRank.특별함, 1)})
            .Set(StatI.towerDamage, 10),

        new ItemDef("우주", new []{new ItemIngredient("빅뱅", ItemRank.특별함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("파이어볼", ItemRank.안흔함, 1),new ItemIngredient("마법봉", ItemRank.흔함, 1)})
            .Set(StatI.towerDamage, 10),
            };

// ===== legendary (34 items) =====
    legendary = new List<ItemDef>{
new ItemDef("이브", new []{new ItemIngredient("이브", ItemRank.히든, 1), new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("영혼 낫", ItemRank.특별함, 1),new ItemIngredient("기억 조각", ItemRank.All, 5)})
    .Set(StatI.atkPower, 5000)
    .Set(StatI.towerDamage, 10)
    .Set(StatI.monoMagic, 500000)
    .Set(StatI.multiMagic, 100000)
    .Set(StatI.range, 500)
    .Set(StatF.Probability, 10f)
    .SetArmor(ArmorType.보스),

    new ItemDef("전쟁 영웅", new []{new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("도적", new []{new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("앨리스", new []{new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("헌터 국왕", new []{new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("추적자", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("프로즌 국왕", new []{new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("다차원", new []{new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("작은 거인", new []{new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("일그러진 영웅", new []{new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("공돌이", new []{new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("추적자", ItemRank.희귀함, 1),new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("메카 군단", new []{new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("과학자", new []{new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("발키리", new []{new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("시빌워", new []{new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("헌터 국왕", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("아담", new []{new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("저격수", new []{new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("시간", new []{new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("프로즌 국왕", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("군인", new []{new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("타이탄", new []{new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("바이오 로봇", ItemRank.희귀함, 1),new ItemIngredient("엑스칼리버", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("차원 문", new []{new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("좀비", new []{new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("꿰뚫는 창", ItemRank.희귀함, 1),new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("태초", new []{new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("죽음", new []{new ItemIngredient("되살아난 영웅", ItemRank.희귀함, 1),new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("저격총", new []{new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("속력", new []{new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("차원 거울", ItemRank.희귀함, 1),new ItemIngredient("추적자", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("탱크", new []{new ItemIngredient("탱크", ItemRank.희귀함, 1),new ItemIngredient("메카 군단", ItemRank.희귀함, 1),new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("캐논쉽", new []{new ItemIngredient("캐논쉽", ItemRank.희귀함, 1),new ItemIngredient("플라즈마 광선", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("대마법사", new []{new ItemIngredient("대마법사", ItemRank.희귀함, 1),new ItemIngredient("전쟁", ItemRank.희귀함, 1),new ItemIngredient("무기의 달인", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("우주", new []{new ItemIngredient("아담", ItemRank.희귀함, 1),new ItemIngredient("굶주림", ItemRank.희귀함, 1),new ItemIngredient("우주", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("메카닉", new []{new ItemIngredient("공돌이", ItemRank.희귀함, 1),new ItemIngredient("타이탄", ItemRank.희귀함, 1),new ItemIngredient("앨리스", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("워프", new []{new ItemIngredient("웜홀", ItemRank.희귀함, 1),new ItemIngredient("영웅", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("치유력", new []{new ItemIngredient("부메랑", ItemRank.희귀함, 1),new ItemIngredient("행성", ItemRank.희귀함, 1),new ItemIngredient("좀비", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("영웅의 딸", new []{new ItemIngredient("도적", ItemRank.희귀함, 1),new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("활", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),

    new ItemDef("사신", new []{new ItemIngredient("사신", ItemRank.희귀함, 1),new ItemIngredient("합금", ItemRank.희귀함, 1),new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("기억 조각", ItemRank.All, 3)})
        .Set(StatI.towerDamage, 10),
        };
        

        hidden = new List<ItemDef>{
        new ItemDef("함선", Array.Empty<ItemIngredient>()),

        new ItemDef("이브", Array.Empty<ItemIngredient>())
            .Set(StatI.towerDamage, 10),

        new ItemDef("해결사", new []{new ItemIngredient("군인", ItemRank.희귀함, 1),new ItemIngredient("저격수", ItemRank.희귀함, 1),new ItemIngredient("과학자", ItemRank.희귀함, 1),new ItemIngredient("공돌이", ItemRank.희귀함, 1)})
            .Set(StatI.towerDamage, 10),
        };

        changed = new List<ItemDef>{

        };

        upperRanked = new List<ItemDef>{

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
            int rows = table[rank].Count;         // 행 개수
            for (int r = 0; r < rows; r++)
            {
                string itemName = table[rank][r].Name;

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

        List<ItemDef> grade = table[(int)rank];
        int rowCount = grade.Count;            // 아이템 개수

        for (int j = 0; j < rowCount; j++)               // 아이템 반복
        {
            Debug.Log($"{grade[j].Name}, {rank}");
            // 0번 열 = 이름

            if(grade[j].Get(StatI.percentageCategory) > (int)PercentageCategory.count || grade[j].Get(StatI.percentKind) > 3)
            {
                QuitProcess($"[ItemCheck] 퍼센트 공격 설정이 잘못됨");
            }

            if (grade[j].Get(StatF.Percent) != 0 ^ grade[j].Get(StatI.percentageCategory) != 0) // 두 값중 하나만 있으면 경고
            {
                QuitProcess($"[ItemCheck] 체력 비례 공격 설정이 잘못됨");
            }

            if(grade[j].Get(StatB.boss) != false && (grade[j].Get(StatI.range) != 0))
            {
                QuitProcess($"[ItemCheck] 보스 공격 설정이 잘못됨");
            }

            if(grade[j].Get(StatF.Probability) != 0 ^
            (grade[j].Get(StatF.monoStun) != 0 || grade[j].Get(StatF.multiStun) != 0 || 
             grade[j].Get(StatI.monoPhysics) != 0 || grade[j].Get(StatI.multiPhysics) != 0 ||
             grade[j].Get(StatI.monoMagic) != 0 || grade[j].Get(StatI.multiMagic) != 0 ||
             grade[j].Get(StatI.range) != 0 || grade[j].Get(StatF.damageUp) != 0 ||
             grade[j].Get(StatB.boss) != false || grade[j].Get(StatF.Percent) != 0))
            {   
                QuitProcess($"확률이 잘못 설정됨");
            }

            
            if((grade[j].Get(StatI.multiPhysics) != 0 || 
            grade[j].Get(StatI.multiMagic) != 0 || 
            grade[j].Get(StatF.multiStun) != 0) ^
            grade[j].Get(StatI.range) != 0)
            {
                QuitProcess($"[ItemCheck] 범위 공격 설정이 잘못됨");
            }

            bool HaveRegenSkill = false;

            for(int i=0;i<(int)RegenKind.count;i++)
            {
                if(HaveRegenSkill) break;
                for(int k=0;k<(int)DamageKind.count; k++)
                {
                    if(HaveRegenSkill) break;
                    for(int l=0;l<(int)DamageTarget.count; l++)
                    {
                        if(grade[j].regenDamage[i,k,l] != 0)
                        {
                            HaveRegenSkill = true;
                            break;
                        }
                    }
                }

                for(int k=0;k<(int)PercentageCategory.count;k++)
                {
                    if(HaveRegenSkill) break;
                    for(int l=0;l<(int)PercentKind.count; l++)
                    {
                        if(HaveRegenSkill) break;
                        if(grade[j].regenPercent[i,k,l] != 0)
                        {
                            HaveRegenSkill = true;
                            break;
                        }
                    }
                }
            }


            Sprite sprite = Resources.Load<Sprite>($"Image/Item/{rank}/{grade[j].Name}");

            Item newItem = new Item(
                grade[j].Name,
                grade[j].Ingredients,
                rank,
                (byte)j,
                sprite,
                grade[j].Get(StatI.atkPower),
                 grade[j].Get(StatI.addAtkPower),
                 grade[j].Get(StatI.nDefense),
                 grade[j].Get(StatI.mAtkPower),
                 grade[j].Get(StatI.mDebuffer),
                 grade[j].Get(StatF.tDamage),
                 grade[j].Get(StatF.hpRegen),
                 grade[j].Get(StatF.mpRegen),
                 grade[j].Get(StatI.moveSpeed),
                 grade[j].Get(StatI.attackSpeed),
                 grade[j].Get(StatI.towerDamage),
                 grade[j].Get(StatI.towerAttackSpd),
                 grade[j].Get(StatF.Probability),
                grade[j].Get(StatI.monoPhysics),
                grade[j].Get(StatI.multiPhysics),
                grade[j].Get(StatI.monoMagic),
                grade[j].Get(StatI.multiMagic),
                grade[j].Get(StatF.monoStun),
                grade[j].Get(StatF.multiStun),
                DataManager.Instance.RoundX(grade[j].Get(StatI.range) * 0.01f, 3),
                grade[j].Get(StatF.Percent),
                grade[j].Armor,
                grade[j].Get(StatB.boss),
                (PercentageCategory)grade[j].Get(StatI.percentageCategory),
                (PercentKind)grade[j].Get(StatI.percentKind),
                grade[j].regenPercent,
                grade[j].regenDamage,
                grade[j].Get(StatF.doublePhysics),
                grade[j].Get(StatF.damageUp),
                grade[j].Get(StatF.AttackRange),
                grade[j].Get(StatI.blink),
                grade[j].Get(StatF.regenStun),
                DataManager.Instance.RoundX(grade[j].Get(StatI.regenRange) * 0.01f, 3)
                );

            newItem.HaveRegenSkill = HaveRegenSkill;

            if (itemList == null)
                Debug.LogError("Error");

            itemList[(int)rank].Add(newItem);

            dict.Add((grade[j].Name, rank), itemList[(int)rank][itemList[(int)rank].Count - 1]);
        }
    }

    public void SetItemParent(int itemRank)
    {
        List<ItemDef> grade = table[itemRank];
        int rowCount = grade.Count;
        for (int j = 0; j < rowCount; j++)
        {
            Item targetItem = FindItem(grade[j].Name, (ItemRank)itemRank);
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

    public IEnumerator GetRandomItems(ItemRank rank, int itemCount, bool logOut = true)
    {
        for(int i=0;i<itemCount;i++)
        {
            GetRandomItem(rank, logOut);
            yield return null;
        } 
    }

    public Item GetRandomItem(ItemRank rank, bool logOut = true, bool rareGacha = false )
    {
        int rand = UnityEngine.Random.Range(rank == ItemRank.희귀함 ? 1 : 0, itemList[(int)rank].Count);

        Item item;
        if((rank == ItemRank.희귀함 || rank == ItemRank.특별함) && UnityEngine.Random.Range(0, 100) <4 && rareGacha)
            item = FindItem("이브", ItemRank.히든);
        else
            item = itemList[(int)rank][rand];

        ItemManager.SetUpState(item);
        SetCannon(item);
        if (logOut)
        {
            string hex = ColorUtility.ToHtmlStringRGB(ItemManager.GetColor(item));
            ItemManager.chat.Push($"<color=#{hex}>{item.Rank}</color> 등급의 {item.Name} 획득");
        }
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
                    UnSetCannon(findItem);
                }
            }

            
            ItemManager.SetUpState(item);
            SetCannon(item);
            ItemManager.Clear(null, false);
        }
        return enough;
    }

    public Dictionary<(string, ItemRank), int> CombineAllItem(Item item)
    {
        Dictionary<(string, ItemRank), int> itemDict = new Dictionary<(string, ItemRank), int>();
        if (item.Name == "행운의 토큰" && item.Rank == ItemRank.희귀함)
        {
            if (item.count >= 3)
            {
                Token(item);
            }
            return itemDict;
        }
        Item WillBeItem = null;
        if(ItemManager.willBeGet != -1)
            WillBeItem = itemList[(int)ItemRank.특별함][ItemManager.willBeGet];
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
                if (NecessaryItem == null || NecessaryItem.Length == 0) continue;
                if (FindItem(NecessaryItem[0].ItemName, NecessaryItem[0].Rank).NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                int extra = 0;
                if(ItemManager.willBeGet != -1 && FindItem(kvp.Key.Item1, kvp.Key.Item2) == WillBeItem && kvp.Value == dict[(Key, Key2)].count + 1)
                {
                    extra++;
                }

                if (extra + dict[(Key, Key2)].count < kvp.Value)
                {
                    isOkay = false;
                    int necessaryCount = Mathf.Max(kvp.Value - dict[(Key, Key2)].count - extra, 0);
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
        item.ItemIngredientDict = itemDict;
        return itemDict;
    }

    public void CombineSmart(Item item)
    {
        var itemDict = item.ItemIngredientDict;

        foreach (var kvp in itemDict.ToList())
        {
            if (kvp.Key.Item2 != ItemRank.흔함) continue;

            int need = kvp.Value;
            int have = dict[(kvp.Key.Item1, ItemRank.흔함)].count;

            int shortage = Mathf.Max(need - have, 0);   // 부족분(0 이상)
            int useCommon = need - shortage;            // 실제로 소모할 흔함 재료

            // 흔함 재료 요구량을 "소모량"으로 바꿈
            if (useCommon <= 0) itemDict.Remove((kvp.Key.Item1, ItemRank.흔함));
            else itemDict[(kvp.Key.Item1, ItemRank.흔함)] = useCommon;

            // 부족분은 만물석으로 대체
            if (shortage > 0)
            {
                var key = ("만물석", ItemRank.All);
                if (itemDict.ContainsKey(key)) itemDict[key] += shortage;
                else itemDict.Add(key, shortage);
            }
        }

        // 재료 충분한지 체크
        foreach (var kvp in itemDict)
            if (dict[(kvp.Key.Item1, kvp.Key.Item2)].count < kvp.Value) return;

        // 실제 차감
        foreach (var kvp in itemDict)
        {
            Item items = dict[kvp.Key];
            items.count -= kvp.Value;

            if (items.count <= 0)
            {
                GotItem.Remove(items);
                DeleteUnrankedItem(items);
                UnSetCannon(items);
            }
        }

        ItemManager.SetUpState(item);
        SetCannon(item);
        ItemManager.Clear(null, false);
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
                ItemIngredient[] NecessaryItem = dict[(Key, key2)].NecessaryItem;
                if (NecessaryItem == Array.Empty<ItemIngredient>()) continue;
                if (FindItem(NecessaryItem[0].ItemName, NecessaryItem[0].Rank).NecessaryItem == Array.Empty<ItemIngredient>()) continue;
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
        if (item.Rank != ItemRank.상위 && item.BossPercentAttack == false && item.Percent != 0 && item.PercentCategory == PercentageCategory.current)
        {
            UnitySet(0, item);
        }
        else if (item.Rank != ItemRank.상위 && item.BossPercentAttack == true)
        {
            UnitySet(1, item);
        }
        else if (item.Rank != ItemRank.상위 && item.MultiStun != 0)
        {
            UnitySet(2, item);
        }
        else if (item.Rank != ItemRank.상위 && item.Percent != 0 && item.PercentCategory == PercentageCategory.max)
        {
            UnitySet(3, item);
        }
        else if (item.Rank == ItemRank.상위)
        {
            UnitySet(4, item);
        }
        else if (
            item.Rank != ItemRank.상위 &&( 
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
        if(item.Name == "함선" && item.Rank == ItemRank.히든)
            shift = ItemRank.특별함 - ItemRank.안흔함;
        if(item.Name == "이브" && item.Rank == ItemRank.히든)
            shift = ItemRank.희귀함 - ItemRank.안흔함;
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
                        Stats.attackSpeedBonus[number] += 25f;
                        Stats.someSortOfSkillEffect[0] += 300;
                        GameManager.Instance.chat.Push("도약 잠금 해제");
                        break;
                    case ItemRank.희귀함:
                        Stats.damage[number] += 4500;
                        Stats.attackDelay[number] = 0.85f;
                        Stats.attackSpeedBonus[number] += 45f;
                        break;
                    case ItemRank.전설적인:
                        Stats.damage[number] += 9000;
                        Stats.attackDelay[number] = 0.70f;
                        Stats.attackSpeedBonus[number] += 215f;
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
                        Stats.attackSpeedBonus[number] -= 25f;
                        break;
                    case ItemRank.희귀함:
                        Stats.damage[number] -= 4500;
                        Stats.attackSpeedBonus[number] -= 45f;
                        break;
                    case ItemRank.전설적인:
                        Stats.damage[number] -= 9000;
                        Stats.attackSpeedBonus[number] -= 215f;
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
        for (int i = 0; i < DataManager.targetNumberMax; i++)
        {
            bool t = currentItem[i].Remove(item);
            if (t)
            {
                StatsDown(item, i);
                DeleteRankedItem(item, i);  
            }
        }
    }

    
    private void SetCannon(Item item)
    {
        switch(item.Rank)
        {
            case ItemRank.흔함:
                Cannon.SetCannon(1, 0);
                break;
            case ItemRank.안흔함:
                Cannon.SetCannon(5, 1);
                break;
            case ItemRank.특별함:
                Cannon.SetCannon(25, 5);
                break;
            case ItemRank.희귀함:
                Cannon.SetCannon(100, 25);
                break;
            case ItemRank.전설적인:
                break;
            case ItemRank.히든:
                break;
        }
    }

    private void UnSetCannon(Item item)
    {
        switch(item.Rank)
        {
            case ItemRank.흔함:
                Cannon.SetCannon(-1, 0);
                break;
            case ItemRank.안흔함:
                Cannon.SetCannon(-5, -1);
                break;
            case ItemRank.특별함:
                Cannon.SetCannon(-25, -5);
                break;
            case ItemRank.희귀함:
                Cannon.SetCannon(-100, -25);
                break;
            case ItemRank.전설적인:
                break;
            case ItemRank.히든:
                break;
        }
    }

    public void StatsUp(Item item, int number)
    {
        if (Stats != null)
        {
            Stats.damage[number] += item.AttackPower;
            Stats.attackSpeedBonus[number] += item.AttackSpeed;
            Stats.HealthRegen[number] += DataManager.Instance.RoundX(item.HealthRegen, 3);
            Stats.manaRegen[number] += DataManager.Instance.RoundX(item.ManaRegen, 3);
            Stats.doublePhysics[number] += item.DoublePhysics;
            Stats.TrueDamage[number] += item.TrueDamage;
            Stats.Radius[number] = Mathf.Max(Stats.Radius[number], DataManager.Instance.RoundX(item.AttackRange * 0.01f,3));
            Stats.someSortOfSkillEffect[0] += item.Blink;
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
        Stats.attackSpeedBonus[number] -= item.AttackSpeed;
        Stats.HealthRegen[number] -= DataManager.Instance.RoundX(item.HealthRegen, 3);
        Stats.manaRegen[number] -= DataManager.Instance.RoundX(item.ManaRegen, 3);
        Stats.doublePhysics[number] -= item.DoublePhysics;
        Stats.TrueDamage[number] -= item.TrueDamage;
        Stats.someSortOfSkillEffect[0] -= item.Blink;
        Cannon.SetCannon(-item.TowerDamage, -item.TowerAttackSpeed);
        if(Stats.Radius[number] <= item.AttackRange)
        {
            Stats.Radius[number] = 0f;
            foreach(Item itemIn in itemList[number])
            {
                if(Stats.Radius[number] < itemIn.AttackRange)
                Stats.Radius[number] = itemIn.AttackRange;

            }
        }

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

                if(image.gameObject.activeSelf)
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
        PercentageCategory PercentageCategory = item.PercentCategory;
        bool BossPercentAttack = item.BossPercentAttack;
        ArmorType attackType = item.AttackType;
        float doubledDamage = item.DoublePhysics;
        PercentKind percentKind = item.PercentKind;
        
        float DamagePercentage = 0f;

        int rand ;
        bool SkillOn = false;

        for(int i=0;i<item.count;i++)
        {
            rand = UnityEngine.Random.Range(0, 10000);
            if (rand < Mathf.Ceil(Probability * 100))
            {
                actor.TakeStunAll(MultiStun, MonoStun, Range);

                actor.TakeDamage_explosions(Percent, attackType, PercentageCategory);

                actor.TakeDamageAll_physics(MultiPhysics, MonoPhysics, Range, attackType, doubledDamage);

                actor.TakeDamageAll_magics(MultiMagic, MonoMagic, Range, false);

                actor.TakeDamageAll_percentage(Percent, Percent, Range, percentKind, PercentageCategory, BossPercentAttack);

                if(name == "좀비" && rank == ItemRank.안흔함)
                    {
                        SkillOn = true;
                    }
            }
        }

        if (SkillOn)
        {
            DamagePercentage += DamageUp;
            if(name == "좀비" && rank == ItemRank.안흔함 && actor.isDead)
                ItemManager.SetUpState(item);
        }
        
        return DamagePercentage;
    }

        public static void QuitProcess(string message)
    {
        Debug.LogError(message);

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // 에디터 플레이 종료
        #elif UNITY_WEBGL
                // WebGL은 프로세스 종료가 불가능(브라우저 탭 강제 종료 불가)
                throw new Exception(message);
        #else
                Application.Quit();        // Unity 쪽 정상 종료 요청
                Environment.Exit(1);       // 프로세스 즉시 종료(Standalone에서 확실)
        #endif
    }
}