using UnityEngine;

// ゲームの状態
public enum GameState
{
    InGame,         // ゲーム中
    GameClear,      // ゲームクリア
    GameOver,       // ゲームオーバー
    GameEnd,        // ゲーム終了
}

public class GameManager : MonoBehaviour
{
    public static GameState gameState = GameState.InGame;

    void Start()
    {
        //初期状態はゲーム中からはじまる
        gameState = GameState.InGame;
    }
}
