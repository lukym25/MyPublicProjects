using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public float destroyAfter;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyMe());
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(gameObject);
    }
}
