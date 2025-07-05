using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSet : MonoBehaviour
{

    public PlayerStats player;

    private Vector2 Health;
    private Vector2 Mana;

    private Slider hpBar;
    private Slider mpBar;
    private Transform stats;
    private TextMeshProUGUI hpTextWhite;
    private TextMeshProUGUI hpTextBlack;
    private TextMeshProUGUI mpText;

    void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = transform.Find("HPBar")?.GetComponentInChildren<Slider>();
        mpBar = transform.Find("MPBar")?.GetComponentInChildren<Slider>();

        if (hpBar == null) Debug.LogError("❌ HPBar(Slider)를 찾지 못했습니다.");
        if (mpBar == null) Debug.LogError("❌ MPBar(Slider)를 찾지 못했습니다.");

        hpTextWhite = transform.Find("HPBar/Background/HPText_White")?.GetComponent<TextMeshProUGUI>();
        hpTextBlack = transform.Find("HPBar/Fill Area/Fill/HPText_Black")?.GetComponent<TextMeshProUGUI>();
        stats = transform.Find("Stats")?.GetComponent<Transform>();

        mpText = mpBar?.GetComponentInChildren<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        Health = player.GetHP();
        Mana = player.GetMP();

        if (Health.y > 0)
        {
            hpBar.value = Health.x / Health.y;
            string hpDisplay = $"{Mathf.FloorToInt(Health.x)} / {Mathf.FloorToInt(Health.y)}";
            hpTextWhite.text = hpDisplay;
            hpTextBlack.text = hpDisplay;
        }

        if (Mana.y > 0)
        {
            mpBar.value = Mana.x / Mana.y;
            mpText.text = $"{Mathf.FloorToInt(Mana.x)} / {Mathf.FloorToInt(Mana.y)}";
        }

        SetStats();
    }

    public void SetStats()
    {
        Vector3 state = player.GetStats();
        stats.Find("Damage").GetComponentInChildren<TextMeshProUGUI>().text = $"공격력 : {state.x}";
        stats.Find("AttackSpeed").GetComponentInChildren<TextMeshProUGUI>().text = $"공격속도 : {(1/state.y).ToString("0.000")} (공속 보너스 : {state.z}%)";
    }
}
