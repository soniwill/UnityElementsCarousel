using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingDigit : MonoBehaviour
{
    [SerializeField] private Queue<char> _nextDigits;

    private  int _value;

    public int Value
    {
        get { return _value; }
        set
        {
            if (_value != value)
            {
                _nextDigits.Enqueue(Convert.ToChar(value));
            }
        }
    }
    
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    

}