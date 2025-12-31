using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkForward : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 targetPosition;    // 이동할 목표 위치
    private float moveSpeed;

    private Transform begin;
    private List<Vector3> waypoints;
    private int currentIndex = 0;
    [NonSerialized] public float StunTime = 0;


    private float rotationSpeed = 5f;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;
    }

    void Start()
    {

        Vector3 center = GameManager.Instance.PlayerZone.transform.position;
        moveSpeed = GetComponent<EnemyStats>().GetDamageInfo().moveSpeed *0.01f;



        if (begin == null)
        {
            begin = GameManager.Instance.magicZone;  // ✅ 이름으로 자동 연결
            if (begin == null)
            {
                Debug.LogError("MagicZone 오브젝트를 찾을 수 없습니다!");
                return;
            }
        }

        Vector3 origin = begin.position;
        float position = center.x - origin.x;
        waypoints = new List<Vector3>
        {
            center + new Vector3(-position, 0, -position),
            center + new Vector3(position, 0, -position),
            center + new Vector3(position, 0, position),
            center + new Vector3(1 - position, 0, position)
        };

        if (waypoints.Count > 0)
            targetPosition = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (StunTime > 0f)
        {
            StunTime -= Time.fixedDeltaTime;
            anim.CrossFade("Idle", 0f);
        }
        else
        {
            anim.CrossFade("Walking", 0f);
        }

        if (StunTime <= 0f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                    currentIndex = 0;

                targetPosition = waypoints[currentIndex];
            }
        }

    }
}
