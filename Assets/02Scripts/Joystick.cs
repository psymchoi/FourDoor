using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
    public static Joystick _uniqueInstance;

    public Transform player;
    public Transform stick;

    GameObject _posPlayerStart;

    Vector3 stickFirstPos;      // 조이스틱의 처음위치
    Vector3 joyVec;             // 조이스틱의 벡터(방향)
    float radius;               // 조이스틱 배경의 반지름
    bool moveFlag;              // 플레이어 움직임 스이치

    public bool MOVE
    {
        get { return moveFlag; }
        set { moveFlag = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _uniqueInstance = this;
        radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        stickFirstPos = stick.transform.position;

        float can = transform.parent.GetComponent<RectTransform>().localScale.x;
        radius = radius * can;

        moveFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveFlag)
        {
            //player.transform.Translate(Vector3.forward * Time.deltaTime * 5);
        }
    }

    public void Drag(BaseEventData _Data)
    {
        moveFlag = true;
        PointerEventData data = _Data as PointerEventData;
        Vector3 pos = data.position;

        // 조이스틱을 이동시킬 방향을 구한다. (상하좌우)
        joyVec = (pos - stickFirstPos).normalized;

        // 조이스틱의 처음 위치와 현재 내가 터치하고 있는 위치의 거리를 구함.
        float dis = Vector3.Distance(pos, stickFirstPos);

        if (dis < radius)
        {
            stick.position = stickFirstPos + joyVec * dis;
            PlayerController._uniqueInstance.PLAYERACTION = PlayerController.ePlyAction.LANTERN_WALK;
        }
        else
        {
            stick.position = stickFirstPos + joyVec * radius;
            PlayerController._uniqueInstance.PLAYERACTION = PlayerController.ePlyAction.LANTERN_RUN;
        }
    }

    public void DragEnd()
    {
        stick.position = stickFirstPos;     // 스틱을 원래 위치로.
        joyVec = Vector3.zero;
        moveFlag = false;
    }
}
