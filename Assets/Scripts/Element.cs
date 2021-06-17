using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    bool isUnsetId = true;
    int _id;
    public int id{
        get => _id;
        set
        {
            if (isUnsetId)
                _id = value;
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
