using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject Menu1;
    public GameObject GameSetting;
    public GameObject Options;
        
    [SerializeField]
    private float timeToHide = 0.4f;
    [SerializeField]
    private float timeShow = 0.4f;

    private int currentMenu = 1;
    private CanvasGroup CG;
    private RectTransform RT;
    private bool doingAction = false;

    private void Start()
    {
        StartCoroutine(ChangePanel(1, 0.5f));
    }

    private void Update()
    {

        if (CG != null)
        {
            if (CG.alpha > 0)
            {
                CG.alpha -= Time.deltaTime / timeToHide;
            } else
            {
                CG.gameObject.SetActive(false);
                CG.alpha = 1;
                CG = null;
            }
        }

        else if (RT != null)
        {
            if (RT.position.y > 540)
            {

                RT.position = new Vector3(RT.position.x, RT.position.y - 1000 * Time.deltaTime / timeShow, 0);
            } else
            {
                doingAction = false;
                RT = null;
            }
        }
    }

    public void PlayButton()
    {
        if (!doingAction)
        {
            Hidepanel(currentMenu);
            currentMenu = 2;
            StartCoroutine(ChangePanel(currentMenu, 0));

            doingAction = true;
        }
    }

    public void OptionsButton()
    {
        if (!doingAction)
        {
            Hidepanel(currentMenu);
            currentMenu = 3;
            StartCoroutine(ChangePanel(currentMenu, 0));

            doingAction = true;
        }
    }

    public void BackButton()
    {
        if (!doingAction)
        {
            Hidepanel(currentMenu);
            currentMenu = 1;
            StartCoroutine(ChangePanel(currentMenu, 0));

            doingAction = true;
        }
    }

    public void PlayGame()
    {
        if (!doingAction)
        {
            FindObjectOfType<SceneLoader>().SwitchSceneTo(1);
            doingAction = true;
        }
    }

    public void ExitButton()
    {
        if (!doingAction)
        {
            Application.Quit();
        }
    }

    private void Hidepanel(int panelNum)
    {
        switch (panelNum)
        {
            case 1:
                CG = Menu1.GetComponent<CanvasGroup>();
                break;
            case 2:
                CG = GameSetting.GetComponent<CanvasGroup>();
                break;
            case 3:
                CG = Options.GetComponent<CanvasGroup>();
                break;
            default:
                break;
        }
    }

    IEnumerator ChangePanel(int panelNum, float plusTime)
    {
        yield return new WaitForSeconds(timeToHide + plusTime);

        switch (panelNum)
        {
            case 1:
                RT = Menu1.GetComponent<RectTransform>();
                RT.position = new Vector3(RT.position.x, RT.position.y + 1000, 0);

                Menu1.SetActive(true);
                break;
            case 2:
                RT = GameSetting.GetComponent<RectTransform>();
                RT.position = new Vector3(RT.position.x, RT.position.y + 1000, RT.position.z);

                GameSetting.SetActive(true);
                break;
            case 3:
                RT = Options.GetComponent<RectTransform>();
                RT.position = new Vector3(RT.position.x, RT.position.y + 1000, 0);

                Options.SetActive(true);
                break;
            default:
                break;
        }
    } 
}
