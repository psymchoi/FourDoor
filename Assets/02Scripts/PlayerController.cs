using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ePlyAction
    {
        NOTHING_IDLE_STANDING,
        NOTHING_IDLE_AROUND,
        NOTHING_WALK,
        NOTHING_RUN,
        NOTHING_JUMP,
        EQUIP_LANTERN,
        LANTERN_IDLE_STANDING,
        LANTERN_IDLE_AROUND,
        LANTERN_WALK,
        LANTERN_RUN,
        LANTERN_JUMP,
        DIE
    }

    public static PlayerController _uniqueInstance;

    [SerializeField] GameObject _lantern;
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float runSpeed = 5.0f;
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform player, playerArms;

    Animator _aniCtrl;
    ePlyAction _curAction;
    Vector3 _mov;

    float MoveX;
    float MoveZ;
    float _timeCheck;
    bool _isDead;
    bool _equipLantern;


    public ePlyAction PLAYERACTION
    {
        get { return _curAction; }
        set { _curAction = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        _aniCtrl = GetComponent<Animator>();

        _lantern.SetActive(false);
        _equipLantern = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
            return;

        if (InGameController._uniqueInstance.NOWGAMESTATE == InGameController.eGameState.PLAY)
        {
            MoveX = Input.GetAxis("Horizontal");      // a, d
            MoveZ = Input.GetAxis("Vertical");        // w, s          
            
            _mov = new Vector3(MoveX, 0, MoveZ);
            _mov = (_mov.magnitude > 1) ? _mov.normalized : _mov;

            PlayerAniAction();

            Cursor.lockState = CursorLockMode.Locked;
            RotateCamera();  
        }
    }

    /// <summary>
    /// 플레이어 Animations 동작..
    /// </summary>
    public void PlayerAniAction()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {// 숫자 1을 눌렀을 때..
            _equipLantern = !_equipLantern;
            if (_equipLantern)
            {// 런턴 ON
                _timeCheck = 0;
                _lantern.SetActive(true);
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.LANTERN_ON);
            }
            else
            {// 랜턴 OFF
                _timeCheck = 0;
                _lantern.SetActive(false);
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.LANTERN_OFF);
            }
        }

        if (_equipLantern)
        {// 랜턴을 들 때..
            _timeCheck += Time.deltaTime;
            if (MoveX == 0 && MoveZ == 0)
            {// wsad값이 없는경우
                if (MoveX != 0)
                {// 회전ad값이 있는경우     =>  걷는 모션
                    if (Input.GetKeyDown(KeyCode.Space))
                    {// 점프
                        ChangedAction(ePlyAction.LANTERN_JUMP);
                    }
                    else
                    {// 걷기
                        ChangedAction(ePlyAction.LANTERN_WALK);
                    }
                }
                else
                {// 회전ad값이 없는경우     =>  가만히 있는 모션
                    if (Input.GetKeyDown(KeyCode.Space))
                    {// 점프
                        ChangedAction(ePlyAction.LANTERN_JUMP);
                    }
                    else
                    {
                        if (_timeCheck >= 0.2f)
                            ChangedAction(ePlyAction.LANTERN_IDLE_STANDING);
                        else
                            ChangedAction(ePlyAction.EQUIP_LANTERN);
                    }
                }
            }
            else
            {// ws값이 있는경우
                if (Input.GetKey(KeyCode.LeftShift))
                {// shift키와 w키가 눌렸을 때
                    if (Input.GetKeyDown(KeyCode.Space))
                    {// 점프
                        ChangedAction(ePlyAction.LANTERN_JUMP);
                    }
                    else
                    {// 달리기 및 랜턴장착모션
                        if (_timeCheck >= 0.2f)
                            ChangedAction(ePlyAction.LANTERN_RUN);
                        else
                            ChangedAction(ePlyAction.EQUIP_LANTERN);
                    }
                }
                else
                {// 안달리고 있을 때
                    if (Input.GetKeyDown(KeyCode.Space))
                    {// 점프
                        ChangedAction(ePlyAction.LANTERN_JUMP);
                    }
                    else
                    {// 걷기 및 랜턴장착모션
                        if (_timeCheck >= 0.2f)
                            ChangedAction(ePlyAction.LANTERN_WALK);
                        else
                            ChangedAction(ePlyAction.EQUIP_LANTERN);
                    }
                }
            }
        }
        else
        {// 랜턴을 안들때..
            if (MoveX == 0 && MoveZ == 0)
            {// wsad값이 없는경우
                if (MoveX != 0)
                {// 회전ad값이 있는경우     =>  걷는 모션
                    _timeCheck = 0;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.LANTERN_JUMP);
                    }
                    else
                    {
                        ChangedAction(ePlyAction.NOTHING_WALK);
                    }
                }
                else
                {// 회전ad값이 없는경우     =>  가만히 있는 모션
                    _timeCheck += Time.deltaTime;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.NOTHING_JUMP);
                    }
                    else
                    {
                        if (_timeCheck >= 2.5f)
                            ChangedAction(ePlyAction.NOTHING_IDLE_AROUND);
                        else
                            ChangedAction(ePlyAction.NOTHING_IDLE_STANDING);
                    }
                }
            }
            else
            {// ws값이 있는경우
                _timeCheck = 0;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.NOTHING_JUMP);
                    }
                    else
                    {// shift키와 w키가 눌렸을 때       =>  달리는 모션;
                        ChangedAction(ePlyAction.NOTHING_RUN);
                    }
                }
                else
                { // 걷는모션;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.NOTHING_JUMP);
                    }
                    else
                    {// shift키와 w키가 눌렸을 때       =>  달리는 모션;
                        ChangedAction(ePlyAction.NOTHING_WALK);
                    }
                }
            }
        }
    }

    void RotateCamera()
    {
        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = _mouseX * mouseSensitivity;
        float rotAmountY = _mouseY * mouseSensitivity;

        Vector3 rotPlayeArms = playerArms.transform.rotation.eulerAngles;
        Vector3 rotPlayer = player.transform.rotation.eulerAngles;

        rotPlayeArms.x -= rotAmountY;
        rotPlayeArms.z = 0;
        rotPlayer.y += rotAmountX;

        playerArms.rotation = Quaternion.Euler(rotPlayeArms);
        player.rotation = Quaternion.Euler(rotPlayer);
    }

    void ChangedAction(ePlyAction state)
    {
        if (_isDead)
            return;

        switch(state)
        {
            case ePlyAction.NOTHING_WALK:
                transform.Translate(_mov * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.NOTHING_RUN:
                transform.Translate(_mov * runSpeed * Time.deltaTime);
                break;
            case ePlyAction.LANTERN_WALK:
                transform.Translate(_mov * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.LANTERN_RUN:
                transform.Translate(_mov * runSpeed * Time.deltaTime);
                break;
            case ePlyAction.DIE:
                _isDead = true;
                _aniCtrl.SetTrigger("Death");
                break;
            default:
                break;
        }
        _aniCtrl.SetInteger("AniState", (int)state);
        _curAction = state;
    }

    /// <summary>
    /// 벽에 부딪힐 시 뚫고 나가지않게하기 위해서..
    /// </summary>
    /// <param name="Collider"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Selectable"))
        {
            _mov = Vector3.zero;
        }
        else
        {
            // _mov = Vector3.forward;
           // _mov = new Vector3(MoveX, 0, MoveZ);
           //_mov = (_mov.magnitude > 1) ? _mov.normalized : _mov;
        }
    }
}


// *JoyStick Move* in Update()
//CameraAngle += TouchField.TouchDist.x * CameraAngleSpeed;

//Camera.main.transform.position = transform.position + Quaternion.AngleAxis(CameraAngle, Vector3.up) * new Vector3(0, 2.0f, -1.5f);
//Camera.main.transform.rotation = transform.rotation
//    = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);

//if(Joystick._uniqueInstance.MOVE)
//{
//    _timeCheck = 0;
//    ChangedAction(_curAction);
//}
//else
//{
//    _timeCheck += Time.deltaTime;
//    if(_timeCheck >= 1.5f)
//    {
//        ChangedAction(ePlyAction.IDLE_AROUND);
//    }
//    else
//    {
//        ChangedAction(ePlyAction.IDLE_STANDING);
//    }
//}
