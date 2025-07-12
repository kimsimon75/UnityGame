using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkForward : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 targetPosition;    // 이동할 목표 위치
    private float moveSpeed;

    private GameObject begin;
    private List<Vector3> waypoints;
    private int currentIndex = 0;
    public float StunTime = 0;


    private float rotationSpeed = 5f;
    private float mapSize;
    public GameObject map;

    void Awake()
    {
        Animator anim = GetComponent<Animator>();
        map = GameManager.Instance.gameObject;
        anim.applyRootMotion = false;
    }

    void Start()
    {

        moveSpeed = GetComponent<EnemyStats>().GetDamageInfo().moveSpeed;

        mapSize = map.GetComponent<Renderer>().bounds.size.x;

        float position = mapSize * 7 / 20;


        if (begin == null)
        {
            begin = GameObject.Find("MagicZone");  // ✅ 이름으로 자동 연결
            if (begin == null)
            {
                Debug.LogError("MagicZone 오브젝트를 찾을 수 없습니다!");
                return;
            }
        }

        Vector3 origin = begin.transform.position;
        waypoints = new List<Vector3>
        {
            origin + new Vector3(0, 0, -position),
            origin + new Vector3(position, 0, -position),
            origin + new Vector3(position, 0, 0),
            origin + new Vector3(1, 0, 0)
        };

        if (waypoints.Count > 0)
            targetPosition = waypoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(StunTime > 0f)
            StunTime -= Time.deltaTime;



    }
    void FixedUpdate()
    {
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
