using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element 
{
    private Vector2 _position;
    private Bounds _bounds;
    
    public Element(Vector2 position, Vector2 size)
    {
        _position = position;
        _bounds = new Bounds(position, size);
        
        Debug.Log($"max: {_bounds.max} min: {_bounds.min}");
    }

    public Vector2 Size => _bounds.size;

    public Vector2 Position
    {
        get
        {
            return _position;
        }

        set
        {
            if(_position==value) return;

            _position = value;
            _bounds.center = _position;
        }
    }

    public Bounds Bounds => _bounds;


}
