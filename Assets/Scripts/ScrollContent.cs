using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour
{
    public float viewNodePos = 100f;
    public ScrollRect scrollRect;
    public RectTransform viewportTransform;
    RectTransform contentTransform;

    private void Awake()
    {
        contentTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Node node = MapGenerator.instance.GetButtonPosY();
        if (node != null)
        {
            RectTransform nodeRect = node.rectTransform;
            float contentHeight = contentTransform.rect.height;
            float viewportHeight = viewportTransform.rect.height;
            float movePos = nodeRect.position.y - viewNodePos;
            Vector2 newVector = contentTransform.anchoredPosition;
            newVector.y -= movePos;
            if (newVector.y > 0) newVector.y = 0;
            if (newVector.y < viewportHeight - contentHeight) newVector.y = viewportHeight - contentHeight;
            contentTransform.anchoredPosition = newVector;
        }
    }
}
