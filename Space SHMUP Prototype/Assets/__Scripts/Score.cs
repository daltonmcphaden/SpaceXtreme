using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static int score; // score variable
    public static int highScore; // highscore variable

    public static void AddScore(int newScoreValue) // adds to the score when called on enemy destruction
    {
        score += newScoreValue;
    }
    
    
}
