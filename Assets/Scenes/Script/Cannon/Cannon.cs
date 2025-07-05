using System;
using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [NonSerialized] public Transform target;
    float detectRange = 5f;
    bool isAttack = false;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float attackCooldown = 2f;
    private float attackDelay;
    public int attackCooldownBuff = 0;
    public int damage = 10;
    void Start()
    {
        attackDelay = attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        attackDelay = attackCooldown * (attackCooldownBuff* 0.01f + 1);
        if (target == null || Vector3.Distance(target.position, transform.position) > detectRange)
        {
            FindClosestEnemy(transform.position, detectRange, LayerMask.GetMask("Enemy"));
        }

        if (target != null)
        {
            // 바라보기
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f; // 수평 회전만 하게 y 제거

            Quaternion rot = Quaternion.LookRotation(dir); // 기본 방향
            rot *= Quaternion.Euler(0f, 90f, 0f); // y축 기준 90도 추가 회전

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
        if (!isAttack && target!= null)
            StartCoroutine(Fire());
    }

    public void FindClosestEnemy(Vector3 origin, float range, LayerMask enemyLayer)
    {
        Collider[] hits = Physics.OverlapSphere(origin, range, enemyLayer);

        float closestDist = float.MaxValue;
        Transform closest = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(hit.transform.position, origin);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }
        target = closest;
    }

    public IEnumerator Fire()
    {
        isAttack = true;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        CannonBallMove move = projectile.GetComponent<CannonBallMove>();

        move.Targeting(target);
        move.SetDamage(damage);
        
        
        yield return new WaitForSeconds(attackDelay);
        isAttack = false;
    }
}
