using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private bool alive =true;
    public CapsuleCollider mcollider;
    private float livetime = 0.1f;
    private readonly float firepower = 1500;
    bool launched = false;
    public ShellShrapnel shrap; 
    // Start is called before the first frame update
    void Start()
    {
        mcollider.enabled = false;
        this.GetComponent<Rigidbody>().detectCollisions = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (launched && alive)
        {
            mcollider.enabled = true;
            this.GetComponent<Rigidbody>().detectCollisions = true;
            livetime -= Time.deltaTime;
        }
        if (livetime <= 0)
        {
            var shrapinit = GameObject.Instantiate(shrap);
            shrapinit.Fragementate(this.gameObject.GetComponent<Rigidbody>());
            alive = false;
            GameObject.Destroy(this.gameObject);
        
        
        
        
        
        }
    }
    public void directShellHitOnShell()
    {
        alive = false;
        GameObject.Destroy(this.gameObject);

    }
    internal void Launch(Vector3 eulerAngles)
    {
      //  Debug.Log(eulerAngles);
        launched = true;
        this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.gameObject.transform.parent = null;
        this.gameObject.GetComponent<Rigidbody>().AddForce(eulerAngles * firepower);
    }

    public static explicit operator Shell(GameObject v)
    {
        return v.GetComponent<Shell>();
    }
}
