
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;
    [NonSerialized] public ActionScript[] action = new ActionScript[DataManager.targetNumberMax];
    [NonSerialized] public OriginStatFor6 originStatFor6;
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
    public ItemManager item; // 절대 private로 전환 금지

    public GameObject items;
    public ItemScrollView scrollView;
    AoeIndicatorLite ring;

    public ItemManager ItemManager => item;
    public TargetDetector Detector;
    public SlideInSpawner slider;
    public GameObject CrossPrefab;

    public GameObject PlayerZone;
    public GameObject Count;

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

    [NonSerialized] public PlayerStats[] playerStats = new PlayerStats[DataManager.targetNumberMax];
    [NonSerialized] public GameObject[] player = new GameObject[DataManager.targetNumberMax];

    public GameObject[] playerUnit = new GameObject[DataManager.targetNumberMax];
    [NonSerialized] public int playerCharacter = 1;

    EnemyStats pawnEnemy;
    EnemyStats go_pawnEnemy;
    EnemyStats 삼십적;
    EnemyStats 사십적;
    EnemyStats 오십적;

    public Energy energy;

    public ChatManager chat;

    int enemyCount = 3;

    public List list;

    bool[] isSkill = new bool[DataManager.NumCount -1];
    [SerializeField] private GameObject keyValue;
    private Image[] keyValueImages;
    public Image[] Images => keyValueImages;
    [NonSerialized] public int KeyValueNumber = DataManager.NumCount;

    [NonSerialized] public int[] skillEnergy = new int[DataManager.NumCount-1];
    [NonSerialized] public float[] skillCoolInit = new float[DataManager.NumCount-1];
    [NonSerialized] public SkillCool[] skillCooldown = new SkillCool[DataManager.NumCount-1];
    [NonSerialized] public float[] skillIndicate = new float[DataManager.NumCount-1];

    public GameObject Lightnings;
    public GameObject StoryCannons;
    [NonSerialized] public CannonManager cannonManager;

    public TextMeshProUGUI[] unitCountTexts;

    GameObject[] cooldownImage;

    public bool SkillToggle = false;

    public bool TeleportOn = false;
    public Transform magicZone;
    public Slider HPBar;
    public Slider MPBar;
    public TextMeshProUGUI PlayerStatsText;

    public Transform PlayerSummon;
    
    [NonSerialized] public int UnitCount;

    void Awake()
    {
        Instance = this;
        GetComponent<DataManager>().Init();
        for(int i=0;i<DataManager.targetNumberMax;i++)
        {
            player[i] = Instantiate(playerUnit[playerCharacter],PlayerSummon.transform.position,Quaternion.Euler(0, 180, 0),PlayerZone.transform);
            action[i] = player[i].GetComponent<ActionScript>();
            playerStats[i] = player[i].GetComponent<PlayerStats>();

            player[i].GetComponent<SetPlayer>().Init(i);
        }


        UnitCount = 0;
        scrollView.Init();
        cannonManager = PlayerZone.GetComponent<CannonManager>();
        cannonManager.Init();


        keyValueImages =
           keyValue.GetComponentsInChildren<Image>(includeInactive: true)
                   .Where(img => img.GetComponent<Button>() != null)
                   .ToArray();
        items = item.transform.Find("Items").gameObject;

        Detector = item.GetComponent<TargetDetector>();
        item.SetList();

                 // 초기 채팅들 Push
        ring = GetComponent<AoeIndicatorLite>();
        timeLeft = 0f;
        roundText.text = "라운드 시작 전";
        item.list.GetMemoriesParts(1);
        item.list.GetSoulParts(5);
        item.list.GetRandomItem(ItemRank.안흔함);
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();
        slider.SpawnPanelsSequentially();

        cooldownImage = new GameObject[DataManager.NumCount-1];

        for(int i=0; i<DataManager.NumCount - 1; i++)
        {
            cooldownImage[i] = keyValueImages[i].transform.Find("Image/CooldownBG").gameObject;
            skillCooldown[i] = new SkillCool();
        }

        item.willBeGet = UnityEngine.Random.Range(0, item.list.itemList[(int)ItemRank.특별함].Count);



        for (int i = 0; i < DataManager.NumCount; i++)
        {
            keyValueImages[i].AddComponent<KeyButton>();
            keyValueImages[i].GetComponent<KeyButton>().number = i;
        }
        skillEnergy[(int)DataManager.Num.Q] = 700;
        skillEnergy[(int)DataManager.Num.W] = 100;
        skillEnergy[(int)DataManager.Num.E] = 500;
        skillEnergy[(int)DataManager.Num.Z] = 90;
        skillEnergy[(int)DataManager.Num.X] = 200;
        skillEnergy[(int)DataManager.Num.C] = 620;

        skillCoolInit[(int)DataManager.Num.Q] = 3f;
        skillCoolInit[(int)DataManager.Num.W] = 17f;
        skillCoolInit[(int)DataManager.Num.E] = 100f;
        skillCoolInit[(int)DataManager.Num.Z] = 4.5f;
        skillCoolInit[(int)DataManager.Num.X] = 40f;
        skillCoolInit[(int)DataManager.Num.C] = 170f;

        skillIndicate[(int)DataManager.Num.Q] = 0f;
        skillIndicate[(int)DataManager.Num.W] = 0f;
        skillIndicate[(int)DataManager.Num.E] = 6f;
        skillIndicate[(int)DataManager.Num.Z] = 0f;
        skillIndicate[(int)DataManager.Num.X] = 6f;
        skillIndicate[(int)DataManager.Num.C] = 0f;

    }

    void Start()
    {

    }

    void Update()
    {

        item.list.FindItem("기억 조각", ItemRank.All).count = 1000;

        timeLeft -= Time.deltaTime;
        pawnTime -= Time.deltaTime;
        pawnCooltime -= Time.deltaTime;
        go_pawnTime -= Time.deltaTime;
        go_pawnCooltime -= Time.deltaTime;

        삼십타임 -= Time.deltaTime;
        사십타임 -= Time.deltaTime;
        오십타임 -= Time.deltaTime;

        if (items.activeSelf)
        {

        }
        else
        {

            for (int i = 0; i < skillCooldown.Length; i++)
            {
                GameObject CooldownTimer = cooldownImage[i];
    
                if ((SkillToggle ? playerStats[DataManager.targetNumberMax -1].someSortOfSkillCooldown[i] : skillCooldown[i]).Remaining <= 0)
                {
                    CooldownTimer.SetActive(false);
                }
                else
                {
                    CooldownTimer.GetComponentInChildren<TextMeshProUGUI>(true).text = 
                    ((int)(SkillToggle ? playerStats[DataManager.targetNumberMax -1].someSortOfSkillCooldown[i] : skillCooldown[i]).Remaining + 1).ToString();
                    CooldownTimer.SetActive(true);
                }
            }

            for (int i = 0; i < DataManager.NumCount-1; i++)
                SkillApply(i);

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Trigger((int)DataManager.Num.Q);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Trigger((int)DataManager.Num.W);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Trigger((int)DataManager.Num.E);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Trigger((int)DataManager.Num.Z);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                Trigger((int)DataManager.Num.X);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Trigger((int)DataManager.Num.C);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                SkillToggle = !SkillToggle;
            }


        }

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
            else if (round != 1)
            {
                item.list.GetSoulParts(2);
            }

            if (!CrossInstance.IsDestroyed() && penalty == 0)
            {
                Destroy(CrossInstance);
            }

            if (round == 1) item.list.GetRandomItem(ItemRank.안흔함);
            if (round == 3) item.list.GetRandomItem(ItemRank.안흔함);
            if (round == 4)
            {
                string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.yellow);
                Item willBeGetItem = item.list.itemList[(int)ItemRank.특별함][item.willBeGet];
                item.SetUpState(willBeGetItem);
                item.chat.Push($"<color=#{hex}>{ItemRank.특별함}</color> 등급의 {willBeGetItem.Name} 획득");
                item.willBeGet = -1;
            }
            if (round == 41)
            {
                item.list.GetSoulParts(5);
            }

            timeLeft = init - 1;

            if (round % 10 == 0)
            {
                EnemyStats boss = summoner.BossSummoner(Boss);
                timeLeft = bossInit - 1;
                boss.baseMoveSpeed = 230f;
                boss.moveSpeed = 230;

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
                if (condition == 15) targetObject = pawnEnemy = summoner.BossSummoner(Pawn, round, DataManager.Instance.bonusBossState[PawnCount][0], DataManager.Instance.bonusBossState[PawnCount++][1]);
                else
                {
                    targetObject = go_pawnEnemy = summoner.BossSummoner(Go_Pawn, round, DataManager.Instance.bonusBossState2[Go_PawnCount][0], DataManager.Instance.bonusBossState2[Go_PawnCount++][1]);
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

    public void Trigger(int target)
    {
        if (!items.activeSelf)
        {   
            if(target == DataManager.NumCount -1)
            {
                SkillToggle = !SkillToggle;
            }
            else if(!SkillToggle)
            {   

                 if (energy.currentEnergy >= skillEnergy[target] && skillCooldown[target].IsReady)
                {
                    isSkill[target] = true;
                    keyValueImages[target].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                    if (skillIndicate[target] > 0)
                        ring.SetRing(skillIndicate[target], true);
                }           
                else if (skillCooldown[target].Remaining > 0) chat.Push($"스킬이 준비중입니다.");
                else chat.Push($"에너지가 모자랍니다.");
            }
            else if(SkillToggle) 
            {
                if(playerStats[DataManager.targetNumberMax -1].someSortOfSkillCooldown[target].IsReady)
                {                
                    if(target == (int)DataManager.Num.Q)
                    {
                        TeleportOn = true;
                        keyValueImages[(int)DataManager.Num.Q].GetComponent<UnityEngine.UI.Outline>().enabled = true;
                    }
                    else
                        action[DataManager.targetNumberMax -1].GetComponent<Skill>().ApplyAttackBuff(target);
                }
                else chat.Push($"스킬이 준비중입니다.");
            }

        }
    }
    public void SkillApply(int skillNum)
    {
        if (isSkill[skillNum] == true && Input.anyKeyDown)
        {
            if (Input.GetMouseButtonDown(0) && !UiRayUtil.IsPointerOverUIExcept(LayerMask.GetMask("Text")))
            {
                if (skillIndicate[skillNum] > 0)
                {
                    SkillDetail(skillNum, Detector.hit);
                    energy.currentEnergy -= skillEnergy[skillNum];
                    skillCooldown[skillNum].Start(skillCoolInit[skillNum]);
                }
                else
                {
                    if (Detector.hit.transform != null && Detector.hit.transform.GetComponent<Actor>() != null)
                    {
                        SkillDetail(skillNum, Detector.hit);
                        energy.currentEnergy -= skillEnergy[skillNum];
                        skillCooldown[skillNum].Start(skillCoolInit[skillNum]);
                    }
                }

            }
            ring.SetRing(0f, false);
            keyValueImages[skillNum].GetComponent<UnityEngine.UI.Outline>().enabled = false;
            isSkill[skillNum] = false;
        }
    }

    public void SkillDetail(int target, RaycastHit hitInfo)
    {
        switch ((DataManager.Num)target)
        {
            case DataManager.Num.Q:
                hitInfo.transform.GetComponent<Actor>().TakeStunAll(0, 5, 0);
                hitInfo.transform.GetComponent<Actor>().TakeDamageAll_magics(0, 12500000, 0);
                hitInfo.transform.GetComponent<Actor>().TakeDamageAll_percentage(0, 7, 0, PercentKind.magics, PercentageCategory.current);
                break;
            case DataManager.Num.W:
                hitInfo.transform.GetComponent<Actor>().TakeStunAll(0, 2, 0);
                hitInfo.transform.GetComponent<Actor>().TakeDamageAll_magics(0, 22500, 0);
                break;
            case DataManager.Num.E:
                {
                    Highlightable[] highlightables = ring.ring.GetComponent<AOEBlueHighlighter>()._inside.ToArray();
                    foreach (Highlightable highlightable in highlightables)
                    {
                        highlightable.GetComponent<Actor>().TakeDamageAll_magics(0, 7000000, 0, true);
                    }
                }

                break;
            case DataManager.Num.Z:
                hitInfo.transform.GetComponent<EnemyStats>().DestroySelf();
                break;
            case DataManager.Num.X:
                {
                    Highlightable[] highlightables = ring.ring.GetComponent<AOEBlueHighlighter>()._inside.ToArray();
                    foreach (Highlightable highlightable in highlightables)
                    {
                        highlightable.GetComponent<Actor>().TakeStunAll(0, 3, 0);
                    }
                }

                break;
            case DataManager.Num.C:
                hitInfo.transform.GetComponent<Actor>().TakePoisonAll(5, 20, 6f);
                break;
            case DataManager.Num.D:
                break;
        }
    }
    public void SetActiveRecursively(GameObject root, bool on)
    {
        // 비활성 포함 전부 가져와서 activeSelf를 직접 세팅
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            t.gameObject.SetActive(on);
    }

    public void SetCountScript()
    {
        Count.SetActive(true);
        Count.GetComponent<CountScript>().slider.value = 0;
    }
}
