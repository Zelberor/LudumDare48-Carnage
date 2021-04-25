using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    private GameObject zPlusDoor = null;
	private GameObject zMinusDoor = null;

	private Vector2 xzDimensions;

	private Vector3 zPlusDoorGoal = Vector3.zero;
	private Vector3 zMinusDoorGoal = Vector3.zero;

	public void externalConstructor(Vector2 xzDimensions) {
		zPlusDoor = new GameObject();
		zPlusDoor.transform.SetParent(this.transform);
		zPlusDoor.transform.localPosition = Vector3.zero;
		zMinusDoor = new GameObject();
		zMinusDoor.transform.SetParent(this.transform);
		zMinusDoor.transform.localPosition = Vector3.zero;

		this.xzDimensions = xzDimensions;
	}

	public void setDoorMesh(Mesh mesh) {
		RoomTools.addMeshWithCol(zPlusDoor, mesh, RoomTools.doorRenderMaterial, RoomTools.doorPhysicMaterial);
		RoomTools.addKinematicRigidbody(zPlusDoor);
		RoomTools.addMeshWithCol(zMinusDoor, mesh, RoomTools.doorRenderMaterial, RoomTools.doorPhysicMaterial);
		RoomTools.addKinematicRigidbody(zMinusDoor);
		//Rotate zMinusDoor
		zMinusDoor.transform.Rotate(new Vector3(0f, 180f, 0f));
	}

	public void openTrapDoor() {
		zPlusDoorGoal = new Vector3(0f, 0f, xzDimensions.y/2 - 0.1f);
		zMinusDoorGoal = new Vector3(0f, 0f, - (xzDimensions.y/2 - 0.1f));
	}

    // Update is called once per frame
    void Update()
    {
        if (zPlusDoorGoal != zPlusDoor.transform.localPosition) {
			zPlusDoor.transform.localPosition = Vector3.Lerp(zPlusDoor.transform.position, zPlusDoorGoal, Time.deltaTime * RoomTools.doorSpeed);
		}
		if (zMinusDoorGoal != zMinusDoor.transform.localPosition) {
			zMinusDoor.transform.localPosition = Vector3.Lerp(zMinusDoor.transform.position, zMinusDoorGoal, Time.deltaTime * RoomTools.doorSpeed);
		}
    }
}
