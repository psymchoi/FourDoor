﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    public enum eGameState
    {
        NONE,
        READY,
        START,
        PLAY,
        END,
        RESULT,
    }

    public static InGameController _uniqueInstance;

    [SerializeField] GameObject _prefabPlayer;
    [SerializeField] GameObject _plyStartPosition;
    [SerializeField] GameObject[] _itemList;
    [SerializeField] Light[] _candleLight;

    eGameState _curGameState;

    public eGameState NOWGAMESTATE
    {
        get { return _curGameState; }
        set { _curGameState = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        _curGameState = eGameState.READY;
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
                break;
        }
    }

    public void GameReady()
    {
        _curGameState = eGameState.READY;
    }

    public void GameMapSetting()
    {
        _curGameState = eGameState.START;
        // 플레이어 생성.
        GameObject go = Instantiate(_prefabPlayer, _plyStartPosition.transform.position, _plyStartPosition.transform.rotation);
        
        // 카메라 워킹위치 세팅.
        Transform tf = GameObject.FindGameObjectWithTag("CameraPosRoot").transform;
        Camera.main.GetComponent<ActionCamera>().SetCaemraActionRoot(tf);

        // 촛대 밝기 상태
        for(int n = 0; n < _candleLight.Length; n++)
        {
            _candleLight[n].range = 0f;
        }

        // 레코드판 및 피아노맨
    }
}
