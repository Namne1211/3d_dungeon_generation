using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ClickedItem : MonoBehaviour
{
    public Slot clickedSlot;
    
    
    [SerializeField] TextMeshProUGUI txt_Name;
    [SerializeField] TextMeshProUGUI txt_Weight;
    [SerializeField] TextMeshProUGUI txt_Value;
    [SerializeField] TextMeshProUGUI txt_Stack;

    [SerializeField] TextMeshProUGUI txt_Description;

    private void OnEnable()
    {
        SetUp();
    }

    void SetUp()
    {   
        //Update Text
        txt_Name.text = clickedSlot.itemInSlot.name;
        txt_Description.text = clickedSlot.itemInSlot.description;
    }
    
}