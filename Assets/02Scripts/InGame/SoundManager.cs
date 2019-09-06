using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _uniqueInstance;

    public enum eBGMType
    {
        LOBBY   = 0,
        INGAME,
    }

    public enum eEffType
    {
        BTN,
        LANTERN_ON,
        LANTERN_OFF,
        DOOR_OPEN,
        DOOR_CLOSE,
        SHOP_OPEN,
        SHOP_CLOSE,
        CLOWN_TryOPEN_KNOCK,
        CLOWN_DIE_SCREAM,
        ZOMBIE_SCREAM,
        ZOMBIE_DIE_SCREAM,
        GHOST_DIE_SCREAM,
        MATCHES_SOUND,
        SHOTGUN_EQUIP,
        SHOTGUN_FIRE,
        SHOTGUN_RELOADING,
        CONGRATULATIONS,
    }


    [SerializeField] AudioClip[] _bgmClips;
    [SerializeField] AudioClip[] _effClips;

    AudioSource _bgmPlayer;
    List<AudioSource> _ltEffPlayer;

    public static SoundManager INSTANCE
    {
        get { return _uniqueInstance; }
    }
    public AudioSource BGM
    {
        get { return _bgmPlayer; }
        set { _bgmPlayer = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        _bgmPlayer = GetComponent<AudioSource>();
        _ltEffPlayer = new List<AudioSource>();

        SoundManager._uniqueInstance.PlayBGMSound(SoundManager.eBGMType.LOBBY);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        foreach(AudioSource item in _ltEffPlayer)
        {
            if(!item.isPlaying)
            {
                _ltEffPlayer.Remove(item);
                Destroy(item.gameObject);
                break;
            }
        }
    }

    public void PlayBGMSound(eBGMType type, float vol = 0.7f, bool isloop = true)
    {
        _bgmPlayer.clip = _bgmClips[(int)type];
        _bgmPlayer.volume = vol;
        _bgmPlayer.loop = isloop;

        _bgmPlayer.Play();
    }

    public void PlayEffSound(eEffType type, float vol = 0.7f, bool isloop = false)
    {
        GameObject go = new GameObject("EffSound");
        go.transform.SetParent(transform);

        AudioSource AS = go.AddComponent<AudioSource>();
        AS.clip = _effClips[(int)type];
        AS.volume = vol;
        AS.loop = isloop;

        AS.Play();

        _ltEffPlayer.Add(AS);
    }
}
