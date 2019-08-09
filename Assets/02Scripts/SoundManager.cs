using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
        DOOR_TryOPEN_KNOCK,
        ZOMBIE_SCREAM,
        CLOWN_SCREAM,
    }

    public static SoundManager _uniqueInstance;
    [SerializeField] AudioClip[] _bgmClips;
    [SerializeField] AudioClip[] _effClips;

    AudioSource _bgmPlayer;
    List<AudioSource> _ltEffPlayer;

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        _bgmPlayer = GetComponent<AudioSource>();
        _ltEffPlayer = new List<AudioSource>();
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
