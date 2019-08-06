using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum ePlyAction
    {
        IDLE_STANDING,
        IDLE_AROUND,
        WALK,
        RUN,
        JUMP,
        DIE
    }

    public static PlayerController _uniqueInstance;

    // [SerializeField] FixedTouchField TouchField;
    // protected float CameraAngle;
    // protected float CameraAngleSpeed = 0.2f;
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float runSpeed = 5.0f;
    public float _rotateSpeed = 180;
    float MoveX;
    float MoveZ;
    bool _isDead;

    Animator _aniCtrl;
    ePlyAction _curAction;
    Vector3 _mov;

    float _timeCheck;

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

            // Vector3 mov = new Vector3(MoveX, 0, MoveZ);
            // mov = (mov.magnitude > 1) ? mov.normalized : mov;
          
            transform.Rotate(Vector3.up * MoveX * _rotateSpeed * Time.deltaTime);
            
            if (MoveX == 0 && MoveZ == 0)
            {// wsad값이 없는경우
                if(MoveX != 0)
                {// 회전ad값이 있는경우     =>  걷는 모션;
                    _timeCheck = 0;
                    ChangedAction(ePlyAction.WALK);
                }
                else
                {// 회전ad값이 없는경우     =>  가만히 있는 모션;
                    _timeCheck += Time.deltaTime;
                    if(_timeCheck >= 2.5f)
                        ChangedAction(ePlyAction.IDLE_AROUND);
                    else
                        ChangedAction(ePlyAction.IDLE_STANDING);

                }
            }
            else
            {// ws값이 있는경우
                _timeCheck = 0;
                if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.JUMP);
                    }
                    else
                    {// shift키와 w키가 눌렸을 때       =>  달리는 모션;
                        ChangedAction(ePlyAction.RUN);                    
                    }
                }
                else
                { // 걷는모션;
                    if (Input.GetKey(KeyCode.Space))
                    {
                        ChangedAction(ePlyAction.JUMP);
                    }
                    else
                    {// shift키와 w키가 눌렸을 때       =>  달리는 모션;
                        ChangedAction(ePlyAction.WALK);
                    }        
                }
            }
          
        }
    }

    void ChangedAction(ePlyAction state)
    {
        if (_isDead)
            return;

        switch(state)
        {
            case ePlyAction.WALK:
                transform.Translate(_mov * MoveZ * walkSpeed * Time.deltaTime);
                break;
            case ePlyAction.RUN:
                transform.Translate(_mov * MoveZ * runSpeed * Time.deltaTime);
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
            _mov = Vector3.zero;
        else
            _mov = Vector3.forward;
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
