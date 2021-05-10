using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float sensitivity { private get;  set; }
    public Rigidbody rg;
    public GameObject player;
    public GameObject head;
    public Shotgun shotty;
    private float movingSpeed; //gets set in script
	private float axisAcceleration = 10f;
	private Vector3 lastTargetVelocityXZ = Vector3.zero;
    public bool isGrounded = true;
	public float groundedTimeout = 0f;
    public float health { get; private set; }
	private float jumpTime = 0f;
    private float ImmunityFrames =0;
    private bool immunityFramesActive = false;
    private float cd;
    private bool isloaded = false;
    private bool SpeedUPActive = false;
    private float SpeedUPDuration =0f;
    private bool DamageUPActive = false;
    private float DamageUPDuration = 0f;
    private float reloadAnimTimeout = 0f;
	private bool reloadAnimDone = true;

	private float walkAnimStopTime = 0f;
	private bool walkAnim = false;

	private bool shootReleased = true;


	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        sensitivity = 1f;
        health = 100;
        rg = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
		anim = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetVelocityXZ = Vector3.zero;

		jumpTime -= Time.deltaTime;
        ImmunityFrames -= Time.deltaTime;
        cd -= Time.deltaTime;
        SpeedUPDuration -= Time.deltaTime;
        DamageUPDuration -= Time.deltaTime;
        reloadAnimTimeout -= Time.deltaTime;
		groundedTimeout -= Time.deltaTime;
		if (walkAnimStopTime >= 0f )
			walkAnimStopTime -= Time.deltaTime;
        if (DamageUPDuration < 0)
        {
            DamageUPDuration = 0;
            DamageUPActive = false;
        }
        if (SpeedUPDuration < 0)
        {
            SpeedUPDuration = 0;
            SpeedUPActive = false;
        }
        if (reloadAnimTimeout <= 0)
        {
            reloadAnimTimeout = 0;
            if (false == DamageUPActive)
            {
                triggerReloadAnimation();
            }
        }
        if (ImmunityFrames < 0)
        {
            ImmunityFrames = 0;
            immunityFramesActive = false;
        }
		if (groundedTimeout <= 0f) {
			groundedTimeout = 0f;
			isGrounded = false;
		}
		if (jumpTime <= 0f)
			jumpTime = 0f;
		
		if (SpeedUPActive) {
        	movingSpeed = 20f;
		} else {
			movingSpeed = (isGrounded) ? 10f : 7f;
		}

        float lrInput = Input.GetAxisRaw("Horizontal");
		float fbInput = Input.GetAxisRaw("Vertical");
		bool jumpInput = Input.GetButtonDown("Jump");
		bool jumpInputHold = Input.GetButton("Jump");
        float firePress = Input.GetAxisRaw("Fire1");
        float mouseXPress = Input.GetAxisRaw("Mouse X");
        float mouseYPress = Input.GetAxisRaw("Mouse Y");

        targetVelocityXZ = new Vector3(targetVelocityXZ.x + lrInput, targetVelocityXZ.y, targetVelocityXZ.z + fbInput);

        if (firePress != 0)
        {
            Shoot();
        } else {
			shootReleased = true;
		}
        if (cd <= 0)
        {
            cd = 0;
            if (!isloaded)
            {
                shotty.Reload();
                isloaded = true;
				shootReleased = false;
            }
        }

		//Jumping
        if (jumpInput && isGrounded) {
			jumpTime = 0.3f;
        } else if (!jumpInputHold) {
			jumpTime = 0f;
		}
		if (jumpTime > 0 && jumpInputHold) {
			rg.velocity = new Vector3(rg.velocity.x, 5f, rg.velocity.z);
		}

        targetVelocityXZ = targetVelocityXZ.normalized * movingSpeed;
		targetVelocityXZ = Vector3.Lerp(lastTargetVelocityXZ, targetVelocityXZ, axisAcceleration * Time.deltaTime);
		lastTargetVelocityXZ = targetVelocityXZ;

        //MouseX:
        player.transform.Rotate(Vector3.up, mouseXPress*sensitivity,Space.Self);
        Vector3 rotatedTargetVelocityXZ = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0) * targetVelocityXZ;
      
		//MouseY:
        head.transform.Rotate(Vector3.left, mouseYPress * sensitivity, Space.Self);
        if (head.transform.localRotation.eulerAngles.x < 285 && head.transform.localRotation.eulerAngles.x > 110)
        {
            head.transform.localRotation = Quaternion.Euler(-75, 0, 0);
        }
        if (head.transform.localRotation.eulerAngles.x > 75 && head.transform.localRotation.eulerAngles.x < 80)
        {
            head.transform.localRotation = Quaternion.Euler(75, 0, 0);
        }


		bool playerInsideDimensionXZ = true;
		bool playerInsideHeight = true;
		bool playerWayTooHigh = false;
		Room currentRoom = RoomTools.lastGeneratedRoom;
		if (currentRoom != null) {
			Vector3 currentDimensions = currentRoom.getDimensions();
			float currentFloorHeight = currentRoom.getFloorHeight();
			playerInsideDimensionXZ = (currentDimensions.x/2 - Mathf.Abs(player.transform.position.x)) > 1.0f && (currentDimensions.z/2 - Mathf.Abs(player.transform.position.z)) > 1.0f;
			playerInsideHeight = (player.transform.position.y - (currentFloorHeight + currentDimensions.y)) < -0.5;
			playerWayTooHigh = (player.transform.position.y - (currentFloorHeight + currentDimensions.y)) > currentDimensions.y*2;
		}

		

    	if(!playerWayTooHigh && (playerInsideHeight || playerInsideDimensionXZ)) {
   			rg.velocity = new Vector3(rotatedTargetVelocityXZ.x, rg.velocity.y, rotatedTargetVelocityXZ.z);
		}

		if (Mathf.Abs(rg.velocity.x) > 0.2f || Mathf.Abs(rg.velocity.z) > 0.2f) {
			anim.SetBool("Walk", true);
		} else {
			if (walkAnim && anim.GetBool("Walk")) {
				walkAnimStopTime = 0.3f;
			}
			if (walkAnimStopTime <= 0) {
				anim.SetBool("Walk", false);
			}
			walkAnim = false;
		}
    }

    private void triggerReloadAnimation()
    {
		if (!reloadAnimDone) {
        	anim.SetTrigger("Reload");
			reloadAnimDone = true;
		}
    }

    private void Shoot()
    {
        
        if (!isloaded || !shootReleased)
            return;

		shotty.Fire(this.gameObject.transform.position);
		isloaded = false;
        if (DamageUPActive) {
            cd = 0.15f;
        }
        else {  
            reloadAnimTimeout = 0.400f;
            cd = 0.700f;
			reloadAnimDone = false;
        }

		anim.SetTrigger("Shoot");
        
    }

    public void Hit(int dmg)
    {
        Debug.Log(health);
        if (immunityFramesActive)
            return;
        health -= dmg;
        immunityFramesActive = true;
        ImmunityFrames = 1.0f;
        if (health <= 0)
        {
            GameManager.PlayerDed();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
      // Debug.Log(other.gameObject);
        switch (other.tag)
        {
            case "ShellShrapnel":
            case "Shell":
              
                return;
              
            case "ChainsawNose":
                Hit(20);
                other.gameObject.GetComponentInParent<ChainsawNose>().Rest();
                return;
            case "HealthUP":
                health += 20;
				other.gameObject.tag = "Untagged";
                GameObject.Destroy( other.gameObject.GetComponentInParent<HealthPickup>().gameObject);
                //Debug.Log(health);
                return;
            case "SpeedUP":
                SpeedUPActive = true;
				other.gameObject.tag = "Untagged";
                GameObject.Destroy(other.gameObject.GetComponentInParent<SpeedPickup>().gameObject);
                SpeedUPDuration = 5f;
                return;
            case "DamageUP":
                DamageUPActive = true;
				other.gameObject.tag = "Untagged";
                GameObject.Destroy(other.gameObject.GetComponentInParent<DamagePickup>().gameObject);
                DamageUPDuration = 5f;
                return;
            default: return;
        }
    }

	public void OnCollisionStay(Collision collisionInfo) {
		foreach (var item in collisionInfo.contacts)
		{
			if (item.point.y < player.transform.position.y + 0.25f) {
				isGrounded = true;
				groundedTimeout = 0.2f;
			}
		}
	}
}
