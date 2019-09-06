using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public Dictionary<string, Achievment> achievments = new Dictionary<string, Achievment>();
    static AchievementManager _uniqueInstance;

    public GameObject achievmentPrefab;
    public GameObject visualAchievment;
    public Sprite[] unlockedSprite;
    //public Sprite[] sprites;
    public Text textPoints;
   
    public static AchievementManager Instance
    {
        get
        {
            if(_uniqueInstance == null)
            {
                _uniqueInstance = GameObject.FindObjectOfType<AchievementManager>();
            }
            return AchievementManager._uniqueInstance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.DeleteAll();
        //PlayerPrefs.DeleteKey("Points");
        textPoints.text
               = string.Format("ARIVAL : {0} / {1}", PlayerPrefs.GetInt("Points"), "20");

        CreateAchievment("General", "Press W", "Press W to unlock this achievment", 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {// w를 누르면 이벤트 발생
            EarnAchievment("Press W");
        }
    }

    /// <summary>
    /// 업적달성했는 푯말 복사 후 만들어 준 다음 코루틴시킨다.
    /// </summary>
    /// <param name="title"></param>
    public void EarnAchievment(string title)
    {
        if(achievments[title].EarnAchievment())
        {
            GameObject achievment = (GameObject)Instantiate(visualAchievment);
            SetAchievmentInfo("EarnCanvas", achievment, title);
            textPoints.text 
                = string.Format("ARIVAL : {0} / {1}", PlayerPrefs.GetInt("Points"), "20");
            StartCoroutine(HideAchievment(achievment));
        }
    }

    /// <summary>
    /// 3초동안 업적달성 푯말 띄어주는 역할.
    /// </summary>
    /// <param name="achievment"></param>
    /// <returns></returns>
    public IEnumerator HideAchievment(GameObject achievment)
    {
        yield return new WaitForSeconds(3);
        Destroy(achievment);
    }

    /// <summary>
    /// 처음 게임을 킬 때 업적들 목록
    /// </summary>
    /// <param name="parent">객체가 생성될 위치</param>
    /// <param name="title">업적제목</param>
    /// <param name="description">업적내용</param>
    /// <param name="points">업적포인트</param>
    /// <param name="spriteIndex">사실상 노필요..</param>
    public void CreateAchievment(string parent, string title, string description, int points, int spriteIndex)
    {
        GameObject achievment = (GameObject)Instantiate(achievmentPrefab);

        Achievment newAchievment = new Achievment(title, description, points, spriteIndex, achievment);

        achievments.Add(title, newAchievment);

        SetAchievmentInfo(parent, achievment, title);
    }
    public void SetAchievmentInfo(string parent, GameObject achievment, string title)
    {
        achievment.transform.SetParent(GameObject.Find(parent).transform);
        achievment.transform.localScale = new Vector3(1, 1, 1);
        achievment.transform.GetChild(0).GetComponent<Text>().text = title;
        achievment.transform.GetChild(1).GetComponent<Text>().text = achievments[title].Description;
        achievment.transform.GetChild(2).GetComponent<Text>().text = achievments[title].Points.ToString();
        //achievment.transform.GetChild(3).GetComponent<Image>().sprite = sprites[achievments[title].SpriteIndex];
    }
}
