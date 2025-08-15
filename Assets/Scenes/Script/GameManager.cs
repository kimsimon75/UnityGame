
using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;

    private const float init = 35f;
    private const float bossInit = 75f;
    private float timeLeft = float.MinValue;  // 타이머 시작 시간 (초)
    private float pawnTime = float.MinValue;  // 타이머 시작 시간 (초)
    private float pawnCooltime = float.MinValue;  // 타이머 시작 시간 (초)
    private float go_pawnTime = float.MinValue;  // 타이머 시작 시간 (초)
    private float go_pawnCooltime = float.MinValue;  // 타이머 시작 시간 (초)
    private float 삼십타임 = float.MinValue;// 타이머 시작 시간 (초)
    private float 사십타임 = float.MinValue;// 타이머 시작 시간 (초)
    private float 오십타임 = float.MinValue;
    private int round = 0;
    

    public bool RareGet = false;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public Summoner summoner;
    public ItemManager item;

    public ItemManager ItemManager => item;

    public SlideInSpawner slider;
    public GameObject CrossPrefab;

    public GameObject PlayerZone;

    private GameObject CrossInstance;

    private int penalty = 0;

    public GameObject Boss;

    public GameObject Pawn;
    public GameObject Go_Pawn;
    int PawnCount = 0;
    int Go_PawnCount = 0;
    public GameObject 삼십;
    public GameObject 사십;
    public GameObject 오십;

    EnemyStats pawnEnemy = null;
    EnemyStats go_pawnEnemy = null;
    EnemyStats 삼십적 = null;
    EnemyStats 사십적 = null;
    EnemyStats 오십적 = null;

    byte enemyCount = 3;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = 0f;
        roundText.text = "라운드 시작 전";
        item = GetComponentInChildren<ItemManager>();
        item.list.GetMemoriesParts(1);
        for (int i = 0; i < 3; i++)
            item.list.GetRandomItem(ItemRank.흔함);
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();

    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        pawnTime -= Time.deltaTime;
        pawnCooltime -= Time.deltaTime;
        go_pawnTime -= Time.deltaTime;
        go_pawnCooltime -= Time.deltaTime;

        삼십타임 -= Time.deltaTime;
        사십타임 -= Time.deltaTime;
        오십타임 -= Time.deltaTime;

        item.list.FindItem("기억 조각", ItemRank.All).count = 10;

        if (timeLeft > -1)
        {

        }
        else
        {
            roundText.text = $"{++round}라운드";

            if (penalty != 0)
            {
                item.chat.Push("패널티로 인해 라운드 보상을 받을 수 없습니다");
                penalty--;
            }
            else
            {
                item.list.GetRandomItem(ItemRank.흔함);
                item.list.GetRandomItem(ItemRank.흔함);
            }

            if (!CrossInstance.IsDestroyed() && penalty == 0)
            {
                Destroy(CrossInstance);
            }

            if (round == 3) item.list.GetRandomItem(ItemRank.안흔함);
            if (round == 5) item.list.GetRandomItem(ItemRank.안흔함);
            if (round == 6) item.list.GetRandomItem(ItemRank.특별함);
            if (round == 15) item.list.GetRandomItem(ItemRank.희귀함);
            if (round == 41)
            {
                item.list.GetRandomItem(ItemRank.흔함);
                item.list.GetRandomItem(ItemRank.흔함);
                item.list.GetRandomItem(ItemRank.흔함);
                item.list.GetRandomItem(ItemRank.흔함);
                item.list.GetRandomItem(ItemRank.흔함);
            }

            timeLeft = init - 1;

            if (round % 10 == 0)
            {
                EnemyStats boss = summoner.BossSummoner(Boss);
                timeLeft = bossInit - 1;
                boss.moveSpeed = 230f;

            }
            else
                StartCoroutine(summoner.SummonLoop());

            if (round == DataManager.삼십라운드)
            {
                삼십적 = summoner.BossSummoner(삼십, round, 5255000, 90, true);
                삼십타임 = 75 - 1f;
            }
            if (round == DataManager.사십라운드)
            {
                사십적 = summoner.BossSummoner(사십, round, 21000000, 170, true);
                사십타임 = 75 - 1f;
            }
            if (round == DataManager.오십라운드)
            {
                오십적 = summoner.BossSummoner(오십, round, 49050000, 325, true);
                오십타임 = 75 - 1f;
            }


            // 여기에 타이머 끝났을 때 실행할 코드 추가
        }

        if (round <= 60)
        {
            SetBonusBoss(round, 15);
            SetBonusBoss(round, 25);
        }
        OnceUponATime(round, DataManager.삼십라운드, 삼십적);
        OnceUponATime(round, DataManager.사십라운드, 사십적);
        OnceUponATime(round, DataManager.오십라운드, 오십적);
        
        timerText.text = $"{Mathf.Max(Mathf.Floor((timeLeft + 1) / 60), 0)}:{Mathf.Ceil(timeLeft) - 60 * Mathf.Max(Mathf.Floor((timeLeft + 1) / 60), 0)}";
    }

    public int GetRound() { return round; }


    public void SetBonusBoss(int round, int condition)
    {
        RectTransform normal = slider.panelA.GetChild(condition == 15 ? 1 : 2) as RectTransform;
        TextMeshProUGUI[] text = normal.GetComponentsInChildren<TextMeshProUGUI>();
        float time = condition == 15 ? pawnTime : go_pawnTime;
        float cooltime = condition == 15 ? pawnCooltime : go_pawnCooltime;
        EnemyStats targetObject = condition == 15 ? pawnEnemy : go_pawnEnemy;
        if (round < condition)
        {
            text[0].text = $"{condition}라운드부터 생성";
            text[1].text = "";
        }
        else
        {
            if (cooltime <= -1)
            {
                if (condition == 15) targetObject = pawnEnemy = summoner.BossSummoner(Pawn, round, DataManager.Instance.bonusBossState[PawnCount][0],DataManager.Instance.bonusBossState[PawnCount++][1]);
                else
                {
                    targetObject = go_pawnEnemy = summoner.BossSummoner(Go_Pawn,round,DataManager.Instance.bonusBossState2[Go_PawnCount][0], DataManager.Instance.bonusBossState2[Go_PawnCount++][1]);
                    Transform targetTransform = targetObject.GetComponent<Transform>();
                    targetTransform.localScale = targetTransform.localScale / 2;
                }
                if (condition == 15)
                {
                    pawnCooltime = 299f;
                    pawnTime = 19f;
                }
                else
                {
                    Debug.Log("hello");
                    go_pawnCooltime = 300f;
                    go_pawnTime = 19f;
                }
                Debug.Log(go_pawnTime);
                time = condition == 15 ? pawnTime : go_pawnTime;
                cooltime = condition == 15 ? pawnCooltime : go_pawnCooltime;
                targetObject.SetBoss(true);
            }
            if (!targetObject.IsDestroyed() && time <= -1)
            {
                Destroy(targetObject.gameObject);
                penalty += 2;

                Vector3 spawnPos = new Vector3(-16, 0, 16);

                // 👇 뒤를 보게 회전 설정
                Quaternion rot = Quaternion.Euler(-90, -90, 0);

                CrossInstance = Instantiate(CrossPrefab, spawnPos, rot, PlayerZone.transform);
            }

            string pawnLabel = (condition == 15) ? "졸병" : "고졸병";
            if (targetObject.IsDestroyed())
            {
                text[0].text = $"재생성({pawnLabel})";
                text[1].text = $"{Mathf.Max(Mathf.Floor((cooltime + 1) / 60), 0)}:{Mathf.Ceil(cooltime) - 60 * Mathf.Max(Mathf.Floor((cooltime + 1) / 60), 0)}";
            }
            else
            {
                text[0].text = $"{pawnLabel}";
                text[1].text = $"{Mathf.Max(Mathf.Floor((time + 1) / 60), 0)}:{Mathf.Ceil(time) - 60 * Mathf.Max(Mathf.Floor((time + 1) / 60), 0)}";
            }
        }
    }

    public void OnceUponATime(int round, int condition, EnemyStats enemyStats)
    {
        int panelNumber = 3;
        EnemyStats target = null;
        float time = 0;
        string label = "string";
        switch (condition)
        {
            case DataManager.삼십라운드:
                panelNumber = enemyCount;
                target = 삼십적;
                time = 삼십타임;
                label = "삼십적";
                break;
            case DataManager.사십라운드:
                panelNumber = enemyCount + 1;
                target = 사십적;
                time = 사십타임;
                label = "사십적";
                break;
            case DataManager.오십라운드:
                panelNumber = enemyCount + 2;
                target = 오십적;
                time = 오십타임;
                label = "오십적";
                break;
        }
        if (panelNumber <= 2) return;
        RectTransform normal = slider.panelA.GetChild(panelNumber).GetComponent<RectTransform>();
        TextMeshProUGUI[] text = normal.GetComponentsInChildren<TextMeshProUGUI>();

        if (round < condition)
        {
            text[0].text = $"{condition}라운드부터 생성";
            text[1].text = "";
        }
        else
        {

            if (!target.IsDestroyed() && time <= -1)
            {
                Destroy(target.gameObject);
                penalty += 2;

                Vector3 spawnPos = new Vector3(-16, 0, 16);

                // 👇 뒤를 보게 회전 설정
                Quaternion rot = Quaternion.Euler(-90, -90, 0);

                CrossInstance = Instantiate(CrossPrefab, spawnPos, rot, PlayerZone.transform);
            }
            
            if (target.IsDestroyed())
            {
                slider.DeleteSlider(panelNumber);
                enemyCount--;
            }
            if (!normal.IsDestroyed())
            {
                text[0].text = $"{label}";
                text[1].text = $"{Mathf.Max(Mathf.Floor((time + 1) / 60), 0)}:{Mathf.Ceil(time) - 60 * Mathf.Max(Mathf.Floor((time + 1) / 60), 0)}";
            }
        }
    }
}
