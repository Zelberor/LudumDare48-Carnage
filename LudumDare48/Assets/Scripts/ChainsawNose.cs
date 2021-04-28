using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChainsawNose : MonoBehaviour
{

    public GameObject healthPrefab, SpeedPrefab, DamagePrefab;

    //private int health = 1000;
    private GameObject[] PowerUPprefabs;
    private int health=75;
    public Rigidbody rb;
    public NavMeshAgent agent;
    public GameObject player;
    private float downtime = 0f;

    private static float HealthWeight = 8f, SpeedWeight = 3f, DamageWeight = 0.5f, NothingWeight = 100f;
    private float[] weights = { HealthWeight, SpeedWeight, DamageWeight, NothingWeight };

    private bool unRegistered = false;

	private Animator anim;

	private bool dead = false;

    NavMeshAgent mem;
  //  public GameManager gm;
  //  private bool usegm = true;
    // Start is called before the first frame update
    void Start()
    {
        // rb.isKinematic = false;

        PowerUPprefabs = new GameObject[4];      //     { healthPrefab, SpeedPrefab, DamagePrefab, null };
        PowerUPprefabs[0] = healthPrefab;
        PowerUPprefabs[1] = SpeedPrefab;
        PowerUPprefabs[2] = DamagePrefab;
        PowerUPprefabs[3] = null;
        downtime = 2.5f;
        player = GameObject.FindGameObjectWithTag("Player");
		anim = this.GetComponentInChildren<Animator>();
    }   

    // Update is called once per frame
    void Update()
    {
        if((-0.25  > player.transform.position.y - this.gameObject.transform.position.y))
        {
            mem = this.gameObject.GetComponent<NavMeshAgent>();
            this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            rb.isKinematic = false;
            return;
        }
        else 
        {
            this.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            if (downtime != 0)
            {
                rb.isKinematic = true;

            }

        }
        if (downtime == 0 && agent.isOnNavMesh)
            agent.destination = player.transform.position;
        else {
            downtime -= Time.deltaTime;
            downtime = Mathf.Max(downtime, 0);
            rb.isKinematic = true;
			if (agent.isOnNavMesh)
            	agent.destination = player.gameObject.transform.position; 
        }

		if (!dead) {
			if (agent.hasPath && agent.destination != this.transform.position) {
				anim.SetBool("Walk", true);
			} else {
				anim.SetBool("Walk", false);
			}

			Vector2 xzPos = new Vector2(this.transform.position.x, this.transform.position.z);
			Vector2 xzPlayerPos = new Vector2(player.transform.position.x, player.transform.position.z);


			if ((xzPos - xzPlayerPos).magnitude < 2.5f ) {
				anim.SetTrigger("Attack");
			}
		}
    }

    public void Rest()
    {
        //if hit, take a nap ;)
        downtime = 0.5f;
        agent.destination = gameObject.transform.position;
        rb.isKinematic = false;
        
    
    
    
    
    }


    private void OnTriggerEnter(Collider other)
    {
     //   Debug.Log("TriggerEnterEnemy");
        switch (other.tag)
        {
            case "ShellShrapnel":
                Hit(5);
                foreach (var item in other.gameObject.GetComponents<SphereCollider>())
                {
                    item.enabled = false;

                }
                Destroy(other);
                return;
            case "Shell":
                Hit(30);
                foreach (var item in other.gameObject.GetComponents<CapsuleCollider>())
                {
                    item.enabled = false;

                }
              	((Shell) other.gameObject).directShellHitOnShell();
                return;
            case "WoodShrapnel":
                Hit(2);
                foreach (var item in other.gameObject.GetComponents<BoxCollider>())
                {
                    item.enabled = false;

                }
                Destroy(other);
                break;
            case "Player":
                Rest();
                return;
            default: return;
          

        }
    }

    private void Hit(int v)
    {
       	// Debug.Log(health);
        health -= v;
        health = Mathf.Max(0, health);
        if (health == 0)
        {
            Killme();
        }
		if (!dead) {
	   		anim.SetTrigger("TakeDamage");
		}
    }

    private void Killme()
    {
		dead = true;
        this.GetComponent<NavMeshAgent>().updatePosition = false;
		this.GetComponent<NavMeshAgent>().updateRotation = false;
        this.rb.isKinematic = false;
        this.rb.useGravity = false;
        rb.velocity = Vector3.zero;

        this.rb.detectCollisions = false;
        this.GetComponent<BoxCollider>().enabled = false;

        var PUP = GameManager.roll<GameObject>(PowerUPprefabs, weights);
        if (PUP != null)
        {
            PUP = GameObject.Instantiate(PUP);
            PUP.transform.position = this.gameObject.transform.position;

        }
        if (!unRegistered)
        {
            RoomTools.entityDestroyed("ChainsawNose");
            unRegistered = true;
        }

		anim.SetBool("Walk", true);
		anim.SetBool("Die", true);
        GameObject.Destroy(this.gameObject,5);
    }
}
