using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class AdjustSpriteRelativeToParent : AdjustRelativeToParent
{
    [SerializeField, HideInInspector] RectTransform _myRectTransform;
    [SerializeField, HideInInspector] RectTransform _parentRectTransf;
    [SerializeField, HideInInspector] SpriteRenderer _mySpriteRenderer;

    private void Reset() {
        Start();
    }

    private void Start() {
        _parentRectTransf = transform.parent as RectTransform;
        _mySpriteRenderer = GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        _myRectTransform = transform as RectTransform;
    }

    private void Update() {
 
#if UNITY_EDITOR
        //if we are in execute in edit mode, in editor, we might not have some references yet.
        //so establish them if needed:
        if(_mySpriteRenderer == null  ||  _parentRectTransf == null  || _myRectTransform == null) {
            Start();
        }
#endif
    }
    
    public override void KeepSizeRelativeToParent()
    {
        var pxWidth = _parentRectTransf.rect.width;            //width  of the scaled UI-Object in pixel
        var pxHeight = _parentRectTransf.rect.height;        //height of the scaled UI-Object in pixel
 
        if (float.IsNaN(pxHeight)  ||  float.IsNaN(pxWidth)) {
            //unity hasn't not yet initialized (usually happens during start of the game)
            return;
        }
 
        float spriteSizeX = 1;
        float spriteSizeY = 1;
 
        if (_mySpriteRenderer) {
            spriteSizeX = _mySpriteRenderer.sprite.bounds.size.x;  //width  of the unscaled sprite in pixel
            spriteSizeY = _mySpriteRenderer.sprite.bounds.size.y;  //height of the unscaled sprite in pixel
        }
 
        //scale that needs to be applied to the SpriteRender-Gameobject to give it the same screen size as the UI-Image
        var scaleX = pxWidth / spriteSizeX;
        var scaleY = pxHeight / spriteSizeY;
 
       
        //create new SpriteGameObject in UI
        this.gameObject.layer = LayerMask.NameToLayer("UI");            //culling layer, if needed
        _myRectTransform.localScale = new Vector3(scaleX, scaleY, 1F);    //scale our SpriteRenderer
 
 
        //position self in the middle of parent
        _myRectTransform.anchoredPosition3D = new Vector3(0,0, _myRectTransform.anchoredPosition3D.z);
        _myRectTransform.anchorMin = Vector2.one * 0.5f;
        _myRectTransform.anchorMax = Vector2.one * 0.5f;
 
        var myLocalScale_x = Mathf.Max(0.0001f, _myRectTransform.localScale.x);
        var myLocalScale_y = Mathf.Max(0.0001f, _myRectTransform.localScale.y);
 
        _myRectTransform.sizeDelta = new Vector2( Mathf.Max(0.0001f, _parentRectTransf.rect.width / myLocalScale_x),
            Mathf.Max(0.0001f, _parentRectTransf.rect.height / myLocalScale_y) );
    }
}