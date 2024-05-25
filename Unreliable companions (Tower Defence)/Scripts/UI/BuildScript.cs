using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildScript : MonoBehaviour
{
    private ShopScript shopScript;

    [SerializeField]
    private GameObject ImageObject, explosionPrefab;

    [SerializeField]
    private RectTransform BuildingRange;

    [SerializeField]
    private GameObject[] buildings;

    private bool placingBuilding = false;
    private int placingBuildingId;

    private BuildingSpot[] buildingSpots;

    private void Awake()
    {
        shopScript = GetComponent<ShopScript>();

        FindBuildingSpots();
    }

    private void LateUpdate()
    {
        BuildingMenu();

        CheckForCancel();
    }

    private void FindBuildingSpots()
    {
        GameObject[] spots = GameObject.FindGameObjectsWithTag("BuildingSpot");
        buildingSpots = new BuildingSpot[spots.Length];

        for (int i = 0; i < spots.Length; i++)
        {
            buildingSpots[i].spotGameObject = spots[i].gameObject;
            buildingSpots[i].isEmpty = true;
        }
    }

    public void StartBuilding(int buildingId, int cost)
    {
        placingBuilding = true;
        placingBuildingId = buildingId;

        BuildingInfo buildingInfo = buildings[buildingId].GetComponent<BuildingInfo>();
        ImageObject.GetComponent<Image>().sprite = buildingInfo.sprite;
        ImageObject.SetActive(true);

        float ratio = Camera.main.orthographicSize * Camera.main.aspect;

        BuildingRange.localScale = new Vector2(buildingInfo.range * ratio, buildingInfo.range * ratio);
    }

    private float CalcuateWorldToViewportRatio()
    {
        //converting range to canvas size
        Vector2 pointA = new Vector2(0, 0);
        Vector2 pointB = new Vector2(1, 0);

        pointA = Camera.main.WorldToScreenPoint(pointA);
        pointB = Camera.main.ScreenToWorldPoint(pointB);

        float ratio = Vector2.Distance(pointA, pointB);

        return ratio;
    }

    private void BuildingMenu()
    {
        if (!placingBuilding) { return; }

        ImageObject.transform.position = Input.mousePosition;

        SnapToClosestBuildingSpot();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int? closestSpotId = FindClosestBuildingSpotId();

            if (closestSpotId == null) { return; }
            
            Build(closestSpotId.Value);            
        }
    }

    private void SnapToClosestBuildingSpot()
    {
        int? closestSpotId = FindClosestBuildingSpotId();
        if(closestSpotId == null) { return; }

        ImageObject.transform.position = Camera.main.WorldToScreenPoint(buildingSpots[closestSpotId.Value].spotGameObject.transform.position);
    }

    private int? FindClosestBuildingSpotId()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float maxDistance = 5;
        int id = -1;

        for (int i = 0; i < buildingSpots.Length; i++)
        {
            if (buildingSpots[i].isEmpty)
            {
                float distance = Vector2.Distance(buildingSpots[i].spotGameObject.transform.position, mousePosition);

                if (distance < maxDistance)
                {
                    maxDistance = distance;
                    id = i;
                }
            }
        }

        if (id != -1)
        {
            return id;
        } 

        return null;
    }

    private void Build(int spotId)
    {
        buildingSpots[spotId].isEmpty = false;

        Vector3 buildingSpotPosition = buildingSpots[spotId].spotGameObject.transform.position;
        Vector3 spawnPosition = new Vector3(buildingSpotPosition.x, buildingSpotPosition.y, buildings[placingBuildingId].transform.position.z);

        Instantiate(buildings[placingBuildingId], spawnPosition, Quaternion.identity);

        SoundManager.instance.PlaySound("BuildingBuild");

        placingBuilding = false;
        ImageObject.SetActive(false);
    }

    private void CheckForCancel()
    {
        if(!placingBuilding) { return; }
        if (!Input.GetKeyDown(KeyCode.Mouse1)) { return; }     

        ImageObject.SetActive(false);
        placingBuilding = false;

        SoundManager.instance.PlaySound("Explosion");

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(ImageObject.transform.position);
        Instantiate(explosionPrefab, new Vector3(mousePosition.x, mousePosition.y, explosionPrefab.transform.position.z), Quaternion.identity);

        shopScript.BuildingSold(placingBuildingId);
    }

    public void BuildingDestroyed(Vector2 position)
    {
        for (int i = 0; i < buildingSpots.Length; i++)
        {
            if((Vector2)buildingSpots[i].spotGameObject.transform.position == position)
            {
                buildingSpots[i].isEmpty = true;
                return;
            }
        }
    }

    [System.Serializable]
    public struct BuildingSpot
    {
        public bool isEmpty;
        public GameObject spotGameObject;
    }
}

