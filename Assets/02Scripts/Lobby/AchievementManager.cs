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
    public string[] description;
    public Text textPoints;
    //public Sprite[] sprites;

    int fadeTime = 2;
   
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

        /* 테스트용 */
        //CreateAchievment("General", "W", "???", 1, 0);
        //CreateAchievment("General", "S", "Press S", 1, 0);
        //CreateAchievment("General", "WASD", "Basic Operation", 1, 0, new string[] { "W", "S" });

        CreateAchievment("General", description[3], "Hunter", "???", 1, 0);                 // 샷건
        CreateAchievment("General", description[4], "So Scratchy..", "???", 1, 0);          // 레코드판
        CreateAchievment("General", description[5], "Was it working..?", "???", 1, 0);      // 피아노맨
        CreateAchievment("General", description[6], "★Grand Slam★", "???", 1, 0, new string[] { "Hunter", "So Scratchy..", "Was it working..?" });      // 아이템 다 모으면
    }

    // Update is called once per frame
    // 업적 테스트용~~
    //void Update()
    //{
    //    //if (BaseGameManager._uniqueInstance.CURGAMESTATE == BaseGameManager.eStageState.INGAME)
    //    {// 게임 안에서일때만 업적 성취가 일어나도록.
    //        if (Input.GetKeyDown(KeyCode.W))
    //        {// w를 누르면 이벤트 발생
    //            EarnAchievment("W", 0);
    //        }
    //        if (Input.GetKeyDown(KeyCode.S))
    //        {// w를 누르면 이벤트 발생
    //            EarnAchievment("S", 1);
    //        }
    //    }
    //}

    /// <summary>
    /// 업적달성했는 푯말 복사 후 만들어 준 다음 코루틴시킨다.
    /// </summary>
    /// <param name="title"></param>
    public void EarnAchievment(string title)
    {
        if(achievments[title].EarnAchievment())
        {
            GameObject achievment = (GameObject)Instantiate(visualAchievment);
            SetAchievmentInfo("EarnCanvas", achievment, title, achievments[title].AchievmentContent);
            textPoints.text 
                = string.Format("ARIVAL : {0} / {1}", PlayerPrefs.GetInt("Points"), "20");
            StartCoroutine(FadeAchievment(achievment));
        }
    }

    /// <summary>
    /// 3초동안 업적달성 푯말 띄어주는 역할.
    /// </summary>
    /// <param name="achievment"></param>
    /// <returns></returns>
    /*public IEnumerator HideAchievment(GameObject achievment)
    {
        yield return new WaitForSeconds(3);
        Destroy(achievment);
    }*/

    /// <summary>
    /// 업적 Fade in / Fade Out
    /// </summary>
    /// <param name="achievment"></param>
    /// <returns></returns>
    IEnumerator FadeAchievment(GameObject achievment)
    {
        CanvasGroup canvasGroup = achievment.GetComponent<CanvasGroup>();

        float rate = 1.0f / fadeTime;

        int startAlpha = 0;
        int endAlpha = 1;

        for(int i = 0; i < 2; i++)
        {// fade in, fade out 두번 돌리기 위해서
            float progress = 0.0f;

            while (progress < 1.0f)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

                progress += rate * Time.deltaTime;

                yield return null;
            }

            yield return new WaitForSeconds(2);

            startAlpha = 1;
            endAlpha = 0;
        }

        Destroy(achievment);
    }

    /// <summary>
    /// 처음 게임을 킬 때 업적들 목록 생성.
    /// </summary>
    /// <param name="parent">객체가 생성될 위치</param>
    /// <param name="title">업적제목</param>
    /// <param name="description">업적내용</param>
    /// <param name="points">업적포인트</param>
    /// <param name="spriteIndex">사실상 노필요..</param>
    public void CreateAchievment(string parent, string achievmentContent, string title, string description, int points, int spriteIndex, string[] dependencies = null)
    {
        GameObject achievment = (GameObject)Instantiate(achievmentPrefab);      // 업적 내용 프리팹 클론복사.
        Achievment newAchievment = new Achievment(achievmentContent, title, description, points, spriteIndex, achievment);
        achievments.Add(title, newAchievment);
        SetAchievmentInfo(parent, achievment, title);

        if(dependencies != null)
        {
            foreach(string achievmentTitle in dependencies)
            {
                Achievment dependency = achievments[achievmentTitle];
                dependency.Child = title;
                newAchievment.AddDependency(achievments[achievmentTitle]);

                // Dependency = Press Space <-- Child = Press W
                // NewAchievment = Press W --> Press Space
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent">생성되는 위치</param>
    /// <param name="achievment">생성되는 오브젝트</param>
    /// <param name="title">업적 제목</param>
    /// <param name="achievementContent">업적 내용 (완료시 넣을 설명)</param>
    public void SetAchievmentInfo(string parent, GameObject achievment, string title, string achievementContent = null)
    {
        achievment.transform.SetParent(GameObject.Find(parent).transform);
        achievment.transform.localScale = new Vector3(1, 1, 1);
        achievment.transform.GetChild(0).GetComponent<Text>().text = title;
        
        if(achievementContent == null)
            achievment.transform.GetChild(1).GetComponent<Text>().text = achievments[title].Description;
        else
            achievment.transform.GetChild(1).GetComponent<Text>().text = achievementContent;

        //achievment.transform.GetChild(2).GetComponent<Text>().text = achievments[title].Points.ToString();
        //achievment.transform.GetChild(3).GetComponent<Image>().sprite = sprites[achievments[title].SpriteIndex];
    }
    
}
