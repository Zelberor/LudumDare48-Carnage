using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float sensitivity { private get;  set; }
    public Rigidbody rg;
    public GameObject player;
    public GameObject head;
    //   private GameManager gameManager;
    public Shotgun shotty;
    private float movingSpeed = 10f;
    private readonly float JumpForce = 100.0f;
    // private readonly float fallingAcceleration = -9.80665f;


    public bool isGrounded = true;
    public float health { get; private set; }
    // private Camera cam;
    private float jumplock = 0;
    //  private float changeLock = 0;
    Vector3 vel = new Vector3(0, 0, 0);
    private float ImmunityFrames =0;
    private bool immunityFramesActive = false;
    private float cd;
    private bool isloaded = true;
    private bool aircontrol = true;
    private bool SpeedUPActive = false;
    private float SpeedUPDuration =0f;
    private bool DamageUPActive = false;
    private float DamageUPDuration = 0f;
    private float PeterCD = 0f;
	private bool PeterCDDone = true;

	private float walkStopTime = 0f;
	private bool walk = false;

	private bool shootReleased = true;


	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        sensitivity = 1f;
        //   cam = GameObject.Find("PlayerMain").GetComponentInChildren<Camera>();

        health = 100;
        rg = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        // gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        //   cam = Camera.main;
		anim = this.GetComponentInChildren<Animator>();
    }

    internal void setMovementEnabelt(bool v)
    {
        this.aircontrol = v;
        
    }

    // Update is called once per frame
    void Update()
    {
        movingSpeed = (SpeedUPActive) ? 25f : 14f;
        vel = new Vector3(0, 0, 0);
        //   rg.gravityScale = 1;
        isGrounded = ((Mathf.Abs(rg.velocity.y) < 0.001f));
        ImmunityFrames -= Time.deltaTime;
        jumplock -= Time.deltaTime;
        cd -= Time.deltaTime;
        SpeedUPDuration -= Time.deltaTime;
        DamageUPDuration -= Time.deltaTime;
        PeterCD -= Time.deltaTime;
		if (walkStopTime >= 0f )
			walkStopTime -= Time.deltaTime;
        if (jumplock < 0) jumplock = 0f;

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
        if (PeterCD <= 0)
        {
            PeterCD = 0;
            if (false == DamageUPActive)
            {
                PeterSchreibHierDeinenRanzRein();
            }
        
        }

        if (ImmunityFrames < 0)
        {
            ImmunityFrames = 0;
            immunityFramesActive = false;
        }
        //float horizontalSpeed;
        float horizontalInpit = Input.GetAxis("Horizontal"); // ad , lr sidestep
        float heightInput = Input.GetAxis("Jump");         //spaaaaaaace
        float walkInput = Input.GetAxis("Vertical");           //ws, vh Vorrrrrrw�rrrts!
        float firePress = Input.GetAxis("Fire1");               //bumm!
        float mouseXPress = Input.GetAxis("Mouse X");           //drehen
        float mouseYPress = Input.GetAxis("Mouse Y");           //Kopf nicken
        if (horizontalInpit != 0)
        {

            var looking_dir = (int)Input.GetAxisRaw("Horizontal");
            vel = new Vector3(vel.x + looking_dir, vel.y, vel.z);



        }
        if (walkInput != 0)
        {

            var walkingforward = (int)Input.GetAxisRaw("Vertical");
            vel = new Vector3(vel.x, vel.y, vel.z + walkingforward);



        }
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
                shotty.Reload(DamageUPActive);
                isloaded = true;
				shootReleased = false;
            }
        }
        if (jumplock == 0 && isGrounded)
        {
            rg.AddForce(new Vector2(0, (JumpForce * heightInput * rg.mass)));

        }

        vel = vel.normalized * movingSpeed;

        //MouseX:

        player.transform.Rotate(Vector3.up, mouseXPress*sensitivity,Space.Self);
        vel = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0) * vel;
        vel = new Vector3(vel.x, rg.velocity.y, vel.z);
      

        head.transform.Rotate(Vector3.left, mouseYPress * sensitivity, Space.Self);



        
        if (head.transform.localRotation.eulerAngles.x < 285 && head.transform.localRotation.eulerAngles.x > 110)
        {
            head.transform.localRotation = Quaternion.Euler(-75, 0, 0);
       
        }
        if (head.transform.localRotation.eulerAngles.x > 75 && head.transform.localRotation.eulerAngles.x < 80)
        {

            head.transform.localRotation = Quaternion.Euler(75, 0, 0);
        }

    	if(aircontrol)
   			rg.velocity = vel;

		if (Mathf.Abs(rg.velocity.x) > 0.2f || Mathf.Abs(rg.velocity.z) > 0.2f) {
			anim.SetBool("Walk", true);
		} else {
			if (walk && anim.GetBool("Walk")) {
				walkStopTime = 0.3f;
			}
			if (walkStopTime <= 0) {
				anim.SetBool("Walk", false);
			}
			walk = false;
		}
      
    }

    private void PeterSchreibHierDeinenRanzRein()
    {
		if (!PeterCDDone) {
        	anim.SetTrigger("Reload");
			PeterCDDone = true;
		}
    }

    private void Shoot()
    {
        
        if (!isloaded || !shootReleased)
            return;

        if (DamageUPActive)
        {
            shotty.Fire(this.gameObject.transform.position);
            cd = 0.15f;
            isloaded = false;
        }
        else { 
            shotty.Fire(this.gameObject.transform.position);
            PeterCD = 0.400f;
            cd = 0.700f;
            isloaded = false;
			PeterCDDone = false;
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
        ImmunityFrames = 1.5f;
        if (health <= 0)
        {
            GameManager.PlayerDed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
      // Debug.Log(other.gameObject);
        switch (other.tag)
        {
            case "ShellShrapnel":
            case "Shell":
              
                return;
              
            case "ChainsawNose":
                Hit(20);
                //  Destroy(other);
                other.gameObject.GetComponentInParent<ChainsawNose>().Rest();
                return;
            case "HealthUP":
                health += 20;
				other.gameObject.tag = "";
                GameObject.Destroy( other.gameObject.GetComponentInParent<HealthPickup>().gameObject);
                //Debug.Log(health);
                return;
            case "SpeedUP":
                SpeedUPActive = true;
				other.gameObject.tag = "";
                GameObject.Destroy(other.gameObject.GetComponentInParent<SpeedPickup>().gameObject);
                SpeedUPDuration = 5f;
                return;
            case "DamageUP":
                DamageUPActive = true;
				other.gameObject.tag = "";
                GameObject.Destroy(other.gameObject.GetComponentInParent<DamagePickup>().gameObject);
                DamageUPDuration = 5f;
                return;













            default: return;
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        }
       
    }







}
