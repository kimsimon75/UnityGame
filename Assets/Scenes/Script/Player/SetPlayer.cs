using NUnit.Framework.Internal;
using UnityEngine;

public class SetPlayer : MonoBehaviour
{
    PlayerStats playerStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameObject.AddComponent<ActionScript>();
        playerStats = gameObject.AddComponent<PlayerStats>();
        gameObject.AddComponent<HoldScanner>();
        gameObject.AddComponent<PlayerAttack>();
        gameObject.AddComponent<Skill>();
        gameObject.AddComponent<Outline>();
        gameObject.AddComponent<AgentMove>();

        Outline outline = gameObject.GetComponent<Outline>();
        
        outline.OutlineWidth = 0.25f;
        outline.OutlineColor = new Color(0, 1, 0, 1);
    }
    
    public void Init(int index)
    {
        playerStats.alterEgoPlayer = index;

        if (index == DataManager.targetNumberMax - 1)
        {
            gameObject.AddComponent<AuraDebuffScanner>();
            gameObject.AddComponent<PlayerBar>();
            gameObject.AddComponent<StatsWindow>();
            gameObject.AddComponent<SkillManager>();
            gameObject.AddComponent<AutoAttack>();
            GameManager.Instance.originStatFor6 = gameObject.AddComponent<OriginStatFor6>();
        }
    }
}
