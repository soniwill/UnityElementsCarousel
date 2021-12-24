using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Element 
{
    private Vector2 _position;
        private Bounds _bounds;
    
        protected Element(Vector2 position)
        {
            _position = position;
        }
}
