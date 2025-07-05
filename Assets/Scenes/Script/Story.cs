using UnityEngine;

public class Story : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentHealth = 0;
    public float maxHealth = 0;
    int[] story = new int[13];
    byte level = 0;
    void Start()
    {
        story[0] = 100;
        story[1] = 100;
        story[2] = 100;
        story[3] = 100;
        story[4] = 100;
        story[5] = 100;
        story[6] = 100;
        story[7] = 100;
        story[8] = 100;
        story[9] = 100;
        story[10] = 100;
        story[11] = 100;
        story[12] = 100;

        currentHealth = maxHealth = story[level++];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamageAll(float damageAll, float damage, float detectRange)
    {
          currentHealth = Mathf.Max(currentHealth - damage - damageAll, 0f);
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth = story[level++];
        }
    }
}
