using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("HP・索敵範囲")]
    [SerializeField] private int _hp = 10;
    [SerializeField] private float _reactionDistance = 7.0f;

    [Header("弾丸のプレハブ・速度・発射口")]
    [SerializeField] private GameObject _bulettPrefab;
    [SerializeField] private float _shootSpeed = 5.0f;
    [SerializeField] private Transform _gateTransform;

    [Header("対象ボディ")]
    [SerializeField] private GameObject _bossBody;
    private Animator _animator;

    private GameObject _player;
    private bool _inDamage = false;              // ダメージ中フラグ

    void Start()
    {
        _animator = _bossBody.GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // ゲーム中以外とダメージ中は何もしない
        if (GameManager.gameState != GameState.InGame || _inDamage)
        {
            if (_inDamage)
            {
                // ダメージ中点滅させる
                float val = Mathf.Sin(Time.time * 50);
                if (val > 0)
                {
                    _bossBody.GetComponent<SpriteRenderer>().enabled = true;// スプライト表示
                }
                else
                {
                    _bossBody.GetComponent<SpriteRenderer>().enabled = false;// スプライト非表示
                }
            }
            return;
        }

        if (_hp > 0)
        {
            if(_player != null)
            {
                float dist = Vector2.Distance(transform.position, _player.transform.position);
                _animator.SetBool("InAttack", dist <= _reactionDistance);
            }
        }
        else
        {
            _animator.SetBool("InAttack", false);
        }        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Arrow")
        {
            GetDamage(collision.gameObject, "Arrow");
        }
        if (collision.gameObject.tag == "Sword")
        {
            GetDamage(collision.gameObject, "Sword");
        }
    }

    // ダメージ処理
    void GetDamage(GameObject target, string attackType)
    {
        if (_inDamage) return; //すでにダメージ中なら重ねて処理はしない

        if (attackType == "Arrow")
        {
            ArrowController arrow = target.gameObject.GetComponent<ArrowController>();
            _hp -= arrow.attackPower;
        }
        else if (attackType == "Sword")
        {
            _hp -= 1;
        }

        if (_hp > 0) //体力が残ればダメージ演出
        {
            _inDamage = true;            // ダメージフラグ ON
            Invoke("DamageEnd", 0.25f);
        }
        else //死亡時
        {
            SoundManager.soundManager.SEPlay(SEType.EnemyKilled);//SEを鳴らす

            SoundManager.soundManager.StopBgm();//BGM停止
            SoundManager.soundManager.SEPlay(SEType.GameClear);//ゲームクリアSEを鳴らす

            GetComponent<CircleCollider2D>().enabled = false;
            _animator.SetTrigger("IsDead");
            Destroy(gameObject, 1.0f);
            GameManager.gameState = GameState.GameClear;
        }
    }

    // ダメージ終了
    void DamageEnd()
    {
        _inDamage = false; // ダメージフラグ OFF
        _bossBody.GetComponent<SpriteRenderer>().enabled = true; // スプライトを元に戻す
    }

    public void Attack()
    {
        if(_player != null)
        {
            float dx = _player.transform.position.x - _gateTransform.position.x;
            float dy = _player.transform.position.y - _gateTransform.position.y;
            float rad = Mathf.Atan2(dy, dx);
            float angle = rad * Mathf.Rad2Deg;
            Quaternion r = Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(_bulettPrefab, _gateTransform.position, r);
            float x = Mathf.Cos(rad);
            float y = Mathf.Sin(rad);
            Vector3 v = new Vector3(x, y) * _shootSpeed;
            Rigidbody2D rbody = bullet.GetComponent<Rigidbody2D>();
            rbody.AddForce(v, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _reactionDistance);
    }
}
