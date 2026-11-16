using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("矢の速度・発射間隔")]
    [SerializeField] private float _shootSpeed = 12.0f;    // 矢の速度
    [SerializeField] private float _shootDelay = 0.25f;    // 発射間隔

    [Header("矢・弓のプレハブ")]
    [SerializeField] private GameObject _arrowPrefab;      // 矢のプレハブ
    [SerializeField] private GameObject _bowPrefab;        // 弓のプレハブ

    [Header("剣・斬撃のプレハブ")]
    [SerializeField] private GameObject _swordPrefab;        // 剣のプレハブ
    [SerializeField] private GameObject _swordEffectPrefab;        // 斬撃のプレハブ

    //発動したコルーチン情報が入る変数
    private Coroutine _swordAttackCoroutine; 
    private Coroutine _arrowAttackCoroutine; 

    private GameObject _bowObj;         // 弓のゲームオブジェクト
    private GameObject _swordObj;         // 剣のゲームオブジェクト

    //InputSystem上の特定のActionが入る変数
    private InputAction _bowAttackAction;           // 弓Attackアクション
    private InputAction _swordAttackAction;           // 剣Attackアクション

    private float _pressTime;                    // 長押し時間記録

    private PlayerMove _playerMove;　//PlayerMoveコンポーネントを参照する変数

    private UIController _uiCnt; //UIControllerコンポーネントを参照する変数

    // 弓Attackアクションのコールバック
    void OnAttackCallback(InputAction.CallbackContext context)
    {
        // 矢を持っている
        if (ItemKeeper.hasArrows > 0)
        {
            if(_swordAttackCoroutine == null)
            ShootArrow();       //矢を発射
        }
    }

    // 剣Attackアクションのコールバック
    void OnLongPressStarted(InputAction.CallbackContext context)
    {
        Slash(); //剣による攻撃
    }

    //矢を発射
    void ShootArrow()
    {
        if (_arrowAttackCoroutine == null)
        {
            SoundManager.soundManager.SEPlay(SEType.Shoot);//SEを鳴らす

            float currentAngleZ = _playerMove.GetAngle(); //現在のPlayer角度を取得

            _bowObj.transform.localScale = new Vector3(1, 1, 1); //弓の変形を戻す
            ItemKeeper.hasArrows -= 1; // 矢を減らす
                            // 矢のゲームオブジェクトを作る(進行方向に回転)
            Quaternion r = Quaternion.Euler(0, 0, currentAngleZ);
            GameObject arrowObj = Instantiate(_arrowPrefab, transform.position, r);
            if (_pressTime >= 1)
            {
                ArrowController arrow = arrowObj.GetComponent<ArrowController>();
                arrow.attackPower *= 2;     //攻撃力２倍
            }
            // 矢を発射するベクトルを作る
            float x = Mathf.Cos(currentAngleZ * Mathf.Deg2Rad);
            float y = Mathf.Sin(currentAngleZ * Mathf.Deg2Rad);
            Vector3 v = new Vector3(x, y) * _shootSpeed;
            // 矢に力を加える
            Rigidbody2D body = arrowObj.GetComponent<Rigidbody2D>();
            body.AddForce(v, ForceMode2D.Impulse);


            _uiCnt.UpdateItemCount(ItemType.arrow, ItemKeeper.hasArrows);    	                // 矢を更新
            _uiCnt.UpdatePower(0);                                                  // パワーを更新

            //SoundManager.soundManager.SEPlay(SEType.Shoot);     // SE再生（矢を射る）

            _arrowAttackCoroutine = StartCoroutine(ArrowAttack()); //コルーチンを発動してその情報を変数に入れる
        }
    }

    //0.2秒待ってから、自分の発動情報を入れているかもしれない変数の中身をnullに解放する
    IEnumerator ArrowAttack()
    {
        yield return new WaitForSeconds(0.2f); //0.2秒　ここで待つ
        _arrowAttackCoroutine = null;
    }

    //剣を振る
    void Slash()
    {
        //ソードも、矢もコルーチンがまだ発動してなければ実行(null)
        if(_swordAttackCoroutine == null && _arrowAttackCoroutine == null)
        {
            SoundManager.soundManager.SEPlay(SEType.Slash);//SEを鳴らす

            _bowObj.SetActive(false); //弓の存在を一時消しておく
            float currentAngleZ = _playerMove.GetAngle(); //現在のPlayer角度を取得
            Quaternion r = Quaternion.Euler(0, 0, currentAngleZ + 90); //角度をラジアン値に変換
            //しかるべき角度でソードエフェクトを生成
            GameObject swordEffectObj = Instantiate(_swordEffectPrefab, transform.position, r);
            swordEffectObj.transform.SetParent(transform);　//Playerをエフェクトの親とする

            //Playerの角度情報（すなわち向き）に応じて生成したエフェクトの位置を調整
            if (currentAngleZ >= -135 && currentAngleZ <= -45)
            {
                swordEffectObj.transform.localPosition = new Vector3(0,0.3f,0);
            }
            else if (currentAngleZ >= 45 && currentAngleZ <= 135)
            {
                swordEffectObj.transform.localPosition = new Vector3(0, 0.2f, 0);
            }

            //ソードを維持→やがて消滅　を時限式に行うコルーチンを発動
            _swordAttackCoroutine = StartCoroutine(SwordAttack(swordEffectObj));
        }
    }

    //コルーチンSwordAttackの作成（ソードを維持→やがて消滅）
    IEnumerator SwordAttack(GameObject swordEffectObj)
    {
        yield return new WaitForSeconds(0.3f); //0.3秒経つまで待つ
        Destroy(swordEffectObj); //ソードエフェクトを消す
        _swordAttackCoroutine = null; //コルーチン情報をnullにする
        _bowObj.SetActive(true);  //一時的に非表示にしていた矢を復活
    }

    //このスクリプトが無くなることがあれば ※Playerオブジェごといなくなる、など
    //誤作動防止のためにコールバックを解放しておく
    void OnDisable()
    {
        if (_bowAttackAction != null) //変数にアクションが指定されていれば
        {
            _bowAttackAction.canceled -= OnAttackCallback;    // キャンセルコールバック削除
        }
        if (_swordAttackAction != null) //変数にアクションが指定されていれば
        {
            _swordAttackAction.started -= OnLongPressStarted;  // キャンセルコールバック削除
        }
    }

    void Start()
    {
        _playerMove = GetComponent<PlayerMove>(); //変数playerMoveがPlayerのPlayerMoveコンポーネントを参照

        // 攻撃アイテムをプレイヤーキャラクターに配置
        Vector3 pos = transform.position;
        _bowObj = Instantiate(_bowPrefab, pos, Quaternion.identity);
        _bowObj.transform.SetParent(transform); // 弓の親にプレイヤーキャラクターを設定する

        _swordObj = Instantiate(_swordPrefab, pos, Quaternion.identity);
        _swordObj.transform.SetParent(transform); // 剣の親にプレイヤーキャラクターを設定する

        //Attackアクションにコールバックを設定
        PlayerInput input = GetComponent<PlayerInput>();
        _bowAttackAction = input.currentActionMap.FindAction("Attack");
        _bowAttackAction.canceled += OnAttackCallback;                  // キャンセルコールバック登録

        _swordAttackAction = input.currentActionMap.FindAction("Jump");
        _swordAttackAction.started += OnLongPressStarted;                 // 開始コールバック登録

        InputActionMap uiMap = input.actions.FindActionMap("UI");   // UIマップ取得
        uiMap.Disable();                                            // UIマップ無効化

        _uiCnt = GameObject.FindFirstObjectByType<UIController>();   // シーン中のUIController取得
    }

    void Update()
    {
        float currentAngleZ = _playerMove.GetAngle();　//現在のPlayer角度を取得

        DrawAttackItemPosition(currentAngleZ);

        //矢を持っている
        if (ItemKeeper.hasArrows > 0)
        {
            //長押し時間
            _pressTime = _bowAttackAction.GetTimeoutCompletionPercentage();
            if (_pressTime > 0)
            {
                //長押し時間に合わせて弓を変形させる
                _bowObj.transform.localScale = new Vector3(1 - (_pressTime * 0.2f), 1 + (_pressTime * 0.5f), 1);

                _uiCnt.UpdatePower(_pressTime);	  // パワーを更新
            }
        }
    }

    //弓と剣の位置（回転）と、order in layerの前後を決めるメソッド
    void DrawAttackItemPosition(float currentAngleZ)
    {
        int orderNumber = 4; // 描画アイテムの オーダーインレイヤー(キャラクターの前にする)

        // 弓描画の優先順位
        if (currentAngleZ > 30 && currentAngleZ < 150)
        {
            // 上向き
            orderNumber = 2; // 描画アイテムの オーダーインレイヤー(キャラクターの後ろにする)
        }
        // 弓の回転
        _bowObj.transform.rotation = Quaternion.Euler(0, 0, currentAngleZ);
        // 弓の配置
        _bowObj.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        // 描画の優先順位
        _bowObj.GetComponent<SpriteRenderer>().sortingOrder = orderNumber;

        orderNumber = 4; //リセット

        // 剣描画の優先順位
        if (currentAngleZ >= -45 && currentAngleZ < 45) // 右向き
        {
            _swordObj.transform.localPosition = new Vector3(0.2f, 0.53f, 0);
            orderNumber = 2; // 描画アイテムの オーダーインレイヤー(キャラクターの後ろにする)
        }
        else if (currentAngleZ >= 45 && currentAngleZ <= 135)  // 上向き
        {
            _swordObj.transform.localPosition = new Vector3(-0.3f, 0.53f, 0);
            orderNumber = 2; // 描画アイテムの オーダーインレイヤー(キャラクターの後ろにする)
        }
        else if (currentAngleZ >= -135 && currentAngleZ <= -45) // 下向き
        {
            _swordObj.transform.localPosition = new Vector3(0.25f, 0.25f, 0);
        }
        else  // 左向き
        {
            _swordObj.transform.localPosition = new Vector3(-0.05f, 0.18f, 0);
        }
        _swordObj.GetComponent<Sword>().GetSwordObject().GetComponent<SpriteRenderer>().sortingOrder = orderNumber;

    }
}
