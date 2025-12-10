using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountScript : MonoBehaviour
{
    public Slider slider;
    public Image image;
    public bool setImage = false;
    public TextMeshProUGUI text;
    [NonSerialized] public int number;
    ItemManager itemManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemManager = GameManager.Instance.ItemManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            return;
        }

        if (setImage == true)
        {
            slider.maxValue = itemManager.list.FindItem(image.sprite.name, DataManager.Instance.imageDict[image.sprite]).count;
        }
        else
        {
            switch ((DataManager.Num)number)
            {
                case DataManager.Num.Q:
                    slider.maxValue = itemManager.list.FindItem("기억 조각", ItemRank.All).count;
                    break;
                case DataManager.Num.W:
                    slider.maxValue = itemManager.list.FindItem("기억 조각", ItemRank.All).count / 2;
                    break;
                case DataManager.Num.E:
                    slider.maxValue = itemManager.list.FindItem("기억 조각", ItemRank.All).count / 4;
                    break;
                case DataManager.Num.Z:
                case DataManager.Num.X:
                case DataManager.Num.C:
                    slider.maxValue = itemManager.list.FindItem("영혼 파편", ItemRank.All).count;
                    break;
            }

            image.sprite = DataManager.Instance.sprites[1][number];
        }


        text.text = slider.value.ToString();
    }

    public void SetNumber(int number)
    {
        setImage = false;
        slider.value = 0;
        this.number = number; 
    }
}
