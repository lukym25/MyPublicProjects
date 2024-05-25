using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsInfoScript : MonoBehaviour
{
    private ShopScript shopscript;
    private BuildScript buildScript;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private GameObject buildingInfoPanel, explosionPrefab;

    [SerializeField]
    private Image infoSprite;

    [SerializeField]
    private Text nameText, descriptionText, damageText, attackSpeedText, projectileSpeedText, rangeText, levelText, updgradeCostText;

    [SerializeField]
    private LayerMask layerMask;

    [HideInInspector]
    public BuildingInfo lastBuildingInfo;

    private void Awake()
    {
        shopscript = GetComponent<ShopScript>();
        buildScript = GetComponent<BuildScript>();
    }

    // Update is called once per frame
    void Update()
    {
        ChakedIfClickedOnBuilding();
    }

    private void ChakedIfClickedOnBuilding() 
    {
        if(!Input.GetKeyDown(KeyCode.Mouse0)) { return; }

        if (buildingInfoPanel.activeInHierarchy)
        {
            bool mouseInInfo = RectTransformUtility.RectangleContainsScreenPoint(buildingInfoPanel.GetComponent<RectTransform>(), Input.mousePosition, null);

            if (mouseInInfo)
            {
                return;
            }
        }

        Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D objectsInPosition = Physics2D.Linecast(mousePositionInWorld, mousePositionInWorld, layerMask);

        if (objectsInPosition.collider == null)
        {
            buildingInfoPanel.SetActive(false);
        }
        else
        {
            AssignValuesToPanel(objectsInPosition.transform.gameObject);

            buildingInfoPanel.transform.position = CheckFitOnScreen();
            buildingInfoPanel.SetActive(true);
        }
    }

    private void AssignValuesToPanel(GameObject objectInPosition)
    {
        //assign values
        BuildingInfo buildingInfo = objectInPosition.GetComponent<BuildingInfo>();

        nameText.text = buildingInfo.buildingName;
        infoSprite.sprite = buildingInfo.sprite;
        descriptionText.text = buildingInfo.description;
        damageText.text = "Damage : " + buildingInfo.damage;
        attackSpeedText.text = "Attack Speed : " + buildingInfo.attackSpeed;
        projectileSpeedText.text = "Projectile Speed : " + buildingInfo.projectileSpeed;
        rangeText.text = "Range : " + buildingInfo.range;
        levelText.text = "Level " + buildingInfo.level;

        int cost = (int)(shopscript.FindBuildingCost(buildingInfo.buildingName) * Mathf.Pow((1 + 0.5f), buildingInfo.level));
        updgradeCostText.text = "Upgrade for " + cost;

        lastBuildingInfo = buildingInfo;
    }

    private Vector3 CheckFitOnScreen()
    {
        Vector2 position = (Vector2)Input.mousePosition + offset;
        float pixelsOf;
        RectTransform rectTransform = buildingInfoPanel.gameObject.GetComponent<RectTransform>();

        float boxHight = rectTransform.rect.y;
        float boxWidth = rectTransform.rect.x;

        float screenHight = Screen.height;
        float screenWidth = Screen.width;

        //top
        pixelsOf = position.y - screenHight;
        position = pixelsOf >= 0 ? position - new Vector2(0, pixelsOf + 40) : position;

        //bottom
        pixelsOf = position.y - boxHight;
        position = pixelsOf <= 0 ? position + new Vector2(0, pixelsOf + 40) : position;

        //right
        pixelsOf = position.x + boxWidth - screenWidth;
        position = pixelsOf >= 0 ? position - new Vector2(pixelsOf + 40, 0) : position;

        //left
        pixelsOf = position.x;
        position = pixelsOf <= 0 ? position + new Vector2(0, pixelsOf + 40) : position;

        return position;
    }


    public void Sell()
    {
        Vector2 position = lastBuildingInfo.gameObject.transform.position;

        Instantiate(explosionPrefab, new Vector3(position.x, position.y, explosionPrefab.transform.position.z), Quaternion.identity);

        shopscript.BuildingSold(lastBuildingInfo.buildingName, 0.7f, lastBuildingInfo.level);        
        buildScript.BuildingDestroyed(position);

        SoundManager.instance.PlaySound("Explosion");

        buildingInfoPanel.SetActive(false);
        Destroy(lastBuildingInfo.gameObject);
    }

    public void Upgrade()
    {
        int UpgradeCost = (int)(shopscript.FindBuildingCost(lastBuildingInfo.buildingName) * Mathf.Pow((1 + 0.5f), lastBuildingInfo.level));
        if (!shopscript.EnoughMoney(UpgradeCost)) { return; }

        shopscript.CoinsValueChanged(-UpgradeCost);

        lastBuildingInfo.damage *= 1.25f;
        lastBuildingInfo.attackSpeed *= 1.25f;
        lastBuildingInfo.projectileSpeed *= 1.25f;
        lastBuildingInfo.range *= 1.25f;
        lastBuildingInfo.level++;

        SoundManager.instance.PlaySound("BuildingUpgrade");

        AssignValuesToPanel(lastBuildingInfo.gameObject);
    }
}
