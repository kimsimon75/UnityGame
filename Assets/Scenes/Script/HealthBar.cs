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

    void Awake()
    {
        stats = GetComponent<EnemyStats>();
        story = GetComponent<Story>();
    }

    void Start()
    {

        GameObject canvasGO = new GameObject("HP_Canvas", typeof(Canvas));
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.localPosition = new Vector3(0, 2, 0);
        if (stats == null)
            canvasGO.transform.localPosition = new Vector3(0, 0, 0.01f);
        canvasGO.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0.15f);
        canvasGO.AddComponent<Billboard>();
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        // 슬라이더 생성
        GameObject sliderGO = new GameObject("HP_Bar", typeof(Slider));
        sliderGO.transform.SetParent(canvasGO.transform, false);
        RectTransform sliderRT = sliderGO.GetComponent<RectTransform>();
        sliderRT.sizeDelta = new Vector2(1.5f, 0.1f); // 가로 100, 세로 10
        slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 1;

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

        slider.fillRect = fillRT;         // ✅ 필수
        slider.handleRect = null;         // ✅ 없으면 null 명시
        slider.direction = Slider.Direction.LeftToRight;

    }

    void Update()
    {
        float ratio = 0;
        if (stats == null)
            ratio = story.currentHealth / story.maxHealth;
        else
            ratio = stats.CurrentHealth / stats.MaxHealth;

        slider.value = ratio;

        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }
}
