using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = score.ToString();
    }

    public void KillPoints(bool headShot)
    {
        if (headShot.Equals(true))
        {
            score += 130;
            UpdateScoreText();
        }
        else if (headShot.Equals(false))
        {
            score += 90;
            UpdateScoreText();
        }
    }

    public void HitmarkerPoints()
    {
        score += 10;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}
