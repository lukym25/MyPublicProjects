using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestrucatableObject : MonoBehaviour
{
    public ParticleSystem destroyObjectEffect;
    public void DestroyMe()
    {
        destroyObjectEffect.gameObject.transform.parent = null;
        destroyObjectEffect.gameObject.transform.localScale = new Vector3(1, 1, 1);

        destroyObjectEffect.Play();

        Destroy(gameObject);
    }
}
