using UnityEngine;
using System.Collections.Generic;

public class Room {

	private static int id = 0;
	private Vector3 dimensions; //x: width, y:height, z:length
	private float floorHeight;
	private GameObject roomObj;
	private GameObject doorObj = null;
	private TrapDoor door;

	public Room(Vector3 dimensions, float floorHeight) {
		this.dimensions = dimensions;
		this.floorHeight = floorHeight;
		++Room.id;
		roomObj = new GameObject("Room" + Room.id);
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
		Vector2 xzDimensions = new Vector2(dimensions.x, dimensions.z);
		GameObject wallObj = new GameObject("Wall");
		wallObj.transform.SetParent(roomObj.transform);
		wallObj.transform.localPosition = Vector3.zero;
		generateWall(wallObj, xzDimensions, xzDimensions, 0f, dimensions.y);
		return true;
	}

	//Called when createNextRoom is called
	private bool generateTransitionWall(Vector3 nextDimensions, float transitionHeight) {
		Vector2 xzDimensions = new Vector2(dimensions.x, dimensions.z);
		Vector2 xzNextDimensions = new Vector2(nextDimensions.x, nextDimensions.z);
		GameObject wallObj = new GameObject("TransitionWall");
		wallObj.transform.SetParent(roomObj.transform);
		wallObj.transform.localPosition = Vector3.zero;
		generateWall(wallObj, xzNextDimensions, xzDimensions, -transitionHeight, 0f);
		return true;
	}

	private bool generateTrapDoor() {
		doorObj = new GameObject("Door" + Room.id);
		doorObj.transform.SetParent(roomObj.transform);
		doorObj.transform.localPosition = Vector3.zero;
		door = doorObj.AddComponent<TrapDoor>();

		//Generate only +z side of trapdoor
		return true;
	}

	private bool generateEntities() {
		return true;
	}

	public bool openTrapDoor() {
		return true;
	}

	public Room createNextRoom(Vector3 nextRoomDimensions, float transitionHeight) {
		if (transitionHeight < 0)
			transitionHeight = 0;
		if (!(transitionHeight == 0 || this.dimensions == nextRoomDimensions)) {
			generateTransitionWall(nextRoomDimensions, transitionHeight);
		}
		return new Room(nextRoomDimensions, this.floorHeight - nextRoomDimensions.y - transitionHeight);
	}

	private bool generateWall(GameObject host, Vector2 bottomSize, Vector2 topSize, float lowerHeight, float upperHeight) {
		QuadMesh meshBuilder = new QuadMesh();
		Vector3 bottomLeft, topLeft, topRight, bottomRight;

		//+x Wall
		bottomLeft = new Vector3(bottomSize.x/2, lowerHeight, bottomSize.y/2);
		topLeft = new Vector3(topSize.x/2, upperHeight, topSize.y/2);
		topRight = new Vector3(topSize.x/2, upperHeight, -topSize.y/2);
		bottomRight = new Vector3(bottomSize.x/2, lowerHeight, -bottomSize.y/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//-x Wall
		bottomLeft = new Vector3(-bottomSize.x/2, lowerHeight, -bottomSize.y/2);
		topLeft = new Vector3(-topSize.x/2, upperHeight, -topSize.y/2);
		topRight = new Vector3(-topSize.x/2, upperHeight, topSize.y/2);
		bottomRight = new Vector3(-bottomSize.x/2, lowerHeight, bottomSize.y/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//+z Wall
		bottomLeft = new Vector3(-bottomSize.x/2, lowerHeight, bottomSize.y/2);
		topLeft = new Vector3(-topSize.x/2, upperHeight, topSize.y/2);
		topRight = new Vector3(topSize.x/2, upperHeight, topSize.y/2);
		bottomRight = new Vector3(bottomSize.x/2, lowerHeight, bottomSize.y/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);
		//-z Wall
		bottomLeft = new Vector3(bottomSize.x/2, lowerHeight, -bottomSize.y/2);
		topLeft = new Vector3(topSize.x/2, upperHeight, -topSize.y/2);
		topRight = new Vector3(-topSize.x/2, upperHeight, -topSize.y/2);
		bottomRight = new Vector3(-bottomSize.x/2, lowerHeight, -bottomSize.y/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);

		Mesh wallMesh = meshBuilder.GetMesh();
		RoomTools.setMeshWithCol(host, wallMesh, RoomTools.wallRenderMaterial, RoomTools.wallPhysicMaterial);
		return true;
	}

	public Vector3 getDimensions() {
		return this.dimensions;
	}

	public float getFloorHeight() {
		return floorHeight;
	}
}

public static class RoomTools {
	public static PhysicMaterial wallPhysicMaterial;
	public static PhysicMaterial doorPhysicMaterial;
	public static Material wallRenderMaterial;
	public static Material doorRenderMaterial;
	public static GameObject entityParent;
	public static float trapDoorThickness;

	public static void setMeshWithCol(GameObject host, Mesh mesh, Material renderMaterial, PhysicMaterial physicMaterial) {
		MeshFilter mFilter = host.AddComponent<MeshFilter>();
		mFilter.mesh = mesh;
		MeshRenderer renderer = host.AddComponent<MeshRenderer>();
		renderer.material = renderMaterial;
		MeshCollider collider = host.AddComponent<MeshCollider>();
		collider.material = physicMaterial;
	}
}