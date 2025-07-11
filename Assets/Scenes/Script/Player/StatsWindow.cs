using TMPro;
using UnityEngine;

public class StatsWindow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerStats stats;
    private TextMeshProUGUI[] texts;
    void Start()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var status = stats.GetStats();
        texts[0].text = $"공격력 : {status.Item1}";
        texts[1].text = $"공격 속도 : {(1/status.Item2).ToString("F3")}";
        texts[2].text = $"공속 보너스 : {status.Item3}%";
        texts[3].text = $"방어력 감소 : {status.Item4}";
        texts[4].text = $"체력 재생 : {status.Item5}";
        texts[5].text = $"마나 재생 : {status.Item6}";
        texts[6].text = $"마법증폭 : {status.Item7}";
        texts[7].text = $"마법방어력 감소 : {status.Item8}";
        texts[8].text = $"방어무시 : {status.Item9}";
        texts[9].text = $"이동속도 감소 : {status.Item10}";
    }
}
