using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCamera : MonoBehaviour
{
    public enum eStateCamera
    {
        NONE = 0,
        WORKING,
        CHANGE_FOLLOW,
        FOLLOW,
    }

    [SerializeField] float _movSpeed;
    [SerializeField] float _rotSpeed;
    [SerializeField] Vector3 _followOffset;

    GameObject _ply;
    Transform _tfRootPos;
    Transform _posPlayer;
    Transform _lookPos;
    eStateCamera _cameraState;
    List<Vector3> _ltPositions;
    Vector3 _posGoal;

    float _timeCheck;
    int _currentIndex;
    int _nextIndex;

    // Start is called before the first frame update
    void Start()
    {
        _ltPositions = new List<Vector3>();

        Transform tf = GameObject.FindGameObjectWithTag("CameraPosRoot").transform;
        Camera.main.GetComponent<ActionCamera>().SetCaemraActionRoot(tf);
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.END)
            return;

        switch (_cameraState)
        {
            case eStateCamera.NONE:
                // 2초 뒤에 카메라 동작..
                _timeCheck += Time.deltaTime;
                if (_timeCheck > 2)
                    _cameraState = eStateCamera.WORKING;
                break;
            case eStateCamera.WORKING:
                if(Vector3.Distance(transform.position, _ltPositions[_nextIndex]) <= 0.3f)
                {
                    _currentIndex = _nextIndex;
                    _nextIndex = _currentIndex + 1;
                    if(_nextIndex >= _ltPositions.Count)
                    {
                        _nextIndex = _currentIndex;
                        _cameraState = eStateCamera.CHANGE_FOLLOW;
                    }
                }
                transform.position = Vector3.MoveTowards(transform.position, _ltPositions[_nextIndex], _movSpeed * Time.deltaTime);
                transform.LookAt(_tfRootPos);
                break;
            case eStateCamera.CHANGE_FOLLOW:
                Vector3 tp = _posPlayer.position + _followOffset;
                Quaternion tq = Quaternion.LookRotation(_lookPos.position - tp);
                transform.position = Vector3.Slerp(transform.position, tp, Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, tq, Time.deltaTime);
                if(Vector3.Distance(transform.position, tp) <= 0.2f)
                {
                    transform.position = tp;
                    transform.LookAt(_lookPos);
                    _cameraState = eStateCamera.FOLLOW;
                    InGameController._uniqueInstance.NOWGAMESTATE = InGameController.eGameState.START;
                    InGameController._uniqueInstance.PLAYIMG.SetActive(false);
                }
                break;
            case eStateCamera.FOLLOW:
                float currAngleY = Mathf.LerpAngle(transform.eulerAngles.y, _posPlayer.eulerAngles.y, Time.deltaTime);

                Quaternion rot = Quaternion.Euler(0, currAngleY, 0);
                _posGoal = _posPlayer.position 
                    - (rot * Vector3.forward * _followOffset.z) + (Vector3.up * _followOffset.y);

                // 이것을 해준 이유는 카메라 이동에따른 캐릭터프리팹과 카메라의 겹침 현상을 없애기 위해서이다.
                if(PlayerController._uniqueInstance.PLAYERACTION == PlayerController.ePlyAction.LANTERN_RUN ||
                    PlayerController._uniqueInstance.PLAYERACTION == PlayerController.ePlyAction.RIFLE_RUN)
                {// 플레이어가 랜턴을 들고 달릴 때
                    transform.position = Vector3.MoveTowards(transform.position, _posGoal -
                        (rot * Vector3.forward * _followOffset.z * 3.5f), 2 * _movSpeed * Time.deltaTime);               
                }
                //else if(PlayerController._uniqueInstance.PLAYERACTION == PlayerController.ePlyAction.LANTERN_JUMP
                //    || PlayerController._uniqueInstance.PLAYERACTION == PlayerController.ePlyAction.NOTHING_JUMP)
                //{// 플레이어가 랜턴을 들때와 안뜰때 점프
                //    transform.position = Vector3.MoveTowards(transform.position, _posGoal +
                //        (rot * Vector3.up * _followOffset.y * 3.5f), 10 * _movSpeed * Time.deltaTime);
                //}
                else
                {// 플레이어의 나머지 행동
                    transform.position = Vector3.MoveTowards(transform.position, _posGoal, 2 * _movSpeed * Time.deltaTime);
                }
                
                //transform.LookAt(_lookPos);
                break;
        }
    }

    public void SetCaemraActionRoot(Transform tf)
    {
        _currentIndex = 0;
        _tfRootPos = tf;
        for(int n = 0; n < _tfRootPos.childCount; n++)
        {
            _ltPositions.Add(_tfRootPos.GetChild(n).position);
        }
        transform.position = _ltPositions[_currentIndex];
        transform.LookAt(_tfRootPos);
        _nextIndex = _currentIndex + 1;
        _cameraState = eStateCamera.NONE;
        _posPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        _lookPos = GameObject.FindGameObjectWithTag("LookPos").transform;
    }
}
