using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievment : MonoBehaviour
{
    string name;
    string description;
    bool unlocked;
    int points;
    int spriteIndex;

    GameObject achievmentRef;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    public int Points
    {
        get { return points; }
        set { points = value; }
    }
    public int SpriteIndex
    {
        get { return spriteIndex; }
        set { spriteIndex = value; }
    }

    public Achievment(string name, string description, int points, int spriteIndex, GameObject achievmentRef)
    {
        this.name = name;
        this.description = description;
        this.unlocked = false;
        this.points = points;
        this.spriteIndex = spriteIndex;
        this.achievmentRef = achievmentRef;
        LoadAchievment();
    }


    public bool EarnAchievment()
    {
        if(!unlocked)
        {// 업적 달성이 안되어 있는 상태라면 if문 안으로. (미완료 => 완료로 sprite들이 변경 됨.)

            achievmentRef.GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[0];
            achievmentRef.transform.GetChild(3).GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[1];
            achievmentRef.transform.GetChild(4).GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[2];
            SaveAchievment(true);
            //achievmentRef.GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite_MARK;
            //unlocked = true;
            return true;
        }
        return false;
    }

    public void SaveAchievment(bool value)
    {
        unlocked = value;

        int tmpPoints = PlayerPrefs.GetInt("Points");

        PlayerPrefs.SetInt("Points", tmpPoints += points);

        PlayerPrefs.SetInt(name, value ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadAchievment()
    {
        unlocked = PlayerPrefs.GetInt(name) == 1 ? true : false;

        if(unlocked)
        {
            AchievementManager.Instance.textPoints.text 
                = string.Format("ARIVAL : {0} / {1}", PlayerPrefs.GetInt("Points"), "20");
            achievmentRef.GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[0];
            achievmentRef.transform.GetChild(3).GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[1];
            achievmentRef.transform.GetChild(4).GetComponent<Image>().sprite = AchievementManager.Instance.unlockedSprite[2];
        }
    }
}
