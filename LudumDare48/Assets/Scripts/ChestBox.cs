using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBox : MonoBehaviour
{

    public GameObject healthPrefab, SpeedPrefab, DamagePrefab;
   
    private int health = 1000;
    private GameObject[] PowerUPprefabs;
    public WoodShrapnel shrapnel;
    public GameObject pivotxp, pivotxn, pivotyp, pivotyn, pivott;
    private bool unRegistered = false;
    private static float HealthWeight = 5f, SpeedWeight = 5f, DamageWeight = 3f, NothingWeight = 75f;
    private float[] weights = { HealthWeight, SpeedWeight, DamageWeight, NothingWeight };
    // Start is called before the first frame update
    void Start()
    {
        PowerUPprefabs = new GameObject[4];      //     { healthPrefab, SpeedPrefab, DamagePrefab, null };
        PowerUPprefabs[0] = healthPrefab;
        PowerUPprefabs[1] = SpeedPrefab;
        PowerUPprefabs[2] = DamagePrefab;
        PowerUPprefabs[3] = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            GoCommitDie();
        } 
    }

    private void GoCommitDie()
    {
        if (!unRegistered)
        {
            RoomTools.entityDestroyed("Crate");
            unRegistered = true;
        }
        GameObject.Destroy(this.gameObject);
        var PUP = GameManager.roll<GameObject>(PowerUPprefabs, weights);
        if (PUP != null)
        {
            PUP = GameObject.Instantiate(PUP);
            PUP.transform.position = this.gameObject.transform.position;
        
        }
    
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ShellShrapnel")
        {
            health = -10;
            OnTriggerEnterShrapnel(other);
            foreach (var item in other.gameObject.GetComponents<SphereCollider>())
            {
                item.enabled = false;
            
            }
        }
        if (other.tag == "Shell")
        { 
            health = 0;
            GenerateKaboom();
            ((Shell)other.gameObject).directShellHitOnShell();
            foreach (var item in other.gameObject.GetComponents<CapsuleCollider>())
            {
                item.enabled = false;

            }
        }


         }

    private void GenerateKaboom()
    {

        var shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotxp.transform.position;
        shrapnelinit.transform.rotation = pivotxp.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);
        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotxn.transform.position;
                shrapnelinit.transform.rotation = pivotxn.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);

        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivott.transform.position;
        shrapnelinit.transform.rotation = pivott.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);


        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotyp.transform.position;
        shrapnelinit.transform.rotation = pivotyp.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);

        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotyn.transform.position;
        shrapnelinit.transform.rotation = pivotyn.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);
      shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotxp.transform.position;
        shrapnelinit.transform.rotation = pivotxp.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);
        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotxn.transform.position;
        shrapnelinit.transform.rotation = pivotxn.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);

        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivott.transform.position;
        shrapnelinit.transform.rotation = pivott.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);


        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotyp.transform.position;
        shrapnelinit.transform.rotation = pivotyp.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);

        shrapnelinit = GameObject.Instantiate(shrapnel);
        shrapnelinit.transform.position = pivotyn.transform.position;
        shrapnelinit.transform.rotation = pivotyn.transform.rotation;
        shrapnelinit.Launch(this.gameObject.transform.position);
    }

    void OnTriggerEnterShrapnel(Collider other)
    {
        var vector = (other.attachedRigidbody.gameObject.transform.position - this.gameObject.transform.position);
      //  Debug.Log(vector);
        
        var xabs = Mathf.Abs(vector.x); var yabs = Mathf.Abs(vector.z);
        var res = Mathf.Max(xabs, yabs);
        var shrapnelinit = GameObject.Instantiate(shrapnel);
        if (res == xabs)
        {
            if (vector.x == xabs)
            {
                //Debug.Log("px");
                shrapnelinit.transform.position = pivotxp.transform.position;
                shrapnelinit.transform.rotation = pivotxp.transform.rotation;

            }
            else
            {


               // Debug.Log("nx");
                shrapnelinit.transform.position = pivotxn.transform.position;
                shrapnelinit.transform.rotation = pivotxn.transform.rotation;


            }

        }
        else {

          
            if (vector.y == yabs)
            {  //Debug.Log("py");
                shrapnelinit.transform.position = pivotyp.transform.position;
                shrapnelinit.transform.rotation = pivotyp.transform.rotation;

            }
            else
            {
              //  Debug.Log("ny");
                shrapnelinit.transform.position = pivotyn.transform.position;
                shrapnelinit.transform.rotation = pivotyn.transform.rotation;


            }


        }
        Destroy(other.gameObject);
        shrapnelinit.Launch(this.gameObject.transform.position);





    }
}
