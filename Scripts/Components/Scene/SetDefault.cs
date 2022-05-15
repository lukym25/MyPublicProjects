using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefault : MonoBehaviour
{
    [SerializeField]
    private GameObject[] joysticks;

    private void Awake()
    {
        HideJoySticks();
    }

    private void HideJoySticks()
    {
        if (Settings.mobileMode)
        {
            foreach (GameObject JS in joysticks)
            {
                JS.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject JS in joysticks)
            {
                JS.SetActive(false);
            }
        }
    }

}
