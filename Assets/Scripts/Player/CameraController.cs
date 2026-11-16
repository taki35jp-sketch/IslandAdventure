using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("追跡対象")]
    public GameObject otherTarget;      // ターゲット

    void Update()
    {
        //ゲーム中でなければ何もしない
        if (GameManager.gameState != GameState.InGame) return;

        if (otherTarget != null)　//対象がいる場合
        {
            //プレイヤーと対象の中間でカメラ位置を固定
            Vector2 pos = Vector2.Lerp(transform.position, otherTarget.transform.position, 0.5f);
            Camera.main.transform.position = new Vector3(pos.x, pos.y, -10);
        }
        else
        {
            // カメラをプレイヤーの位置と連動させる
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }
}
