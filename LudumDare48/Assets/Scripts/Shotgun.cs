using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public GameObject pSpawn, nSpawn,b,t;
    public Shell preFap;
    private Shell[] shells = new Shell[2];
    // Start is called before the first frame update
    void Start()
    {
        //Reload();
    }

    // Update is called once per frame
    void Update()
    {
        if (shells[0] == null || shells[1] == null)
            return;
        shells[0].transform.position = pSpawn.transform.position;
        shells[1].transform.position = nSpawn.transform.position;
    }

    internal void Reload()
    {
        shells[0] = GameObject.Instantiate(preFap);
        shells[1] = GameObject.Instantiate(preFap);
    }

    internal void Fire(Vector3 eulerAngles)
    {
       // Debug.Log("Schuss");
        var vec = t.transform.position - b.transform.position;
        vec = vec.normalized;
        if (shells[0] == null)
            return;
        shells[0].Launch(vec);
        shells[0] = null;
        shells[1].Launch(vec);
        shells[1] = null;
    }
}
