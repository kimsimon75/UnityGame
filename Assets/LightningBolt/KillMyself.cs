using System;
using UnityEngine;

public class KillMyself : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PlayerStats player;
    private float threshold01;   // hitTiming + duration
    private float info;
    void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        info += Time.deltaTime;

        float progress01 = info / player.attackCooldown[player.GetComponent<ActionScript>().targetNumber] % 1f;
        if (progress01 >= threshold01)
        {
            Destroy(gameObject);
        }
    }

    public void Init (float duration)
    {
        threshold01 = duration;
    }
}
