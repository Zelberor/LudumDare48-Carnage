using UnityEngine;
using System;
using System.Collections.Generic;

public class Room {

	private static int id = 0;
	private Vector3 dimensions; //x: width, y:height, z:length
	private float floorHeight;
	private GameObject roomObj;
	private GameObject doorObj = null;
	private TrapDoor door;

	private Color roomColor;

	public Room(Vector3 dimensions, float floorHeight, Color color) {
		this.roomColor = color;
		this.dimensions = dimensions;
		this.floorHeight = floorHeight;
		Room.id = Room.id + 1;
		roomObj = new GameObject("Room" + Room.id);
		roomObj.transform.Translate(new Vector3(0f, floorHeight, 0f));
		this.generate();
		RoomTools.lastGeneratedRoom = this;
	}

	private bool generate() {
		generateRoomWall();
		generateTrapDoor();
		generateLight();
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

	private void generateLight() {
		GameObject lightObj = new GameObject("Light");
		lightObj.transform.SetParent(roomObj.transform);
		lightObj.transform.localPosition = new Vector3(0, dimensions.y/2, 0);
		Light light = lightObj.AddComponent<Light>();
		light.type = LightType.Point;
		light.range = Math.Max(dimensions.x, dimensions.z);
		light.shadows = LightShadows.None;
		light.intensity = 70/light.range;
		light.color = roomColor;
	}

	private bool generateTrapDoor() {
		doorObj = new GameObject("Door" + Room.id);
		doorObj.transform.SetParent(roomObj.transform);
		doorObj.transform.localPosition = Vector3.zero;
		door = doorObj.AddComponent<TrapDoor>();
		door.externalConstructor(new Vector2(dimensions.x, dimensions.z));

		//Generate only +z side of trapdoor
		QuadMesh meshBuilder = new QuadMesh();
		Vector3 bottomLeft, topLeft, topRight, bottomRight;

		float upperHeight = 0f;
		float lowerHeight = -RoomTools.trapDoorThickness;

		//upper plane
		bottomLeft = new Vector3(-dimensions.x/2, upperHeight, 0f);
		topLeft = new Vector3(-dimensions.x/2, upperHeight, dimensions.z/2);
		topRight = new Vector3(dimensions.x/2, upperHeight, dimensions.z/2);
		bottomRight = new Vector3(dimensions.x/2, upperHeight, 0f);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);

		//lower plane
		bottomLeft = new Vector3(-dimensions.x/2, lowerHeight, dimensions.z/2);
		topLeft = new Vector3(-dimensions.x/2, lowerHeight, 0f);
		topRight = new Vector3(dimensions.x/2, lowerHeight, 0f);
		bottomRight = new Vector3(dimensions.x/2, lowerHeight, dimensions.z/2);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);

		//mid plane
		bottomLeft = new Vector3(-dimensions.x/2, lowerHeight, 0f);
		topLeft = new Vector3(-dimensions.x/2, upperHeight, 0f);
		topRight = new Vector3(dimensions.x/2, upperHeight, 0f);
		bottomRight = new Vector3(dimensions.x/2, lowerHeight, 0f);
		meshBuilder.addQuad(bottomLeft, topLeft, topRight, bottomRight);

		Mesh doorMesh = meshBuilder.getMesh();
		door.setDoorMesh(doorMesh);
		return true;
	}

	private bool generateEntities() {
		int halfRangeX = (int) (dimensions.x/2 - 0.5f);
		int halfRangeZ = (int) (dimensions.z/2 - 0.5f);
		for (int x = -halfRangeX; x < halfRangeX; ++x) {
			for (int z = -halfRangeZ; z < halfRangeZ; ++z) {
				GameObject entity = RoomTools.rollAndInstanceEntity();
				if (entity != null) {
					entity.transform.position = new Vector3((float) x, floorHeight, (float) z);
				}
			}
		}
		return true;
	}

	public bool openTrapDoor() {
		door.openTrapDoor();
		return true;
	}

	public Room createNextRoom(Vector3 nextRoomDimensions, float transitionHeight, Color color) {
		if (transitionHeight < 0)
			transitionHeight = 0;
		if (!(transitionHeight == 0 || this.dimensions == nextRoomDimensions)) {
			generateTransitionWall(nextRoomDimensions, transitionHeight);
		} else {
			transitionHeight = 0f;
		}
		return new Room(nextRoomDimensions, this.floorHeight - nextRoomDimensions.y - transitionHeight, color);
	}

	public Room createRandNextRoom() {
		Vector3 nextRoomDimensions = this.dimensions;
		float transitionHeight = 0f;
		Color color = this.roomColor;

		float transitionRnd = UnityEngine.Random.Range(-RoomTools.transitionVariation/2, RoomTools.transitionVariation/2);

		float rnd = UnityEngine.Random.Range(0.0f, 1.0f);

		if(rnd <= RoomTools.biggerModeProb) {
			color = RoomTools.getRndLightColor();
			nextRoomDimensions = RoomTools.getRndDimensions(this.dimensions, RoomTools.maxDimensions);
		} else if (rnd <= RoomTools.biggerModeProb + RoomTools.smallerModeProb) {
			color = RoomTools.getRndLightColor();
			nextRoomDimensions = RoomTools.getRndDimensions(RoomTools.minDimensions, this.dimensions);
		}

		transitionHeight = Math.Max(Math.Abs(nextRoomDimensions.x - this.dimensions.x), Math.Abs(nextRoomDimensions.z - this.dimensions.z)) / 2 + transitionRnd;
		transitionHeight = Math.Max(RoomTools.minTransition, transitionHeight);

		return createNextRoom(nextRoomDimensions, transitionHeight, color);
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

		Mesh wallMesh = meshBuilder.getMesh();
		RoomTools.addMeshWithCol(host, wallMesh, RoomTools.wallRenderMaterial, RoomTools.wallPhysicMaterial);
		return true;
	}

	public Vector3 getDimensions() {
		return this.dimensions;
	}

	public float getFloorHeight() {
		return floorHeight;
	}

	public int getID() {
		return id;
	}

	public void DestroyRoom() {
		GameObject.Destroy(roomObj, 2f);
	}

	public static int getCurrentID() {
		return Room.id;
	}
}

