using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;

    int score;

    const string scorePrefix = "Score: ";
    void Start()
    {

        score = 0;

        ShowInfo();
    }

    public void IncScore(int value)
    {
        score += value;
        ShowInfo();
    }


    void ShowInfo()
    {
        scoreText.text = scorePrefix +  score;
    }
}