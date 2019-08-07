using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManger : MonoBehaviour
{
    [SerializeField] string[] selectableTag;
    [SerializeField] Material[] highlightMaterial;
    [SerializeField] Material[] defaultMaterial;

    Transform _selection;

    // Update is called once per frame
    void Update()
    {
        if(_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();

            if (_selection.CompareTag("Selectable"))
                selectionRenderer.material = defaultMaterial[0];
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
                    selectionRenderer.material = highlightMaterial[0];      // 기본 Material.
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
