using System;
using UnityEngine;

public class OriginStatFor6 : MonoBehaviour
{
    
    [NonSerialized] public int targetNumber = 5;
    [NonSerialized] public float blendingTime = 0f;
    private GameObject itemList;
    ItemScrollView ScrollView;
    ItemManager itemManager;
    [NonSerialized] public GameObject[] alterEgoPlayers = new GameObject[DataManager.targetNumberMax];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        itemList = GameManager.Instance.items;
        ScrollView = GameManager.Instance.scrollView;
        itemManager = GameManager.Instance.ItemManager;
        alterEgoPlayers = GameManager.Instance.player;
    }

    // Update is called once per frame
    void Update()
    {
            if (!itemList.gameObject.activeInHierarchy)
            for (int i = 0; i < DataManager.targetNumberMax; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    targetNumber = i;
                    ScrollView.ImageInit(itemManager.list.currentItem[targetNumber], true);
                    alterEgoPlayers[i].GetComponent<ActionScript>().TriggerHold();
                }

                if (Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    targetNumber = i;
                    ScrollView.ImageInit(itemManager.list.currentItem[targetNumber], true);
                    alterEgoPlayers[i].GetComponent<ActionScript>().TriggerHold();
                }
            }
    }
}
