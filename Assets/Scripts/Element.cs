using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    ParticleSystem endParticle;
    TextMeshPro textMesh;

    void Start()
    {
        endParticle = GetComponentInChildren<ParticleSystem>();
        textMesh = GetComponent<TextMeshPro>();
    }

    public IEnumerator destroyElement(float destroyTime)
    {
        for (float elapsedTime = 0; elapsedTime < destroyTime; elapsedTime += Time.deltaTime)
        {
            yield return new WaitForEndOfFrame();
            transform.localScale = Vector3.one * (1 - elapsedTime / destroyTime);
        }
        endParticle.Play();
        yield return new WaitForSeconds(endParticle.main.duration);
        Destroy(gameObject);
    }

    public IEnumerator moveTarget(Vector3 target,float delayTime, float moveTime)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Vector3 displacement = target - startPosition;
        yield return new WaitForSeconds(delayTime);
        while(elapsedTime < moveTime)
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
                mapManager.clickElement(id);
            }
        }
    }
}
