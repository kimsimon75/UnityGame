using System.Linq;
using UnityEngine;

public class CannonManager : MonoBehaviour
{
    Cannon[] cannons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        
    }
    void Start()
    {
        cannons = GetComponentsInChildren<Cannon>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCannon(int damage, int speed)
    {
        foreach (Cannon cannon in cannons)
        {
            if (cannon != null)
            {
                cannon.damage += damage;
                cannon.attackCooldownBuff += speed;

            }
        }
    }
}
