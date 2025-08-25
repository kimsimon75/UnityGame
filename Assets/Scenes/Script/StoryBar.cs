using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider StoryHealth;
    public TextMeshProUGUI Level;
    public TextMeshProUGUI healthBar;
    public TextMeshProUGUI backgroundBar;
    public Story target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentHealth = target.currentHealth;
        float maxHealth = target.maxHealth;

        float ratio = currentHealth / maxHealth;

        StoryHealth.value = ratio;

        healthBar.text = $"{(int)currentHealth:N0}/{(int)maxHealth:N0}";
        backgroundBar.text = $"{(int)currentHealth:N0}/{(int)maxHealth:N0}";
        Level.text = target.level.ToString();
    }
}
