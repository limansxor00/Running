using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("AutoDelete", 1);
    }

    void AutoDelete()
    {
        GameObject.Destroy(gameObject);
    }
   
}
