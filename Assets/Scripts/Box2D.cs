using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Box2D
{
    private Rect _rect;
    private Vector2 _position;
    protected Box2D(Vector2 position, Vector2 size)
    {
        _position = position;
        
        //_position = position;
        var rectX = position.x - size.x / 2;
        var rectY = position.y - size.y / 2;
        var rectPos = new Vector2(rectX, rectY);
        _rect = new Rect(rectPos, size);
        //_rect = new Rect(position, size);
    }

    public virtual Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public virtual Rect Rect
    {
        get => _rect;
        set => _rect = value;
    }

    public abstract bool CheckIfElementIsVisible(Cell cell);
    public abstract bool CheckIfElementIsFullyVisible(Cell cell);
}

public static class GridUtility
{
    public static Bounds RectTransformToBounds(RectTransform rectTransform)
    {
        Rect teste;
        //teste.Overlaps()
        var min = Vector2.positiveInfinity;
        var max = Vector2.negativeInfinity;
        if (rectTransform == null) return new Bounds();

        var boundsCorners = new Vector3[4];
        
        
        rectTransform.GetWorldCorners(boundsCorners);

        foreach (var boundsCorner in boundsCorners)
        {
            min = Vector2.Min(min, boundsCorner);
            max = Vector2.Max(max, boundsCorner);
        }

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }
    
}