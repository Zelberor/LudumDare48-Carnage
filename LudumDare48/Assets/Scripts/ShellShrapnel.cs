using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellShrapnel : MonoBehaviour
{

    public GameObject ushrapnel, oshrapnel, lshrapnel, rshrapnel, cshrapnel;
    float lifeTIM = 15f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTIM -= Time.deltaTime;
        if (lifeTIM <= 0)
        {
            //YEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEET
            GameObject.Destroy(this.gameObject);
        
        }
    }

   public void Fragementate(Rigidbody rawData)
    {
        this.transform.position = rawData.position;
        var ushrapnelrigid = ushrapnel.GetComponent<Rigidbody>();
        var oshrapnelrigid = oshrapnel.GetComponent<Rigidbody>();
        var lshrapnelrigid = lshrapnel.GetComponent<Rigidbody>();
        var rshrapnelrigid = rshrapnel.GetComponent<Rigidbody>();
        var cshrapnelrigid = cshrapnel.GetComponent<Rigidbody>();
        var raw = rawData.velocity;

        ushrapnelrigid.velocity = raw;
        oshrapnelrigid.velocity = raw;
        lshrapnelrigid.velocity = raw;
        rshrapnelrigid.velocity = raw;
        cshrapnelrigid.velocity = raw;

        ushrapnelrigid.AddForce(new Vector3(0,-.15f,0));
        oshrapnelrigid.AddForce(new Vector3(0, .15f, 0));
        cshrapnelrigid.AddForce(.15f*raw.normalized);
        lshrapnelrigid.AddForce(new Vector3(.15f, 0, 0));
        lshrapnelrigid.AddForce(new Vector3(-.15f, 0, 0));

    }
}
