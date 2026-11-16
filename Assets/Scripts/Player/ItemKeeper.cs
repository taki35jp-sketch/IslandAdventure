using UnityEngine;

public class ItemKeeper : MonoBehaviour
{
    public static float hasKeys = 0.0f;
    public static float hasArrows = 0.0f;

    private UIController _uiCnt;

    void Start()
    {
        _uiCnt = GameObject.FindFirstObjectByType<UIController>();   // シーン中のUIController取得
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            ItemData item = collision.gameObject.GetComponent<ItemData>();
            SaveDataManager.SetUsed(item.id);   // 取ったことを記録

            if (item.itemType == ItemType.arrow)
            {
                hasArrows += item.value;
                _uiCnt.UpdateItemCount(ItemType.arrow, hasArrows);
            }
            else if (item.itemType == ItemType.key)
            {
                hasKeys += item.value;
                _uiCnt.UpdateItemCount(ItemType.key, hasKeys);
            }
            else if (item.itemType == ItemType.life)
            {
                if (PlayerMove.life < 1.0f)
                    PlayerMove.life += item.value;
                if (PlayerMove.life >= 1.0f) PlayerMove.life = 1.0f; //上限はあくまで1.0f
                _uiCnt.UpdateLife(PlayerMove.life);
            }

            //取得演出
            collision.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            Rigidbody2D rbody = collision.gameObject.GetComponent<Rigidbody2D>();
            rbody.gravityScale = 2.5f;
            rbody.AddForce(new Vector2(0, 6), ForceMode2D.Impulse);
            Destroy(collision.gameObject, 0.5f);

            SoundManager.soundManager.SEPlay(SEType.ItemGet);//SEを鳴らす

        }
    }
}
