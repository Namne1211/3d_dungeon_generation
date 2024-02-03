using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item")]
[System.Serializable]
public class Items : ScriptableObject
{
    #region test
    //private static int current_id;
    //public Items()
    //{
    //    id = current_id;
    //    current_id++;
    //}
    //[Header("Don't touch id!!!")]
    #endregion
    public int id;

    public string itemName;

    [TextArea(3,3)] public string description;

    //For prefabs and icons
    public GameObject prefab;
    public Texture icon;
    
    //basic propertys
    public int maxStack;
}