using UnityEngine;
using UnityEngine.InputSystem;

public class SavePoint : MonoBehaviour
{
    private bool _isPlayerOn = false;   // プレイヤーが触れているか
    private InputAction _submitAction;  // PlayerマップのSaveアクション

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _isPlayerOn = true;

            // UIマップは無効化されているので、Save専用アクションを変数_submitActionで参照できるようにする
            PlayerInput input = collision.gameObject.GetComponent<PlayerInput>();
            _submitAction = input.actions.FindAction("Player/Save");
            _submitAction.Enable();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _isPlayerOn = false;
        }
    }

    void Update()
    {
        if (GameManager.gameState != GameState.InGame) return;

        // 触れている時にSaveアクションボタンが押されたらセーブ
        if (_isPlayerOn && _submitAction.WasPressedThisFrame())
        {
            SaveDataManager.Save(transform.position);
        }
    }
}
