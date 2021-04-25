using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

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
	public int maxChainsawNoses;
	public GameObject cratePrefab;
	public float crateProb;
	public int maxCrates;
	public int maxRooms = 10;

	private Room currentRoom = null;
	private List<Room> rooms = new List<Room>();

	private NavMeshSurface navMesh = null;

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
		rooms.Add(currentRoom);

		RoomTools.spawnEntities.Add("Crate", new EntityPrefab(cratePrefab, crateProb, maxCrates));
		RoomTools.spawnEntities.Add("ChainsawNose", new EntityPrefab(chainsawNosePrefab, chainsawNoseProb, maxChainsawNoses));

		goToNextRoom();
		goToNextRoom();
    }

	public Room getCurrentRoom() {
		return currentRoom;
	}

	public void goToNextRoom() {
		Room oldRoom = getCurrentRoom();
		currentRoom = currentRoom.createRandNextRoom();
		rooms.Add(currentRoom);
		if (rooms.Count > maxRooms) {
			//Add proper Destroy
			rooms[0].DestroyRoom();
			rooms.RemoveAt(0);
		}
		updateNavMesh(oldRoom);
		oldRoom.openTrapDoor();
	}

	private void updateNavMesh(Room oldRoom) {
		float overlap = 0.5f;
		if (navMesh == null) {
			navMesh = this.gameObject.AddComponent<NavMeshSurface>();
			navMesh.collectObjects = CollectObjects.Volume;
			navMesh.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
		}
		Vector3 currentDimensions = getCurrentRoom().getDimensions();
		Vector3 oldDimensions = oldRoom.getDimensions();
		float currentFloorHeight = getCurrentRoom().getFloorHeight();
		float oldFloorHeight = oldRoom.getFloorHeight();
		navMesh.navMeshData = null;
		float maxXDimension = Math.Max(currentDimensions.x, oldDimensions.x);
		float maxZDimension = Math.Max(currentDimensions.z, oldDimensions.z);
		float combinedHeight = Math.Abs(oldFloorHeight - currentFloorHeight) + oldDimensions.y;
		Vector3 navSize = new Vector3(maxXDimension + overlap, combinedHeight, maxZDimension + overlap);
		navMesh.size = navSize;
		navMesh.center = new Vector3(0f, currentFloorHeight + navSize.y/2 - overlap - trapDoorThickness, 0f);
		navMesh.BuildNavMesh();
	}
}