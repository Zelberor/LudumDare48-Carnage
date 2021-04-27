using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodShrapnel : MonoBehaviour
{

    public GameObject[] cubes;
    float ttl = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0)
        {
            delet();
        
        }
    }

    private void delet()
    {
        Destroy(this.gameObject);
    }


    internal void Launch(Vector3 pos)
    {

        foreach (var cube in cubes)
        {
            ///   cube.GetComponent<Rigidbody>().AddForce((cube.transform.position - pos).normalized * .015f);
            var connectingVector = (cube.transform.position - pos).normalized;
            var connectingVectorNoY = new Vector3(connectingVector.x, 0, connectingVector.z).normalized;
            connectingVector = Vector3.Normalize( new Vector3(0, .75f, 0) + connectingVectorNoY);
            cube.GetComponent<Rigidbody>().AddForce(connectingVector * .03f);









        }




        }
}
