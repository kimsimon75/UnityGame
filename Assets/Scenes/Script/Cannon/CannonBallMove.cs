using System;
using UnityEngine;

public class CannonBallMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [NonSerialized] public Transform target;
    [NonSerialized] public float speed = 10f;

    private float Damage;
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
        if (other.CompareTag("Enemy"))
        {
            EnemyStats stats = other.transform.GetComponent<EnemyStats>();
            if (stats != null)
                stats.TakeDamageAll(Damage, 0, 3f);
            else
                other.transform.GetComponent<Story>().TakeDamageAll(Damage, 0, 3f);
            Destroy(gameObject); // 또는 다른 처리
        }
    }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }
}
