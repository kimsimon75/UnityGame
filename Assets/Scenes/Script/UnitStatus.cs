using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatus : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI[] text;
    public Slider slider;
    public ActionScript action;
    public GameObject status;
    void Start()
    {
        

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
                float ratio = stats.CurrentHealth / stats.MaxHealth;
                slider.value = ratio;
                TextMeshProUGUI[] texts = slider.GetComponentsInChildren<TextMeshProUGUI>();
                string s = $"{Mathf.Ceil(stats.CurrentHealth)}/ {stats.MaxHealth}";
                texts[0].text = s;
                texts[1].text = s;

                var Info = stats.GetDamageInfo();
                text[0].text = $"방어력 : {Info.Item1}";
                text[1].text = $"이동속도 : {Info.Item2 * 100}";
                text[2].text = $"방어 타입 : {Info.Item3}";
            }
            else if (target.GetComponentInParent<Cannon>() != null)
            {
                slider.gameObject.SetActive(false);
                var Info = target.GetComponentInParent<Cannon>().GetDamageInfo();
                text[0].text = $"공격력 : {Info.Item1}";
                text[1].text = $"공격속도 : {Info.Item2}";
                text[2].text = $"공격 타입 : {Info.Item3}";
            }
            else
            {
                slider.gameObject.SetActive(false);
                var Info = target.GetComponentInParent<Story>().GetDamageInfo();
                text[0].text = $"방어력 : {Info.Item1}";
                text[1].text = $"스토리 레벨 : {Info.Item2}";
                text[2].text = $"방어 타입 : {Info.Item3}";
            }
        }

    }
}
