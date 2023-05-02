using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCave.Generator.CaveMesh
{
	public class MarchingCubesMeshGenerator : MonoBehaviour
	{
		public MarchingCubesGrid squareGrid;
		public float cubeSize = .6f;


		private List<Vector3> _vertices;
		private List<int> _triangles;


		public void GenerateMesh(int[,] map, float squareSize)
		{
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
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


			mesh.vertices = _vertices.ToArray();
			mesh.triangles = _triangles.ToArray();
			mesh.RecalculateNormals();
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
		private void CreateTriangle(Node a, Node B, Node c)
		{
			_triangles.Add(a.VertexIndex);
			_triangles.Add(B.VertexIndex);
			_triangles.Add(c.VertexIndex);
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

		//void OnDrawGizmos()
		//{
		//	if (squareGrid != null)
		//	{
  //              for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
  //              {
  //                  for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
  //                  {

  //                      Gizmos.color = (squareGrid.Squares[x, y].TopLeft.IsActive) ? Color.black : Color.white;
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].TopLeft.Position, Vector3.one * cuBeSize);

		//				Gizmos.color = (squareGrid.Squares[x, y].TopRight.IsActive) ? Color.black : Color.white;
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].TopRight.Position, Vector3.one * cuBeSize);

		//				Gizmos.color = (squareGrid.Squares[x, y].BottomRight.IsActive) ? Color.black : Color.white;
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].BottomRight.Position, Vector3.one * cuBeSize);

		//				Gizmos.color = (squareGrid.Squares[x, y].BottomLeft.IsActive) ? Color.black : Color.white;
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].BottomLeft.Position, Vector3.one * cuBeSize);


		//				Gizmos.color = Color.grey;
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].CentreTop.Position, Vector3.one * .15f);
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].CentreRight.Position, Vector3.one * .15f);
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].CentreBottom.Position, Vector3.one * .15f);
		//				Gizmos.DrawCube(squareGrid.Squares[x, y].CentreLeft.Position, Vector3.one * .15f);

		//			}
		//		}
		//	}
		//}
	}
}

