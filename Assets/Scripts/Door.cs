using UnityEngine;

public class Door : MonoBehaviour
{
    private UIController _uiCnt; //UIControllerスクリプトを参照できる変数
    private string _id; //セーブ用ID

    void Start()
    {
        //UIControllerスクリプトを探してくる
        _uiCnt = GameObject.FindFirstObjectByType<UIController>();
        _id = SaveDataManager.MakeId(gameObject);
        if (SaveDataManager.IsUsed(_id))
        {
            Destroy(gameObject);
        }
    }

    //接触した時
    //①相手がPlayerタグをもつものであれば
    //②鍵が1以上所持されていれば
    //・ドアは消滅 ・UIControllerの鍵テキストの更新
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(ItemKeeper.hasKeys > 0)
            {
                SoundManager.soundManager.SEPlay(SEType.DoorOpen);//SEを鳴らす

                ItemKeeper.hasKeys--; //1減らす
                Destroy(gameObject); //ドア消滅
                _uiCnt.UpdateItemCount(ItemType.key, ItemKeeper.hasKeys); //UI更新
                SaveDataManager.SetUsed(_id); //開錠したのでリストに刻まれる
            }
            else
            {
                SoundManager.soundManager.SEPlay(SEType.DoorClosed);//SEを鳴らす
            }
        }
    }    　
}
