using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManger : MonoBehaviour
{
    [SerializeField] string[] selectableTag;
    [SerializeField] Material[] highlightMaterial;
    [SerializeField] Material[] defaultMaterial;
    [SerializeField] GameObject[] _door;

    Transform _selection;
    bool[] _onClick;

    void Awake()
    {
        _onClick = new bool[_door.Length];    
    }

    // Update is called once per frame
    void Update()
    {
        if(_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();

            if (_selection.CompareTag("Selectable"))
            {
                for (int n = 0; n < _door.Length; n++)
                {
                    if (_selection.gameObject == _door[n])
                    {
                        if (_onClick[n])
                        {
                            selectionRenderer.material = highlightMaterial[2];
                            _door[n].tag = "Nothing";
                            break;
                        }
                        else
                        {
                            selectionRenderer.material = defaultMaterial[0];
                            _door[n].tag = selectableTag[0];
                            break;
                        }
                    }
                }
            }
            else if (_selection.CompareTag("SelectableShop"))
                selectionRenderer.material = defaultMaterial[1];

            _selection = null;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1.5f))
        {// 카메라 레이케스트 거리가 1.5f 내에 있으면 발생..
            var selection = hit.transform;
            if(selection.CompareTag(selectableTag[0]))
            {// Tag가 selectableTag일 때 Material을 바꿔준다.
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    for (int n = 0; n < _door.Length; n++)
                    {
                        if (selection.gameObject == _door[n])
                        {
                            if (_onClick[n])
                            {
                                selectionRenderer.material = highlightMaterial[3];      
                                break;
                            }
                            else
                            {
                                selectionRenderer.material = highlightMaterial[0];      
                                break;
                            }
                        }
                    }
                }

                if(Input.GetMouseButtonDown(0))
                {// 왼쪽 마우스 버튼이 눌렸을 때
                    for(int n = 0; n < _door.Length; n++)
                    {
                        if(selection.gameObject == _door[n])
                        {
                            _onClick[n] = !_onClick[n];
                            Debug.Log(_onClick[n]);
                            if (_onClick[n])
                            {
                                selectionRenderer.material = highlightMaterial[2];      
                                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_OPEN);
                                selection = null;
                                break;
                            }
                            else
                            {
                                selectionRenderer.material = highlightMaterial[2];
                                SoundManager._uniqueInstance.PlayEffSound(SoundManager.eEffType.DOOR_CLOSE);
                                break;
                            }
                        }
                    }
                }
                _selection = selection;
            }
            else if(selection.CompareTag(selectableTag[1]))
            {
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial[2];      // 투명 Material.
                }
                _selection = selection;
            }
        }
    }
}
