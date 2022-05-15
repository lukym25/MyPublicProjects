using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoBehaviour
{
    //PlayerHp
    [SerializeField]
    private Slider hpSlider;
    private float targetHP;
    private float hpSlidingSpeed;
    private bool hpSliding;

    //PlayerStam
    [SerializeField]
    private Slider staminaSlider;
    private float targetStamina;
    private float staminaSlidingSpeed;
    private bool stamingaSliding;

    //BossHp
    [SerializeField]
    private Slider bossHpSlider;
    private float targetBossHp;
    private float bossHpSlidingSpeed;
    private bool bossHpSliding;

    [SerializeField]
    private CanvasGroup bossAphaGameObject;
    private bool showBossSlider;

    private void Update()
    {
        if (hpSliding)
        {
            MoveSlider(hpSlider, targetHP, hpSlidingSpeed, ref hpSliding, 1);
        }

        if (stamingaSliding)
        {
            MoveSlider(staminaSlider, targetStamina, staminaSlidingSpeed, ref stamingaSliding, 1.5f);
        }

        if (bossHpSliding)
        {
            MoveSlider(bossHpSlider, targetBossHp, bossHpSlidingSpeed, ref bossHpSliding, 0.75f);
        }


        if (showBossSlider)
        {
            bossAphaGameObject.alpha += Time.deltaTime/2;

            if (bossAphaGameObject.alpha >= 1)
            {
                bossAphaGameObject.alpha = 1;
                showBossSlider = false;
            }
        }
    }

    public void ChangeHpOnUISlider(float newHP)
    {
        hpSliding = true;
        targetHP = newHP;
        hpSlidingSpeed = hpSlider.value - newHP;
    }

    public void ChangeStaminaOnUISlider(float newStamina)
    {
        stamingaSliding = true;
        targetStamina = newStamina;
        staminaSlidingSpeed = staminaSlider.value - newStamina;
    }

    public void ChangeBossHPOnUISlider(float newHP)
    {
        bossHpSliding = true;
        targetBossHp = newHP;
        bossHpSlidingSpeed = bossHpSlider.value - newHP;
    }

    public void ShowBossSlider()
    {
        showBossSlider = true;
    }

    private void MoveSlider(Slider slider, float targetValue, float slidingSpeed, ref bool condicion, float AnimSpeed)
    {
        slider.value -= slidingSpeed * Time.deltaTime * AnimSpeed;
        if (Mathf.Abs(slider.value - targetValue) < 0.01f)
        {
            condicion = false;
        }
    }
}
