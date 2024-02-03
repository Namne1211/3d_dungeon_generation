using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    //Variables
    [SerializeField] private Slot[] slots = new Slot[40];
    [SerializeField] private Slot[] hbSlots = new Slot[4];
    [SerializeField] private GameObject clickedWindow;
    
    [SerializeField] private GameObject InventoryUI;
    [SerializeField] private GameObject HotBarUI;
    
    public GameObject itemHolder;
    private int currentHBblock;
    private GameObject currentObj;
    private PlayerController _controller;
    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
       //If slot have nothing turn off childs
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemInSlot == null)
            {
                for (int k = 0; k < slots[i].transform.childCount; k++)
                {
                    slots[i].transform.GetChild(k).gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void Update()
    {   
        //Turn on Inventory
        if(!InventoryUI.activeInHierarchy && Input.GetKeyDown(KeyCode.Tab))
        {
            HotBarUI.SetActive(false);
            _controller.inventoryOn = true;
            InventoryUI.SetActive(true);
        }
        //Turn off Inventory
        else if(InventoryUI.activeInHierarchy && Input.GetKeyDown(KeyCode.Tab)|| Input.GetKeyDown(KeyCode.Escape))
        {
            HotBarUI.SetActive(true);
            _controller.inventoryOn = false;
            InventoryUI.SetActive(false);
            clickedWindow.SetActive(false);
        }

        HotBarhandle();
    }

    
    public void PickUpItem(ItemObject obj)
    {
        bool alreadyExis=false;
        int place = 0;
        // Loop all slot
        for (int i = 0; i < slots.Length; i++)
        {
            //check slot
            if (slots[i].itemInSlot != null && slots[i].itemInSlot.id == obj.itemStats.id && slots[i].amountInSlot != slots[i].itemInSlot.maxStack)
            {
                alreadyExis = true;
                place = i;
                break;
            }

        }

        if (alreadyExis)
        {
            //If not hit max stack 
            if(!WillHitMaxStack(place, obj.amount))
            {
                slots[place].amountInSlot += obj.amount;
                Destroy(obj.gameObject);
                slots[place].SetStats();
            }
            else //Create new slot and fill max stack
            {
                int result = NeededToFill(place);
                obj.amount = RemainingAmount(place, obj.amount);
                slots[place].amountInSlot +=result;
                slots[place].SetStats();
                //create new slot if object amount>0
                if(obj.amount!=0) PickUpItem(obj);
                Destroy(obj.gameObject);
            }
        }
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].itemInSlot == null) //If there is no item yet
                {
                    slots[i].itemInSlot = obj.itemStats;
                    slots[i].amountInSlot = obj.amount;
                    Destroy(obj.gameObject);
                    slots[i].SetStats();
                    break;
                }

            }
        }
    }

    public void HotBarUpdate()
    {
        for (int i = 0; i < hbSlots.Length; i++)
        {
            hbSlots[i].itemInSlot = slots[i].itemInSlot;
            hbSlots[i].amountInSlot = slots[i].amountInSlot;
            hbSlots[i].SetStats();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void HotBarhandle()
    {
        // Get the scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        // Check if the scroll wheel has been scrolled up
        if (scrollInput > 0f)
        {
            currentHBblock++;
            if (currentHBblock > hbSlots.Length-1)
            {
                currentHBblock = 0;
            }
        }
        // Check if the scroll wheel has been scrolled down
        else if (scrollInput < 0f)
        {
            currentHBblock--;
            if (currentHBblock < 0)
            {
                currentHBblock = hbSlots.Length-1;
            }
        }
        
        //change hotbar size
        for (int i = 0; i < hbSlots.Length; i++)
        {
            if (i == currentHBblock)
            {
                hbSlots[i].transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                hbSlots[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
        
        //update item holding 
        if (hbSlots[currentHBblock].itemInSlot != null)
        {
            if (currentObj == null)
            {
                GameObject holdingObj = Instantiate(hbSlots[currentHBblock].itemInSlot.prefab, itemHolder.transform.position,
                    Quaternion.identity,itemHolder.transform);
                currentObj = holdingObj;
            }
            else
            {
                Destroy(currentObj);
                GameObject holdingObj = Instantiate(hbSlots[currentHBblock].itemInSlot.prefab, itemHolder.transform.position,
                    Quaternion.identity,itemHolder.transform);
                currentObj = holdingObj;
            }
        }else{
            if (currentObj != null)
            {
                Destroy(currentObj);
            }
        }
        
        //drop item
        if (Input.GetKeyUp(KeyCode.G) && hbSlots[currentHBblock].itemInSlot != null)
        {
            Destroy(currentObj);
            //Instantiate Item
            GameObject droppedItem = Instantiate(hbSlots[currentHBblock].itemInSlot.prefab,
                    itemHolder.transform.position, Quaternion.identity);
            droppedItem.GetComponent<ItemObject>().amount = hbSlots[currentHBblock].amountInSlot;
            
            //update hotbar
            hbSlots[currentHBblock].itemInSlot = null;
            hbSlots[currentHBblock].amountInSlot = 0;
            hbSlots[currentHBblock].SetStats(); 

            //update inventory
            slots[currentHBblock].itemInSlot = hbSlots[currentHBblock].itemInSlot;
            slots[currentHBblock].amountInSlot =hbSlots[currentHBblock].amountInSlot;
            slots[currentHBblock].SetStats();
            
        }
    }
    //MaxStack
    bool WillHitMaxStack(int index, int amount)
    {
        return (slots[index].itemInSlot.maxStack <= slots[index].amountInSlot + amount);
    }
    
    //MaxStack-InSlot
    int NeededToFill(int index)
    {
        return slots[index].itemInSlot.maxStack - slots[index].amountInSlot;
    }
    //(InSlot+Amount) - MaxStack
   int RemainingAmount(int index, int amount)
    {
        return  (slots[index].amountInSlot + amount)-slots[index].itemInSlot.maxStack;
    }
}