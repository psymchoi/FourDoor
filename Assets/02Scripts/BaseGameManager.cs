using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseGameManager : MonoBehaviour
{
    public static BaseGameManager _uniqueInstance;

    public enum eLoadingState
    {
        NONE,
        START,
        LOADING,
        UNLOADING,
        END,
    }

    public enum eStageState
    {
        NONE = 0,
        LOBBY,
        INGAME,
    }

    eLoadingState _curGameLoad;
    eStageState _curStage;

    public eLoadingState CURGAMESTATE
    {
        get { return _curGameLoad; }
        set { _curGameLoad = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;

        SceneManager.LoadSceneAsync("LobbyScene", LoadSceneMode.Additive);
        // SoundManager._uniqueInstance.PlayBGMSound(SoundManager.eBGMType.LOBBY);
        _curStage = eStageState.LOBBY;
    }

    public IEnumerator LoadingScene(string[] loadName = null, string[] unloadName = null)
    {
        AsyncOperation AO;

        int amount;

        if (unloadName == null)
            amount = 0;
        else
            amount = unloadName.Length;

        _curGameLoad = eLoadingState.UNLOADING;
        for(int n = 0; n < amount; n++)
        {
            AO = SceneManager.UnloadSceneAsync(unloadName[n]);
            while(!AO.isDone)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }

        _curGameLoad = eLoadingState.LOADING;
        if (loadName == null)
            amount = 0;
        else
            amount = loadName.Length;

        for(int n = 0; n < amount; n++)
        {
            AO = SceneManager.LoadSceneAsync(loadName[n], LoadSceneMode.Additive);
            while(!AO.isDone)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadName[amount - 1]));

        if(_curStage == eStageState.LOBBY)
        {
            SoundManager._uniqueInstance.PlayBGMSound(SoundManager.eBGMType.LOBBY);
        }
        else if(_curStage == eStageState.INGAME)
        {
            SoundManager._uniqueInstance.PlayBGMSound(SoundManager.eBGMType.INGAME);
        }

        _curGameLoad = eLoadingState.END;
    }

   

    /// <summary>
    /// 인게임에서 로비로 전환되는 씬.
    /// </summary>
    /// <param name="stage"></param>
    public void SceneMoveAtLobby(eStageState stage)
    {
        _curStage = stage;

        string[] unloadStage = new string[1];
        unloadStage[0] = "LobbyScene";

        string[] loadStage = new string[1];
        loadStage[0] = "InGameScene";

        StartCoroutine(LoadingScene(loadStage, unloadStage));
    }

    /// <summary>
    /// GameStart 시 로딩되는 씬.
    /// </summary>
    /// <param name="stage"></param>
    public void SceneMoveAtGame(eStageState stage)
    {
        _curStage = stage;
        string[] unloadStage = new string[1];
        string[] loadStage = new string[1];

        if(stage == eStageState.LOBBY)
        {
            unloadStage[0] = "InGameScene";
            loadStage[0] = "LobbyScene";
        }
        else
        {
            Debug.Log("Error");
        }
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(LoadingScene(loadStage, unloadStage));
    }

}
