using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(101)]
[RequireComponent(typeof(CustomLayoutGroup))]
public class RectChildrenManager : MonoBehaviour
{
    
    private CustomLayoutGroup _customLayoutGroup;
    private RectTransform _rectTransform;
    private Rect containerRect;

    public List<Rect> rectList;
    // Start is called before the first frame update
    void Start()
    {
        rectList = new List<Rect>();
        _customLayoutGroup = GetComponent<CustomLayoutGroup>();
        
        _customLayoutGroup.OnDetectNewChildren += UpdateRectList;
        _rectTransform = GetComponent<RectTransform>();
        containerRect = _rectTransform.GetWorldRect();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //only called when a child is added or removed.
    private void UpdateRectList(IEnumerable<Transform> transforms)
    {
        // I don't need and it's not wise to mess with transforms collection once I know that the CustomLayoutGroupColumnConfig
        // is handling some setting with it.

        // what I want is the rect of each MovableElement. So:
        var MovableElements = GetComponentsInChildren<MovableElement>();
        
        rectList.Clear();
        foreach (var element in MovableElements)
        {
            element.SetContainerRect(containerRect);
            // var rectTransform = element.GetComponent<RectTransform>();
            // if(!rectTransform) continue;
            // rectList.Add(rectTransform.GetWorldRect());
        }
    }
}
