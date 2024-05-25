using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lukas.MyClass;

public class CoreHpScript : MySingelton<CoreHpScript>
{
    [SerializeField]
    private PlayerUiScript playerUiScript;

    [SerializeField]
    private int maxHp;

    private int currentHp;

    public bool gameOver = false;

    private void Awake()
    {
        currentHp = maxHp;
    }

    private void Start()
    {
        playerUiScript.Spawnhearts(currentHp);
    }

    private void CoreDamaged()
    {
        if(gameOver) { return; }
        currentHp--;

        playerUiScript.TakeDamage(1);

        SoundManager.instance.PlaySound("Hurt");

        if (currentHp <= 0)
        {
            gameOver = true;
            GameOver();
        }
    }

    private void GameOver()
    {      

        playerUiScript.GameOverScreen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Destroy(collision.gameObject);

                CoreDamaged();
            }
        }
    }
}
