using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public Vector2Int id
    {
        get;
        set;
    }

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

    public MapManager mapManager
    {
        set; private get;
    }

    void Start()
    {
    }

    public IEnumerator moveTarget(Vector3 target, float moveTime)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 displacement = target - startPosition;
        while(elapsedTime <= moveTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            transform.position = displacement * (1 - Mathf.Pow((1 - (elapsedTime / moveTime)), 5)) + startPosition;
        }
        transform.position = target;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<BoxCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                Debug.Log($"{elementValue}: ({id.x}, {id.y})");
                mapManager.clickElement(id);
            }
        }
    }
}
