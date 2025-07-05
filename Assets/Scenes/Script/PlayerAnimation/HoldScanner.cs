using UnityEngine;

public class HoldScanner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ActionScript action;
    private PlayerStats stats;
    private Animator anim;

    void Start()
    {
        action = GetComponent<ActionScript>();
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // 타깃 재검색
        if (action.target == null ||
            Vector3.Distance(action.target.position, transform.position) > stats.detectRange)
        {
            FindClosestEnemy(transform.position, stats.detectRange, LayerMask.GetMask("Enemy"));
        }

        if (action.target != null)
        {
            
            Vector3 dir = (action.target.position - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }

        if (action.isAttack) return;   // 쿨다운 중이면 대기
        if (action.target != null)
        {
            // 바라보기

            // ★ Attack 상태가 아닐 때에만 전환
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                !(anim.IsInTransition(0) &&
                anim.GetNextAnimatorStateInfo(0).IsName("Attack")))
            {    // 내부에서 쿨타임·파라미터 세팅
                anim.CrossFade("Attack", stats.blendingTime);
            }
        }
        else
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                anim.CrossFade("Idle", stats.blendingTime);
        }
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
        action.target = closest;
    }
}
