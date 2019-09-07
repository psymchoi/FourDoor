using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager _uniqueInstance;
    [SerializeField] GameObject _gameTitle;
    [SerializeField] GameObject _startBtn;
    [SerializeField] GameObject _settingBtn;
    [SerializeField] GameObject _quitBtn;
    [SerializeField] GameObject _settingObj;

    [SerializeField] Text _controlManual;
    [SerializeField] Text _controlManualExplain;
    [SerializeField] GameObject _optionManual;
    [SerializeField] GameObject _itemManual;

    BaseGameManager.eStageState _curStageIdx;

    bool _achievmentBtn;
    public bool ACHIEVMENT
    {
        get { return _achievmentBtn; }
        set { _achievmentBtn = value; }
    }

    void Start()
    {
        _uniqueInstance = this;
        _achievmentBtn = false;
    }

    public void SettingBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _gameTitle.SetActive(false);
        _startBtn.SetActive(false);
        _settingBtn.SetActive(false);
        _quitBtn.SetActive(false);
        _settingObj.SetActive(true);
    }
    public void ManualBtn()
    {// 조작법 버튼 클릭.
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _controlManual.gameObject.SetActive(true);
        _controlManualExplain.gameObject.SetActive(true);
        _controlManual.text = " Move : \n RUN : \n LANTERN : \n SHOTGUN : \n CLICK : \n SHOOT : ";
        _controlManualExplain.text = " w a s d \n Left Shift \n Press 1 \n Press 2 \n Left Mouse \n Right Mouse";
        _optionManual.SetActive(false);
        _itemManual.SetActive(false);
        _achievmentBtn = false;
    }
    public void OptionBtn()
    {// 옵션 보튼 클릭.
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _controlManual.gameObject.SetActive(false);
        _controlManualExplain.gameObject.SetActive(false);
        _optionManual.SetActive(true);
        _itemManual.SetActive(false);
        _achievmentBtn = false;
    }
    public void ItemExplainBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _controlManual.gameObject.SetActive(false);
        _controlManualExplain.gameObject.SetActive(false);
        _optionManual.SetActive(false);
        _itemManual.SetActive(true);
        _achievmentBtn = false;
    }
    public void AchievmentBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _controlManual.gameObject.SetActive(false);
        _controlManualExplain.gameObject.SetActive(false);
        _optionManual.SetActive(false);
        _itemManual.SetActive(false);
        _achievmentBtn = true;
    }

    public void BackBtn()
    {// 뒤로가기 버튼 클릭.
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        _gameTitle.SetActive(true);
        _startBtn.SetActive(true);
        _settingBtn.SetActive(true);
        _quitBtn.SetActive(true);
        _settingObj.SetActive(false);
        _achievmentBtn = false;
    }

    public void QuitBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN, OptionMenu._uniqueInstance.EFF_VOL.value);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif
    }
}
