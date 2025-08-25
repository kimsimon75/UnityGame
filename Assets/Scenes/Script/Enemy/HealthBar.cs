using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    private EnemyStats stats;
    private Story story;

    private Slider slider;

    private Image fillImage;
    public Transform worldBar;
    GameObject sliderGO;
    public RectTransform _hpBarTrans; // UI 좌표를 넣기 위한 클래스변수 선언
    public Vector3 _hpBarOffset; // UI좌표로 변환한 후 세부조정을 위해 벡터3
    
    
    void UpdateHpBarPos() // 체력바가 항상 유닛을 따라 다니도록 해주는 메서드
    {
        // 이 유닛의 위치를 가져와서 (월드 좌표)
        Vector3 unitPos = transform.position;

        // 위에서 가져온 월드좌표를 UI좌표로 변환한 후 세부조정 값을 유니티에서 조정한 값을 더해줌
        Vector3 screenPos = Camera.main.WorldToScreenPoint(unitPos + _hpBarOffset) - new Vector3(0, 0, 16);

        // 널 체크 (아직 구현 안한 캐릭터들을 위해서)
        if(sliderGO != null)
        {
            // 해당 체력바의 UI좌표를 위에서 변환한 캐릭터의 UI좌표로 바꿔줌(해당 체력바는 유니티에서 객체를 드래그&드랍으로 지정해줘야 함)
            sliderGO.GetComponent<RectTransform>().position = screenPos;
        }
    }


    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        story = GetComponent<Story>();
            
        // 1) 인스펙터로 할당 가능하게 해두고, 비어 있으면 찾기
        if (!worldBar)
        {
            // a) 내 부모 중에 Summoner가 있을 때
            GameManager summoner = GetComponentInParent<GameManager>();
            if (summoner)
                worldBar = summoner.transform.Find("Player1Zone/MagicZone/Player UI/WorldBars");
            else
                Debug.LogError("[HealthBar] Screen-space Canvas를 찾지 못했습니다. 인스펙터에 summoner 할당하세요.");


            // b) 그래도 못 찾으면 씬의 Screen-Space 캔버스 아무거나

        }

        if (!worldBar)
        {
            Debug.LogError("[HealthBar] Screen-space Canvas를 찾지 못했습니다. 인스펙터에 Canvas를 할당하세요.");
            enabled = false; // 이후 Start/Update 막기
            return;
        }
    }

    void Start()
    {

        // 슬라이더 생성
        sliderGO = new GameObject("HP_Bar", typeof(Slider));
        sliderGO.transform.SetParent(worldBar, false);
        RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(100f, 12f); // 가로 100, 세로 10
        slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;

        if (stats == null)
        {
            story.bar = sliderGO.gameObject;
            _hpBarOffset = new Vector3(0, 4f, 0);
        }
        else
        {
            stats.bar = sliderGO.gameObject;
            _hpBarOffset = new Vector3(0, 2f, 0);

            if (stats.boss == true)
            {
                _hpBarOffset = new Vector3(0, 6f, 0);
            }
        }

        // 배경 생성
        GameObject bgGO = new GameObject("Background", typeof(Image));
        bgGO.transform.SetParent(sliderGO.transform, false);
        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImage = bgGO.GetComponent<Image>();
        bgImage.color = Color.black;
        slider.targetGraphic = bgImage; // ✅ 꼭 필요!

        // Fill Area 생성
        GameObject fillAreaGO = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRT = fillAreaGO.GetComponent<RectTransform>();

        fillAreaRT.anchorMin = new Vector2(0f, 0f);
        fillAreaRT.anchorMax = new Vector2(1f, 1f);
        fillAreaRT.pivot = new Vector2(0.5f, 0.5f);
        fillAreaRT.sizeDelta = Vector2.zero;

        

        // Fill 생성
        GameObject fillGO = new GameObject("Fill", typeof(Image));
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        RectTransform fillRT = fillGO.GetComponent<RectTransform>();

        fillRT.anchorMin = new Vector2(0f, 0f);
        fillRT.anchorMax = new Vector2(1f, 1f);
        fillRT.pivot = new Vector2(0.5f, 0.5f);
        fillRT.sizeDelta = Vector2.zero;

        fillImage = fillGO.GetComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Sliced;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.material = Resources.Load<Material>("Material/MPGlossMat");

        slider.fillRect = fillRT;         // ✅ 필수
        slider.handleRect = null;         // ✅ 없으면 null 명시
        slider.direction = Slider.Direction.LeftToRight;
        UpdateHpBarPos();

    }

    void Update()
    {
        float ratio = 0;
        UpdateHpBarPos();
        if (stats == null)
            ratio = story.currentHealth / story.maxHealth;
        else
            ratio = stats.CurrentHealth / stats.MaxHealth;

        slider.value = ratio;

        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }
}
