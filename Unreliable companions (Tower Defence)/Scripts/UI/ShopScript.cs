using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private BuildScript buildScript;

    private PlayerUiScript playerUiScript;

    [SerializeField]
    private int startingCoins;

    [Header("Costs")]
    [SerializeField]
    private BuildingCost[] buildingCosts;

    private int coins;

    private void Awake()
    {
        coins = startingCoins;

        buildScript = GetComponent<BuildScript>();
        playerUiScript = GetComponent<PlayerUiScript>();
    }

    private void Start()
    {
        CoinsValueChanged(0);

        playerUiScript.SetCosts(buildingCosts);
    }

    public void BuyBackFireCannon()
    {
        if (coins >= buildingCosts[0].cost)
        {
            CoinsValueChanged(-buildingCosts[0].cost);

            SoundManager.instance.PlaySound("BuildingBought");

            buildScript.StartBuilding(0, buildingCosts[0].cost);
        }
    }

    public void BuildingSold(string buildingName, float multiplier = 1, int level = 1)
    {
        for (int i = 0; i < buildingCosts.Length; i++)
        {
            if (buildingCosts[i].name == buildingName)
            {
                CoinsValueChanged((int)(CoinsForLevel(buildingCosts[i].cost, level) * multiplier));
            }
        }
    }

    public void BuildingSold(int id, float multiplier = 1, int level = 1)
    {
        for (int i = 0; i < buildingCosts.Length; i++)
        {
            if (i == id)
            {
                CoinsValueChanged((int)(CoinsForLevel(buildingCosts[i].cost, level) * multiplier));
            }
        }
    }

    //add coins for each level buililding have
    private float CoinsForLevel(int startingCoins, int level)
    {
        if(level == 0) { return 0; }

        return startingCoins * Mathf.Pow(1 + 0.5f, level-1) + CoinsForLevel(startingCoins, level-1);
    }

    public int FindBuildingCost(string buildingName)
    {
        foreach(BuildingCost bc in buildingCosts)
        {
            if (bc.name == buildingName)
            {
                return bc.cost;
            }
        }
        return buildingCosts[buildingCosts.Length - 1].cost;
    }

    public bool EnoughMoney(int amount)
    {
        return coins - amount >= 0;
    }


    public void CoinsValueChanged(int amount)
    {
        coins += amount;

        playerUiScript.CoinsAmountChanged(coins);
    }

    [System.Serializable]
    public struct BuildingCost 
    {
        public string name;
        public int cost;
    }
}
