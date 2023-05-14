using System;
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
		private int _passageRadius;
		
		public MapRoomController(int width, int height, float squareSize, int passageRadius)
        {
			_width = width;
			_height = height;
			_squareSize = squareSize;
			_passageRadius = passageRadius;

		}

		public void ConnectClosestRooms(List<MapRoom> allRooms, ref int[,] regionsMap, bool forceAccessibilityFromMainRoom = false, bool connectRooms = true)
		{

			int bestDistance = 0;
			Coord bestTileA = new Coord();
			Coord bestTileB = new Coord();
			MapRoom bestRoomA = new MapRoom();
			MapRoom bestRoomB = new MapRoom();
			List<MapRoom> roomListA = new List<MapRoom>();
			List<MapRoom> roomListB = new List<MapRoom>();
			bool possibleConnectionFound = false;

			if (forceAccessibilityFromMainRoom)
			{
				foreach (MapRoom room in allRooms)
				{
					if (room.IsAccesibleFromMainRoom)
					{
						roomListB.Add(room);
					}
					else
					{
						roomListA.Add(room);
					}
				}
			}
			else
			{
				roomListA = allRooms;
				roomListB = allRooms;
			}

			foreach (MapRoom roomA in roomListA)
			{
				if (!forceAccessibilityFromMainRoom)
				{
					possibleConnectionFound = false;
					if (roomA.ConnectedRooms.Count > 0)
					{
						continue;
					}
				}

				foreach (MapRoom roomB in roomListB)
				{
					// Si roomB es igual que roomA o ya están conectadas, 
					// se pasa a la siguiente iteración
					if (roomA == roomB || roomA.IsConnected(roomB))
					{
						continue;
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

				if (possibleConnectionFound && !forceAccessibilityFromMainRoom && connectRooms)
				{
					CreatePassage(ref regionsMap, bestRoomA, bestRoomB, bestTileA, bestTileB);
				} 
				else if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
					ConnectRooms(bestRoomA, bestRoomB);
					//Debug.DrawLine(CoordToWorldPoint(bestTileA), CoordToWorldPoint(bestTileB), Color.green, 10);
				}
			}

			if (possibleConnectionFound && forceAccessibilityFromMainRoom && connectRooms)
			{

				CreatePassage(ref regionsMap, bestRoomA, bestRoomB, bestTileA, bestTileB);
				ConnectClosestRooms(allRooms, ref regionsMap, true, connectRooms);
			}

			if (!forceAccessibilityFromMainRoom)
			{
				ConnectClosestRooms(allRooms, ref regionsMap, true, connectRooms);
			}
		}

		void CreatePassage(ref int[,] regionsMap, MapRoom roomA, MapRoom roomB, Coord tileA, Coord tileB)
		{
			ConnectRooms(roomA, roomB);
			//Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 10);

			List<Coord> line = GetLine(tileA, tileB);
			foreach (Coord c in line)
			{
				DrawCircle(ref regionsMap, c, _passageRadius);
			}
		}

		void DrawCircle(ref int[,] regionsMap, Coord c, int r)
		{
			for (int x = -r; x <= r; x++)
			{
				for (int y = -r; y <= r; y++)
				{
					if (x * x + y * y <= r * r)
					{
						int drawX = c.tileX + x;
						int drawY = c.tileY + y;
						if (IsInMapRange(drawX, drawY))
						{
							regionsMap[drawX, drawY] = 0;
						}
					}
				}
			}
		}

		List<Coord> GetLine(Coord from, Coord to)
		{
			List<Coord> line = new List<Coord>();

			int x = from.tileX;
			int y = from.tileY;

			int dx = to.tileX - from.tileX;
			int dy = to.tileY - from.tileY;

			bool inverted = false;
			int step = Math.Sign(dx);
			int gradientStep = Math.Sign(dy);

			int longest = Mathf.Abs(dx);
			int shortest = Mathf.Abs(dy);

			if (longest < shortest)
			{
				inverted = true;
				longest = Mathf.Abs(dy);
				shortest = Mathf.Abs(dx);

				step = Math.Sign(dy);
				gradientStep = Math.Sign(dx);
			}

			int gradientAccumulation = longest / 2;
			for (int i = 0; i < longest; i++)
			{
				line.Add(new Coord(x, y));

				if (inverted)
				{
					y += step;
				}
				else
				{
					x += step;
				}

				gradientAccumulation += shortest;
				if (gradientAccumulation >= longest)
				{
					if (inverted)
					{
						x += gradientStep;
					}
					else
					{
						y += gradientStep;
					}
					gradientAccumulation -= longest;
				}
			}

			return line;
		}

		// Conecta dos salas del mapa entre sí
		public void ConnectRooms(MapRoom roomA, MapRoom roomB)
		{
			if (roomA.IsAccesibleFromMainRoom)
			{
				roomB.SetAccessibleFromMainRoom();
			}
			else if (roomB.IsAccesibleFromMainRoom)
			{
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.ConnectedRooms.Add(roomB);
			roomB.ConnectedRooms.Add(roomA);
		}

		public Vector2 CoordToWorldPoint(Coord tile)
		{
			Vector2 worldPosition = new Vector2(-_width / 2 + .5f + tile.tileX, -_height / 2 + .5f + tile.tileY);
			worldPosition *= _squareSize;
			//Debug.Log($"[WorldPos]: {worldPosition}");
			return worldPosition;
		}

		// Comprueba que una coordenada no esté fuera del mapa generado
		bool IsInMapRange(int x, int y)
		{
			return x >= 0 && x < _width && y >= 0 && y < _height;
		}
	}

}