public static class RoomTools {
	public static PhysicMaterial wallPhysicMaterial;
	public static PhysicMaterial doorPhysicMaterial;
	public static Material wallRenderMaterial;
	public static Material doorRenderMaterial;
	public static GameObject entityParent;
	public static float trapDoorThickness;
	public static float doorSpeed;

	//room settings
	//biggerModeProb: bigger dimensions, other color
	public static float biggerModeProb = 0.2f;
	//smallerModeProb: smaller dimensions, other color
	public static float smallerModeProb = 0.3f;
	//rest: same dimensions, same color
	public static Vector3 maxDimensions;
	public static Vector3 minDimensions;
	public static float transitionVariation;
	public static float minTransition;
	public static Room lastGeneratedRoom;

	public static Dictionary<string, EntityPrefab> spawnEntities = new Dictionary<string, EntityPrefab>();

	public static void addMeshWithCol(GameObject host, Mesh mesh, Material renderMaterial, PhysicMaterial physicMaterial, bool convex = false) {
		MeshFilter mFilter = host.AddComponent<MeshFilter>();
		mFilter.mesh = mesh;
		MeshRenderer renderer = host.AddComponent<MeshRenderer>();
		renderer.material = renderMaterial;
		MeshCollider collider = host.AddComponent<MeshCollider>();
		collider.material = physicMaterial;
		collider.convex = convex;
	}

	public static void addKinematicRigidbody(GameObject host) {
		Rigidbody rig = host.AddComponent<Rigidbody>();
		rig.isKinematic = true;
		rig.useGravity = false;
		rig.constraints = RigidbodyConstraints.FreezeAll;
	}

	public static GameObject rollAndInstanceEntity() {
		float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
		float baseProb = 0f;
		foreach (var item in spawnEntities)
		{
			if (rnd <= item.Value.getProbability() + baseProb) {
				return item.Value.getInstance();
			}
			baseProb += item.Value.getProbability();
		}
		return null;
	}

	public static void entityDestroyed(string name) {
		EntityPrefab ePrefab = null;
		if (spawnEntities.TryGetValue(name, out ePrefab)) {
			ePrefab.entityDestroyed();
		} else {
			Debug.LogError("entityDestroyed: error: EntityPrefab " + name + " does not exist");
		}
	}

	public static Color getRndLightColor() {
		Color color = new Color(1, 1, 1, 1);
		float toneDown = UnityEngine.Random.Range(0.0f, 0.45f);
		//choose first Color to tone down
		float rnd1 = UnityEngine.Random.Range(0.0f, 1.0f);
		if (rnd1 <= 0.20f)
			color.r -= toneDown;
		else if (rnd1 <= 0.40f)
			color.g -= toneDown;
		else if (rnd1 <= 0.60f)
			color.b -= toneDown;

		float rnd2 = UnityEngine.Random.Range(0.0f, 1.0f);
		if (rnd2 <= 0.20f)
			color.r -= toneDown;
		else if (rnd2 <= 0.40f)
			color.g -= toneDown;
		else if (rnd2 <= 0.60f)
			color.b -= toneDown;
		return color;
	}

	public static Vector3 getRndDimensions(Vector3 minDimensions, Vector3 maxDimensions, bool totallyRandomHeight = true) {
		int rndX = UnityEngine.Random.Range((int) minDimensions.x, (int) maxDimensions.x);
		rndX = (rndX >> 1) << 1; //make power of 2
		int rndZ = UnityEngine.Random.Range((int) minDimensions.z, (int) maxDimensions.z);
		rndZ = (rndZ >> 1) << 1; //make power of 2
		int rndY;
		if (totallyRandomHeight)
			rndY = UnityEngine.Random.Range((int) RoomTools.minDimensions.y, (int) RoomTools.maxDimensions.y);
		else
			rndY = UnityEngine.Random.Range((int) minDimensions.y, (int) maxDimensions.y);
		//rndY = (rndY >> 1) << 1; //make power of 2 -- not needed
		return new Vector3(rndX, rndY, rndZ);
	}
}