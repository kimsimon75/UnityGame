using UnityEngine;
using UnityEngine.UI;

public class GradientHealthBar : MonoBehaviour
{
    [Tooltip("체력 비율(0~1) 제어용 Slider")]
    public Slider healthSlider;

    // Fill Area 의 Image 
    private Image fillImage;

    // 최대체력, 현재체력은 CharacterStats 같은 스크립트에서 관리된다고 가정
    private EnemyStats stats;

    void Awake()
    {
        // 부모(캐릭터) 쪽 Stats 가져오기
        stats = GetComponentInParent<EnemyStats>();
        if (healthSlider == null)
        {
            Debug.LogError("GradientHealthBar: healthSlider를 할당해주세요.");
            enabled = false;
            return;
        }

        // Fill Area 안의 Image 컴포넌트 찾아두기
        fillImage = healthSlider.fillRect.GetComponent<Image>();
        if (fillImage == null)
        {
            Debug.LogError("GradientHealthBar: healthSlider.fillRect에 Image가 없습니다.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // 캐릭터 체력 비율 계산 (0~1)
        float ratio = stats.CurrentHealth / stats.MaxHealth;

        // Slider 값 갱신 (fillAmount에 반영됨)
        healthSlider.value = ratio;

        // Color 그라데이션: 0→빨강, 1→녹색
        // Lerp(a, b, t): t=0일 때 a, t=1일 때 b
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }
}
