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
        //EQUIP_LANTERN,
        LANTERN_IDLE_STANDING,
        LANTERN_IDLE_AROUND,
        LANTERN_WALK,
        LANTERN_RUN,
        LANTERN_JUMP,
        RIFLE_IDLE,
        RIFLE_WALK,
        RIFLE_RUN,
        RIFLE_FIRE,
        DIE
    }

    public static PlayerController _uniqueInstance;

    [SerializeField] GameObject _lantern;
    [SerializeField] GameObject _shotgun;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform player, playerArms;

    Animator _aniCtrl;
    ePlyAction _curAction;
    Vector3 _mov;

    float MoveX;
    float MoveZ;
    float xAxisClamp = 0;
    float _timeCheck;
    bool _isDead;
    bool _equipLantern;
    bool _equipShotgun;


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
        _shotgun.SetActive(false);
        _equipLantern = false;
        _equipShotgun = false;
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
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {// 랜턴 들기 = 1번 키
            if (_equipShotgun)
                _equipShotgun = !_equipShotgun;

            _equipLantern = !_equipLantern;
            if(_equipLantern)
            {// 랜턴 ON
                _lantern.SetActive(true);
                _shotgun.SetActive(false);
                _aniCtrl.SetTrigger("HOLD_LANTERN");
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.LANTERN_ON);
            }
            else
            {// 랜턴 OFF
                _lantern.SetActive(false);
                _aniCtrl.SetTrigger("NOTHING_HOLD");
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.LANTERN_OFF);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {// 샷건 들기 = 2번 키
            if(!SelectionManger._uniqueInstance.BOUGHT)
            {// 샷건 구입 여부(x)
                return;
            }
            else
            {// 샷건 구입 여부(o)
                if (_equipLantern)
                {
                    _equipLantern = !_equipLantern;
                    SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.LANTERN_OFF);
                }

                _equipShotgun = !_equipShotgun;
                if(_equipShotgun)
                {// 샷건 ON
                    _shotgun.SetActive(true);
                    _lantern.SetActive(false);
                    ChangedAction(ePlyAction.RIFLE_IDLE);
                    SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.SHOTGUN_EQUIP);
                }
                else
                {// 샷건 OFF
                    _shotgun.SetActive(false);
                    _aniCtrl.SetTrigger("NOTHING_HOLD");
                }
            }
        }

        if(_equipLantern)
        {// 랜턴 들고 있을 때 행동.
            if(MoveX == 0 && MoveZ == 0)
            {// 아무 이동이 없을 경우.
                if(MoveX != 0)
                {
                    ChangedAction(ePlyAction.LANTERN_WALK);
                }
                else
                {
                    ChangedAction(ePlyAction.LANTERN_IDLE_STANDING);
                }
            }
            else
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {// Space = 달리기
                    ChangedAction(ePlyAction.LANTERN_RUN);
                }
                else
                {
                    ChangedAction(ePlyAction.LANTERN_WALK);
                }
            }
        }
        else if(_equipShotgun)
        {// 샷건 들고 있을 때 행동.
            if(Input.GetMouseButtonDown(0))
            {
                ChangedAction(ePlyAction.RIFLE_FIRE);
                return;
            }

            if (MoveX == 0 && MoveZ == 0)
            {// 아무 이동이 없을 경우.
                if (MoveX != 0)
                {
                    ChangedAction(ePlyAction.RIFLE_WALK);
                }
                else
                {
                    ChangedAction(ePlyAction.RIFLE_IDLE);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {// Space = 달리기
                    ChangedAction(ePlyAction.RIFLE_RUN);
                }
                else
                {
                    ChangedAction(ePlyAction.RIFLE_WALK);
                }
            }
        }
        else if(!_equipLantern && !_equipShotgun)
        {// 아무것도 들고 있지 않을 때 행동.
            if (MoveX == 0 && MoveZ == 0)
            {// 아무 이동이 없을 경우.
                if (MoveX != 0)
                {
                    ChangedAction(ePlyAction.NOTHING_WALK);
                }
                else
                {
                    ChangedAction(ePlyAction.NOTHING_IDLE_STANDING);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {// Space = 달리기
                    ChangedAction(ePlyAction.NOTHING_RUN);
                }
                else
                {
                    ChangedAction(ePlyAction.NOTHING_WALK);
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

        xAxisClamp -= rotAmountY;

        Vector3 rotPlayeArms = playerArms.transform.rotation.eulerAngles;
        Vector3 rotPlayer = player.transform.rotation.eulerAngles;

        rotPlayeArms.x -= rotAmountY;
        rotPlayeArms.z = 0;
        rotPlayer.y += rotAmountX;

        if(xAxisClamp > 90)
        {
            xAxisClamp = 90;
            rotPlayeArms.x = 90;
        }
        else if(xAxisClamp < -90)
        {
            xAxisClamp = -90;
            rotPlayeArms.x = 270;
        }

        playerArms.rotation = Quaternion.Euler(rotPlayeArms);
        player.rotation = Quaternion.Euler(rotPlayer);
    }

    public void ChangedAction(ePlyAction state)
    {
        if (_isDead)
            return;

        switch(state)
        {
            case ePlyAction.NOTHING_IDLE_STANDING:
                break;
            case ePlyAction.NOTHING_WALK:
                transform.Translate(_mov * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.NOTHING_RUN:
                transform.Translate(_mov * runSpeed * Time.deltaTime);
                break;
            case ePlyAction.LANTERN_IDLE_STANDING:
                break;
            case ePlyAction.LANTERN_WALK:
                transform.Translate(_mov * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.LANTERN_RUN:
                transform.Translate(_mov * runSpeed * Time.deltaTime);
                break;
            case ePlyAction.RIFLE_IDLE:
                break;
            case ePlyAction.RIFLE_WALK:
                transform.Translate(_mov * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.RIFLE_RUN:
                transform.Translate(_mov * runSpeed * Time.deltaTime);
                break;
            case ePlyAction.RIFLE_FIRE:
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.SHOTGUN_FIRE);
                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.SHOTGUN_RELOADING);
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
        //else
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
