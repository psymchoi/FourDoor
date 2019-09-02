using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float _bulSpeed;

    void Start()
    {
        _bulSpeed = 1.0f;    
    }

    void Update()
    {
        transform.Translate(Vector3.forward * _bulSpeed);
        Destroy(this.gameObject, 1.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Monster"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            if (MonsterController._uniqueInstacne.MONSTERTYPE == MonsterController.eMonsterType.CLOWN)
            {
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.CLOWN_DIE_SCREAM);
            }
            else if (MonsterController._uniqueInstacne.MONSTERTYPE == MonsterController.eMonsterType.GHOST)
            {
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.GHOST_DIE_SCREAM);
            }
            else if (MonsterController._uniqueInstacne.MONSTERTYPE == MonsterController.eMonsterType.ZOMBIE)
            {
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.ZOMBIE_DIE_SCREAM);
            }
        }
    }
}
