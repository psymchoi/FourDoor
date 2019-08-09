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
    }

    [SerializeField] eMonsterType _monsterKind;
    Animator _aniCtrl;
    NavMeshAgent _naviAgent;

    List<Vector3> _ltPoints;
    Transform _tfPlayer;
    GameObject _tryOpenObj;
    eMonsterAction _monsterAction;
    Vector3 _posTarget;

    bool _isSelectedAI;
    float _timeCheck;
    float _rndTryOpen;

    void Awake()
    {
        _aniCtrl = GetComponent<Animator>();
        _naviAgent = GetComponent<NavMeshAgent>();

        _tryOpenObj = GameObject.FindGameObjectWithTag("Selectable");
    }

    // Update is called once per frame
    void Update()
    {
        if(_tfPlayer == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                _tfPlayer = go.transform;
        }
        else
        {
            switch(_monsterAction)
            {
                case eMonsterAction.WALK:
                    if (Vector3.Distance(transform.position, _tfPlayer.position) < 4.0f)
                    {// 몬스터가 플레이어하고 거리가 **이하일때 플레이어에게 달려간다.
                        _naviAgent.destination = _tfPlayer.position;
                    }
                    else if(Vector3.Distance(transform.position, _tryOpenObj.transform.position) < 2.0f
                        && _monsterKind == eMonsterType.CLOWN)
                    {// 광대만 허용. 문하고 거리가 **이하가 될때 문여는시늉 에니메이션.
                        ChangedAction(eMonsterAction.CLOWN_TryOPEN);
                    }
                    break;
                case eMonsterAction.CLOWN_TryOPEN:
                    _timeCheck += Time.deltaTime;
                    _rndTryOpen = Random.Range(3, 8);
                    
                    if (_timeCheck >= _rndTryOpen)
                    {// 해당시간이 지나면 문여는 시늉하다가 없어짐.
                        Destroy(gameObject);
                    }
                    break;
            }
        }
        ProcessAI();
    }

    public void ProcessAI()
    {
        if (_isSelectedAI)
            return;

        _posTarget = GameObject.Find("MonsterSpawnPosition").transform.position;
        _naviAgent.SetDestination(_posTarget);
        _isSelectedAI = true;
    }

    public void ChangedAction(eMonsterAction state)
    {
        switch(state)
        {
            case eMonsterAction.WALK:
                // 모든 몬스터가 있는 에니메이션
                _naviAgent.enabled = true;
                if (_monsterKind == eMonsterType.ZOMBIE)
                    _naviAgent.speed = 2.5f;
                else if (_monsterKind == eMonsterType.CLOWN)
                    _naviAgent.speed = 4.0f;
                else if (_monsterKind == eMonsterType.GHOST)
                    _naviAgent.speed = 5.0f;

                _naviAgent.stoppingDistance = 0;
                    break;
            case eMonsterAction.CLOWN_TryOPEN:
                // 광대만 있는 에니메이션
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_TryOPEN_KNOCK);
                _naviAgent.enabled = false;
                break;
        }
        _aniCtrl.SetInteger("AniState", (int)state);
        _monsterAction = state;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {// 부딪힌 물체가 Player인 경우

        }
        else if(other.CompareTag("Selectable"))
        {// 부딪힌 물체가 문인 경우
            Destroy(gameObject);
        }
    }

}
