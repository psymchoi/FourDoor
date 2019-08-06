using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManger : MonoBehaviour
{
    [SerializeField] string selectableTag = "Selectable";
    [SerializeField] Material highlightMaterial;
    [SerializeField] Material defaultMaterial;

    Transform _selection;

    // Update is called once per frame
    void Update()
    {
        if(_selection != null)
        {
            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1.5f))
        {
            var selection = hit.transform;
            if(selection.CompareTag(selectableTag))
            {// Tag가 selectableTag일 때 메테리얼을 바꿔준다.
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                    selectionRenderer.material = highlightMaterial;

                _selection = selection;
            }
        }
    }
}
