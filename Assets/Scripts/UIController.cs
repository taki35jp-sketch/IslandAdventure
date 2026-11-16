using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UIテキスト")]
    [SerializeField] private TextMeshProUGUI _arrowText;
    [SerializeField] private TextMeshProUGUI _keyText;

    [Header("UIスライダー")]
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private Slider _powerSlider;

    [Header("ステータスオブジェクト")]
    [SerializeField] private GameObject _gameStatePanel;
    [SerializeField] private GameObject _stateImage;
    [SerializeField] private GameObject _buttonPanel;

    [Header("Buttonコンポーネント")]
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _quitButton;

    [Header("ゲームクリア時のステータス画像")]
    [SerializeField] private Sprite _gameClearImage;

    public void UpdateItemCount(ItemType itemType, float count)
    {
        if (itemType == ItemType.arrow) _arrowText.text = Mathf.Floor(count).ToString();
        if (itemType == ItemType.key) _keyText.text = Mathf.Floor(count).ToString();
    }

    public void UpdateLife(float life)
    {
        if(life <= 0)
        {
            _gameStatePanel.SetActive(true);
        }
        _lifeSlider.value = life;
    }

    public void UpdatePower(float power)
    {
        _powerSlider.value = power;
    }

    public void Retry()
    {
        SoundManager.playingBGM = BGMType.None;

        // セーブデータがあれば最後にセーブした状態でセーブポイントから再開
        if (SaveDataManager.HasSaveData())
        {
            SaveDataManager.LoadAndRestart();
        }
        else
        {
            PlayerMove.life = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene("Title");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);　//Title作るまでの仮
    }

    void Start()
    {
        _gameStatePanel.SetActive(false);

        UpdateLife(PlayerMove.life);
        UpdateItemCount(ItemType.arrow, ItemKeeper.hasArrows);
        UpdateItemCount(ItemType.key, ItemKeeper.hasKeys);
    }

    void Update()
    {
        if(GameManager.gameState == GameState.GameClear)
        {
            _stateImage.SetActive(true);
            _stateImage.GetComponent<Image>().sprite = _gameClearImage;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

                // Playerマップの無効化とUIマップの有効化
                PlayerInput input = player.GetComponent<PlayerInput>();        // PlayerInput取得
                input.currentActionMap.Disable();                       // Player無効化
                input.SwitchCurrentActionMap("UI");                     // アクションマップをUIに変更
                input.currentActionMap.Enable();                        // UIマップ有効化
            }

            GameManager.gameState = GameState.GameEnd;
            Invoke("ToTitle", 3.0f);
        }
    }

    void ToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
