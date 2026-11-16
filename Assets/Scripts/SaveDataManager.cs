using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// セーブデータの管理（シーンに置かずに使えるstaticクラス）
public static class SaveDataManager
{
    // 「開けたDoor・開けたItemBox・取ったItem」のID一覧
    // staticなのでシーンを移動しても中身が残る
    private static List<string> _usedIds = new List<string>();

    // trueのとき、次のシーン開始時にプレイヤーをセーブポイントに配置する
    public static bool isStartFromSavePoint = false;

    // PlayerPrefsのキー名
    private const string KEY_SAVE_DATA = "SaveData";

    // オブジェクトごとのIDを作る（シーン名＋タグ＋配置座標）
    public static string MakeId(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        int x = Mathf.RoundToInt(pos.x * 10);
        int y = Mathf.RoundToInt(pos.y * 10);
        return SceneManager.GetActiveScene().name + "_" + obj.tag + "_" + x + "_" + y;
    }

    // すでに開けた・取ったオブジェクトか？
    public static bool IsUsed(string id)
    {
        return _usedIds.Contains(id);
    }

    // 開けた・取ったことを記録する
    public static void SetUsed(string id)
    {
        if (!_usedIds.Contains(id))     // 同じIDを2回入れない
        {
            _usedIds.Add(id);
        }
    }

    // セーブデータがあるか？
    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(KEY_SAVE_DATA);
    }

    // 現在の状態をJSONにしてPlayerPrefsにセーブする
    public static void Save(Vector3 savePointPos)
    {
        SaveData data = new SaveData();
        data.usedIds = new List<string>(_usedIds);  // コピーして入れる
        data.sceneName = SceneManager.GetActiveScene().name;
        data.savePointPosition = savePointPos;
        data.keys = ItemKeeper.hasKeys;
        data.arrows = ItemKeeper.hasArrows;

        string json = JsonUtility.ToJson(data);     // SaveData → JSON文字列
        PlayerPrefs.SetString(KEY_SAVE_DATA, json);
        PlayerPrefs.Save();
        Debug.Log("セーブしました：" + json);
    }

    // PlayerPrefsからJSONを読み込んでSaveDataに戻す
    static SaveData LoadData()
    {
        string json = PlayerPrefs.GetString(KEY_SAVE_DATA);
        return JsonUtility.FromJson<SaveData>(json);    // JSON文字列 → SaveData
    }

    // セーブした状態に戻して、セーブポイントから再開する
    public static void LoadAndRestart()
    {
        SaveData data = LoadData();

        _usedIds = new List<string>(data.usedIds);
        ItemKeeper.hasKeys = data.keys;
        ItemKeeper.hasArrows = data.arrows;
        PlayerMove.life = 1.0f;     // ライフは全回復で再開

        isStartFromSavePoint = true;
        SceneManager.LoadScene(data.sceneName);
    }

    // セーブポイントの位置
    public static Vector3 GetSavePointPosition()
    {
        return LoadData().savePointPosition;
    }

    // すべてを最初の状態に戻す（セーブデータも消す）
    public static void ResetAll()
    {
        _usedIds.Clear();
        ItemKeeper.hasKeys = 0.0f;      // ItemKeeperの初期値と合わせる
        ItemKeeper.hasArrows = 1.0f;
        PlayerMove.life = 1.0f;
        RoomManager.currentDoorNumber = 0;
        isStartFromSavePoint = false;

        PlayerPrefs.DeleteKey(KEY_SAVE_DATA);
        PlayerPrefs.Save();
    }
}
