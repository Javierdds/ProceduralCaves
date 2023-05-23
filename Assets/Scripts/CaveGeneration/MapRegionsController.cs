using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCave.Generator
{
	public struct Coord
	{
		public int tileX;
		public int tileY;

		public Coord(int x, int y)
		{
			tileX = x;
			tileY = y;
		}

		public bool HasDefaultValue()
        {
			return tileX == 0 && tileY == 0;
        }
	}

	public class MapRegionsController
	{

		private int[,] _regionsMap;
		private int _roomThresholdSize;
		private int _wallThresholdSize;
		private float _squareSize;
		private int _passageRadius;
		private bool _connectRooms;

		public MapRegionsController(int[,] map, int roomThreshold, int wallThreshold, float squareSize)
        {
			_regionsMap = map;
			_roomThresholdSize = roomThreshold;
			_wallThresholdSize = wallThreshold;
			_squareSize = squareSize;
		}

        public MapRegionsController(int[,] regionsMap, int roomThresholdSize, int wallThresholdSize, float squareSize, bool connectRooms, int passageRadius)
			: this(regionsMap, roomThresholdSize, wallThresholdSize, squareSize)
        {
			_passageRadius = passageRadius;
			_connectRooms = connectRooms;
        }

        public int[,] ProcessMap()
		{
			List<List<Coord>> wallRegions = GetRegions(1);

			foreach (List<Coord> wallRegion in wallRegions)
			{
				if (wallRegion.Count < _wallThresholdSize)
				{
					foreach (Coord tile in wallRegion)
					{
						_regionsMap[tile.tileX, tile.tileY] = 0;
					}
				}
			}

			List<List<Coord>> roomRegions = GetRegions(0);
			List<MapRoom> survivingRooms = new List<MapRoom>();

			foreach (List<Coord> roomRegion in roomRegions)
			{
				if (roomRegion.Count < _roomThresholdSize)
				{
					foreach (Coord tile in roomRegion)
					{
						_regionsMap[tile.tileX, tile.tileY] = 1;
					}
				}
                else
                {
					survivingRooms.Add(new MapRoom(roomRegion, _regionsMap));
                }
			}

			survivingRooms.Sort();
			survivingRooms[0].IsMainRoom = true;
			survivingRooms[0].IsAccesibleFromMainRoom = true;

			MapRoomController roomController = new(_regionsMap.GetLength(0), _regionsMap.GetLength(1), _squareSize, _passageRadius);
			roomController.ConnectClosestRooms(survivingRooms, ref _regionsMap, _connectRooms);

			return _regionsMap;
		}

		List<List<Coord>> GetRegions(int tileType)
		{
			List<List<Coord>> regions = new List<List<Coord>>();
			int[,] _regionsMapFlags = new int[_regionsMap.GetLength(0), _regionsMap.GetLength(1)];

			for (int x = 0; x < _regionsMapFlags.GetLength(0); x++)
			{
				for (int y = 0; y < _regionsMapFlags.GetLength(1); y++)
				{
					if (_regionsMapFlags[x, y] == 0 && _regionsMap[x, y] == tileType)
					{
						List<Coord> newRegion = GetRegionTiles(x, y);
						regions.Add(newRegion);

						foreach (Coord tile in newRegion)
						{
							_regionsMapFlags[tile.tileX, tile.tileY] = 1;
						}
					}
				}
			}

			return regions;
		}

		List<Coord> GetRegionTiles(int startX, int startY)
		{
			List<Coord> tiles = new List<Coord>();
			int[,] _regionsMapFlags = new int[_regionsMap.GetLength(0), _regionsMap.GetLength(1)];
			int tileType = _regionsMap[startX, startY];

			Queue<Coord> queue = new Queue<Coord>();
			queue.Enqueue(new Coord(startX, startY));
			_regionsMapFlags[startX, startY] = 1;

			while (queue.Count > 0)
			{
				Coord tile = queue.Dequeue();
				tiles.Add(tile);

				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
				{
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
					{
						if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
						{
							if (_regionsMapFlags[x, y] == 0 && _regionsMap[x, y] == tileType)
							{
								_regionsMapFlags[x, y] = 1;
								queue.Enqueue(new Coord(x, y));
							}
						}
					}
				}
			}

			return tiles;
		}

		// Comprueba que una coordenada no esté fuera del mapa generado
		bool IsInMapRange(int x, int y)
		{
			return x >= 0 && x < _regionsMap.GetLength(0) && y >= 0 && y < _regionsMap.GetLength(1);
		}

		
	}
}

