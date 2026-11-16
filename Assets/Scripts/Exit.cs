using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [Header("移動先となるシーン名")]
    [SerializeField] private string _sceneName;

    [Header("出入り口の識別番号・方向")]
    public int doorNumber;
    public Direction direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //static変数に自分の識別番号を授ける
            RoomManager.currentDoorNumber = doorNumber;
            Invoke("Load", 0.2f); //0.2秒後に目的に切替
        }
    }

    //シーンを切り替えるメソッド
    private void Load()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
