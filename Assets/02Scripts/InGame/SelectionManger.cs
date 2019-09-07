using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManger : MonoBehaviour
{
    public static SelectionManger _uniqueInstance;
    [SerializeField] string[] selectableTag;
    [SerializeField] Material[] highlightMaterial;
    [SerializeField] Material[] defaultMaterial;
    [SerializeField] GameObject[] _door;
    [SerializeField] GameObject[] _item;
    [SerializeField] GameObject[] _itemActive;
    [SerializeField] GameObject[] _candleLight;
    [SerializeField] Slider[] _doorCloseSlider;
    [SerializeField] Text _gameExplain;

    Transform _selection;
    bool[] _onClick;
    bool[] _timeBought;
    float[] _curValue;
    int _divideOpenDoorNum;

    public bool[] BOUGHT
    {
        get { return _timeBought; }
        set { _timeBought = value; }
    }
    public int DoorOpenNum
    {
        get { return _divideOpenDoorNum; }
        set { _divideOpenDoorNum = value; }
    }
    public Text GAME_EXPLAIN
    {
        get { return _gameExplain; }
        set { _gameExplain = value; }
    }

    void Awake()
    {
        _uniqueInstance = this;

        _gameExplain.text = "";
        _onClick = new bool[_door.Length];
        _timeBought = new bool[_item.Length];
        _curValue = new float[_doorCloseSlider.Length];

        // _onClick     true = open / false = close
        for (int n = 0; n < _door.Length; n++)
            _onClick[n] = true;

        for(int n = 0; n < _item.Length; n++)
            _timeBought[n] = false;

        for (int n = 0; n < _doorCloseSlider.Length; n++)
            _curValue[n] = 1.0f;

        _divideOpenDoorNum = 23;
    }

    // Update is called once per frame
    void Update()
    {
        if(InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.PLAY)
        {
            if(Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1))
                _gameExplain.text = "";

            if (_selection != null)
            {
                var selectionRenderer = _selection.GetComponent<Renderer>();

                if (_selection.CompareTag("Selectable") ||
                    _selection.CompareTag("Nothing"))
                {
                    for (int n = 0; n < _door.Length; n++)
                    {
                        if (_selection.gameObject == _door[n])
                        {
                            if (_onClick[n])
                            {// 열려있는 상태에서의 문 닫으려고할때 Material
                                selectionRenderer.material = highlightMaterial[2];
                                if (n % 2 == 0)
                                {
                                    _door[n + 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                }
                                else
                                {
                                    _door[n - 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                }
                                break;
                            }
                            else
                            {// 닫혀있는 상태에서의 원래 문 Material
                                selectionRenderer.material = defaultMaterial[0];
                                if (n % 2 == 0)
                                {
                                    _door[n + 1].GetComponent<Renderer>().material = defaultMaterial[0];
                                }
                                else
                                {
                                    _door[n - 1].GetComponent<Renderer>().material = defaultMaterial[0];
                                }
                                break;
                            }
                        }
                    }
                }
                else if (_selection.CompareTag(selectableTag[2]))
                {// 상점.
                    selectionRenderer.material = defaultMaterial[1];
                    _gameExplain.text = "";
                }
                else if (_selection.CompareTag(selectableTag[3]))
                {// 아이템.
                    if (_selection.gameObject == _item[0])
                    {// 샷건
                        _item[0].GetComponent<Renderer>().material = defaultMaterial[2];
                    }
                    else if (_selection.gameObject == _item[1])
                    {// 레코드 선반
                        _item[1].GetComponent<Renderer>().material = defaultMaterial[3];
                    }
                    else if (_selection.gameObject == _item[2])
                    {// 피아노 의자
                        _item[2].GetComponent<Renderer>().material = defaultMaterial[4];
                    }
                    //selectionRenderer.material = defaultMaterial[4];
                }
                else if (_selection.CompareTag(selectableTag[4]))
                {// 촛대.
                    for(int n = 0; n < _candleLight.Length; n++)
                    {
                        if(_selection.gameObject == _candleLight[n])
                        {
                            _candleLight[n].GetComponent<Renderer>().material = defaultMaterial[5];
                            break;
                        }
                    }
                }
                else if (_selection.CompareTag(selectableTag[5]))
                {// 베개
                    _gameExplain.text = "";
                }
                _selection = null;
            }

            for (int n = 0; n < _doorCloseSlider.Length; n++)
            {
                if (!_onClick[n * 2])
                {// 문이 닫혀있을 때 슬라이더 이동
                    if(_doorCloseSlider[n].value <= 0.01f)
                    {// 슬라이더 시간이 다되면 문이 열림
                        InGameController._uniqueInstance.DOORCOUNT--;
                        _onClick[n * 2] = !_onClick[n * 2];
                        _onClick[n * 2 + 1] = _onClick[n * 2];
                        SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_OPEN);
                        _door[n * 2].GetComponent<Renderer>().material 
                            = _door[n * 2 + 1].GetComponent<Renderer>().material
                            = highlightMaterial[2];
                        _door[n * 2].tag = _door[n * 2 + 1].tag = "Nothing";

                        _curValue[n] = 0.03f;

                        _selection = null;
                        continue;
                    }

                    _doorCloseSlider[n].value = Mathf.MoveTowards(_curValue[n], 0.01f, Time.deltaTime / _divideOpenDoorNum);
                    _curValue[n] = (float)_doorCloseSlider[n].value;
                }
                else if(_onClick[n * 2])
                {// 문이 열려있을 때
                    _doorCloseSlider[n].value = _curValue[n];
                }
            }
            
                
            /*
             * 카메라 레이를 발사했을 때 나타나는 Material.
             * */
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1.5f))
            {// 카메라와 바라보는 물체의 거리가 1.5f 내에 있으면 발생.
                var selection = hit.transform;
                if(selection.CompareTag("Selectable") ||
                    selection.CompareTag("Nothing"))
                {// 카메라 레이포인트에 비췄을 때 바꿀 Material.
                 // 문
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        for (int n = 0; n < _door.Length + 1; n++)
                        {
                            if (selection.gameObject == _door[n])
                            {
                                if (_onClick[n])
                                {// 닫혀있는데 열려고하는 Material(투명한)
                                    selectionRenderer.material = highlightMaterial[3];
                                    if (n % 2 == 0)
                                    {
                                        _door[n + 1].GetComponent<Renderer>().material = highlightMaterial[3];
                                    }
                                    else
                                    {
                                        _door[n - 1].GetComponent<Renderer>().material = highlightMaterial[3];
                                    }
                                    break;
                                }
                                else
                                {// 열려있는데 닫을려고하는 Material(회색)
                                    selectionRenderer.material = highlightMaterial[0];
                                    if (n % 2 == 0)
                                    {
                                        _door[n + 1].GetComponent<Renderer>().material = highlightMaterial[0];
                                    }
                                    else
                                    {
                                        _door[n - 1].GetComponent<Renderer>().material = highlightMaterial[0];
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    /*
                     * 레이가 찍히는 상태에서 버튼을 눌렀을 때 발생하는 이벤트.
                     * */
                    if (Input.GetMouseButtonDown(0))
                    {// 오른쪽 마우스 버튼을 눌렀을 때 이벤트 발생.
                        for(int n = 0; n < _door.Length + 1; n++)
                        {
                            if(selection.gameObject == _door[n])
                            {
                                _onClick[n] = !_onClick[n];
                                if (_onClick[n])
                                {// open
                                 /* Material을 원본걸로 바꿈(옆에 같이있는 문도포함). 
                                  * 문 열리는 소리. 
                                  * 태그를 'Nothing'으로 바꿔준다. */
                                    InGameController._uniqueInstance.DOORCOUNT--;
                                    selectionRenderer.material = highlightMaterial[2];      
                                    SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_OPEN);

                                    _door[n].tag = "Nothing";
                                    if (n % 2 == 0)
                                    {
                                        _door[n + 1].tag = "Nothing";
                                        _door[n + 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                        _onClick[n + 1] = !_onClick[n + 1];
                                        _curValue[n / 2] = 0.03f;
                                    }
                                    else
                                    {
                                        _door[n - 1].tag = "Nothing";
                                        _door[n - 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                        _onClick[n - 1] = !_onClick[n - 1];
                                        _curValue[n / 2] = 0.03f;
                                    }

                                    selection = null;
                                    break;
                                }
                                else
                                {// close
                                    if (InGameController._uniqueInstance.DOORCOUNT >=
                                         InGameController._uniqueInstance.LIMITDOORCOUNT)
                                    {// 문 닫은 개수가 3보다 크면 더이상 닫을 수 없다고 text 띄워줌.
                                        _onClick[n] = !_onClick[n];
                                        _gameExplain.text = "더이상의 문을 닫을 수 없다..";
                                        return;
                                    }
                                    /* 닫은 문의 개수(한정3개)를 카운트해줌과 동시에, 
                                     * Materiald을 투명하게 바꿈(옆에 같이있는 문도포함). 
                                     * 문 닫히는 소리. 
                                     * 태그를 'Selectable'로 바꿔준다. */
                                    InGameController._uniqueInstance.DOORCOUNT++;
                                    selectionRenderer.material = highlightMaterial[2];
                                    SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_CLOSE);

                                    _door[n].tag = selectableTag[0];
                                    if (n % 2 == 0)
                                    {
                                        _door[n + 1].tag = selectableTag[0];
                                        _door[n + 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                        _onClick[n + 1] = !_onClick[n + 1];
                                        _curValue[n / 2] = 1.0f;
                                    }
                                    else
                                    {
                                        _door[n - 1].tag = selectableTag[0];
                                        _door[n - 1].GetComponent<Renderer>().material = highlightMaterial[2];
                                        _onClick[n - 1] = !_onClick[n - 1];
                                        _curValue[n / 2] = 1.0f;

                                    }
                                    break;
                                }
                            }
                        }
                    }
                    _selection = selection;
                }
                else if (selection.CompareTag(selectableTag[2]))
                {// 상점
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial[2];      // 투명 Material.
                        _gameExplain.text = "텅 빈 옷장이다.";
                    }
                    _selection = selection;
                }
                else if (selection.CompareTag(selectableTag[3]))
                {
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial[4];
                    }

                    if(Input.GetMouseButtonDown(0))
                    {
                        if(selection.gameObject == _item[0])
                        {// 샷건 30
                            if (_timeBought[0])
                            {
                                _gameExplain.text = "이미 소지하고 있다.";
                                return;
                            }

                            if (InGameController._uniqueInstance.CRYSTAL >= 1)
                            {
                                _timeBought[0] = true;
                                _item[0].SetActive(false);
                                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);
                                PlayerController._uniqueInstance.ChangedAction(PlayerController.ePlyAction.RIFLE_IDLE);
                                InGameController._uniqueInstance.CRYSTAL -= 1;

                                AchievementManager.Instance.EarnAchievment("Hunter");
                            }
                            else
                            {
                                _gameExplain.text = "90 크리스탈";
                                return;
                            }
                        }
                        else if(selection.gameObject == _item[1])
                        {// 레코드 50
                            if (_timeBought[1])
                            {
                                _gameExplain.text = "이미 소지하고 있다.";
                                return;
                            }

                            if (InGameController._uniqueInstance.CRYSTAL >= 1)
                            {
                                _timeBought[1] = true;
                                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);
                                InGameController._uniqueInstance.CRYSTAL -= 1;
                                _itemActive[0].SetActive(true);
                                _gameExplain.text = "외부인의 움직임이 둔해진다..";

                                AchievementManager.Instance.EarnAchievment("So Scratchy..");
                            }
                            else
                            {
                                _gameExplain.text = "50 크리스탈";
                                return;                             
                            }
                        }
                        else if(selection.gameObject == _item[2])
                        {// 피아노 200
                            if (_timeBought[2])
                            {
                                _gameExplain.text = "이미 소지하고 있다.";
                                return;
                            }

                            if (InGameController._uniqueInstance.CRYSTAL >= 1)
                            {
                                _timeBought[2] = true;
                                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.BTN);
                                InGameController._uniqueInstance.CRYSTAL -= 1;
                                _itemActive[1].SetActive(true);

                                AchievementManager.Instance.EarnAchievment("Was it working..?");
                            }
                            else
                            {
                                _gameExplain.text = "200 크리스탈";
                                return;
                            }
                        }
                    }
                    _selection = selection;
                }
                else if (selection.CompareTag(selectableTag[4]))
                {
                    var selectionRenderer = selection.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        selectionRenderer.material = highlightMaterial[4];
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        if(InGameController._uniqueInstance.CRYSTAL >= 5)
                        {
                            for (int n = 0; n < _candleLight.Length; n++)
                            {
                                if(selection.gameObject == _candleLight[n])
                                {
                                    SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.MATCHES_SOUND);
                                    InGameController._uniqueInstance.CRYSTAL -= 5;
                                    InGameController._uniqueInstance.CANDLELIGHT[n].range += 1.5f;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            _gameExplain.text = "5 크리스탈";
                            return;
                        }
                    }
                    _selection = selection;
                }
                else if (selection.CompareTag(selectableTag[5]))
                {
                    _gameExplain.text = "잠이나 자고싶다..";
                    _selection = selection;
                }
            }
        }
    }
}
