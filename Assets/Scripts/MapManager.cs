using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> elements;
    List<List<Element>> elementList;
    const int width = 8;
    const int height = 8;
    [SerializeField]
    int matchSize = 3;
    [SerializeField]
    Vector2 itemSize = new Vector2(1, 1);
    Vector2 startPosition;
    void Start()
    {
        elementList = new List<List<Element>>();
        startPosition = transform.position - (transform.localScale / 2);
        for (int i = 0; i < width; ++i)
        {
            elementList.Add(new List<Element>());
            for (int j = 0; j < height; ++j)
            {
                Vector2 instancePosition = itemSize;
                instancePosition.x *= i;
                instancePosition.y *= j;
                instancePosition += startPosition + (itemSize / 2);
                int newElementValue = Random.Range(0, elements.Count);
                GameObject newElementObject = Instantiate(elements[newElementValue], instancePosition, Quaternion.identity);
                Element newElement = newElementObject.GetComponent<Element>();
                elementList[i].Add(newElement);
                newElement.elementValue = newElementValue;
            }
        }
        testScanAlgorithm();
    }

    List<Vector2Int> scanHorLine(int lineIdx)
    {
        List<Vector2Int> scoreList = new List<Vector2Int>();
        List<Vector2Int> scanBuffer = new List<Vector2Int>();
        int pastValue = -1;
        for (int i = 0; i < width; ++i)
        {
            if (pastValue == -1)
            {
                pastValue = elementList[i][lineIdx].elementValue;
                scanBuffer.Add(new Vector2Int(i, lineIdx));
            }
            else if (pastValue == elementList[i][lineIdx].elementValue)
            {
                scanBuffer.Add(new Vector2Int(i, lineIdx));
            }
            else
            {
                pastValue = elementList[i][lineIdx].elementValue;
                if(scanBuffer.Count >= matchSize)
                {
                    foreach (Vector2Int scanElement in scanBuffer)
                        scoreList.Add(scanElement);
                }
                scanBuffer.Clear();
            }
        }
        return scoreList;
    }

    List<Vector2Int> scanVerLine(int lineIdx)
    {
        List<Vector2Int> scoreList = new List<Vector2Int>();
        List<Vector2Int> scanBuffer = new List<Vector2Int>();
        int pastValue = -1;
        for (int i = 0; i < height; ++i)
        {
            if (pastValue == -1)
            {
                pastValue = elementList[lineIdx][i].elementValue;
                scanBuffer.Add(new Vector2Int(lineIdx, i));
            }
            else if (pastValue == elementList[lineIdx][i].elementValue)
            {
                scanBuffer.Add(new Vector2Int(lineIdx, i));
            }
            else
            {
                pastValue = elementList[i][lineIdx].elementValue;
                if(scanBuffer.Count >= matchSize)
                {
                    foreach (Vector2Int scanElement in scanBuffer)
                        scoreList.Add(scanElement);
                }
                scanBuffer.Clear();
            }
        }
        return scoreList;
    }

    Dictionary<Vector2Int, bool> scanWholeMap()
    {
        Dictionary<Vector2Int, bool> scoreElementList = new Dictionary<Vector2Int, bool>();
        for (int i = 0; i < width; ++i)
        {
            List<Vector2Int> lineScoreList = scanHorLine(i);
            foreach (Vector2Int scoreElement in lineScoreList)
                scoreElementList[scoreElement] = true;
        }
        for (int i = 0; i < height; ++i)
        {
            List<Vector2Int> lineScoreList = scanVerLine(i);
            foreach (Vector2Int scoreElement in lineScoreList)
                scoreElementList[scoreElement] = true;
        }
        return scoreElementList;
    }

    /// Debug
    void testScanAlgorithm()
    {
        Dictionary<Vector2Int, bool> scanResult = scanWholeMap();
        foreach(Vector2Int element in scanResult.Keys)
        {
            elementList[element.x][element.y].GetComponentInChildren<TextMeshPro>().faceColor = new Color(1, 0, 0);
        }
    }
    /// End Debug

    // Update is called once per frame
    void Update()
    {
        
    }
}
