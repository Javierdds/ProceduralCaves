using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCave.Generator.CaveMesh
{
	public class MarchingCubesMeshGenerator : MonoBehaviour
	{
		public MarchingCubesGrid squareGrid;
		public float cubeSize = .6f;


		private MeshFilter _caveMesh;
		private MeshFilter _wallsMesh;
		private MeshCollider _customCollider;
		private List<Vector3> _vertices;
		private List<int> _triangles;

		Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
		List<List<int>> outlines = new List<List<int>>();
		HashSet<int> checkedVertices = new HashSet<int>();

        public void GenerateMesh(int[,] map, float squareSize, MeshFilter caveMesh, MeshFilter wallsMesh)
		{
			_caveMesh = caveMesh;
			_wallsMesh = wallsMesh;

			triangleDictionary.Clear();
			outlines.Clear();
			checkedVertices.Clear();
			_vertices = new List<Vector3>();
			_triangles = new List<int>();

			squareGrid = new MarchingCubesGrid(map, squareSize);

			for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
			{
				for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
				{
					TriangulateSquare(squareGrid.Squares[x, y]);
				}
			}


			Mesh mesh = new Mesh();

			mesh.vertices = _vertices.ToArray();
			mesh.triangles = _triangles.ToArray();
			mesh.RecalculateNormals();

			int tileAmount = 1;
			Vector2[] uvs = new Vector2[_vertices.Count];
			for (int i = 0; i < _vertices.Count; i++)
			{
				float percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize, _vertices[i].x) * tileAmount;
				float percentY = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize, _vertices[i].y) * tileAmount;
				uvs[i] = new Vector2(percentX, percentY);
			}
			mesh.uv = uvs;

			_caveMesh.mesh = mesh;


			Generate2DColliders();
			//if (is2D)
			//{
			//	Generate2DColliders();
			//}
			//else
			//{
			//	CreateWallMesh();
			//}
		}

		void CreateWallMesh()
		{

			CalculateMeshOutlines();

			List<Vector3> wallVertices = new List<Vector3>();
			List<int> wallTriangles = new List<int>();
			Mesh wallMesh = new Mesh();
			float wallHeight = 5;

			foreach (List<int> outline in outlines)
			{
				for (int i = 0; i < outline.Count - 1; i++)
				{
					int startIndex = wallVertices.Count;
					wallVertices.Add(_vertices[outline[i]]); // left
					wallVertices.Add(_vertices[outline[i + 1]]); // right
					wallVertices.Add(_vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
					wallVertices.Add(_vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

					wallTriangles.Add(startIndex + 0);
					wallTriangles.Add(startIndex + 2);
					wallTriangles.Add(startIndex + 3);

					wallTriangles.Add(startIndex + 3);
					wallTriangles.Add(startIndex + 1);
					wallTriangles.Add(startIndex + 0);
				}
			}
			wallMesh.vertices = wallVertices.ToArray();
			wallMesh.triangles = wallTriangles.ToArray();
			_wallsMesh.mesh = wallMesh;

			MeshCollider wallCollider = _wallsMesh.gameObject.AddComponent<MeshCollider>();
			wallCollider.sharedMesh = wallMesh;
		}

		public void GenerateCollider()
		{
			if(_customCollider == null)
				_customCollider = gameObject.AddComponent<MeshCollider>();

			_customCollider.sharedMesh = _wallsMesh.mesh;
		}

		void Generate2DColliders()
		{

			EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
			for (int i = 0; i < currentColliders.Length; i++)
			{
				Destroy(currentColliders[i]);
			}

			CalculateMeshOutlines();

			foreach (List<int> outline in outlines)
			{
				EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
				Vector2[] edgePoints = new Vector2[outline.Count];

				for (int i = 0; i < outline.Count; i++)
				{
					edgePoints[i] = new Vector2(_vertices[outline[i]].x, _vertices[outline[i]].y);
				}
				edgeCollider.points = edgePoints;
			}

		}

		// Según la configuración de cuadrado, genera un mesh dados los nodos 
		// que correspondan con esa configuración
		private void TriangulateSquare(MarchingCubesSquare square)
        {
			switch (square.SquareConfiguration)
			{
				case 0:
					break;

				// Square of 1 pointss:
				case 1:
					MeshFromPoints(square.CentreBottom, square.BottomLeft, square.CentreLeft);
					break;
				case 2:
					MeshFromPoints(square.CentreRight, square.BottomRight, square.CentreBottom);
					break;
				case 4:
					MeshFromPoints(square.CentreTop, square.TopRight, square.CentreRight);
					break;
				case 8:
					MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreLeft);
					break;

				// Square of 2 pointss:
				case 3:
					MeshFromPoints(square.CentreRight, square.BottomRight, square.BottomLeft, square.CentreLeft);
					break;
				case 6:
					MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.CentreBottom);
					break;
				case 9:
					MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreBottom, square.BottomLeft);
					break;
				case 12:
					MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreLeft);
					break;
				case 5:
					MeshFromPoints(square.CentreTop, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft, square.CentreLeft);
					break;
				case 10:
					MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.CentreBottom, square.CentreLeft);
					break;

				// Square of 3 points:
				case 7:
					MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CentreLeft);
					break;
				case 11:
					MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.BottomLeft);
					break;
				case 13:
					MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft);
					break;
				case 14:
					MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CentreBottom, square.CentreLeft);
					break;

				// Square of 4 points:
				case 15:
					MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
					break;
			}
		}

		// Dada una lista de nodos, asigna los nodos como vértices del mesh
		// y por cada lista de nodos a partir de 3, crea un triángulo
		private void MeshFromPoints(params Node[] points)
		{
			AssignVertices(points);

			if (points.Length >= 3)
				CreateTriangle(points[0], points[1], points[2]);
			if (points.Length >= 4)
				CreateTriangle(points[0], points[2], points[3]);
			if (points.Length >= 5)
				CreateTriangle(points[0], points[3], points[4]);
			if (points.Length >= 6)
				CreateTriangle(points[0], points[4], points[5]);

		}

		// Dados tres nodos (vértices de un triángulo), crea un triángulo
		private void CreateTriangle(Node a, Node b, Node c)
		{
			_triangles.Add(a.VertexIndex);
			_triangles.Add(b.VertexIndex);
			_triangles.Add(c.VertexIndex); 
			
			Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
			
			AddTriangleToDictionary(triangle.vertexIndexA, triangle);
			AddTriangleToDictionary(triangle.vertexIndexB, triangle);
			AddTriangleToDictionary(triangle.vertexIndexC, triangle);
		}

		void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
		{
			if (triangleDictionary.ContainsKey(vertexIndexKey))
			{
				triangleDictionary[vertexIndexKey].Add(triangle);
			}
			else
			{
				List<Triangle> triangleList = new List<Triangle>();
				triangleList.Add(triangle);
				triangleDictionary.Add(vertexIndexKey, triangleList);
			}
		}

		// Recorre la lista de nodos y los añade a la lista de vértices
		private void AssignVertices(Node[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				// Si el nodo aún no ha sido asignado, se añade a la lista de vértices
				if (points[i].VertexIndex == -1)
				{
					points[i].VertexIndex = _vertices.Count;
					_vertices.Add(points[i].Position);
				}
			}
		}

		void CalculateMeshOutlines()
		{

			for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
			{
				if (!checkedVertices.Contains(vertexIndex))
				{
					int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
					if (newOutlineVertex != -1)
					{
						checkedVertices.Add(vertexIndex);

						List<int> newOutline = new List<int>();
						newOutline.Add(vertexIndex);
						outlines.Add(newOutline);
						FollowOutline(newOutlineVertex, outlines.Count - 1);
						outlines[outlines.Count - 1].Add(vertexIndex);
					}
				}
			}
		}

		void FollowOutline(int vertexIndex, int outlineIndex)
		{
			outlines[outlineIndex].Add(vertexIndex);
			checkedVertices.Add(vertexIndex);
			int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

			if (nextVertexIndex != -1)
			{
				FollowOutline(nextVertexIndex, outlineIndex);
			}
		}

		int GetConnectedOutlineVertex(int vertexIndex)
		{
			List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

			for (int i = 0; i < trianglesContainingVertex.Count; i++)
			{
				Triangle triangle = trianglesContainingVertex[i];

				for (int j = 0; j < 3; j++)
				{
					int vertexB = triangle[j];
					if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
					{
						if (IsOutlineEdge(vertexIndex, vertexB))
						{
							return vertexB;
						}
					}
				}
			}

			return -1;
		}

		bool IsOutlineEdge(int vertexA, int vertexB)
		{
			List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
			int sharedTriangleCount = 0;

			for (int i = 0; i < trianglesContainingVertexA.Count; i++)
			{
				if (trianglesContainingVertexA[i].Contains(vertexB))
				{
					sharedTriangleCount++;
					if (sharedTriangleCount > 1)
					{
						break;
					}
				}
			}
			return sharedTriangleCount == 1;
		}

		struct Triangle
		{
			public int vertexIndexA;
			public int vertexIndexB;
			public int vertexIndexC;
			int[] vertices;

			public Triangle(int a, int b, int c)
			{
				vertexIndexA = a;
				vertexIndexB = b;
				vertexIndexC = c;

				vertices = new int[3];
				vertices[0] = a;
				vertices[1] = b;
				vertices[2] = c;
			}

			public int this[int i]
			{
				get
				{
					return vertices[i];
				}
			}


			public bool Contains(int vertexIndex)
			{
				return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
            }
        }
    }
}

