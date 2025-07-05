using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager gameManager;

    private const float init = 37f;
    private float timeLeft = float.MinValue;  // 타이머 시작 시간 (초)
    private int round = 0;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundText;
    public Summoner summoner;
    public ItemManager item;

    void Awake()
    {
        gameManager = this;
    }

    void Start()
    {
        timeLeft = 5f;
        roundText.text = "라운드 시작 전";
        round++;
        item = GetComponentInChildren<ItemManager>();
                for (int i = 0; i < 10; i++)
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
            summoner.StartCoroutine(summoner.SummonLoop());


                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
                item.list.GetRandomItem(ItemRank.Common);
            if (round == 5)
            {
                item.list.GetRandomItem(ItemRank.Uncommon);
                item.list.GetRandomItem(ItemRank.Uncommon);
            }
            roundText.text = $"{round++}라운드";

            timeLeft = init - 1;
            // 여기에 타이머 끝났을 때 실행할 코드 추가
        }
        timerText.text = $"{Mathf.Max(Mathf.Floor(timeLeft / 60), 0)}:{Mathf.Ceil(timeLeft) - Mathf.Max(Mathf.Floor(timeLeft / 60), 0)}";
    }
    
}
