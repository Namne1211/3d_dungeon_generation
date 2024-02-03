using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

public class InventoryUIInteraction : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    [SerializeField] GameObject clickedItemUI;

    public Transform draggedItemParent;
    public Transform draggedItem;

    private bool _dragged;
    
    //On Begin Drag 
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Check for items
        if (transform.GetComponent<Slot>().itemInSlot == null)
            return;
        _dragged = true;
        draggedItemParent = transform;
        draggedItem = draggedItemParent.GetComponentInChildren<RawImage>().transform;
        draggedItemParent.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        draggedItem.SetParent(FindObjectOfType<Canvas>().transform);
    }
    
    //On Drag
    public void OnDrag(PointerEventData eventData)
    {
        if(!_dragged) return;
        draggedItem.position = Input.mousePosition;
        draggedItem.GetComponent<RawImage>().raycastTarget = false;
    }
    
    //On end Drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if(!_dragged) return;
        draggedItem.SetParent(draggedItemParent);
        draggedItem.localPosition = new Vector3(0, 0, 0);
        draggedItem.GetComponent<RawImage>().raycastTarget = true;
        draggedItemParent.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
        draggedItemParent.GetComponent<Slot>().SetStats();
        draggedItem = null;
        draggedItemParent = null;

    }
    
    //On Click
    public void OnPointerClick(PointerEventData eventData)
    {
        //Check for item or already display
        if (eventData.pointerClick.GetComponent<Slot>().itemInSlot == null || clickedItemUI.activeInHierarchy)
            return;
        
        //move the UI to the mouse and show it
        clickedItemUI.transform.position = Input.mousePosition + new Vector3(clickedItemUI.GetComponent<RectTransform>().rect.width * 1.5f / 2 +1,-(clickedItemUI.GetComponent<RectTransform>().rect.height * 1.5f / 2 -1),0);
        clickedItemUI.GetComponent<ClickedItem>().clickedSlot = eventData.pointerClick.GetComponent<Slot>();
        clickedItemUI.SetActive(true);
    }
    
    //On Exit
    public void OnPointerExit(PointerEventData eventData)
    {   
        //Turn off UI
        clickedItemUI.SetActive(false);
    }
    
}