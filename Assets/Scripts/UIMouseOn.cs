using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOn : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        bool flag = EventSystem.current.IsPointerOverGameObject();
        if (flag)
        {
            Debug.Log("°¨ÁöµÊ");
        }
    }
}
