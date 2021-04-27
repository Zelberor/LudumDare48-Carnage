using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    private float rotateSpeed = 1f;
    public GameObject representation;

	private float timeLeft = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, rotateSpeed);
        representation.transform.localPosition = new Vector3(representation.transform.localPosition.x, 1f + 0.5f * (1f + Mathf.Sin(Time.time)), representation.transform.localPosition.z);
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0) {
			Destroy(this.gameObject);
		}
    }
}
