using UnityEngine;
namespace ProceduralCave.Generator.Mesh
{
	public class MarchingCubesMeshGenerator : MonoBehaviour
	{
		public MarchingCubesGrid squareGrid;
		public float cubeSize = .6f;

		public void GenerateMesh(int[,] map, float squareSize)
		{
			squareGrid = new MarchingCubesGrid(map, squareSize);
		}

		void OnDrawGizmos()
		{
			if (squareGrid != null)
			{
                for (int x = 0; x < squareGrid.Squares.GetLength(0); x++)
                {
                    for (int y = 0; y < squareGrid.Squares.GetLength(1); y++)
                    {

                        Gizmos.color = (squareGrid.Squares[x, y].TopLeft.IsActive) ? Color.black : Color.white;
						Gizmos.DrawCube(squareGrid.Squares[x, y].TopLeft.Position, Vector3.one * cubeSize);

						Gizmos.color = (squareGrid.Squares[x, y].TopRight.IsActive) ? Color.black : Color.white;
						Gizmos.DrawCube(squareGrid.Squares[x, y].TopRight.Position, Vector3.one * cubeSize);

						Gizmos.color = (squareGrid.Squares[x, y].BottomRight.IsActive) ? Color.black : Color.white;
						Gizmos.DrawCube(squareGrid.Squares[x, y].BottomRight.Position, Vector3.one * cubeSize);

						Gizmos.color = (squareGrid.Squares[x, y].BottomLeft.IsActive) ? Color.black : Color.white;
						Gizmos.DrawCube(squareGrid.Squares[x, y].BottomLeft.Position, Vector3.one * cubeSize);


						Gizmos.color = Color.grey;
						Gizmos.DrawCube(squareGrid.Squares[x, y].CentreTop.Position, Vector3.one * .15f);
						Gizmos.DrawCube(squareGrid.Squares[x, y].CentreRight.Position, Vector3.one * .15f);
						Gizmos.DrawCube(squareGrid.Squares[x, y].CentreBottom.Position, Vector3.one * .15f);
						Gizmos.DrawCube(squareGrid.Squares[x, y].CentreLeft.Position, Vector3.one * .15f);

					}
				}
			}
		}
	}
}

