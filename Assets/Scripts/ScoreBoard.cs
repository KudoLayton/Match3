using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void updateScoreBoard(int score, int speed, int maxSpeed)
    {
        textMesh.text =
            $"Max Speed\n{maxSpeed} blocks/min\nSpeed\n{speed} blocks/min\nScore {score} blocks";
    }
}
