using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column
{
   private int _visibleElementsNumber;
   private int _elementsTotalCapacity;
   private float _velocity;
   private Vector2 _movementDirection;

   private List<Cell> _cells;

   public Column(int elementsNumber)
   {
      _elementsTotalCapacity = elementsNumber;
      _cells = new List<Cell>(elementsNumber);
   }
   
}
