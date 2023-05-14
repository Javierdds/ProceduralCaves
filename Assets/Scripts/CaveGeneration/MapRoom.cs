using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralCave.Generator
{
    public class MapRoom : IComparable<MapRoom>
    {
        private List<Coord> _tiles;
        private List<Coord> _edgeTiles;
        private List<MapRoom> _connectedRooms;
        private int _roomSize;
        private bool _isMainRoom;
        private bool _isAccesibleFromMainRoom;

        public List<Coord> Tiles { get => _tiles; set => _tiles = value; }
        public List<Coord> EdgeTiles { get => _edgeTiles; set => _edgeTiles = value; }
        public List<MapRoom> ConnectedRooms { get => _connectedRooms; set => _connectedRooms = value; }
        public int RoomSize { get => _roomSize; set => _roomSize = value; }
        public bool IsMainRoom { get => _isMainRoom; set => _isMainRoom = value; }
        public bool IsAccesibleFromMainRoom { get => _isAccesibleFromMainRoom; set => _isAccesibleFromMainRoom = value; }

        public MapRoom()
        {
        }

        public MapRoom(List<Coord> roomTiles, int[,] map)
        {
            _tiles = roomTiles;
            _roomSize = _tiles.Count;
            _connectedRooms = new List<MapRoom>();
            _isMainRoom = false;
            _isAccesibleFromMainRoom = false;

            // Se comprueban cuáles de las coordenadas adyadcentes
            // a las coordenadas de la sala son muros
            _edgeTiles = new List<Coord>();
            foreach (Coord tile in _tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        // Se excluyen las coordenadas diagonales
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                _edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        // Comprueba si la sala proporcionada por parámetro está conectada
        // a la sala actual
        public bool IsConnected(MapRoom otherMapRoomsController)
        {
            return _connectedRooms.Contains(otherMapRoomsController);
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!_isAccesibleFromMainRoom)
            {
                _isAccesibleFromMainRoom = true;
                foreach (MapRoom connectedRoom in _connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public int CompareTo(MapRoom otherRoom)
        {
            return otherRoom.RoomSize.CompareTo(RoomSize);
        }
    }
}

