using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static int score;
    public static int highScore;

    public static void AddScore(int newScoreValue)
    {
        score += newScoreValue;
    }
    

}
