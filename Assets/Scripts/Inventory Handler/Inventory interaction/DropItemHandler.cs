using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject player;
    public void OnDrop(PointerEventData eventData)
    {

        try
        {
            GameObject itemdrop = player.GetComponent<InventorySystem>().itemHolder;
            //Get preferences to the Item
            GameObject dropped = eventData.pointerDrag;
            Slot slot = dropped.GetComponent<Slot>();
            //Get preferences to the player

            //Instantiate Item
            GameObject droppedItem =
                Instantiate(slot.itemInSlot.prefab,
                    itemdrop.transform.position, Quaternion.identity);
            droppedItem.GetComponent<ItemObject>().amount = slot.amountInSlot;
            slot.itemInSlot = null;
            slot.amountInSlot = 0;
            
            //update hot bar
            player.GetComponent<InventorySystem>().HotBarUpdate();
        }
        catch
        {
            //ignore
        }
            
        
        
    }
}