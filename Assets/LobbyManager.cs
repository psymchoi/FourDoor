using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    BaseGameManager.eStageState _curStageIdx;

    public void StartBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);

        int stageIdx = 2;
        if ((int)_curStageIdx != stageIdx)
            _curStageIdx = (BaseGameManager.eStageState)stageIdx;
        else
            _curStageIdx = 0;

        BaseGameManager._uniqueInstance.SceneMoveAtLobby(_curStageIdx);
    }

    public void SettingBtn()
    {

    }

    public void QuitBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif
    }
}
