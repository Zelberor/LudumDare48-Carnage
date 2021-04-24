using UnityEngine;
using System.Collections.Generic;

public class Room {

	private static int id = 0;
	private Vector3 dimensions; //x: width, y:height, z:length
	private float floorHeight;
	private GameObject roomObj;
	private GameObject doorObj;
	private TrapDoor door;

	public Room(Vector3 dimensions, float floorHeight) {
		this.dimensions = dimensions;
		this.floorHeight = floorHeight;
		++Room.id;
		roomObj = new GameObject("Room" + Room.id);
		doorObj = new GameObject("Door" + Room.id);
		doorObj.transform.SetParent(roomObj.transform);
		door = doorObj.AddComponent<TrapDoor>();
		roomObj.transform.Translate(new Vector3(0f, floorHeight, 0f));
		this.generate();
	}

	private bool generate() {
		generateRoomWall();
		generateTrapDoor();
		generateEntities();
		return true;
	}

	private bool generateRoomWall() {
		QuadMesh meshBuilder = new QuadMesh();
		Vector3 bottomLeft, topLeft, topRight, bottomRight;
		float lowerHeight = 0f;
		float upperHeight = dimensions.y;

		//+x Wall
		bottomLeft = new Vector3(dimensions.x/2, lowerHeight, dimensions.z/2);
		topLeft = new Vector3(dimensions.x/2, upperHeight, dimensions.z/2);
		topRight = new Vector3(dimensions.x/2, upperHeight, -dimensions.z/2);
		bottomRight = new Vector3(dimensions.x/2, lowerHeight, -dimensions.z/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//-x Wall
		bottomLeft = new Vector3(-dimensions.x/2, lowerHeight, -dimensions.z/2);
		topLeft = new Vector3(-dimensions.x/2, upperHeight, -dimensions.z/2);
		topRight = new Vector3(-dimensions.x/2, upperHeight, dimensions.z/2);
		bottomRight = new Vector3(-dimensions.x/2, lowerHeight, dimensions.z/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//+z Wall
		bottomLeft = new Vector3(-dimensions.x/2, lowerHeight, dimensions.z/2);
		topLeft = new Vector3(-dimensions.x/2, upperHeight, dimensions.z/2);
		topRight = new Vector3(dimensions.x/2, upperHeight, dimensions.z/2);
		bottomRight = new Vector3(dimensions.x/2, lowerHeight, dimensions.z/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//-z Wall
		bottomLeft = new Vector3(dimensions.x/2, lowerHeight, -dimensions.z/2);
		topLeft = new Vector3(dimensions.x/2, upperHeight, -dimensions.z/2);
		topRight = new Vector3(-dimensions.x/2, upperHeight, -dimensions.z/2);
		bottomRight = new Vector3(-dimensions.x/2, lowerHeight, -dimensions.z/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);

		Mesh wallMesh = meshBuilder.GetMesh();
		MeshFilter mFilter = roomObj.AddComponent<MeshFilter>();
		mFilter.mesh = wallMesh;
		MeshRenderer renderer = roomObj.AddComponent<MeshRenderer>();
		renderer.material = RoomTools.wallRenderMaterial;
		return true;
	}

	//Called when createNextRoom is called
	private bool generateTransitionWall(Vector3 nextDimensions, float transitionHeight) {
		return true;
	}

	private bool generateTrapDoor() {
		return true;
	}

	private bool generateEntities() {
		return true;
	}

	public bool openTrapDoor() {
		return true;
	}

	public Room createNextRoom(Vector3 nextRoomDimensions) {
		float transitionHeight = 0f;
		return new Room(nextRoomDimensions, transitionHeight);
	}
}

public static class RoomTools {
	public static PhysicMaterial wallPhysicMaterial;
	public static PhysicMaterial doorPhysicMaterial;
	public static Material wallRenderMaterial;
	public static Material doorRenderMaterial;
	public static GameObject entityParent;
}