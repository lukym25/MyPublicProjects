using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public ParticleSystem starsEffect;
    public Animator animator;

    private bool opened = false;
    public GameObject item;
    public ItemChestList ChestList;

    // Start is called before the first frame update
    void Start()
    {
        item = transform.GetChild(0).gameObject;
    }
    public void Open()
    {
        if (!opened)
        {
            opened = true;
            animator.SetBool("Open", true);

            StartCoroutine(SetToNormalObject());
            starsEffect.Play();

            item.SetActive(true);

            ChooseItem();
        }
    }

    private IEnumerator SetToNormalObject()
    {
        yield return new WaitForSeconds(1);
        gameObject.layer = 0;
    }

    private void ChooseItem()
    {
        ChestItem CIS = item.GetComponent<ChestItem>(); 

        int random = Random.Range(0, 90);
        int random2;


        if (random < 30 && ChestList.weponList.Length != 0)
        {
            random2 = Random.Range(0, ChestList.weponList.Length - 1);
            Debug.Log("Chest" + random2);
            CIS.tupleInfo = ("weapon", random2);

            CIS.spriteRenderer.sprite = ChestList.weponList[random2].sprite;
        }

        else if (random < 60 && ChestList.abilities.Length != 0)
        {
            random2 = Random.Range(0, ChestList.abilities.Length - 1);
            Debug.Log("Chest" + random2);

            CIS.tupleInfo = ("ability", random2);

            CIS.spriteRenderer.sprite = ChestList.abilities[random2].sprite;
        }


        else if (random < 90 && ChestList.weaponUpgrades.Length != 0)
        {
            random2 = Random.Range(0, ChestList.weaponUpgrades.Length -1);

            CIS.tupleInfo = ("weaponUpgrade", random2);

            CIS.spriteRenderer.sprite = ChestList.weaponUpgrades[random2].sprite;
        } 
        
        else
        {
            random2 = Random.Range(0, ChestList.healPackList.Length - 1);

            CIS.tupleInfo = ("heal", random2);

            CIS.spriteRenderer.sprite = ChestList.healPackList[random2].sprite;
        }
    }
}
