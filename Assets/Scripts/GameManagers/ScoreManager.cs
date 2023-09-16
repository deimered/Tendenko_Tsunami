using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public int score = 0;

    public int Score
    {
        get { return score; }
    }

    public void AddScore(int value)
    {
        score = score + value >= 0 ? score + value : 0;
    }

    public void SetScore(int value)
    {
        if (value >= 0)
            score = value;
    }
}
