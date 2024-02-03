using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class ItemInteraction : MonoBehaviour
{
    Transform _cam;
    [SerializeField] LayerMask itemLayer;
    [SerializeField] InventorySystem inventorySystem;
    
    //Text on hover
    [SerializeField] TextMeshProUGUI textHoveredItem;
    void Start()
    {
        if (Camera.main != null) _cam = Camera.main.transform;
        inventorySystem = GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        //Interact on Raycast 
        //filter out other layer
        if(Physics.Raycast(_cam.position,_cam.forward, out hit,2, itemLayer))
        {
            //Return null if no detected
            if (!hit.collider.GetComponent<ItemObject>())
                return;


            //Text show
            textHoveredItem.text = $"Press 'F' to pick up {hit.collider.GetComponent<ItemObject>().amount}x {hit.collider.GetComponent<ItemObject>().itemStats.name}";
            
            //Pick up item
            if (Input.GetKeyDown(KeyCode.F))
            {
                inventorySystem.PickUpItem(hit.collider.GetComponent<ItemObject>());
                inventorySystem.HotBarUpdate();
            }
            
        }
        else
        {
            //Made Text Plank
            textHoveredItem.text = string.Empty;
        }
    }
}