using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    public enum eGameState
    {
        NONE,
        READY,
        MAPSETTING,
        START,
        PLAY,
        NEXT_STAGE,
        END,
        RESULT,
    }

    public static InGameController _uniqueInstance;

    [SerializeField] GameObject _prefabPlayer;
    [SerializeField] GameObject _plyStartPosition;
    [SerializeField] GameObject[] _itemList;
    [SerializeField] Text _leftTimeTxt;
    [SerializeField] Text _crystalTxt;
    [SerializeField] Text _gameState;
    [SerializeField] Light[] _candleLight;
    [SerializeField] Light _crystal;

    MonsterSpawnControl[] _ctrlSpawn;

    eGameState _curGameState;
    bool _isSpawn;
    float _crstl;
    float _leftTimes;
    int _maxMonsterCount;
    int _doorCount;
    int _limitDoorCount;

    public eGameState NOWGAMESTATE
    {
        get { return _curGameState; }
        set { _curGameState = value; }
    }
    public bool ENABLESPAWN
    {
        get { return _isSpawn; }
    }
    public int DOORCOUNT
    {
        get { return _doorCount; }
        set { _doorCount = value; }
    }
    public int LIMITDOORCOUNT
    {
        get { return _limitDoorCount; }
        set { _limitDoorCount = value; }
    }
    public float TIMECHECK
    {
        get { return _crstl; }
        set { _crstl = value; }
    }
    public Text GAMESTATE
    {
        get { return _gameState; }
        set { _gameState = value; }
    }
    public Light[] CANDLELIGHT
    {
        get { return _candleLight; }
        set { _candleLight = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        _curGameState = eGameState.READY;

        //SoundManager._uniqueInstance.PlayBGMSound(SoundManager.eBGMType.INGAME);
    }

    // Update is called once per frame
    void Update()
    {
        switch(_curGameState)
        {
            case eGameState.READY:
                GameReady();
                GameMapSetting();
                break;
            case eGameState.START:
                _crstl += Time.deltaTime;
                _gameState.text = "GameStart!";
                if(_crstl >= 1.5f)
                {
                    _crstl = 0;
                    _gameState.gameObject.SetActive(false);
                    _curGameState = eGameState.PLAY;
                }
                break;
            case eGameState.PLAY:
                _crstl += Time.deltaTime;
                _leftTimes -= Time.deltaTime;
                _leftTimeTxt.text = _leftTimes.ToString("N1");

                if (_crstl >= 0 && _crstl < 40)
                {
                    _crystal.range = 1.0f;
                    _crystalTxt.text = string.Format("Crystal : {0}", _crstl.ToString("N0"));
                }
                else if(_crstl >= 40 && _crstl < 100)
                {
                    _crystal.range = 6.0f;
                    _crystalTxt.text = string.Format("Crystal.Lv : {0}", _crstl.ToString("N0"));
                }
                else if(_crstl >= 100)
                {
                    _crystal.range = 20.0f;
                    _crystalTxt.text = string.Format("Crystal.Lv : {0}", _crstl.ToString("N0"));
                }

                //if(_leftTimes <= 0)
                //{
                //    _timeCheck = 60.0f;
                //    _curGameState = eGameState.NEXT_STAGE;
                //}
                break;
        }
    }

    public void GameReady()
    {
        _curGameState = eGameState.READY;
        _gameState.text = "READY";
    }

    public void GameMapSetting()
    {
        _curGameState = eGameState.MAPSETTING;
        // 플레이어 생성.
        GameObject go = Instantiate(_prefabPlayer, _plyStartPosition.transform.position, _plyStartPosition.transform.rotation);
        go.transform.parent = transform;
        // 카메라 워킹위치 세팅.
        //Transform tf = GameObject.FindGameObjectWithTag("CameraPosRoot").transform;
        //Camera.main.GetComponent<ActionCamera>().SetCaemraActionRoot(tf);

        // 플레이시간
        _leftTimes = 60.0f;
        _leftTimeTxt.text = _leftTimes.ToString("N1");

        // 스폰 포인트 활성화 및 몬스터스폰 최대마리 수.
        _ctrlSpawn = FindObjectsOfType<MonsterSpawnControl>();
        _isSpawn = true;
        _maxMonsterCount = 6;
        _doorCount = 0;
        _limitDoorCount = 3;

        // 촛대 밝기 상태, 크리스탈 밝기 상태
        for (int n = 0; n < _candleLight.Length; n++)
        {
            _candleLight[n].range = 0f;
        }
        _crystal.range = 1.0f;

        // 크리스탈 = Money
        _crystalTxt.text = string.Format("Crystal : {0}", _crstl.ToString("N0"));
    }

    public void CheckCountMonster()
    {
        int tCount = 0;
        for(int n = 0; n < _ctrlSpawn.Length; n++)
        {
            tCount += _ctrlSpawn[n].CurMonsterCount;
        }

        if (tCount >= _maxMonsterCount)
            _isSpawn = false;
        else
            _isSpawn = true;
    }
}
