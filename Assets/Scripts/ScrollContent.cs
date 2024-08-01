using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour
{
    RectTransform contentTransform;
    public Button[] buttons;
    int index = 0;

    private void Start()
    {
        contentTransform = GetComponent<RectTransform>();
        for(int i = 0; i < buttons.Length; i++)
        {
            RectTransform rectTransform = buttons[i].GetComponent<RectTransform>();
            Debug.Log(rectTransform.position.y + " " + rectTransform.anchoredPosition.y);
        }
        Debug.Log(contentTransform.position.y + " " + contentTransform.anchoredPosition.y);
    }

    public void OnClickTest()
    {
        RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
        float posY = rectTransform.anchoredPosition.y;
        posY -= 100;

        contentTransform.anchoredPosition = new Vector2(contentTransform.anchoredPosition.x, -posY);

        index++;
        if (index == buttons.Length)
        {
            index = 0;
        }
    }
}
