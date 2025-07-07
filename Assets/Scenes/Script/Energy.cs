using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI energyTextBackground;
    public Slider energyBar;
    private float energyRegenBuffer = 0f;
    private float energyRegen = 300f;
    private float maxEnergy = 10000;
    private float currentEnergy = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        energyRegenBuffer += energyRegen * Time.fixedDeltaTime;

        // 2. 누적값이 1 이상이면 정수만큼 회복
        if (energyRegenBuffer >= 1f)
        {
            int regenAmount = Mathf.FloorToInt(energyRegenBuffer);  // 정수만큼 회복
            currentEnergy += regenAmount;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);

            energyRegenBuffer -= regenAmount;  // 버퍼에서 소모한 만큼 빼기 (소수점 유지됨)
        }

        float ratio = currentEnergy / maxEnergy;
        energyBar.value = ratio;
        string s = $"{currentEnergy}/ {maxEnergy}";
        energyText.text = s;
        energyTextBackground.text = s;
    }
}
