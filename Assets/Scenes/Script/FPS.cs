using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    private float deltaTime = 0f;

    [SerializeField] private int size = 25;
    [SerializeField] private Color color = Color.red;
    Rect rect ;

    void Awake()
    {
       rect = GetComponent<RectTransform>().rect;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(300, 90, Screen.width, Screen.height);


        float ms = deltaTime * 1000f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

        GUI.Label(rect, text, style);
    }
}