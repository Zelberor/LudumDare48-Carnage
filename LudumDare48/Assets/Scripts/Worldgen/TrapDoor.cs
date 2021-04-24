using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    private GameObject leftDoor = null;
	private GameObject rightDoor = null;
    void Start()
    {
        leftDoor = new GameObject();
		leftDoor.transform.SetParent(this.transform);
		rightDoor = new GameObject();
		rightDoor.transform.SetParent(this.transform);
    }

	public void setDoorMesh(Mesh mesh) {
		
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
