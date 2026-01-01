using System;
using UnityEngine;

public class KillMyself : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerStats player;
    private float threshold01;   // hitTiming + duration
    public float info;
    void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        info += Time.deltaTime;

        float progress01 = info / player.attackCooldown % 1f;
        if (progress01 >= threshold01)
        {
            gameObject.SetActive(false);
        }
    }

    public void Init (float duration)
    {
        threshold01 = duration;
    }
}
