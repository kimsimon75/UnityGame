using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;

    private const float init = 5f;
    private const float bossInit = 0f;
    private float timeLeft = float.MinValue;  // 타이머 시작 시간 (초)
    private float pawnTime = float.MinValue;  // 타이머 시작 시간 (초)
    private float pawnCooltime = float.MinValue;  // 타이머 시작 시간 (초)
    private float go_pawnTime = float.MinValue;  // 타이머 시작 시간 (초)
    private float go_pawnCooltime = float.MinValue;  // 타이머 시작 시간 (초)
    private int round = 13;

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
    EnemyStats pawnObject = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = 5f;
        roundText.text = "라운드 시작 전";
        item = GetComponentInChildren<ItemManager>();
        item.list.FindItem("기억 조각").count++;
        for (int i = 0; i < 3; i++)
            item.list.GetRandomItem(ItemRank.흔함);
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();

    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        pawnTime -= Time.deltaTime;
        pawnCooltime -= Time.deltaTime;

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

            timeLeft = init - 1;

            if (round % 10 == 0)
            {
                EnemyStats boss = summoner.BossSummoner(Boss);
                timeLeft = bossInit - 1;
                boss.moveSpeed = 230f;
                boss.SetRound(round);
                boss.SetBoss(true);
            }
            else
                StartCoroutine(summoner.SummonLoop());


            // 여기에 타이머 끝났을 때 실행할 코드 추가
        }

        SetBonusBoss(round, 15);
        SetBonusBoss(round, 25);
        timerText.text = $"{Mathf.Max(Mathf.Floor((timeLeft + 1) / 60), 0)}:{Mathf.Ceil(timeLeft) - 60 * Mathf.Max(Mathf.Floor((timeLeft + 1) / 60), 0)}";
    }

    public int GetRound() { return round; }


    public void SetBonusBoss(int round, int condition)
    {
        RectTransform normal = slider.panelA.GetChild(condition == 15 ? 1 : 2) as RectTransform;
        TextMeshProUGUI[] text = normal.GetComponentsInChildren<TextMeshProUGUI>();
        if (round < condition)
        {
            text[0].text = $"{condition}라운드부터 생성";
            text[1].text = "";
        }
        else
        {
            if ((condition == 15 ? pawnCooltime : go_pawnCooltime) <= -1)
            {
                pawnObject = summoner.BossSummoner(condition == 15 ? Pawn : Go_Pawn);
                if (condition == 15)
                {
                    pawnCooltime = 299f;
                    pawnTime = 19f;
                }
                else
                {
                    go_pawnCooltime = 299f;
                    go_pawnTime = 19f;
                }
                pawnObject.SetBoss(true);
            }
            if (!pawnObject.IsDestroyed() && (condition == 15 ? pawnTime : go_pawnTime) <= -1)
            {
                Destroy(pawnObject.gameObject);
                penalty += 2;

                Vector3 spawnPos = new Vector3(-16, 0, 16);

                // 👇 뒤를 보게 회전 설정
                Quaternion rot = Quaternion.Euler(-90, -90, 0);

                CrossInstance = Instantiate(CrossPrefab, spawnPos, rot, PlayerZone.transform);
            }
            
            string pawnLabel = (condition == 15) ? "졸병" : "고졸병";
            if (pawnObject.IsDestroyed())
            {
                text[0].text = $"재생성({pawnLabel})";
                text[1].text = $"{Mathf.Max(Mathf.Floor((pawnCooltime + 1) / 60), 0)}:{Mathf.Ceil(pawnCooltime) - 60 * Mathf.Max(Mathf.Floor((pawnCooltime + 1) / 60), 0)}";
            }
            else
            {
                text[0].text = $"{pawnLabel}";
                text[1].text = $"{Mathf.Max(Mathf.Floor((pawnTime + 1) / 60), 0)}:{Mathf.Ceil(pawnTime) - 60 * Mathf.Max(Mathf.Floor((pawnTime + 1) / 60), 0)}";
            }
        }
    }
}
