using System;
using DigitalRuby.LightningBolt;
using UnityEngine;
using UnityEngine.AI;

public class ActionScript : MonoBehaviour
{
    Animator anim;
    private bool isReady = false;
    private bool isAllReady = false;

    private bool snapCameraOnce = false;
    private PlayerAttack attack;
    private HoldScanner hold;
    private NavMeshAgent agent;
    private AgentMove move;
    private PlayerStats stats;
    private ItemManager item;
    private GameObject itemList;
    private const int targetNumberMax = 6;

    public int TargetNumberMax => targetNumberMax;
    [NonSerialized] public Transform[] target = new Transform[targetNumberMax];
    [NonSerialized] public int targetNumber = 5;
    [NonSerialized] public NavMeshHit point;
    [NonSerialized] public float[] attackDisableTime = new float[targetNumberMax];
    [NonSerialized] public bool[] isStop = new bool[targetNumberMax];
    private bool OnTheStory = false;
    [NonSerialized] public Transform statsTarget = null;
    [NonSerialized] public Camera mainCamera;

    [NonSerialized] public Transform StoryCannon;
    private Transform MagicZone;

    private float zoomSpeed = 10f;
    private float minDistance = 30f;
    private float maxDistance = 110f;
    private Vector3 camOffset = new Vector3(0, 75f, -75f);
    private float targetDistance;
    private float zoomVelocity;
    private float smoothTimeZoom = 0.10f;
    [NonSerialized] public GameObject Lightning;
    [NonSerialized] public GameObject Clone;
    ItemScrollView ScrollView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item = GameManager.Instance.ItemManager;
        itemList = GameManager.Instance.items;
        anim = GetComponent<Animator>();
        attack = GetComponent<PlayerAttack>();
        hold = GetComponent<HoldScanner>();
        agent = GetComponent<NavMeshAgent>();
        move = GetComponent<AgentMove>();
        stats = GetComponent<PlayerStats>();
        Lightning = GameManager.Instance.Lightnings;
        StoryCannon = GameManager.Instance.Cannons.transform;

        TriggerHold();
        mainCamera = Camera.main;
        targetDistance = mainCamera.fieldOfView;
        mainCamera.transform.position = transform.position + camOffset;

        for (int i = 0; i < target.Length; i++)
        {
            target[i] = null;
        }
        Clone = Instantiate(Lightning, transform);
        Clone.GetComponent<LightningBoltScript>().StartObject = gameObject;
        Clone.SetActive(false);
        ScrollView = GameManager.Instance.scrollView;
        MagicZone = GameManager.Instance.magicZone;

        if(item == null)
        {
            Debug.Log("item = null");
        }
        if(item.list == null)
        {
            Debug.Log("itemlist = null");
        }
        var targetQueue = item.list.currentItem[targetNumber];
        if (targetQueue == null || targetQueue.Count == 0)
        {
            Debug.Log("아이템 없음");
            return;
        }
        ScrollView.ImageInit(targetQueue);
    }

    // Update is called once per frame
    void Update()
    {


        if (snapCameraOnce)
        {
            
        mainCamera.transform.position = transform.position + camOffset;
        snapCameraOnce = false;
        }

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
            else if(GameManager.Instance.TeleportOn)
            {
                float height = gameObject.transform.position.y;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, new Vector3(0, height, 0));
                
                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 point = ray.GetPoint(enter);
                    GetComponent<Skill>().Teleport(point);
                    // point는 항상 y = fixedY
                    TriggerHold();
                    GameManager.Instance.Images[(int)DataManager.Num.Q].GetComponent<UnityEngine.UI.Outline>().enabled = false;
                }

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
            }
            else
            {
                Goal = MagicZone.transform;
 
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
            snapCameraOnce = true;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            mainCamera.transform.position = transform.position + camOffset;
        }
        
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                isAllReady = true;
            }
        }

        if (!itemList.gameObject.activeInHierarchy)
            for (int i = 0; i < targetNumberMax; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    targetNumber = i;
                    ScrollView.ImageInit(item.list.currentItem[targetNumber]);
                    TriggerHold();
                }

                if (Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    targetNumber = i;
                    ScrollView.ImageInit(item.list.currentItem[targetNumber]);
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
