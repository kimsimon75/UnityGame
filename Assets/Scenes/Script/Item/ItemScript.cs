
using UnityEngine;
using UnityEngine.UI;
using static MyMathf;
public class ItemScript : MonoBehaviour
{
    GameObject obj;
    ItemManager item;
    private Image[] keyValueImages;

    private Image[] darkImg = new Image[DataManager.NumCount];

    SkillCool[] skillCooldown;
    SkillCool[] someSortOfSkillCooldown;

    Sprite[][] sprites = new Sprite[2][];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skillCooldown = GameManager.Instance.skillCooldown;
        someSortOfSkillCooldown = GameManager.Instance.player.someSortOfSkillCooldown;
        item = GameManager.Instance.ItemManager;
        obj = item.transform.Find("Items").gameObject;

        keyValueImages = GameManager.Instance.Images;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = new Sprite[DataManager.NumCount];
        }

        sprites = DataManager.Instance.sprites;

        SetKey();

        for (int i = 0; i < DataManager.NumCount; i++)
        {
            Image darkImg = keyValueImages[i].GetComponentsInChildren<Image>()[3];
            SetDarker(darkImg, 0.3f);

            darkImg.type = Image.Type.Filled;

            // 1) 세로 채우기(위에서 아래로 줄어드는 쿨다운 같은 UI)
            darkImg.fillMethod = Image.FillMethod.Radial360;
            darkImg.fillOrigin = (int)Image.Origin360.Top; // Top = 2
            darkImg.fillClockwise = false;                      // Inspector의 체크 해제
            darkImg.fillAmount = 0f;

            this.darkImg[i] = darkImg;                            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int i = 0; i < keyValueImages.Length; i++)
            {
                keyValueImages[i].GetComponent<UnityEngine.UI.Outline>().enabled = false;
            }
                obj.SetActive(!obj.activeSelf);

            if (obj.activeSelf)
            {
                for (int i = 0; i < DataManager.NumCount; i++)
                {
                    keyValueImages[i].GetComponentsInChildren<Image>()[1].sprite = keyValueImages[i].GetComponentsInChildren<Image>()[3].sprite = sprites[1][i];
                    GameObject CooldownTimer = keyValueImages[i].transform.Find("Image/CooldownBG").gameObject;
                    CooldownTimer.SetActive(false);
                }
                keyValueImages[(int)DataManager.Num.D].GetComponent<UnityEngine.UI.Outline>().enabled = item.isAllToggle;
                if(item.isAllToggle) keyValueImages[(int)Log2(item.SetSoulParts) + (int)DataManager.Num.Z].GetComponent<UnityEngine.UI.Outline>().enabled = true;
            }
            else
            {
                GameManager.Instance.Count.SetActive(false);
                SetKey();
            }
        }

        if(Input.GetKeyDown(KeyCode.D) && !obj.activeSelf)
        {
            SetKey();
        }


        for (int i = 0; i < DataManager.NumCount-1; i++)
        {
            darkImg[i].fillAmount = GameManager.Instance.itemList.activeSelf ? 0 : 
            (GameManager.Instance.SkillToggle ? (someSortOfSkillCooldown[i].Remaining / GameManager.Instance.player.someSortOfSkillCooltime[i]) :
             (skillCooldown[i].Remaining / GameManager.Instance.skillCoolInit[i]));
        }


        if (!item.transform.Find("Items").gameObject.activeInHierarchy) return;

        if (TryGetNumericKey(out int number))
        {
            item.SetRank(number - 1);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("hello");
            int rank = item.GetRank();
            rank -= 1;
            if (rank < 0) rank = (int)ItemRank.상위;
            item.SetRank(rank);

        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int rank = item.GetRank();
            rank += 1;
            if (rank > (int)ItemRank.상위) rank = 0;
            item.SetRank(rank);
        }
    }
    public void SetKey()
    {

        for (int i = 0; i < DataManager.NumCount; i++)
        {
            keyValueImages[i].GetComponentsInChildren<Image>()[1].sprite = keyValueImages[i].GetComponentsInChildren<Image>()[3].sprite = sprites[(!GameManager.Instance.SkillToggle) ? 0 : 2][i];
            if (i != DataManager.NumCount-1  && GameManager.Instance.skillCooldown[i].Remaining> 0)
            {
                GameObject CooldownTimer = keyValueImages[i].transform.Find("Image/CooldownBG").gameObject;
                CooldownTimer.SetActive(true);
            }
        }
    }
    bool TryGetNumericKey(out int number)
    {
        // 0 ~ 9 알파벳 키 (메인 키보드)
        for (KeyCode kc = KeyCode.Alpha1; kc <= (int)ItemRank.획득 + KeyCode.Alpha0; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                number = kc - KeyCode.Alpha0;   // 열거형 간 정수 차이 = 숫자
                return true;
            }
        }

        // 넘패드 0 ~ 9 도 허용하고 싶으면 추가
        for (KeyCode kc = KeyCode.Keypad1; kc <= (int)ItemRank.획득 + KeyCode.Keypad0; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                number = kc - KeyCode.Keypad0;
                return true;
            }
        }

        number = -1;
        return false;        // 이번 프레임엔 숫자키 입력 없음
    }
    void SetDarker(Image img, float factor) // factor: 0~1 (1=원래 밝기, 0=완전 검정)
    {
        var c = img.color; // 기존 틴트 보존
        img.color = new Color(c.r * factor, c.g * factor, c.b * factor, c.a);
    }
}
