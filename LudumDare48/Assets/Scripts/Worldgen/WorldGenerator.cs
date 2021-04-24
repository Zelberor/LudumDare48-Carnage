using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	public PhysicMaterial wallPhysicMaterial;
	public PhysicMaterial doorPhysicMaterial;
	public Material wallRenderMaterial;
	public Material doorRenderMaterial;
	public Vector3 startingDimensions;
    private float currentFloorHeight = 0f;
	private Room room = null;

    void Start()
    {
        RoomTools.wallPhysicMaterial = this.wallPhysicMaterial;
		RoomTools.doorPhysicMaterial = this.doorPhysicMaterial;
		RoomTools.wallRenderMaterial = this.wallRenderMaterial;
		RoomTools.doorRenderMaterial = this.doorRenderMaterial;
		RoomTools.entityParent = new GameObject("WorldGeneratorEntities");

		room = new Room(startingDimensions, 0f);
    }
}