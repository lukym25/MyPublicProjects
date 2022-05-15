using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour 
{
    public static bool mobileMode = false;

    public Dropdown resolutionDropDopwn;

    Resolution[] resolutions;

    private void Start()
    {
        mobileMode = false;

        FindAllRsolution();
    }

    public void ChangeMode(bool mode)
    {
        mobileMode = mode;
    }

    public void FullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void FindAllRsolution()
    {
        resolutions = Screen.resolutions;

        resolutionDropDopwn.ClearOptions();

        List<string> options = new List<string>();

        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropDopwn.AddOptions(options);
        resolutionDropDopwn.value = currentIndex;
        resolutionDropDopwn.RefreshShownValue();
    }
}
