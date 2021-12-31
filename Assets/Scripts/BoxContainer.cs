using UnityEngine;

public class BoxContainer : Container
{
    
    public struct BoxContainerSettings
    {
        public Vector2 Position;
        public Vector2 Size;
        public int RowNumber;
        public int ColumnNumber; 
    }
    
    
    private Bounds _bounds;
    private Vector2 _position;

    public BoxContainer(Vector2 position, Vector2 size)
    {
        _position = position;
        _bounds = new Bounds(position, size);
    }
    
    public BoxContainer(BoxContainerSettings settings)
    {
        _position = settings.Position;
        var size = settings.Size;
        _bounds = new Bounds(_position, size);
    }
    
    public override bool CheckIfElementIsVisible(Element element)
    {
        var elementBounds = element.Bounds;
        var max = elementBounds.max;
        var min = elementBounds.min;
        
        
        Debug.Log($"element max corner: {max} element min corner: {min}");

        return _bounds.Intersects(elementBounds);
    }

    public override bool CheckIfElementIsFullyVisible(Element element)
    {
        var elementBounds = element.Bounds;
        var point0 = elementBounds.max;
        var point1 = point0 + Vector3.down * elementBounds.size.y;
        var point2 = elementBounds.min;
        var point3 = point2 + Vector3.up * elementBounds.size.y;

        return _bounds.Contains(point0) && _bounds.Contains(point1) && _bounds.Contains(point2) &&
               _bounds.Contains(point3);
        

    }

    // public 
}