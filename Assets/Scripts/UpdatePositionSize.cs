using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSize : MonoBehaviour
{
    private Vector3 _position;
    private bool _hasRectTransform = false;
    private RectTransform _rectTransform;
    private AdjustRelativeToParent _child;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _hasRectTransform = _rectTransform != null;
        _child = transform.childCount>0 ? transform.GetChild(0).GetComponent<AdjustRelativeToParent>(): null;

    }

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            if(_hasRectTransform)
                _rectTransform.anchoredPosition = _position;
            else
                transform.localPosition = _position;
        }
    }

    public void RefreshData(Cell cell)
    {
        
        if (_hasRectTransform)
        {
            _rectTransform.anchoredPosition = cell.Position;
            _rectTransform.sizeDelta = cell.Size;
        }
        else
            transform.localPosition = _position;
        
        if(transform.childCount==0) return;
        
        _child ??= transform.GetChild(0).GetComponent<AdjustRelativeToParent>();
        _child.KeepSizeRelativeToParent();
    }
}
