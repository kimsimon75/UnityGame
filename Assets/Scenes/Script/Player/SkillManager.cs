using System.Collections.Generic;
public class SkillManager
{

    const int playerNumber = 8;
    public delegate void Skill();
    public readonly List<Skill>[] _skillList = CreateSkill();



    private static List<Skill>[] CreateSkill()
    {
        List<Skill>[] arr = new List<Skill>[playerNumber];

        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = new List<Skill>();
        }
        return arr;
    }
    public void CauseDamage(int player)
    {
        foreach (Skill skills in _skillList[player])
        {
            skills.Invoke();
        }
    }
    public void AddSkill(Skill skill, int player)
    {
        _skillList[player]?.Add(skill);
    }
    
        public void RemoveSkill(Skill skill ,int player)
    {
        _skillList[player]?.Remove(skill);
    }

}
