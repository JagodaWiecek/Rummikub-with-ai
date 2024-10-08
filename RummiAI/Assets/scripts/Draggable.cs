using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{
    Transform parentToReturnTo = null;

    GameObject placeholder = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
         Debug.Log("OnBeginDrag");

        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleHeight = 0;
        le.flexibleWidth = 0;
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());


        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("OnDrag");

        this.transform.position = eventData.position;

        int newSiblingIndex = parentToReturnTo.childCount;

        for(int i = 0; i < parentToReturnTo.childCount ;i++)
        {
            if(this.transform.position.x<parentToReturnTo.GetChild(i).position.x)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                    newSiblingIndex--;
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        Destroy(placeholder);
    }
}
