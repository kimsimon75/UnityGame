using System;
using System.Text;
using TMPro;
using UnityEngine;

public class StatsWindow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [NonSerialized] public PlayerStats stats;
    [NonSerialized] public ActionScript action;
    private TextMeshProUGUI PlayerStatsText;
    void Start()
    {
        PlayerStatsText = GameManager.Instance.PlayerStatsText;
        action = GetComponent<ActionScript>();
        stats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
        PlayerStatsText.SetVerticesDirty();       

        int targetNumber = GameManager.Instance.originStatFor6.targetNumber;
        StringBuilder stringBuilder = new StringBuilder();
        int keyValue = GameManager.Instance.KeyValueNumber;

        if (keyValue >= DataManager.NumCount)
        {
            var status = stats.GetStats();
            stringBuilder.AppendLine($"부대 {GameManager.Instance.originStatFor6.targetNumber + 1}");
            stringBuilder.AppendLine($"공격력 : {status.damage[targetNumber]} + {status.damage[targetNumber] * status.damageBonus[targetNumber]:F0}");
            stringBuilder.AppendLine($"공격 속도 : {1 / status.attackCooldown:F3}");
            stringBuilder.AppendLine($"공속 보너스 : {status.attackSpeedBonus:F3}%");
            stringBuilder.AppendLine($"공격력 비례 물리피해(짭플) : {status.doublePhysics[targetNumber]:F3}");
            stringBuilder.AppendLine($"공격 범위 : {status.Radius[targetNumber] * 100}");
            stringBuilder.AppendLine($"체력 재생 : {status.HealthRegen:F3}");
            stringBuilder.AppendLine($"마나 재생 : {status.manaRegen:F3}");
            stringBuilder.AppendLine($"마법증폭 : {status.MagicalBuffer * 100}%");
            stringBuilder.AppendLine($"마법방어력 감소 : {( status.MagicalDebuffer - 1) * 100:F3}%");
            stringBuilder.AppendLine($"방어무시 : {status.TrueDamage * 100:F3}%");
        }
        else if (keyValue != DataManager.NumCount -1)
        {
            if (GameManager.Instance.items.activeSelf)
            {
                switch ((DataManager.Num)keyValue)
                {
                    case DataManager.Num.Q:
                        stringBuilder.AppendLine($"기억 조각 1개를 소모하여 85%의 확률로 흔함과 안흠함중에 하나를 뽑습니다.");
                        break;
                    case DataManager.Num.W:
                        stringBuilder.AppendLine($"기억 조각 2개를 소모하여 70%의 확률로 특별함 하나를 뽑습니다.");
                        break;
                    case DataManager.Num.E:
                        stringBuilder.AppendLine($"기억 조각 4개를 소모하여 70%의 확률로 특별함과 희귀함중에 하나를 뽑습니다.");
                        break;
                    case DataManager.Num.Z:
                        stringBuilder.AppendLine($"영혼 파편 하나를 소모하여 흔함을 하나 얻습니다.");
                        break;
                    case DataManager.Num.X:
                        stringBuilder.AppendLine($"영혼 파편 하나를 소모하여 66%의 확률로 기억 조각 하나를 얻습니다.");
                        break;
                    case DataManager.Num.C:
                        stringBuilder.AppendLine($"");
                        break;
                    case DataManager.Num.D:
                        stringBuilder.AppendLine($"영혼 파편을 자동으로 변환합니다.");
                        break;
                }
            }
            else if (!GameManager.Instance.SkillToggle)
            {
                stringBuilder.AppendLine($"스킬명 : {DataManager.Instance.sprites[0][keyValue].name}");
                stringBuilder.AppendLine($"에너지 소모량 : {GameManager.Instance.skillEnergy[keyValue]}");
                if (GameManager.Instance.skillIndicate[keyValue] > 0)
                    stringBuilder.AppendLine($"범위 : {GameManager.Instance.skillIndicate[keyValue] * 100}");
                else
                    stringBuilder.AppendLine($"범위 : 단일");
                stringBuilder.AppendLine($"스킬 재사용 대기시간 : {GameManager.Instance.skillCoolInit[keyValue]}초");

                string S = "스킬 설명 : ";
                switch ((DataManager.Num)keyValue)
                {
                    case DataManager.Num.Q:
                        S += $"유닛 하나에게 12,500,000의 마법데미지와 7%의 전체체력 비례 데미지를 입힌 후 5초동안 속박시킵니다.";
                        break;
                    case DataManager.Num.W:
                        S += $"유닛 하나에게 22,500의 마법데미지를 입힌 후 2초동안 속박시킵니다.";
                        break;
                    case DataManager.Num.E:
                        S += $"해당 범위에 7,000,000의 고정데미지를 입힙니다.";
                        break;
                    case DataManager.Num.Z:
                        S += $"유닛 하나를 삭제시킵니다.";
                        break;
                    case DataManager.Num.X:
                        S += $"해당 범위의 유닛들을 3초동안 속박시킵니다.";
                        break;
                    case DataManager.Num.C:
                        S += $"독약을 뿌려 해당 유닛 주변 유닛들의 방어력을 20 깎습니다";
                        break;
                    default:
                        S += "";
                        break;
                }
                stringBuilder.AppendLine(S);
            }
            else
            {   
                stringBuilder.AppendLine($"스킬명 : {DataManager.Instance.sprites[2][keyValue].name}");

                string S = "스킬 설명 : ";
                switch ((DataManager.Num)keyValue)
                {
                    case DataManager.Num.Q:
                        S += $"{stats.someSortOfSkillEffect[0]}거리를 도약합니다.";
                        break;
                    case DataManager.Num.W:
                        S += $"{stats.someSortOfSkillDuration[1]}초동안 공격속도를 {stats.someSortOfSkillEffect[1]}% 증가시킵니다.";
                        break;
                    case DataManager.Num.E:
                        S += $"{stats.someSortOfSkillDuration[2]}초동안 공격력을 {stats.someSortOfSkillEffect[2]}% 증가시킵니다.";
                        break;
                    case DataManager.Num.Z:
                        S += $"유닛 하나를 삭제시킵니다.";
                        break;
                    case DataManager.Num.X:
                        S += $"해당 범위의 유닛들을 3초동안 속박시킵니다.";
                        break;
                    case DataManager.Num.C:
                        S += $"독약을 뿌려 해당 유닛 주변 유닛들의 방어력을 20 깎습니다";
                        break;
                    default:
                        S += "";
                        break;
                }
                stringBuilder.AppendLine(S);
            }
        }
        else
        {
            stringBuilder.AppendLine("스킬들을 변경합니다.");
        }

        PlayerStatsText.text = stringBuilder.ToString();
    }
}
