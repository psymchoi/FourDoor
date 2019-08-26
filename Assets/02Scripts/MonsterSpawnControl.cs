using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnControl : MonoBehaviour
{
    public static MonsterSpawnControl _uniqueInstance;

    [SerializeField] GameObject[] _prefabMonster;

    List<GameObject> _ltSpawns;
    Transform _rootRoam;
    Transform[] _roamPoints;

    float _timeSpawn;
    float _timeCheck;
    int _monsNum;
    int _spawnRanNum;
    int _limitCountSpawn;
    bool _EndGame;

    public int CurMonsterCount
    {
        get { return _ltSpawns.Count; }
    }
    public Transform ROOTROAM
    {
        get { return _rootRoam; }
    }

    void Awake()
    {
        _uniqueInstance = this;
        _ltSpawns = new List<GameObject>();

        _timeSpawn = 10.0f;
        _limitCountSpawn = 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rootRoam = transform;
        GatheringRoammingPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.END)
        {
            if (InGameController._uniqueInstance.ENABLESPAWN)
            {
                if (_ltSpawns.Count < _limitCountSpawn)
                {// ltspawns의 크기가 _limitCountSpawn보다 커지면 몬스터 스폰을 중단.
                    _timeCheck += Time.deltaTime;
                    if(_timeSpawn >= _timeCheck)
                    {// 시간이 _timeSpawn보다 크면 몬스터를 스폰시켜준다.
                        _timeCheck = 0;
                        _timeSpawn = Random.Range(5, 10);
                        SpawnMonster();
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        foreach(GameObject go in _ltSpawns)
        {
            if(go == null)
            {
                InGameController._uniqueInstance.CheckCountMonster();
                _ltSpawns.Remove(go);
                break;
            }
        }
    }

    /// <summary>
    /// 몬스터를 스폰시켜주는 메소드.
    /// </summary>
    public void SpawnMonster()
    {
        _monsNum = Random.Range(0, _prefabMonster.Length);              // 몬스터 종류 랜덤넘버.
        _spawnRanNum = Random.Range(0, _prefabMonster.Length + 1);      // 몬스터 스폰위치 랜덤넘버.

        GameObject go = Instantiate(_prefabMonster[_monsNum],
            _roamPoints[_spawnRanNum].position, _roamPoints[_spawnRanNum].rotation);
        go.transform.LookAt(_rootRoam);
        go.transform.parent = transform;

        if(MonsterController._uniqueInstacne.MONSTERTYPE == MonsterController.eMonsterType.ZOMBIE)        
            SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.ZOMBIE_SCREAM);
        
        _ltSpawns.Add(go);
    }

    /// <summary>
    /// 몬스터가 스폰될 지점을 초기화해준다.
    /// </summary>
    void GatheringRoammingPoint()
    {
        if (_rootRoam.childCount == 0)
            return;

        _roamPoints = new Transform[_rootRoam.childCount];
        for(int n = 0; n < _roamPoints.Length; n++)
        {
            _roamPoints[n] = _rootRoam.GetChild(n);
        }
    }
}
