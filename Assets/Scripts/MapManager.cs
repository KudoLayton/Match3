using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> elements;
    List<List<Element>> elementList;
    const int width = 8;
    const int height = 8;
    [SerializeField]
    Vector2 itemSize = new Vector2(1, 1);
    Vector2 startPosition;
    void Start()
    {
        elementList = new List<List<Element>>();
        startPosition = transform.position - (transform.localScale / 2);
        for (int i = 0; i < height; ++i)
        {
            elementList.Add(new List<Element>());
            for (int j = 0; j < width; ++j)
            {
                Vector2 instancePosition = itemSize;
                instancePosition.x *= j;
                instancePosition.y *= i;
                instancePosition += startPosition + (itemSize / 2);
                int newElementValue = Random.Range(0, elements.Count);
                GameObject newElementObject = Instantiate(elements[newElementValue], instancePosition, Quaternion.identity);
                Element newElement = newElementObject.GetComponent<Element>();
                elementList[i].Add(newElement);
                newElement.elementValue = newElementValue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
