using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;

    private const float init = 39f;
    private float timeLeft = float.MinValue;  // 타이머 시작 시간 (초)
    private int round = 0;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public Summoner summoner;
    public ItemManager item;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeLeft = 5f;
        roundText.text = "라운드 시작 전";
        item = GetComponentInChildren<ItemManager>();
        for (int i = 0; i < 3; i++)
            item.list.GetRandomItem(ItemRank.Common);
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft > -1)
        {

        }
        else
        {
            StartCoroutine(summoner.SummonLoop());
            item.list.GetRandomItem(ItemRank.Common);
            item.list.GetRandomItem(ItemRank.Common);

            roundText.text = $"{++round}라운드";

            if (round == 3) item.list.GetRandomItem(ItemRank.Uncommon);
            if (round == 5) item.list.GetRandomItem(ItemRank.Uncommon);
            if (round == 6) item.list.GetRandomItem(ItemRank.Special);

            timeLeft = init - 1;
            // 여기에 타이머 끝났을 때 실행할 코드 추가
        }
        timerText.text = $"{Mathf.Max(Mathf.Floor(timeLeft / 60), 0)}:{Mathf.Ceil(timeLeft) - Mathf.Max(Mathf.Floor(timeLeft / 60), 0)}";
    }

    public int GetRound() { return round; }
}
