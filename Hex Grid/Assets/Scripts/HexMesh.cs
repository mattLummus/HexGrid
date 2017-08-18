using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class HexMesh : MonoBehaviour {
	Mesh hexMesh;
	MeshCollider meshCollider;

	List<Vector3> vertices;
	List<int>     triangles;
	List<Color>   colors;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";

		vertices  = new List<Vector3>();
		triangles = new List<int>();
		colors    = new List<Color>();
	}

	public void TriangulateCells (HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();

		for (int i = 0; i < cells.Length; i++) {
			TriangulateCell(cells[i]);
		}

		hexMesh.vertices  = vertices.ToArray( );
		hexMesh.triangles = triangles.ToArray();
		hexMesh.colors    = colors.ToArray();
		hexMesh.RecalculateNormals();

		meshCollider.sharedMesh = hexMesh;
	}

	void TriangulateCell (HexCell cell) {
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			Triangulate(d, cell);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) {
		Vector3 center = cell.transform.localPosition;
		Vector3 bridge = HexMetrics.GetBridge(direction);

		Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
		Vector3 v3 = v1 + bridge;
		Vector3 v4 = v2 + bridge;

		HexCell neighbor     = cell.GetNeighbor(direction)            ?? cell;
		HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next())     ?? cell;

		Color bridgeColor = (cell.color + neighbor.color) * 0.5f;

		AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3);
		AddTriangleColor(cell.color, ((cell.color + prevNeighbor.color + neighbor.color) / 3f), bridgeColor);
		AddQuad(v1, v2, v3, v4);
		AddQuadColor(cell.color, (cell.color + neighbor.color) * 0.5f);


	}

	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);

		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	void AddTriangleColor(Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	void AddTriangleColor (Color c1, Color c2, Color c3) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		vertices.Add(v4);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}

	void AddQuadColor (Color c1, Color c2) {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}
}