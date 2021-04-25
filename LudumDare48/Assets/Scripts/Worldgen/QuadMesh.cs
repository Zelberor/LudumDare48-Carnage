using UnityEngine;
using System.Collections.Generic;

public class QuadMesh {

	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	public QuadMesh() {
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
	}

	public bool addQuad(Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight) {
		int bottomLeftIndex = -1, topLeftIndex = -1, topRightIndex = -1, bottomRightIndex = -1;

		//bottomLeft
		bottomLeftIndex = vertices.Count;
		vertices.Add(bottomLeft);
		uvs.Add(new Vector2(0, 0));

		//topLeft
		topLeftIndex = vertices.Count;
		vertices.Add(topLeft);
		uvs.Add(new Vector2(0, (topLeft - bottomLeft).magnitude));

		//topRight
		topRightIndex = vertices.Count;
		vertices.Add(topRight);
		uvs.Add(new Vector2((topRight - topLeft).magnitude, (topRight - bottomRight).magnitude));

		//bottomRight
		bottomRightIndex = vertices.Count;
		vertices.Add(bottomRight);
		uvs.Add(new Vector2((bottomRight - bottomLeft).magnitude, 0));

		//Add topLeft->bottomRight->bottomLeft triangle
		triangles.Add(topLeftIndex);
		triangles.Add(bottomRightIndex);
		triangles.Add(bottomLeftIndex);

		//Add topLeft->topRight->bottomRight triangle
		triangles.Add(topLeftIndex);
		triangles.Add(topRightIndex);
		triangles.Add(bottomRightIndex);

		//sanity checks
		if (vertices.Count != uvs.Count) {
			Debug.LogError("addQuad: error: vertices.Count doas not match uvs.Count");
			return false;
		}
		if (vertices.Count % 4 != 0) {
			Debug.LogError("addQuad: error: vertices.Count not a number of 4");
			return false;
		}
		if (triangles.Count % 3 != 0) {
			Debug.LogError("addQuad: error: triangles.Count not a number of 3");
			return false;
		}
		return true;
	}

	public Mesh getMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = this.vertices.ToArray();
		mesh.uv = this.uvs.ToArray();
		mesh.triangles = this.triangles.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}

}