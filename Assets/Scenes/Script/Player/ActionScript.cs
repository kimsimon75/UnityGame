using System;
using DigitalRuby.LightningBolt;
using UnityEngine;
using UnityEngine.AI;

public class ActionScript : MonoBehaviour
{
    Animator anim;
    private bool isReady = false;
    private bool isAllReady = false;
    private PlayerAttack attack;
    private HoldScanner hold;
    private NavMeshAgent agent;
    private AgentMove move;
    private PlayerStats stats;
    public ItemManager item;
    public const int targetNumberMax = 6;
    [NonSerialized] public Transform[] target = new Transform[targetNumberMax];
    [NonSerialized] public int targetNumber = 5;
    public NavMeshHit point;
    [NonSerialized] public float[] attackDisableTime = new float[targetNumberMax];
    [NonSerialized] public bool[] isStop = new bool[targetNumberMax];
    public Actor targetParent = null;
    private bool OnTheStory = false;
    public Transform statsTarget = null;
    public Camera mainCamera;

    public Transform StoryCannon;
    public Transform MagicZone;

    private float zoomSpeed = 10f;
    private float minDistance = 30f;
    private float maxDistance = 110f;
    private Vector3 camOffset = new Vector3(0, 12f, -6f);
    private float targetDistance;
    private float zoomVelocity;
    private float smoothTimeZoom = 0.10f;
    public GameObject Lightning;
    [NonSerialized] public GameObject Clone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        attack = GetComponent<PlayerAttack>();
        hold = GetComponent<HoldScanner>();
        agent = GetComponent<NavMeshAgent>();
        move = GetComponent<AgentMove>();
        stats = GetComponent<PlayerStats>();

        TriggerHold();
        targetDistance = mainCamera.fieldOfView;
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y + 12f, transform.position.z - 6f);

        for (int i = 0; i < target.Length; i++)
        {
            target[i] = null;
        }
        Clone = Instantiate(Lightning, transform);
        Clone.GetComponent<LightningBoltScript>().StartObject = gameObject;
        Clone.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            TriggerStop();
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            TriggerHold();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            isReady = true;
        }
        else if (Input.GetMouseButtonDown(0) &&
    !UiRayUtil.IsPointerOverUIExcept(LayerMask.GetMask("Text")))
        {
            if (isReady)
            {
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
                {
                    target[targetNumber] = hitInfo.transform;
                    TriggerAttack();
                }
                else
                {
                    TriggerHold();
                }
                isReady = false;
            }
            else if (isAllReady)
            {
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
                {
                    for(int i=0;i<targetNumberMax;i++)
                    target[i] = hitInfo.transform;
                    TriggerAttack();
                }
                else
                {
                    TriggerHold();
                }
                isAllReady = false;
            }
            else
            {
                LayerMask mask = LayerMask.GetMask("Enemy", "Cannon");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, mask))
                {
                    statsTarget = hitInfo.transform;
                }

            }

        }
        else if (Input.GetMouseButtonDown(1) &&
    !UiRayUtil.IsPointerOverUIExcept(LayerMask.GetMask("Text")))
        {
            LayerMask enemyLayer = LayerMask.GetMask("Enemy");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, enemyLayer))
            {
                Debug.Log("here");
                target[targetNumber] = hitInfo.transform;
                TriggerAttack();
            }
            else if (Physics.Raycast(ray, out RaycastHit groundHit, 100f))
            {
                if (NavMesh.SamplePosition(groundHit.point, out NavMeshHit navHit, 1.0f, agent.areaMask))
                {
                    point = navHit;
                    TriggerMove();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            OnTheStory = !OnTheStory;
            Transform Goal;

            if (OnTheStory)
            {
                Goal = StoryCannon.transform;
                mainCamera.transform.position = new Vector3(Goal.position.x, Goal.position.y - 1f + 0.083333349f, Goal.position.z) + camOffset;
            }
            else
            {
                Goal = MagicZone.transform;
                mainCamera.transform.position = new Vector3(Goal.position.x, Goal.position.y + 0.073333349f, Goal.position.z) + camOffset;
            }
            TriggerHold();
            point = default;
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.Warp(Goal.position);   // NavMeshAgent 내부 좌표까지 동기화
                agent.ResetPath();           // 남아 있던 경로 제거 (선택)
            }
            else
            {
                // Agent 가 꺼져 있거나 아직 NavMesh 위가 아니면 transform 직접 이동
                transform.position = Goal.position;
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + camOffset;
        }
        
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                isAllReady = true;
            }
        }

        if (!item.gameObject.activeInHierarchy)
            for (int i = 0; i < targetNumberMax; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i + 1))
                {
                    targetNumber = i;
                    TriggerHold();
                }

                if (Input.GetKeyDown(KeyCode.Keypad0 + i + 1))
                {
                    targetNumber = i;
                    TriggerHold();
                }
            }

        float scroll = -Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            targetDistance = Mathf.Clamp(
                targetDistance + scroll * zoomSpeed,
                minDistance,
                maxDistance);
        }
    }

    void LateUpdate()
    {
        mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetDistance,
            ref zoomVelocity,
            smoothTimeZoom
        );

    }

    public void TriggerAttack()
    {
        attack.enabled = true;
        hold.enabled = false;
        agent.ResetPath();

        agent.isStopped = false;
        move.enabled = false;
        isStop[targetNumber] = false;

    }

    public void TriggerHold()
    {
        attack.enabled = false;
        hold.enabled = true;
        agent.isStopped = true;
        move.enabled = false;

        isStop[targetNumber] = false;
        anim.CrossFade("Idle", stats.blendingTime);
    }

    public void TriggerMove()
    {
        attack.enabled = false;
        hold.enabled = false;
        agent.isStopped = false;
        move.enabled = true;

        target[targetNumber] = null;
        isStop[targetNumber] = false;
        anim.CrossFade("Walking", stats.blendingTime);

    }

    public void TriggerStop()
    {
        attack.enabled = false;
        hold.enabled = false;
        agent.isStopped = true;
        target[targetNumber] = null;
        isStop[targetNumber] = true;
        anim.CrossFade("Idle", stats.blendingTime);
    }
    
    
    public bool IsAttackDisabledFor(int target)
    {
        return Time.time < attackDisableTime[target];
    }

}
