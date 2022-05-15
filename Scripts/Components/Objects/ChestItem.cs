using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItem : MonoBehaviour
{
    [HideInInspector]
    public (string type, int num) tupleInfo;
    public SpriteRenderer spriteRenderer;

    private bool anmimEnded = false;
    [SerializeField]
    private float animTime = 3;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!anmimEnded)
        {
            animTime -= Time.deltaTime;
            transform.position = new Vector2 (transform.position.x, transform.position.y + 1 * Time.deltaTime);

            if(animTime < 0) 
            {
                animTime = 0;
                anmimEnded = true;
                gameObject.layer = 9;
            }
        }
    }
}    

