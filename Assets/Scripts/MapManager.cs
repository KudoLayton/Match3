using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> elements;
    const int width = 8;
    const int height = 8;
    [SerializeField]
    Vector2 itemSize = new Vector2(1, 1);
    Vector2 startPosition;
    void Start()
    {
        startPosition = transform.position - (transform.localScale / 2);
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                Vector2 instancePosition = itemSize;
                instancePosition.x *= j;
                instancePosition.y *= i;
                instancePosition += startPosition + (itemSize / 2);
                Instantiate(elements[Random.Range(0, elements.Count)], instancePosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
