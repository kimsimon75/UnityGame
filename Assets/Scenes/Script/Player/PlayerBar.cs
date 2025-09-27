using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerStats playerStats;
    public Slider HPBar;
    public Slider MPBar;
    public TextMeshProUGUI[] HPText;
    public TextMeshProUGUI[] MPText;
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        HPText = HPBar.GetComponentsInChildren<TextMeshProUGUI>();
        MPText = MPBar.GetComponentsInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 v1 = playerStats.GetHP();
        Vector2 v2 = playerStats.GetMP();

        HPBar.value = v1.x / v1.y;
        MPBar.value = v2.x / v2.y;

        HPText[0].text = $"{v1.x}/ {v1.y}";
        HPText[1].text = $"{v1.x}/ {v1.y}";

        
        MPText[0].text = $"{v2.x}/ {v2.y}";
    }
}
