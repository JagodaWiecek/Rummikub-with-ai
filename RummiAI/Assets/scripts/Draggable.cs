using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// Klasa do przesuwania i uk³adania 2d p³ytek
/// Dla lepszego korzystania przez u¿ytkownika
/// By gracz móg³ u³o¿yæ p³ytki by by³o mu lepiej graæ
/// </summary>
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{

    Transform parentToReturnTo = null;///do zapamietania gdzie jest rodzic karty przy przesuwaniu
    GameObject placeholder = null; ///na wytworzenie tymczasowego objektu, który pojawia siê przy przesuwaniu
    /// <summary>
    ///Rozpoczêcie przesuwania, zainicjowanie objektu tymczasowego
    /// </summary>
    /// <param name="eventData" - przytrzymywany przycisk myszy></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");

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
    /// <summary>
    ///przesuwanie objektem i placeholderem by by³a mo¿liwoœæ zmieniania kolejnoœci
    /// </summary>
    /// <param name="eventData" - przytrzymywany przycisk myszy></param>
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
    /// <summary>
    /// funkcja koñcz¹ca przesuwanie p³ytki 2d
    /// </summary>
    /// <param name="eventData" - przytrzymywany przycisk myszy></param>
    public void OnEndDrag(PointerEventData eventData) {
       // Debug.Log("OnEndDrag");
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        Destroy(placeholder);
    }

}
