using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum eMonsterType
    {
        ZOMBIE,
        CLOWN,
        GHOST
    }

    public enum eMonsterAction
    {
        WALK,
        CLOWN_TryOPEN,
        WALK_CLOWN,
    }

    public static MonsterController _uniqueInstacne;

    [SerializeField] eMonsterType _monsterKind;
    [SerializeField] LayerMask _viewMask;
    Animator _aniCtrl;
    NavMeshAgent _naviAgent;

    List<Vector3> _ltPoints;
    Transform _tfPlayer;
    eMonsterAction _monsterAction;
    Vector3 _posTarget;

    bool _isSelectedAI;
    float _timeCheck;
    float _rndTryOpen;

    public eMonsterType MONSTERTYPE
    {
        get { return _monsterKind; }
    }

    void Awake()
    {
        _uniqueInstacne = this;
        _aniCtrl = GetComponent<Animator>();
        _naviAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseGameManager._uniqueInstance.CURGAMESTATE != BaseGameManager.eLoadingState.END)
            return;

        //if (InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.END)
        //    Destroy(this.gameObject);

        if (InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.PLAY)
        {
            if (_tfPlayer == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("Player");
                if (go != null)
                    _tfPlayer = go.transform;
            }
            else
            {
                if(SelectionManger._uniqueInstance.BOUGHT[1])
                {// 레코드 효과
                    float _recordEffPercent = Random.Range(0, 11);
                    float _invokeEff = 35.0f;
                    _invokeEff += _recordEffPercent;

                    if (Random.Range(0, 101) < _invokeEff)
                    {// 확률 35~45% 확률로 멈춤 (1프레임당)
                        _naviAgent.speed = 0.0f;
                    }
                    else
                    {
                        if (_monsterKind == eMonsterType.ZOMBIE)
                        {
                            _naviAgent.speed = 2.5f;
                        }
                        else if (_monsterKind == eMonsterType.CLOWN)
                        {
                            _naviAgent.speed = 4.0f;
                        }
                        else if (_monsterKind == eMonsterType.GHOST)
                        {
                            _naviAgent.speed = 2.0f;
                        }
                    }
                }


                switch (_monsterAction)
                {
                    case eMonsterAction.WALK:
                        if (Vector3.Distance(transform.position, _tfPlayer.position) < 4.5f)
                        {// 몬스터가 플레이어하고 거리가 **이하일때 플레이어에게 달려간다.
                            _naviAgent.destination = _tfPlayer.position;
                            transform.LookAt(_tfPlayer);
                        }

                        if (Vector3.Distance(transform.position, MonsterSpawnControl._uniqueInstance.ROOTROAM.position) < 7.0f &&
                           _monsterKind == eMonsterType.CLOWN)
                        {// 광대만 허용. 문하고 거리가 **이하가 될때 문여는시늉 에니메이션.
                            ChangedAction(eMonsterAction.CLOWN_TryOPEN);
                            return;
                        }
                        break;
                    case eMonsterAction.CLOWN_TryOPEN:
                        _timeCheck += Time.deltaTime;
                        _rndTryOpen = Random.Range(5, 8);
                    
                        if (_timeCheck >= _rndTryOpen)
                        {// 해당시간이 지나면 문여는 시늉하다가 없어짐.
                            Destroy(gameObject);
                        }
                        else if (Vector3.Distance(transform.position, _tfPlayer.position) < 5.0f)
                        {
                            //if(!Physics.Linecast(transform.position, _tfPlayer.position, _viewMask))
                            {
                                ChangedAction(eMonsterAction.WALK_CLOWN);
                                transform.LookAt(_tfPlayer);
                            }
                        }
                        break;
                    case eMonsterAction.WALK_CLOWN:
                        _naviAgent.destination = _tfPlayer.position;
                        transform.LookAt(_tfPlayer);
                        break;
                }
            }
            ProcessAI();

        }

    }

    /// <summary>
    /// 몬스터가 스폰될 때 달려갈 목표지점 설정.
    /// </summary>
    public void ProcessAI()
    {
        if (_isSelectedAI)
            return;

        _posTarget = GameObject.Find("MonsterSpawnPosition").transform.position;
        _naviAgent.SetDestination(_posTarget);
        _isSelectedAI = true;
        _timeCheck = 0;
    }

    public void ChangedAction(eMonsterAction state)
    {
        switch(state)
        {
            case eMonsterAction.WALK:
                // 모든 몬스터가 있는 에니메이션
                _naviAgent.enabled = true;
                if (_monsterKind == eMonsterType.ZOMBIE)
                {
                    _naviAgent.speed = 2.5f;
                }
                else if (_monsterKind == eMonsterType.CLOWN)
                {
                    _naviAgent.speed = 4.0f;
                }
                else if (_monsterKind == eMonsterType.GHOST)
                {
                    _naviAgent.speed = 2.0f;
                }
                _naviAgent.stoppingDistance = 0;
                break;
            case eMonsterAction.CLOWN_TryOPEN:
                // 광대만 있는 에니메이션
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.CLOWN_TryOPEN_KNOCK);
                _naviAgent.enabled = false;
                break;
            case eMonsterAction.WALK_CLOWN:
                _naviAgent.enabled = true;
                _naviAgent.destination = _tfPlayer.position;
                _naviAgent.speed = 4.0f;
                break;
        }
        _aniCtrl.SetInteger("AniState", (int)state);
        _monsterAction = state;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {// 부딪힌 물체가 Player인 경우
            Destroy(this.gameObject);
            //GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
            //Destroy(go);
            Destroy(other.gameObject);
            InGameController._uniqueInstance.NOWGAMESTATE = InGameController.eGameState.END;
            InGameController._uniqueInstance.gameObject.SetActive(true);
            InGameController._uniqueInstance.GAMESTATE.text = "GAMEOVER..";
            PlayerController._uniqueInstance.PLYDEAD = true;
        }
        else if(other.CompareTag("Selectable"))
        {// 부딪힌 물체가 문인 경우
            Destroy(gameObject);
            if (_monsterKind == eMonsterType.CLOWN)
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.CLOWN_DIE_SCREAM);
            else if (_monsterKind == eMonsterType.GHOST)
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.GHOST_DIE_SCREAM);
            else if (_monsterKind == eMonsterType.ZOMBIE)
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.ZOMBIE_DIE_SCREAM);
        }
    }

}
