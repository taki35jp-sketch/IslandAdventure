using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private string _sceneName;

    void Start()
    {
        // セーブデータがなければContinueボタンを押せなくする
        _continueButton.GetComponent<Button>().interactable = SaveDataManager.HasSaveData();


        //タイトルBGM
        SoundManager.soundManager.PlayBgm(BGMType.Title);
    }

    public void StartButtonClicked()
    {
        //SaveDataManager.ResetAll();     // すべてリセット
        SceneManager.LoadScene(_sceneName);
    }

    public void ContinueButtonClicked()
    {
        SaveDataManager.LoadAndRestart();   // 最後にセーブした状態でセーブポイントから再開
    }
}
