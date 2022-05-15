using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemyScript : MonoBehaviour
{
    private EnemyBehavior EBS;
    private UIInGame UIS;
    public Animator enemyAnimator;

    public float maxHP = 100;
    public float currentHp = 100;
    public string EnemyType = "common";    

    [SerializeField]
    private ParticleSystem hurtEffect;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name.Remove(4) != "Boss")
        {
            EBS = GetComponent<EnemyBehavior>();
        } else
        {
            UIS = FindObjectOfType<UIInGame>();
        }
        currentHp = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;

        hurtEffect.Play();
        spriteRenderer.color = Color.red;

        if (gameObject.name.Remove(4) != "Boss")
        {
            EBS.stunTime = 0.5f;
        } else
        {
            UIS.ChangeBossHPOnUISlider(currentHp / maxHP);
        }

        if (currentHp <= 0)
        {
            if(EnemyType == "Boss")
            {
                StartCoroutine(DestroyThisObject(0.75f));
                enemyAnimator.SetTrigger("Die");
            } else
            {
                Destroy(gameObject);
            }
        }

        StartCoroutine(GiveEnemyNormalColor());
    }

    private IEnumerator DestroyThisObject(float duration)
    {
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
    }

    private IEnumerator GiveEnemyNormalColor()
    {
        yield return new WaitForSeconds(0.5F);

        spriteRenderer.color = Color.white;
    }

    public void Heal(float amount)
    {
        currentHp += amount;

        if(currentHp > maxHP)
        {
            currentHp = maxHP;
        }
    }

    public void StunMe(float time)
    {
        if (gameObject.name.Remove(4) != "Boss")
        {
            GetComponent<EnemyBehavior>().stunTime = time;
        }
    }
}
