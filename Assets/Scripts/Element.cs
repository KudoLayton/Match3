using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    bool haveElementValueSet = false;
    int _elementValue;
    public int elementValue
    {
        get => _elementValue;
        set
        {
            if (!haveElementValueSet)
            {
                _elementValue = value;
                haveElementValueSet = true;
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
