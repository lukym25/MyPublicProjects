using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUiScript : MonoBehaviour
{
    [Header("Hp")]
    [SerializeField]
    private GameObject heartPrefab;

    private GameObject[] hearts;

    [SerializeField]
    private Sprite fullHpSprite, emptyHpSprite;

    [SerializeField]
    private Transform hearthContainer;

    private int lastHpIndex;

    [Header("Coins")]
    [SerializeField]
    private Text coinTextUi;
    
    [Header("Costs")]
    [SerializeField]
    private BuildingUIInfoCost[] buildingUiCost;

    [Header("GameOverscreen")]    
    [SerializeField]
    private GameObject endScreen, playButton, playAgainButton;

    [SerializeField]
    private Text gameOverText;

    [SerializeField]
    private Animator endScreenAnimator;

    public static bool playFirstTime = true;

    private EnemySpawner enemySpawner;

    private void Awake()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (!playFirstTime)
        {
            endScreen.SetActive(false);
            enemySpawner.StartSpawning();
        }
    }

    public void Spawnhearts(int maxHP)
    {
        hearts = new GameObject[maxHP];

        for (int i = 0; i < maxHP; i++)
        {
            Vector3 pos = hearthContainer.position + new Vector3(i * 15, 0, 0);
            GameObject newhearth = Instantiate(heartPrefab, pos, Quaternion.identity, hearthContainer);
            hearts[i] = newhearth;
        }

        lastHpIndex = maxHP - 1;
    }

    public void SetCosts(ShopScript.BuildingCost[] buildingCost)
    {
        for(int i = 0; i < buildingUiCost.Length; i++)
        {
            for (int ii = 0; ii < buildingCost.Length; ii++)
            {
                if (buildingUiCost[i].name == buildingCost[ii].name)
                {
                    buildingUiCost[i].textField.text = "" + buildingCost[ii].cost;
                }
            }
        }      
    }

    public void TakeDamage(int damageAmount)
    {
        while(damageAmount > 0)
        {
            hearts[lastHpIndex].GetComponent<Image>().sprite = emptyHpSprite;
            lastHpIndex--;
            damageAmount--;
        }
    }

    public void CoinsAmountChanged(int amountOfCoins)
    {
        coinTextUi.text = "" + amountOfCoins;
    }

    public void GameOverScreen()
    {
        gameOverText.text = "Game Over";

        endScreen.SetActive(true);

        endScreenAnimator.SetTrigger("FadeIn");

        enemySpawner.gameOver = true;
    }

    public void PlayAgian()
    {
        playFirstTime = false;

        endScreenAnimator.SetTrigger("FadeOut");

        StartCoroutine(HideBlackScreen());
    }

    public void StartGame()
    {
        endScreenAnimator.SetTrigger("FadeOut");

        StartCoroutine(HideBlackScreen());
    }

    private IEnumerator HideBlackScreen()
    {
        yield return new WaitForSeconds(1);

        endScreen.SetActive(false);

        if (playFirstTime)
        {
            playButton.SetActive(false);
            playAgainButton.SetActive(true);
        } else
        {
            SceneManager.LoadScene(0);
        }
    }

    [System.Serializable]
    public struct BuildingUIInfoCost 
    {
        public string name;
        public Text textField;
    }
}
