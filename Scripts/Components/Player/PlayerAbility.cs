using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    private MainPlayerScript MPS;
    public Ability currentAbility;

    private float cooldownActive = 0;
    private bool isRecharging = false;
    private bool abilitiIsActive = false;
    private Rigidbody2D RB;
    private UIInGame UIScript;

    [Header("Dash")]
    [SerializeField]
    private float dashForce = 50;
    [SerializeField]
    private ParticleSystem dashEffect;
    
    [Header("Shild")]
    [SerializeField]
    private float duration = 5;
    [SerializeField]
    private GameObject shildObject;
    [SerializeField]
    private SpriteRenderer shildSpriteRenderer;
    [SerializeField]
    private float damageReduction = 0.6f;
    private float timeActive = 0;

    private void Start()
    {
        MPS = GetComponent<MainPlayerScript>();
        UIScript = FindObjectOfType<UIInGame>();

        RB = MPS.pRigidBody;
    }

    private void FixedUpdate()
    {
        if (cooldownActive > 0 && isRecharging)
        {
            cooldownActive -= Time.deltaTime;
            if(cooldownActive < 0)
            {
                cooldownActive = 0;
                isRecharging = false;
            }
            UIScript.ChangeStaminaOnUISlider((currentAbility.coolDown - cooldownActive) / currentAbility.coolDown);
        }

        if (abilitiIsActive)
        {
            switch (currentAbility.abilityNum)
            {
                case 2:
                    ShieldUpdate();
                    break;
            }
        }
    }

    public void ActivateAbility()
    {
        if (cooldownActive == 0)
        {
            switch (currentAbility.abilityNum)
            {
                case 1:
                    DashActivate();
                    break;
                case 2:
                    ShieldActivate();
                    break;
                default:
                    DashActivate();
                    UIScript.ChangeStaminaOnUISlider(0);
                    StartCoroutine(StartRechargingStamina(0.7f));
                    break;
            }

            cooldownActive += currentAbility.coolDown;
            abilitiIsActive = true;
        }
    }

    /*/////////////////////////////////////////////////////////////////////////////
                                    Dash Ability
     ////////////////////////////////////////////////////////////////////////////*/

    private void DashActivate()
    {
        RB.velocity = new Vector2 (RB.velocity.x, 0);

        float oldGravity = RB.gravityScale;
        RB.gravityScale = 0;

        RB.AddForce(new Vector2(transform.localScale.x, 0) * dashForce, ForceMode2D.Impulse);

        dashEffect.transform.localScale = transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        dashEffect.Play();

        UIScript.ChangeStaminaOnUISlider(0);

        StartCoroutine(StartRechargingStamina(0.7f));

        StartCoroutine(ChangeToNormalGravity(oldGravity));
    }

    private IEnumerator ChangeToNormalGravity(float gravity)
    {
        yield return new WaitForSeconds(0.25f);

        RB.gravityScale = gravity;
    }

    /*/////////////////////////////////////////////////////////////////////////////
                                    Shild Ability
     ////////////////////////////////////////////////////////////////////////////*/

    private void ShieldActivate()
    {
        shildObject.SetActive(true);

        MPS.PHPS.armorpercent += damageReduction;

        StartCoroutine(EndOfShildEffect(duration));
    }

    private IEnumerator EndOfShildEffect(float time)
    {
        yield return new WaitForSeconds(time);

        shildSpriteRenderer.color = new Color(shildSpriteRenderer.color.r, shildSpriteRenderer.color.g, shildSpriteRenderer.color.b, 0);
        shildObject.SetActive(false);

        timeActive = 0;
        MPS.PHPS.armorpercent -= damageReduction;

        abilitiIsActive = false;
        isRecharging = true;
    }

    private void ShieldUpdate()
    {
        if (timeActive < 0.5)
        {
            float alphaValue = Mathf.Sign(timeActive * 6 - Mathf.PI / 2) / 4 + 0.25f;

            shildSpriteRenderer.color = new Color(shildSpriteRenderer.color.r, shildSpriteRenderer.color.g, shildSpriteRenderer.color.b, alphaValue);
        }

        else if (timeActive > 4)
        {
            float alphaValue = Mathf.Cos(timeActive * 2 * Mathf.PI  - 4) / 8 + 3/8;

            shildSpriteRenderer.color = new Color(shildSpriteRenderer.color.r, shildSpriteRenderer.color.g, shildSpriteRenderer.color.b, alphaValue);
        }
        else {
            shildSpriteRenderer.color = new Color(shildSpriteRenderer.color.r, shildSpriteRenderer.color.g, shildSpriteRenderer.color.b, 0.5f);
        }

        timeActive += Time.deltaTime;
    }

    /*/////////////////////////////////////////////////////////////////////////////
                                    Common
     ////////////////////////////////////////////////////////////////////////////*/

    private IEnumerator StartRechargingStamina(float time)
    {
        yield return new WaitForSeconds(time);

        isRecharging = true;
    }
}
