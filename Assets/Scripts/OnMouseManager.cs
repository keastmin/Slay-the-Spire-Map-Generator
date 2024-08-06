using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private NodeType type;
    RectTransform rectTransform;
    List<Node> nodes;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        nodes = new List<Node>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector2(1.4f, 1.4f);
        nodes = MapGenerator.instance.GetSameTypeNodes(type);
        foreach(Node node in nodes)
        {
            node.OnPointerEnter(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector2(1.0f, 1.0f);
        foreach(Node node in nodes)
        {
            node.OnPointerExit(eventData);
        }
    }
}
