using UnityEngine;

public class BoxContainer : Container
{
    private Bounds _bounds;
    private Vector2 _position;

    public BoxContainer(Vector2 position, Vector2 size)
    {
        _position = position;
        _bounds = new Bounds(position, size);
    }
    
    public override bool CheckIfElementIsVisible(Element element)
    {
        var elementBounds = element.Bounds;
        var max = elementBounds.max;
        var min = elementBounds.min;
        
        
        Debug.Log($"element max corner: {max} element min corner: {min}");

        return _bounds.Intersects(elementBounds);
    }
}