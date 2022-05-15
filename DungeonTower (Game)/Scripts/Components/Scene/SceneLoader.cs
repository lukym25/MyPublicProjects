using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject balckScreen;
    public GameObject[] setActive;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLoader());
    }

    public void SwitchSceneTo(int sceneIndex)
    {
        balckScreen.GetComponent<Animator>().SetTrigger("FadeIn");

        StartCoroutine(ChangeScene(sceneIndex));
    }

    IEnumerator ChangeScene(int sceneIndex)
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator StartLoader()
    {
        yield return new WaitForSeconds(1);

        for(int i = 0; i < setActive.Length; i++)
        {
            setActive[i].SetActive(true);
        }
    }
}
