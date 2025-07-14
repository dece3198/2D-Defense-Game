using UnityEngine;

public enum ItemType
{
    None, Weapon, Armor, Necklace, Ring
}

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Item")]
public class Item : ScriptableObject
{
    public Sprite itemImage;
    public float atk;
    public float skillP;
    public float skillD;
    public Rating itemRating;
    public ItemType itemType;
}
