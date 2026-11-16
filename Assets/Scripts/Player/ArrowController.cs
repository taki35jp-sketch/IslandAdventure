using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float deleteTime = 2; // 削除時間
    public int attackPower = 1;  // 攻撃力
    void Start()
    {
        Destroy(gameObject, deleteTime);    // 一定時間で消す
    }

    void Update()
    {
        
    }

    // ゲームオブジェクトに接触
    void OnCollisionEnter2D(Collision2D collision)
    {
        transform.SetParent(collision.transform);           // 接触したゲームオブジェクトの子にする
        GetComponent<CircleCollider2D>().enabled = false;   // 当たりを無効化する
        GetComponent<Rigidbody2D>().simulated = false;      // 物理シミュレーションを無効化する
    }
}
