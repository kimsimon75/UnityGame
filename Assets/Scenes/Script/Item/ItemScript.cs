
using UnityEngine;
using UnityEngine.UI;
public class ItemScript : MonoBehaviour
{
    GameObject obj;
    ItemManager item;
    private Image[] keyValueImages;

    private Image[] darkImg = new Image[DataManager.NumCount];

    Sprite[][] sprites = new Sprite[2][];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item = GetComponentInChildren<ItemManager>();
        obj = item.gameObject;
        obj.SetActive(!obj.activeSelf);

        keyValueImages = GameManager.Instance.Images;
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = new Sprite[DataManager.NumCount];
        }

        sprites[0][(int)DataManager.Num.Q] = Resources.Load<Sprite>("Key/귀속");
        sprites[0][(int)DataManager.Num.W] = Resources.Load<Sprite>("Key/낙뢰");
        sprites[0][(int)DataManager.Num.E] = Resources.Load<Sprite>("Key/메테오");
        sprites[0][(int)DataManager.Num.Z] = Resources.Load<Sprite>("Key/영혼 흡수");
        sprites[0][(int)DataManager.Num.X] = Resources.Load<Sprite>("Key/지진");
        sprites[0][(int)DataManager.Num.C] = Resources.Load<Sprite>("Key/독약");


        sprites[1][(int)DataManager.Num.Q] = Resources.Load<Sprite>("Key/흔함");
        sprites[1][(int)DataManager.Num.W] = Resources.Load<Sprite>("Key/중급 도박");
        sprites[1][(int)DataManager.Num.E] = Resources.Load<Sprite>("Key/고급 도박");
        sprites[1][(int)DataManager.Num.Z] = Resources.Load<Sprite>("Key/초급 도박");
        sprites[1][(int)DataManager.Num.X] = Resources.Load<Sprite>("Key/기억 조각");
        sprites[1][(int)DataManager.Num.C] = Resources.Load<Sprite>("Key/에너지 탱크");

        SetKey();

        for (int i = 0; i < DataManager.NumCount; i++)
        {
            Image darkImg = keyValueImages[i].GetComponentsInChildren<Image>()[2];
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

    void SetKey()
    {

        for (int i = 0; i < DataManager.NumCount; i++)
        {
            keyValueImages[i].sprite = keyValueImages[i].GetComponentsInChildren<Image>()[2].sprite = sprites[0][i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
            {
                for (int i = 0; i < DataManager.NumCount; i++)
                {
                    keyValueImages[i].sprite = keyValueImages[i].GetComponentsInChildren<Image>()[2].sprite = sprites[1][i];
                }


            }
            else
            {
                SetKey();
            }
        }

        float[] skillCooldown = GameManager.Instance.skillCooldown;

        for (int i = 0; i < DataManager.NumCount; i++)
        {

            darkImg[i].fillAmount = GameManager.Instance.item.isActiveAndEnabled ? 0 : skillCooldown[i] / GameManager.Instance.skillCoolInit[i];
        }


        if (!item.transform.gameObject.activeInHierarchy) return;

        if (TryGetNumericKey(out int number))
        {
            item.SetRank(number - 1);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("hello");
            int rank = item.GetRank();
            rank -= 1;
            if (rank < 0) rank = 7;
            item.SetRank(rank);

        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int rank = item.GetRank();
            rank += 1;
            if (rank > 7) rank = 0;
            item.SetRank(rank);
        }
    }

    bool TryGetNumericKey(out int number)
    {
        // 0 ~ 9 알파벳 키 (메인 키보드)
        for (KeyCode kc = KeyCode.Alpha1; kc <= KeyCode.Alpha8; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                number = kc - KeyCode.Alpha0;   // 열거형 간 정수 차이 = 숫자
                return true;
            }
        }

        // 넘패드 0 ~ 9 도 허용하고 싶으면 추가
        for (KeyCode kc = KeyCode.Keypad1; kc <= KeyCode.Keypad8; kc++)
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
