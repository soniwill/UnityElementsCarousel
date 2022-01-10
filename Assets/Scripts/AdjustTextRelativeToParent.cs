using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class AdjustTextRelativeToParent : AdjustRelativeToParent
{
    [SerializeField, HideInInspector] private RectTransform _myRectTransform;
    [SerializeField, HideInInspector] private RectTransform _parentRectTransf;
    [SerializeField, HideInInspector] private Text _text;

    private bool _resizeTextForBestFit;

    private void Start()
    {
        _parentRectTransf = transform.parent as RectTransform;
        _myRectTransform = transform as RectTransform;
        _text = GetComponent<Text>();
    }

    public override void KeepSizeRelativeToParent()
    {
        _myRectTransform.sizeDelta = _parentRectTransf.sizeDelta;
        if(_resizeTextForBestFit == true) return;
        if (_text)  _text.resizeTextForBestFit = _resizeTextForBestFit = true;
    }
}