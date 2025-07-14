using System.Collections.Generic;
using UnityEngine;
public class SkillManager : MonoBehaviour
{

    const int playerNumber = 8;
    public delegate void Skill(Actor actor, object[] commonSkills);
    public List<Skill>[] skillList;
    static public SkillManager Instance;

    void Awake()
    {
        Instance = this;
        skillList = new List<Skill>[playerNumber];

        for (int i = 0; i < skillList.Length; i++)
        {
            skillList[i] = new List<Skill>();
        }
    }

    public void CauseDamage(Actor actor,object[] Skill, int player)
    {
        foreach (Skill skills in skillList[player])
        {
            skills.Invoke(actor,Skill);
        }
    }
    public void AddSkill(Skill skill, int player)
    {
        skillList[player]?.Add(skill);
    }
    
        public void RemoveSkill(Skill skill ,int player)
    {
        skillList[player]?.Remove(skill);
    }

}
