using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatus : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI[] text;
    public Slider slider;
    private ActionScript action;
    public GameObject status;
    void Start()
    {
        action = GameManager.Instance.action[DataManager.targetNumberMax -1];

    }

    // Update is called once per frame
    void Update()
    {
        if (action.statsTarget == null)
        {
            status.gameObject.SetActive(false);
        }
        else
        {
            status.gameObject.SetActive(true);
            Transform target = action.statsTarget;
            if (target.GetComponentInParent<EnemyStats>() != null)
            {
                slider.gameObject.SetActive(true);
                EnemyStats stats = target.GetComponent<EnemyStats>();
                float ratio = stats.currentHealth / stats.maxHealth;
                slider.value = ratio;
                TextMeshProUGUI[] texts = slider.GetComponentsInChildren<TextMeshProUGUI>();
                string s = $"{Mathf.Ceil(stats.currentHealth):N0}/ {stats.maxHealth:N0}";
                texts[0].text = s;
                texts[1].text = s;

                var Info = stats.GetDamageInfo();
                if (Info.Item1 > 0)
                    text[0].text = $"방어력 : {Info.armor} (-{(Info.armor / (Info.armor + 50f) * 100).ToString("F3")}%)";
                else
                    text[0].text = $"방어력 : {Info.armor}({((1 - Mathf.Pow(0.94f, -Info.armor))* 100).ToString("F3")}%)";
                text[1].text = $"이동 속도 : {Info.moveSpeed}";
                text[2].text = $"방어 타입 : {Info.armorType}";
            }
            else if (target.GetComponentInParent<Cannon>() != null)
            {
                slider.gameObject.SetActive(false);
                var Info = target.GetComponentInParent<Cannon>().GetDamageInfo();
                text[0].text = $"공격력 : {Info.Item1}";
                text[1].text = $"공격 속도 : {(1/Info.Item2).ToString("F3")}({Info.Item4}%)";
                text[2].text = $"공격 타입 : {Info.Item3}";
            }
            else
            {
                slider.gameObject.SetActive(false);
                var Info = target.GetComponentInParent<Story>().GetDamageInfo();
                if (Info.Item1 > 0)
                    text[0].text = $"방어력 : {Info.story} (-{(Info.story / (Info.story + 50f) * 100).ToString("F3")}%)";
                else
                    text[0].text = $"방어력 : {Info.story}({((1 - Mathf.Pow(0.94f, -Info.story))* 100).ToString("F3")}%)";
                text[1].text = $"스토리 레벨 : {Info.level}";
                text[2].text = $"방어 타입 : {Info.armorType}";
            }
        }

    }
}
