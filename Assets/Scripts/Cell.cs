using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : Box2D //todo: talvez herdar de boxGrid tratando como um caso especial onde coluna e linha são sempre igual a 1;
{
   
    //private Vector2 _position
    public Cell(Vector2 position, Vector2 size) : base(position, size)
    { }

    public Vector2 Size => Rect.size;

    public override Vector2 Position
    {
        get => base.Position;

        set
        {
            if(base.Position==value) return;

            base.Position = value;
            var rect = new Rect(Rect) {center = base.Position};
            Rect = rect;
        }
    }

    //public Rect Rect => _rect;


    public override bool CheckIfElementIsVisible(Cell cell)
    {
        throw new System.NotImplementedException();
    }

    public override bool CheckIfElementIsFullyVisible(Cell cell)
    {
        throw new System.NotImplementedException();
    }
}
