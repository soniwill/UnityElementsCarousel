using UnityEngine;

public class BoxGrid : Box2D
{
    
    public struct BoxContainerSettings
    {
        public Vector2 Position;
        public Vector2 Size;
        public int RowNumber;
        public int ColumnNumber; 
    }
    
    
    // private Rect _rect;
    // private Vector2 _position;
    //private 

    public BoxGrid(Vector2 position, Vector2 size) : base(position, size)
    { }

    public BoxGrid(BoxContainerSettings settings) : base(settings.Position, settings.Size)
    {
        // todo: configurar coluna e linhas com os valores que vem da var settings
    }
    
    public override bool CheckIfElementIsVisible(Cell cell)
    {
        var elementRect = cell.Rect;
        var max = elementRect.max;
        var min = elementRect.min;
        
        
        Debug.Log($"element max corner: {max} element min corner: {min}");

        return Rect.Overlaps(elementRect);
    }

    public override bool CheckIfElementIsFullyVisible(Cell cell)
    {

        var r = Rect;
        var a = cell.Rect;
        return r.xMin <= a.xMin && r.yMin <= a.yMin && r.xMax >= a.xMax && r.yMax >= a.yMax;
        // var elementRect = cell.Rect;
        // var point0 = elementRect.max;
        // var point1 = point0 + Vector2.down * elementRect.size.y;
        // var point2 = elementRect.min;
        // var point3 = point2 + Vector2.up * elementRect.size.y;
        //
        // return _rect.Contains(point0) && _rect.Contains(point1) && _rect.Contains(point2) &&
        //        _rect.Contains(point3);
    }

    public void AddColumn()
    {
        
    }

    public void UpdateGrid()
    {
        
    }
}