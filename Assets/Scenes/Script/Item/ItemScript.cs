
using UnityEngine;
using UnityEngine.UI;
public class ItemScript : MonoBehaviour
{
    GameObject obj;
    ItemManager item;
    private Image[] keyValueImages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        item = GetComponentInChildren<ItemManager>();
        obj = item.gameObject;
        obj.SetActive(!obj.activeSelf);

        keyValueImages = GameManager.Instance.Images;
        SetKey();
    }

    void SetKey()
    {

        keyValueImages[DataManager.Z].sprite = Resources.Load<Sprite>("Key/영혼 흡수");
        keyValueImages[DataManager.W].sprite = Resources.Load<Sprite>("Key/낙뢰");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            obj.SetActive(!obj.activeSelf);
            if (obj.activeSelf)
            {
                keyValueImages[DataManager.Q].sprite = Resources.Load<Sprite>("Key/흔함");
                keyValueImages[DataManager.W].sprite = Resources.Load<Sprite>("Key/중급 도박");
                keyValueImages[DataManager.E].sprite = Resources.Load<Sprite>("Key/고급 도박");
                keyValueImages[DataManager.Z].sprite = Resources.Load<Sprite>("Key/초급 도박");
                keyValueImages[DataManager.X].sprite = Resources.Load<Sprite>("Key/기억 조각");
                keyValueImages[DataManager.C].sprite = Resources.Load<Sprite>("Key/에너지 탱크");
            }
            else
            {
                SetKey();
            }
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
    
}
