using System;
using UnityEngine;

public class CannonBallMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [NonSerialized] public Transform target;
    [NonSerialized] public float speed = 10f;

    private int Damage;
    private ArmorType DamageType;
    void Start()
    {

    }

    // Update is called once per frame

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }


        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void Targeting(Transform newTarget)
    {
        target = newTarget;
    }

    void OnTriggerEnter(Collider other)
    {
        if (target!=null && other.gameObject == target.gameObject)
        {
            Actor stats = other.transform.GetComponent<Actor>();
                stats.TakeDamageAll_physics(0, Damage, 0, DamageType ,0 ,0 );
            Destroy(gameObject); // 또는 다른 처리
        }
    }

    public void SetDamage(int damage, ArmorType damageType)
    {
        Damage = damage;
        DamageType = damageType;
    }
}
