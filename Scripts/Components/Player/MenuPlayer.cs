using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    bool onGround = false;
    public ParticleSystem DustWhenFall;

    // Update is called once per frame
    void Update()
    {
        IsOnGround();
    }

    private void IsOnGround()
    {
        bool wasOnFGround = onGround;

        onGround = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(0, -2), new Vector2(1, 0.25f), 0, ~(1 << 6)) != null ? true : false;

        //trigger enter
        if (!wasOnFGround && onGround)
        {
            DustWhenFall.Play();
        }
    }
}
