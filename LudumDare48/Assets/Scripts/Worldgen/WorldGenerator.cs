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
	public float trapDoorThickness;
	public float doorSpeed;

	//next level variations
	public float biggerModeProb = 0.2f;
	public float smallerModeProb = 0.3f;
	public Vector3 maxDimensions = new Vector3(100, 10, 100);
	public Vector3 minDimensions = new Vector3(10, 3, 10);
	public float transitionVariation = 2f;
	public float minTransition = 1f;

	public GameObject chainsawNosePrefab;
	public float chainsawNoseProb;
	public GameObject cratePrefab;
	public float crateProb;
	private Room currentRoom = null;
	private List<Room> rooms;

    void Start()
    {
        RoomTools.wallPhysicMaterial = this.wallPhysicMaterial;
		RoomTools.doorPhysicMaterial = this.doorPhysicMaterial;
		RoomTools.wallRenderMaterial = this.wallRenderMaterial;
		RoomTools.doorRenderMaterial = this.doorRenderMaterial;
		RoomTools.entityParent = new GameObject("WorldGeneratorEntities");
		RoomTools.trapDoorThickness = this.trapDoorThickness;
		RoomTools.doorSpeed = this.doorSpeed;
		RoomTools.biggerModeProb = this.biggerModeProb;
		RoomTools.smallerModeProb = this.smallerModeProb;
		RoomTools.maxDimensions = this.maxDimensions;
		RoomTools.minDimensions = this.minDimensions;
		RoomTools.transitionVariation = this.transitionVariation;
		RoomTools.minTransition = this.minTransition;

		Vector3 testDimensions = new Vector3(15, 5, 20);

		currentRoom = new Room(startingDimensions, 0f, Color.white);

		//spawnEntities.Add(chainsawNoseProb, chainsawNosePrefab);
		RoomTools.spawnEntities.Add(crateProb, cratePrefab);

		getCurrentRoom().openTrapDoor();

		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();
		currentRoom = currentRoom.createRandNextRoom();

    }

	public Room getCurrentRoom() {
		return currentRoom;
	}
}