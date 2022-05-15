using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public CanvasGroup alphaEndScreen;

    public GameObject winOption;
    public GameObject loseOption;

    public bool endScreenFallingDown = false;

    private void Update()
    {
        if (endScreenFallingDown)
        {
            if (alphaEndScreen.alpha <= 0.5f)
            {
                alphaEndScreen.alpha += Time.deltaTime;

                if(alphaEndScreen.alpha > 0.5f)
                {
                    alphaEndScreen.alpha = 0.5f;
                    endScreenFallingDown = false;
                }
            }
        }
    }

    public void ShowEndScreen(bool playerDied)
    {
        endScreenFallingDown = true;

        if (playerDied)
        {
            loseOption.SetActive(true);
        } else
        {
            winOption.SetActive(true);
        }
    }
}
