using UnityEngine;

public enum ItemType
{
    none,
    key,
    arrow,
    life
}

public class ItemData : MonoBehaviour
{
    public ItemType itemType = ItemType.none;
    public float value = 0.0f;

    [HideInInspector] public string id = "";    // セーブ用のID

    void Start()
    {
        // ItemBoxから出たアイテムはIDが設定済み。シーンに置いたアイテムはここで作る
        if (id == "")
        {
            id = SaveDataManager.MakeId(gameObject);
        }

        //// すでに取ったアイテムなら消す
        if (SaveDataManager.IsUsed(id))
        {
            Destroy(gameObject);
        }
    }
}
