using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private MainPlayerScript MPS;

    public Transform handReach;
    public float handRange = 1;
    public LayerMask interActableObjects;
    public Material classicMat;
    public Material glowMat;
    public GameObject curretObject;

    public ItemChestList ChestList;

    private UpgradesOnPlayer playerUpgrades = new UpgradesOnPlayer();
    private float actioveCooldown = 0;
    private float interactCooldown = 0.75f;

    private void Awake()
    {
        MPS = GetComponent<MainPlayerScript>();
    }    

    public void Intertact()
    {
        if(curretObject != null && actioveCooldown == 0)
        {
            switch (curretObject.tag)
            {
                case "Chest":
                    curretObject.GetComponent<ChestScript>().Open();
                    break;
                case "ChestItem":
                    ChestItemInteraction();
                    break;
            }
            actioveCooldown = interactCooldown;
        }
    }

    //CheckIfInRange
    public void FixedUpdate()
    {
        SetCurretObject();

        if (actioveCooldown > 0)
        {
            actioveCooldown -= Time.deltaTime;
            if(actioveCooldown < 0)
            {
                actioveCooldown = 0;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (handReach == null) { return; }

        Gizmos.DrawWireSphere(handReach.position, handRange);
    }

    private void SetCurretObject()
    {
        Collider2D[] interactableObjects = Physics2D.OverlapCircleAll(handReach.position, handRange, interActableObjects);

        int nearest = FindNearestIObject(interactableObjects);

        SetGlow(nearest, interactableObjects);
    }

    private int FindNearestIObject(Collider2D[] interactableObjects)
    {
        int nearest = -1;

        float distance = -1;

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            float curretDistance = Vector2.Distance(gameObject.transform.position, interactableObjects[i].gameObject.transform.position);
            if (curretDistance > distance)
            {
                nearest = i;
                distance = curretDistance;
            }
        }

        return nearest;
    }

    private void SetGlow(int nearest, Collider2D[] interactableObjects)
    {
        if (nearest != -1)
        {
            if (curretObject != interactableObjects[nearest].gameObject)
            {
                if (curretObject != null)
                {
                    curretObject.GetComponent<SpriteRenderer>().material = classicMat;
                }

                curretObject = interactableObjects[nearest].gameObject;
                curretObject.GetComponent<SpriteRenderer>().material = glowMat;
            }
        }
        else if (curretObject != null)
        {
            curretObject.GetComponent<SpriteRenderer>().material = classicMat;
            curretObject = null;
        }
    }

    private void ChestItemInteraction()
    {
        ChestItem CHIS = curretObject.GetComponent<ChestItem>();
        Debug.Log("Dasd");
        if (CHIS.tupleInfo.type == "heal")
        {
            MPS.PHPS.Heal(ChestList.healPackList[CHIS.tupleInfo.num].healAmount);
            Destroy(curretObject);
        }

        //ChangeItem(CHIS.tupleInfo, CHIS);
    }

   /* private void ChangeItem((string type, int num) infoTuple, ChestItem CHIS)
    {
        switch (infoTuple.type)
        {
            case "weapon":
                ChangeWeapon(infoTuple.num, CHIS);
                break;
            case "ability":
                ChangeAbility(infoTuple.num, CHIS);
                break;
            case "weaponUpgrade":
                ChangeWeaponUpgrade(infoTuple.num, CHIS);
                break;
            default:
                break;
        }
    }

    private void ChangeWeapon(int weaponNum, ChestItem CHIS)
    {
        Weapon currentWeapon = MPS.PAS.cuuretWeapon;

        CHIS.spriteRenderer.sprite = currentWeapon.sprite;
        CHIS.tupleInfo = ("weapon", ChestList.GetItemNum(currentWeapon));

        MPS.PAS.cuuretWeapon = ChestList.weponList[weaponNum];
    }

    private void ChangeAbility(int abilityNum, ChestItem CHIS)
    {
        Ability currentAbility = MPS.PABS.currentAbility;

        CHIS.spriteRenderer.sprite = currentAbility.sprite;
        CHIS.tupleInfo = ("ability", ChestList.GetItemNum(currentAbility));

        MPS.PABS.currentAbility = ChestList.abilities[abilityNum];
    }

    private void ChangeWeaponUpgrade(int abilityNum,  ChestItem CHIS)
    {
        MPS.PAS.weaponUpgrades = playerUpgrades;
    }

    public void GetFirstWeapon()
    {
        MPS.PAS.cuuretWeapon = ChestList.weponList[0];
    }

    public void GetFirstAbility()
    {
        MPS.PABS.currentAbility = ChestList.abilities[0];
    }*/
}
