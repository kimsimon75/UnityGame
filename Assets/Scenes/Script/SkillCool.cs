using UnityEngine;

public class SkillCool
{
    private float endTime;

    public float Remaining => Mathf.Max(0f, endTime - Time.time);
    public bool IsReady => Time.time >= endTime;

    public SkillCool()
    {
        endTime = Time.time;
    }

    public float Start(float duration)
    {
        endTime = Time.time + duration;
        return endTime;
    }

    public void Reduce(float amount)
    {
        // endTime을 당기되 현재 시각 아래로는 못 내려가게
        endTime = Mathf.Max(Time.time, endTime - amount);
    }
}

