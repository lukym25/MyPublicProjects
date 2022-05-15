using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpScript : MonoBehaviour
{
    private MainPlayerScript MPS;
    public float maxHp = 100;
    public float currentHp = 100;
    public float armorpercent = 0;

    public ParticleSystem hurtEffect;

    private UIInGame UIScript;

    // Start is called before the first frame update
    void Start()
    {
        MPS = GetComponent<MainPlayerScript>();
        UIScript = FindObjectOfType<UIInGame>();
    }

    public void TakeDamageEffect(float damageAmount)
    {
        float damageDealt = damageAmount - damageAmount * armorpercent;

        currentHp -= damageDealt < 0 ? 1 : damageDealt;

        UIScript.ChangeHpOnUISlider(currentHp / maxHp);

        if (currentHp < 0)
        {
            currentHp = 0;
            Debug.Log("PlayerDied");
            FindObjectOfType<SceneLoader>().SwitchSceneTo(0);
        }

        hurtEffect.Play();
    }

    public void Heal(float amount)
    {
        currentHp += amount;

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        UIScript.ChangeHpOnUISlider(currentHp / maxHp);
    }
}
