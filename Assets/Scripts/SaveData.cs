using System.Collections.Generic;
using UnityEngine;

// セーブする内容をまとめたクラス（JsonUtilityでJSONに変換する）
[System.Serializable]
public class SaveData
{
    public List<string> usedIds = new List<string>();   // 開けたDoor・開けたItemBox・取ったItemのID
    public string sceneName = "";                       // セーブしたシーン名
    public Vector3 savePointPosition;                   // セーブポイントの位置
    public float keys = 0.0f;                           // カギの数
    public float arrows = 0.0f;                         // 矢の数
}
