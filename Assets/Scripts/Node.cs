using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public NodeData data;
    public List<Node> nextNodes;
    public List<Node> floorNodes; // Same Floor Node List
    public List<Node> prevNodes;
    public Image checkImage;
    public NodeType nodeType { get; private set; }
    public RectTransform rectTransform;


    private Color unselectColor;
    private Color selectColor;
    private Button button;
    private Animator animator;
    private Image nodeSprite;
    private bool prevSelect;
    private bool _selectState;
    public bool selectState
    {
        get { return _selectState; }
        set
        {
            _selectState = value;
            ActiveAnimation();
        }
    }

    private void Awake()
    {
        unselectColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        selectColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        nextNodes = new List<Node>();
        prevNodes = new List<Node>();
        nodeSprite = GetComponent<Image>();
        animator = GetComponent<Animator>();
        prevSelect = false;
        _selectState = false;
        button.interactable = false;
    }

    private void OnEnable()
    {
        ActiveAnimation();
    }

    public void NodeSet()
    {
        data = NodeReturn.instance.GetRandomNodeData();
        nodeSprite.sprite = data.sprite;
        nodeType = data.type;
        nodeSprite.color = unselectColor;
    }

    public void NodeSet(NodeData nodeData)
    {
        data = nodeData;
        nodeSprite.sprite = data.sprite;
        nodeType = data.type;
        nodeSprite.color = unselectColor;
    }

    private void ActiveAnimation()
    {
        button.interactable = _selectState;
        animator.SetBool("IsActive", _selectState);
        if (_selectState)
        {
            nodeSprite.color = selectColor;
        }
    }

    public void OnClick()
    {
        prevSelect = true;
        selectState = false;
        StartCoroutine("CheckNode");
        foreach(Node node in floorNodes)
        {
            node.nodeSprite.color = unselectColor;
            node.selectState = false;
        }
        foreach(Node node in nextNodes)
        {
            node.selectState = true;
        }
    }

    IEnumerator CheckNode()
    {
        float startTime = 0.0f;
        while(startTime < 0.2f)
        {
            startTime += Time.deltaTime;
            checkImage.fillAmount = Mathf.Lerp(0.0f, 1.0f, startTime / 0.2f);
            yield return null;
        }
        checkImage.fillAmount = 1.0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nodeSprite.color = selectColor;
        animator.SetBool("IsMouseOn", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nodeSprite.color = (selectState || prevSelect) ? selectColor : unselectColor;
        animator.SetBool("IsMouseOn", false);
    }
}
