using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger _uniqueInstance;

    [SerializeField] GameObject _blackFade;
    [SerializeField] Text _dDay;

    BaseGameManager.eStageState _curStageIdx;
    Animator _anictrl;

    public GameObject FADE
    {
        get { return _blackFade; }
        set { _blackFade = value; }
    }

    void Awake()
    {
        _uniqueInstance = this;
        _anictrl = GetComponent<Animator>();
        _dDay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Fade in이 일어난 후.
    /// </summary>
    public void StartMainLobby()
    {
        _blackFade.SetActive(false);
    }

    /// <summary>
    /// 게임시작 버튼을 눌렀을 경우.
    /// </summary>
    public void StartBtn()
    {
        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);
        _blackFade.SetActive(true);
        _anictrl.SetTrigger("FadeOut_ToInGame");
    }
    /// <summary>
    /// "FadeOut_ToInGame" 에니메이션이 일어나고 난 이후 => 인게임으로.
    /// </summary>
    public void StartInGame()
    {
        int stageIdx = 2;
        if ((int)_curStageIdx != stageIdx)
            _curStageIdx = (BaseGameManager.eStageState)stageIdx;
        else
            _curStageIdx = 0;

        BaseGameManager._uniqueInstance.SceneMoveAtLobby(_curStageIdx);
    }

    /// <summary>
    /// 다음 스테이지로 갈 때 일어나는 이벤트.
    /// </summary>
    public void FadeOutNextStage()
    {
        _blackFade.SetActive(true);
        _dDay.gameObject.SetActive(true);
        _anictrl.enabled = true;
        _anictrl.SetTrigger("FadeOutIn_NextStage");
        _dDay.text = "Day " + InGameController._uniqueInstance.DAYS.ToString();
    }
    public void FadeInNextStage()
    {
        _blackFade.SetActive(false);
        _dDay.gameObject.SetActive(false);
        InGameController._uniqueInstance.NEXTDAY = false;
        _anictrl.enabled = false;
    }

    /// <summary>
    /// 게임에서 플레이어가 죽었을 경우.
    /// </summary>
    public void EndGame()
    {
        _blackFade.SetActive(true);
        _anictrl.SetTrigger("FadeOut_ToLobby");
    }
    /// <summary>
    /// "FadeOut_ToLobby" 에니메이션이 일어난 후 => 로비로.
    /// </summary>
    public void StartLobby()
    {
        int stageIdx = 1;
        if ((int)_curStageIdx != stageIdx)
            _curStageIdx = (BaseGameManager.eStageState)stageIdx;
        else
            _curStageIdx = 0;

        BaseGameManager._uniqueInstance.SceneMoveAtGame(_curStageIdx);
    }
}
