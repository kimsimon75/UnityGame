using System.Text;
using TMPro;
using UnityEngine;

public class StatsWindow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerStats stats;
    public ActionScript action;
    private TextMeshProUGUI texts;
    void Start()
    {
        texts = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

        int targetNumber = action.targetNumber;
        StringBuilder stringBuilder = new StringBuilder();

        if (GameManager.Instance.KeyValueNumber >= DataManager.NumCount)
        {
            var status = stats.GetStats();
            stringBuilder.AppendLine($"부대 {stats.action.targetNumber + 1}");
            stringBuilder.AppendLine($"공격력 : {status.Item1[targetNumber]}");
            stringBuilder.AppendLine($"공격 속도 : {(1 / status.Item2).ToString("F3")}");
            stringBuilder.AppendLine($"공속 보너스 : {status.Item3}%");
            stringBuilder.AppendLine($"공격력 비례 물리피해(짭플) : {status.Item11[targetNumber]}");
            stringBuilder.AppendLine($"공격 범위 : {status.Item12[targetNumber] * 100}");
            stringBuilder.AppendLine($"방어력 감소 : {status.Item4}");
            stringBuilder.AppendLine($"체력 재생 : {DataManager.Instance.RoundX(status.Item5, 3)}");
            stringBuilder.AppendLine($"마나 재생 : {DataManager.Instance.RoundX(status.Item6, 3)}");
            stringBuilder.AppendLine($"마법증폭 : {status.Item7}");
            stringBuilder.AppendLine($"마법방어력 감소 : {status.Item8}");
            stringBuilder.AppendLine($"방어무시 : {status.Item9}");
            stringBuilder.AppendLine($"이동속도 감소 : {status.Item10}");
        }
        else
        {
            if (GameManager.Instance.item.isActiveAndEnabled)
            {
                switch ((DataManager.Num)GameManager.Instance.KeyValueNumber)
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
            else
            {
                int keyValue = GameManager.Instance.KeyValueNumber;
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
                    case DataManager.Num.D:
                        S += $"";
                        break;
                    default:
                        S += "";
                        break;
                }
                stringBuilder.AppendLine(S);
            }
        }

        texts.text = stringBuilder.ToString();
    }
}
