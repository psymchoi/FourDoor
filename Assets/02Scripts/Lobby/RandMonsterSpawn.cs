using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandMonsterSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] _monster;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MonsterSpawn(3.0f));
    }
    
    IEnumerator MonsterSpawn(float _spawnTime)
    {
        GameObject go;
        int _monNum = Random.Range(0, _monster.Length);
        go = Instantiate(_monster[_monNum], this.transform.position, this.transform.rotation);
        go.transform.parent = this.transform;
        yield return new WaitForSeconds(_spawnTime);
        Destroy(go);
        StartCoroutine(MonsterSpawn(Random.Range(8, 12)));
    }
}
