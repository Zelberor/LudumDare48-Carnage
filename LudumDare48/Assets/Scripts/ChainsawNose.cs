using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChainsawNose : MonoBehaviour
{

    public GameObject healthPrefab, SpeedPrefab, DamagePrefab;
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

	private bool isGrounded = false;
	private float groundedTimeout = 0f;

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
		agent = this.gameObject.GetComponent<NavMeshAgent>();
		agent.updateUpAxis = false;
    }   

    // Update is called once per frame
    void Update()
    {
		if (groundedTimeout > 0f)
			groundedTimeout -= Time.deltaTime;
		else
			isGrounded = false;
		
        if((agent.pathStatus == NavMeshPathStatus.PathInvalid && !isGrounded) || dead || downtime > 0f) {
			if (downtime > 0f)
				downtime -= Time.deltaTime;
			if (getAgentEnabled())
            	setAgentEnabled(false);
        } else {
			if (!getAgentEnabled() && isGrounded) {
            	setAgentEnabled(true);
			}
			if (agent.isOnNavMesh)
				agent.destination = player.transform.position;
        }

		if (!dead) {
			if (agent.hasPath && agent.updatePosition && agent.destination != this.transform.position) {
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
    }

    private void OnTriggerEnter(Collider other)
    {
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
       	setAgentEnabled(false);
        this.rb.useGravity = false;
        rb.drag = 0.25f;
		rb.angularDrag = 0.25f;

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

	private void setAgentEnabled(bool enabled) {
		agent.updatePosition = enabled;
		agent.updateRotation = enabled;
		agent.enabled = enabled;
		rb.isKinematic = enabled;
	}

	private bool getAgentEnabled() {
		return agent.updatePosition && agent.updateRotation && rb.isKinematic && agent.enabled;
	}

	public void OnCollisionStay(Collision collisionInfo) {
		foreach (var item in collisionInfo.contacts)
		{
			if (item.point.y < this.transform.position.y + 0.25f) {
				isGrounded = true;
				groundedTimeout = 0.2f;
			}
		}
	}
}
