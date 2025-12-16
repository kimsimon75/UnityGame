using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CountButton : MonoBehaviour, IPointerClickHandler
{
    public Slider slider;
    public CountScript script;
    ChatManager chat;
    ItemManager itemManager;
    int commonCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = GameManager.Instance.ItemManager;
        chat = GameManager.Instance.chat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Insert();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Insert();
    }

    private void Insert()
    {
        if (script.setImage == true)
        {
            Item item = itemManager.list.FindItem(script.image.sprite.name, DataManager.Instance.imageDict[script.image.sprite]);
            ReturnTargetItem(item);
        }
        else
        {
            itemManager.TriggerMany(script.rank,(int)slider.value);
        }
        script.gameObject.SetActive(false);
    }

    private void ReturnTargetItem(Item item)
    {
        int count = (int)slider.value;
        item.count -= count;
        if (item.count == 0)
        {
            itemManager.list.GotItem.Remove(item);
            itemManager.list.DeleteUnrankedItem(item);
        }
        for (int i = 0; i < count; i++)
            {
                if (item.Name == "행운의 토큰" && item.Rank == ItemRank.희귀함)
                {
                    int rand = UnityEngine.Random.Range(0, 100);
                    if (rand < 37)
                    {
                        chat.Push($"<color=#FF8200>노획물</color> 판매 성공!");
                        itemManager.list.GetSoulParts(1);
                        rand = UnityEngine.Random.Range(0, 100);
                        if (rand < 40) itemManager.list.GetMemoriesParts(1);
                    }
                    else chat.Push($"<color=#FF8200>노획물</color> 판매 실패!");
                }
                else if (item.Name == "함선" && item.Rank == ItemRank.히든)
                {
                    string hex = ColorUtility.ToHtmlStringRGB(new Color32(233, 119, 157, 255));
                    chat.Push($"<color=#{hex}>함선</color>을 판매!");
                    itemManager.list.GetMemoriesParts(1);
                    itemManager.list.GetAll(1);
                }
                else if (item.Name == "이브" && item.Rank == ItemRank.히든)
                {
                    string hex = ColorUtility.ToHtmlStringRGB(new Color32(233, 119, 157, 255));
                    chat.Push($"<color=#{hex}>이브</color> 유닛을 판매!");
                    itemManager.list.GetRandomItem(ItemRank.희귀함);
                }
                else switch (item.Rank)
                    {
                        case ItemRank.흔함:
                            commonCount++;
                            if (commonCount >= 3)
                            {
                                commonCount = 0;
                                chat.Push($"<color=Orange>3포인트</color>가 누적!");
                                itemManager.list.GetSoulParts(1);
                                int commonRand = UnityEngine.Random.Range(0, 100);
                                if (commonRand < 35) itemManager.list.GetMemoriesParts(1);
                            }
                            else
                            {
                                string hex = ColorUtility.ToHtmlStringRGB(Color.green);
                                chat.Push($"<color=#{hex}>{ItemRank.흔함}</color> 유닛을 판매하여 {commonCount} 포인트 누적!");
                            }
                            break;
                        case ItemRank.안흔함:
                            int uncommonRand = UnityEngine.Random.Range(0, 100);
                            if (uncommonRand < 50)
                            {
                                string hex = ColorUtility.ToHtmlStringRGB(new Color32(176, 78, 248, 255));
                                chat.Push($"<color=#{hex}>{ItemRank.안흔함}</color> 안흔함 판매 성공!");
                                itemManager.list.GetSoulParts(1);
                            }
                            else
                            {
                                chat.Push($"{ItemRank.안흔함} 판매 실패!");
                            }
                            uncommonRand = UnityEngine.Random.Range(0, 100);
                            if (uncommonRand < 20)
                            {
                                itemManager.list.GetMemoriesParts(1);
                            }
                            break;
                        case ItemRank.특별함:
                            chat.Push($"<color=yellow>{ItemRank.특별함}</color> 유닛을 판매!");
                            itemManager.list.GetSoulParts(1);
                            int special = UnityEngine.Random.Range(0, 100);
                            if (special < 35) itemManager.list.GetMemoriesParts(1);
                            break;
                        case ItemRank.희귀함:
                            chat.Push($"<color=#FF00FF>{ItemRank.희귀함}</color> 유닛을 판매!");
                            itemManager.list.GetSoulParts(2);
                            itemManager.list.GetMemoriesParts(1);
                            break;
                    }
            }
        itemManager.Clear(itemManager.editItem, false);
    }
}
