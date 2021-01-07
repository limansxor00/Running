using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinArrangement : MonoBehaviour
{
    public GameObject ViewObj;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = Instantiate(ViewObj, transform.GetChild(i));
            obj.transform.localPosition = new Vector3(0, -0.3f, 0);
        }
    }

}
