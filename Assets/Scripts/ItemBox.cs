using System;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private Sprite _openImage;
    [SerializeField] private GameObject _itemprefab;
    private bool _isClosed = true; //trueの間は閉じる、開くとfalse
    private string _id;     // セーブ用のID

    void Start()
    {
        // すでに開けた箱なら開いた状態にする
        _id = SaveDataManager.MakeId(gameObject);
        if (SaveDataManager.IsUsed(_id))
        {
            GetComponent<SpriteRenderer>().sprite = _openImage;
            _isClosed = false;
            SpawnItem();    // 中身をまだ取っていなければもう一度出す
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isClosed && collision.gameObject.tag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = _openImage;
            _isClosed = false;
            SaveDataManager.SetUsed(_id);   // 開けたことを記録
            SpawnItem();
        }
    }

    // 中身のアイテムを出す
    void SpawnItem()
    {
        if(_itemprefab == null) return; //どのアイテムなのか指定されていなければ何もしない

        string itemId = _id + "_Item";  // 箱から出るアイテムは箱のIDをもとにする
        if (SaveDataManager.IsUsed(itemId)) return;     // すでに取っていれば出さない

        GameObject itemObj = Instantiate(_itemprefab, transform.position, Quaternion.identity);
        itemObj.GetComponent<ItemData>().id = itemId;
    }
}
