using UnityEngine;

namespace ProceduralCave.Generator.Mesh
{
	/// <summary>
	/// Grid/Matriz de cubos que aplica la técnica Marching Cubes
	/// 
	/// Los elementos del grid son instancias de la clase MarchingCubesSquare
	/// Cada elemento del grid, a su misma vez se divide en 8 nodos, siendo
	/// 4 nodos de control y 4 nodos de referencia
	/// </summary>
	public class MarchingCubesGrid
	{
		private MarchingCubesSquare[,] _squares;

        public MarchingCubesSquare[,] Squares { get => _squares; set => _squares = value; }

        public MarchingCubesGrid(int[,] map, float squareSize)
		{
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			// Se crean los nodos de control del mapa
			ControlNode[,] controlNodes = 
				CreateControlNodes(map, nodeCountX, nodeCountY, mapWidth, mapHeight, squareSize);

			// Se crean los cubos del mapa, a los cuales se les asigna sus nodos
			// de control correspondientes
			// NOTA: Los cubos comparten nodos con sus cubos vecinos
			Squares = CreateSquareGrid(controlNodes, nodeCountX, nodeCountY);
		}


		// Obtiene la posición del nodo en el espacio bidimensional del mundo
		// en base a su posición dentro del 
		public Vector2 GetWorldSpacePositionOfNode(int posX, int posY, float mapWidth, float mapHeight, float squareSize)
		{
			float horizontalWordlPos = -mapWidth / 2 + posX * squareSize + squareSize / 2;
			float verticalWorldPos = -mapHeight / 2 + posY * squareSize + squareSize / 2;

			return new Vector2(horizontalWordlPos, verticalWorldPos);
		}

		// Para el número de nodos en el ancho y alto del mapa,
		// se crea un nodo de control con su posición en el plano bidimensional
		public ControlNode[,] CreateControlNodes(int[,] map, int nodeCountX, int nodeCountY, float mapWidth, float mapHeight, float squareSize)
        {
			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

			for (int x = 0; x < nodeCountX; x++)
			{
				for (int y = 0; y < nodeCountY; y++)
				{
					Vector2 worldPos = GetWorldSpacePositionOfNode(x, y, mapWidth, mapHeight, squareSize);
					controlNodes[x, y] = new ControlNode(worldPos, map[x, y] == 1, squareSize);
				}
			}

			return controlNodes;
		}

		public MarchingCubesSquare[,] CreateSquareGrid(ControlNode[,] controlNodes, int nodeCountX, int nodeCountY)
        {
			MarchingCubesSquare[,] grid = new MarchingCubesSquare[nodeCountX - 1, nodeCountY - 1];
			for (int x = 0; x < nodeCountX - 1; x++)
			{
				for (int y = 0; y < nodeCountY - 1; y++)
				{
					MarchingCubesSquare currentSquare = new MarchingCubesSquare
					{
						TopLeft = controlNodes[x, y + 1],
						TopRight = controlNodes[x + 1, y + 1],
						BottomRight = controlNodes[x + 1, y],
						BottomLeft = controlNodes[x, y]
					};

					grid[x, y] = currentSquare;

				}
			}

			return grid;
		}
	}
}