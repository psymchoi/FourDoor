using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public GameObject _bullet;
    public Transform _firePos;
    GameObject go;

    // Update is called once per frame
    void Update()
    {
        if (PlayerController._uniqueInstance.PLYDEAD)
            return;

        if (PlayerController._uniqueInstance.EQUIPSHOTGUN &&
           Input.GetMouseButtonDown(1) &&
           InGameController._uniqueInstance.CRYSTAL >= 10)
        {
            PlayerController._uniqueInstance.ChangedAction(PlayerController.ePlyAction.RIFLE_FIRE);
            GameObject.Find("FireEff").GetComponent<ParticleSystem>().Play();
            go = Instantiate(_bullet, _firePos.transform.position, _firePos.transform.rotation);
            go.transform.parent = transform;
        }
        else if(PlayerController._uniqueInstance.EQUIPSHOTGUN &&
           Input.GetMouseButtonDown(1) && InGameController._uniqueInstance.CRYSTAL < 10)
        {
            SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.SHOTGUN_RELOADING);
            SelectionManger._uniqueInstance.GAME_EXPLAIN.text = "10 크리스탈";
        }
    }
}
