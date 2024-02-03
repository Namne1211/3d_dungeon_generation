using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Slot : MonoBehaviour, IDropHandler
{
    
    //Variables
    public Items itemInSlot;
    public int amountInSlot;
    
    [SerializeField] private RawImage icon;
    [SerializeField] private TextMeshProUGUI txtAmount;
    private GameObject _player;
    // ReSharper disable Unity.PerformanceAnalysis
    //Set the slot info
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetStats()
    {
        //Turn on
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        
        //Icons and Text
        icon = GetComponentInChildren<RawImage>();
        txtAmount = GetComponentInChildren<TextMeshProUGUI>();

        if(itemInSlot == null)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            return;
        }
            
        //Set Texture and Amount
        icon.texture = itemInSlot.icon;
        txtAmount.text = $"{amountInSlot}x";
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        try
        {
            GameObject dropped = eventData.pointerDrag;
            Slot slot = dropped.GetComponent<Slot>();

            if (slot == this)
                return;
            Items previousItemInSlot =itemInSlot;
            int previousAmountInSlot=amountInSlot;

            
            itemInSlot = slot.itemInSlot;
            amountInSlot = slot.amountInSlot;

            if (previousItemInSlot != null)
            {
                slot.itemInSlot = previousItemInSlot;
            }
            else
            {
                slot.itemInSlot = null;
            }
        
            slot.amountInSlot = previousAmountInSlot;
            
    
            SetStats();
            
            //update hot bar
            _player.GetComponent<InventorySystem>().HotBarUpdate();
        }
        catch
        {
            // ignored
        }
    }
}