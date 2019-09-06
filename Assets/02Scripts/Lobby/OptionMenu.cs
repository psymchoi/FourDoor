using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public static OptionMenu _uniqueInstance;

    public Slider _bgmVol;
    public Slider _effVol;

    public Slider EFF_VOL
    {
        get { return _effVol; }
        set { _effVol = value; }
    }

    void Awake()
    {
        _uniqueInstance = this;    
    }

    public void CurBGMVolume()
    {
        SoundManager._uniqueInstance.BGM.volume = _bgmVol.value;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
