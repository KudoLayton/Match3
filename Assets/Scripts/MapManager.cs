using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq;

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
    Vector2 supplementStartPosition;
    [SerializeField]
    float initalWaitTime = 1;
    [SerializeField]
    float removeTime = 1;
    [SerializeField]
    float supplyTime = 0.5f;
    Stack<Vector2Int> clickElementBuffer;
    bool isClickable = true;
    [SerializeField]
    UnityEvent<int, int, int> updateScore;
    int score;
    int maxSpeed;
    Queue<(float, int)> scoreHistory;


    void Start()
    {
        score = 0;
        maxSpeed = 0;
        clickElementBuffer = new Stack<Vector2Int>();
        elementList = new List<List<Element>>();
        startPosition = transform.position - (transform.localScale / 2);
        supplementStartPosition = transform.position;
        supplementStartPosition.x -= transform.localScale.x / 2;
        supplementStartPosition.y += transform.localScale.y / 2;
        scoreHistory = new Queue<(float, int)>();
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
                newElement.id = new Vector2Int(i, j);
                newElement.mapManager = this;
            }
        }
        StartCoroutine(initialCleanMatches());
    }

    enum SwapDirection
    {
        Right,
        Up
    }
    List<(Vector2Int, SwapDirection)> scanScorePossibility()
    {
        List<(Vector2Int, SwapDirection)> scanResult = new List<(Vector2Int, SwapDirection)>();

        for(int i = 0; i < (width - 1); ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                Vector2Int first = new Vector2Int(i, j);
                Vector2Int second = new Vector2Int(i + 1, j);
                swapElement(first, second);
                if (scanWholeMap().Keys.Count > 0)
                    scanResult.Add((first, SwapDirection.Right));
                swapElement(first, second);
            }
        }

        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < (height - 1); ++j)
            {
                Vector2Int first = new Vector2Int(i, j);
                Vector2Int second = new Vector2Int(i, j + 1);
                swapElement(first, second);
                if (scanWholeMap().Keys.Count > 0)
                    scanResult.Add((first, SwapDirection.Up));
                swapElement(first, second);
            }
        }

        return scanResult;
    }

    void resetMap()
    {
        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                Destroy(elementList[i][j].gameObject);
            }
            elementList[i].Clear();
        }
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
                newElement.id = new Vector2Int(i, j);
                newElement.mapManager = this;
            }
        }
        StartCoroutine(initialCleanMatches());
    }

    public void clickElement(Vector2Int targetElement)
    {
        if (isClickable)
        {
            if (clickElementBuffer.Count == 0)
            {
                clickElementBuffer.Push(targetElement);
                elementList[targetElement.x][targetElement.y].activateElement();
            }
            else
            {
                Vector2Int first = clickElementBuffer.Pop();
                Vector2Int second = targetElement;
                elementList[first.x][first.y].deactivateElement();

                if ((first - second).magnitude == 1)
                {
                    swapElement(first, second);
                    Dictionary<Vector2Int, bool> result = scanWholeMap();
                    if (result.Keys.Count > 0)
                    {
                        StartCoroutine(iterateCleanMatches());
                    }
                    else
                    {
                        swapElement(first, second);
                    }
                }
            }
        }
    }

    IEnumerator iterateCleanMatches()
    {
        isClickable = false;
        for(int matchedScore = removeMatchOnce(); matchedScore > 0; matchedScore = removeMatchOnce())
            yield return new WaitForSeconds(removeTime + supplyTime);
        isClickable = true;
        if (scanScorePossibility().Count == 0)
            resetMap();
    }

    IEnumerator initialCleanMatches()
    {
        isClickable = false;
        yield return new WaitForSeconds(initalWaitTime);
        for(int matchedScore = removeMatchOnce(); matchedScore > 0; matchedScore = removeMatchOnce())
            yield return new WaitForSeconds(removeTime + supplyTime);
        isClickable = true;
        if (scanScorePossibility().Count == 0)
            resetMap();
    }

    void swapElement(Vector2Int first, Vector2Int second) 
    {
        elementList[first.x][first.y].id = second;
        elementList[second.x][second.y].id = first;

        Element tempElement = elementList[first.x][first.y];
        elementList[first.x][first.y] = elementList[second.x][second.y];
        elementList[second.x][second.y] = tempElement;

        Vector3 tempPosition = elementList[first.x][first.y].transform.position;
        elementList[first.x][first.y].transform.position = elementList[second.x][second.y].transform.position;
        elementList[second.x][second.y].transform.position = tempPosition;
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
                scanBuffer.Add(new Vector2Int(i, lineIdx));
            }
        }
        if(scanBuffer.Count >= matchSize)
        {
            foreach (Vector2Int scanElement in scanBuffer)
                scoreList.Add(scanElement);
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
                pastValue = elementList[lineIdx][i].elementValue;
                if(scanBuffer.Count >= matchSize)
                {
                    foreach (Vector2Int scanElement in scanBuffer)
                        scoreList.Add(scanElement);
                }
                scanBuffer.Clear();
                scanBuffer.Add(new Vector2Int(lineIdx, i));
            }
        }
        if(scanBuffer.Count >= matchSize)
        {
            foreach (Vector2Int scanElement in scanBuffer)
                scoreList.Add(scanElement);
        }

        return scoreList;
    }

    Dictionary<Vector2Int, bool> scanWholeMap()
    {
        Dictionary<Vector2Int, bool> scoreElementList = new Dictionary<Vector2Int, bool>();
        for (int i = 0; i < width; ++i)
        {
            List<Vector2Int> lineScoreList = scanVerLine(i);
            foreach (Vector2Int scoreElement in lineScoreList)
                scoreElementList[scoreElement] = true;
        }
        for (int i = 0; i < height; ++i)
        {
            List<Vector2Int> lineScoreList = scanHorLine(i);
            foreach (Vector2Int scoreElement in lineScoreList)
                scoreElementList[scoreElement] = true;
        }
        return scoreElementList;
    }

    int removeMatchOnce()
    {
        Dictionary<Vector2Int, bool> scanResult = scanWholeMap();
        Dictionary<int, bool> changedLines = new Dictionary<int, bool>();
        score += scanResult.Keys.Count;
        scoreHistory.Enqueue((Time.time, scanResult.Keys.Count));

        foreach(Vector2Int element in scanResult.Keys)
        {
            StartCoroutine(elementList[element.x][element.y].destroyElement(removeTime));
            elementList[element.x][element.y] = null;
            changedLines[element.x] = true;
        }

        foreach(int lineIdx in changedLines.Keys)
        {
            elementList[lineIdx].RemoveAll((x) => x == null);
            int supplementNum = height - elementList[lineIdx].Count;
            for(int i = 0; i < supplementNum; ++i)
            {
                Vector2 instancePosition = itemSize;
                instancePosition.x *= lineIdx;
                instancePosition.y *= i;
                instancePosition += supplementStartPosition + (itemSize / 2);
                int newElementValue = Random.Range(0, elements.Count);
                GameObject newElementObject = Instantiate(elements[newElementValue], instancePosition, Quaternion.identity);
                Element newElement = newElementObject.GetComponent<Element>();
                elementList[lineIdx].Add(newElement);
                newElement.elementValue = newElementValue;
                newElement.mapManager = this;
            }

            for(int i = 0; i < height; ++i)
            {
                Vector2 targetPosition = itemSize;
                targetPosition.x *= lineIdx;
                targetPosition.y *= i;
                targetPosition += startPosition + (itemSize / 2);
                elementList[lineIdx][i].id = new Vector2Int(lineIdx, i);
                StartCoroutine(elementList[lineIdx][i].moveTarget(targetPosition, removeTime, supplyTime));
            }
        }
        return scanResult.Count;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (scoreHistory.Count > 0)
                for (float minTime = Time.time - 60; scoreHistory.Peek().Item1 < minTime; scoreHistory.Dequeue()) ;
        }
        catch (System.InvalidOperationException) { }
        int speed = (from x in scoreHistory select x.Item2).Sum();
        maxSpeed = Mathf.Max(speed, maxSpeed);
        updateScore.Invoke(score, speed, maxSpeed);
    }
}
