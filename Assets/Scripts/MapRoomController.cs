using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCave.Generator
{
	/// <summary>
	/// Clase encargada de conectar las regiones del mapa entre sí
	/// </summary>
	public class MapRoomController
	{
		private int _width;
		private int _height;
		private float _squareSize;
		
		public MapRoomController(int width, int height, float squareSize)
        {
			_width = width;
			_height = height;
			_squareSize = squareSize;
        }

		public void ConnectClosestRooms(List<MapRoom> allRooms)
		{

			int bestDistance = 0;
			Coord bestTileA = new Coord();
			Coord bestTileB = new Coord();
			MapRoom bestRoomA = new MapRoom();
			MapRoom bestRoomB = new MapRoom();
			bool possibleConnectionFound = false;

			foreach (MapRoom roomA in allRooms)
			{
				possibleConnectionFound = false;

				foreach (MapRoom roomB in allRooms)
				{
					if (roomA == roomB)
					{
						continue;
					}
					if (roomA.IsConnected(roomB))
					{
						possibleConnectionFound = false;
						break;
					}

					for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
					{
						for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
						{
							Coord tileA = roomA.EdgeTiles[tileIndexA];
							Coord tileB = roomB.EdgeTiles[tileIndexB];
							int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

							if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
							{
								bestDistance = distanceBetweenRooms;
								possibleConnectionFound = true;
								bestTileA = tileA;
								bestTileB = tileB;
								bestRoomA = roomA;
								bestRoomB = roomB;
							}
						}
					}
				}

				if (possibleConnectionFound)
				{
					CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				}
			}
		}

		void CreatePassage(MapRoom roomA, MapRoom roomB, Coord tileA, Coord tileB)
		{
			ConnectRooms(roomA, roomB);
			Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);
		}

		// Conecta dos salas del mapa entre sí
		public void ConnectRooms(MapRoom roomA, MapRoom roomB)
		{
			roomA.ConnectedRooms.Add(roomB);
			roomB.ConnectedRooms.Add(roomA);
		}

		public Vector2 CoordToWorldPoint(Coord tile)
		{
			return new Vector2(-_width / 2 + .5f + tile.tileX, -_height / 2 + .5f + tile.tileY);
		}
	}

}

