using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int bossRound = 0;


    public const int 삼십라운드 = 30;
    public const int 사십라운드 = 41;
    public const int 오십라운드 = 51;

    public Dictionary<Sprite, ItemRank> imageDict;

    public enum Num
    {
        Q,
        W,
        E,
        Z,
        X,
        C,
        D,
    }

    [NonSerialized] public static readonly int NumCount = Enum.GetValues(typeof(Num)).Length;

    public Sprite[][] sprites;

    public int[][] enemyStats = new int[76][]
    {
        new[] {0, 0 },
        new[] {338, 0 },
        new[] {425, 0 },
        new[] {575, 0 },
        new[] {750, 2 },
        new[] {950, 3 },
        new[] {1275, 3 },
        new[] {1625, 4 },
        new[] {2150, 5 },
        new[] {2550, 6 },
        new[] {119625, 13}, //10라 보스
        new[] {2750, 7 },
        new[] {3500, 8 },
        new[] {4475, 9 },
        new[] {5600, 11 },
        new[] {9559, 12 },
        new[] {13103, 14 },
        new[] {15931, 16 },
        new[] {17793, 17 },
        new[] {20048, 18 },
        new[] {701250, 29 }, // 20라 보스
        new[] {23270, 19 },
        new[] {25776, 20 },
        new[] {33652, 21 },
        new[] {45466, 22 },
        new[] {56743, 23 },
        new[] {67662, 36 },
        new[] {83235, 37 },
        new[] {93080, 38 },
        new[] {114560, 40 },
        new[] {6187500, 115 }, // 30라 보스
        new[] {450170, 53 },
        new[] {553420, 54 },
        new[] {660800, 56 },
        new[] {745878, 57 },
        new[] {899266, 69 },
        new[] {1025892, 70 },
        new[] {1143184, 82 },
        new[] {1197700, 82 },
        new[] {1, 39 },
        new[] {23925000, 170 }, //40라 보스
        new[] {165200, 94 },
        new[] {2065000, 95 },
        new[] {2891000, 96 },
        new[] {3717000, 97 },
        new[] {4956000, 110 },
        new[] {5782000, 111 },
        new[] {6195000, 112 },
        new[] {7161420, 123 },
        new[] {7847000, 124 },
        new[] {87450000, 245 }, // 50라 보스
        new[] {16380000, 124 },
        new[] {17850000, 125 },
        new[] {19740000, 126 },
        new[] {21000000, 138 },
        new[] {23992500, 139 },
        new[] {27195000, 140 },
        new[] {29874600, 140 },
        new[] {31920000, 141 },
        new[] {34650000, 142 },
        new[] {179850000, 350 }, // 60라 보스
        new[] {65345000, 177 },
        new[] {68363000, 178 },
        new[] {75349000, 179                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         },
        new[] {79341000, 180 },
        new[] {84926200, 183 },
        new[] {93812000, 187 },
        new[] {101796000, 181 },
        new[] {108960200, 182 },
        new[] {115668200, 206 },
        new[] {124251000, 180 },
        new[] {135549488, 186 },
        new[] {139297600, 180 },
        new[] {155787792, 194 },
        new[] {165767792, 190 },
        new[] {161775792, 191 }
    };

    public int[][] finalBossStats = new int[3][]{

        new[] {101450248, 360 },
        new[] {106804500, 380 },
        new[] {112158752, 395 },
    };

    public int[][] bonusBossState = new int[][]{
        new[] {18000, 10 },
        new[] {418010, 10 },
    };

    public int[][] bonusBossState2 = new int[][]{
        new[] {4875000, 140 },
    };

    public int[] bossReword = new int[6]{
        1,
        2,
        2,
        3,
        4,
        3,
    };
    static public DataManager Instance = null;
    void Awake()
    {
        Instance = this;
        imageDict = new Dictionary<Sprite, ItemRank>();

        sprites = new Sprite[2][]
        {
            new Sprite[]
            {
                Resources.Load<Sprite>("Key/귀속"),
                Resources.Load<Sprite>("Key/낙뢰"),
                Resources.Load<Sprite>("Key/메테오"),
                Resources.Load<Sprite>("Key/영혼 흡수"),
                Resources.Load<Sprite>("Key/지진"),
                Resources.Load<Sprite>("Key/독약"),
                Resources.Load<Sprite>("Key/도핑"),
            },
            new Sprite[]
            {
                Resources.Load<Sprite>("Key/흔함"),
                Resources.Load<Sprite>("Key/중급 도박"),
                Resources.Load<Sprite>("Key/고급 도박"),
                Resources.Load<Sprite>("Key/초급 도박"),
                Resources.Load<Sprite>("Key/기억 조각"),
                Resources.Load<Sprite>("Key/에너지 탱크"),
                Resources.Load<Sprite>("Key/로테이션"),
            }
        };

    }

    // Update is called once per frame
    void Update()
    {

    }

    public double RoundX(float value, int digits)
    {
        float mul = Mathf.Pow(10f, digits);
        return Mathf.Round(value * mul) / mul;
    }
}
