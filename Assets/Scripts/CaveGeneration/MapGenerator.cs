using ProceduralCave.Generator.CaveMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralCave.Generator
{

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        [SerializeField] private int _mapWidth;
        [SerializeField] private int _mapHeight;
        [SerializeField] private float _mapSquareSize;
        [SerializeField] private int _roomThresholdSize;
        [SerializeField] private int _wallThresholdSize;

        [SerializeField] private string _seed;
        [SerializeField] private bool _useRandomSeed;
        [SerializeField] private int _smoothIterations;
        [SerializeField] private int _neighborInfluenceIndex;

        [Range(40, 55)]
        [SerializeField] private int _fillPercent;

        [SerializeField] private bool _connectRooms;
        [SerializeField] private int _passageRadius;
        [SerializeField] private MeshFilter _caveMesh;
        [SerializeField] private MeshFilter _wallsMesh;

        [Header("Interactives")]
        [SerializeField] private int _coinsAmount;
        [Tooltip("Pivot coins are used to set the defined amount of coins as the original spawned coins, in order to spawn the remaining ones next to them")]
        [SerializeField] private int _pivotCoinsPercentage;
        [SerializeField] private GameObject _coinPrefab;

        private int[,] _map;

        private List<Coord> _availableCoords;
        private Dictionary<GameObject, Coord> _coinsDictionary;
        private GameObject[] _spawnedCoins;
        private int _spawnedCoinsCounter;

        private void Awake()
        {
            _availableCoords = new();
            _spawnedCoinsCounter = 0;
            _spawnedCoins = new GameObject[_coinsAmount];
            _coinsDictionary = new Dictionary<GameObject, Coord>();

            GenerateMap();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            _map = new int[_mapWidth, _mapHeight];

            // Se rellena el mapa
            FillMap();

            // Se suavizan las casillas según las casillas vecinas
            for (int i = 0; i < _smoothIterations; i++)
            {
                SmoothMap();
            }

            // Elimina zonas del mapa que no han sido suavizadas correctamente
            MapRegionsController regions = new MapRegionsController(_map, _roomThresholdSize, _wallThresholdSize, _mapSquareSize, _connectRooms, _passageRadius);
            _map = regions.ProcessMap();

            // Dibuja el mesh del mapa
            MarchingCubesMeshGenerator meshGen = GetComponent<MarchingCubesMeshGenerator>();
            meshGen.GenerateMesh(_map, _mapSquareSize, _caveMesh, _wallsMesh);

            FillMapWithCoins();

            _player.transform.position = GetPlayerSpawnPosition();
        }

        public Vector3 GetPlayerSpawnPosition()
        {
            bool isPositionFound = false;
            Vector3 spawnPosition = new Vector3();
            int centroX = _map.GetLength(0) / 2;
            int centroY = _map.GetLength(1) / 2;
            int maxDistancia = Math.Max(_map.GetLength(0) - centroX - 1, centroX) + Math.Max(_map.GetLength(1) - centroY - 1, centroY);

            for (int distancia = 0; distancia <= maxDistancia && !isPositionFound; distancia++)
            {
                for (int i = centroX - distancia; i <= centroX + distancia && !isPositionFound; i++)
                {
                    for (int j = centroY - distancia; j <= centroY + distancia && !isPositionFound; j++)
                    {
                        if (i >= 0 && i < _map.GetLength(0) && j >= 0 && j < _map.GetLength(1))
                        {
                            if (_map[i, j] == 0)
                            {
                                isPositionFound = true;
                                Coord spawnCoord = new Coord(i, j);
                                spawnPosition = CoordToWorldPoint(spawnCoord);
                            }
                        }
                    }
                }
            }

            return spawnPosition;
        }

        public void FillMap()
        {
            // Cálculo del pseudorandom según semilla
            _seed = _useRandomSeed ? Time.time.ToString() : _seed;
            System.Random pseudoRandom = new System.Random(_seed.GetHashCode());

            // Se rellena el mapa
            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    // Los límites del mapa son paredes
                    if (i == 0 || j == 0 || i == _mapWidth - 1 || j == _mapHeight - 1)
                    {
                        _map[i, j] = 1;
                    }
                    // Si no, según el fill percent, se calcula mediante un pseudorandom
                    // si es una pared o una casilla vacía
                    else
                    {
                        _map[i, j] = pseudoRandom.Next(0, 100) < _fillPercent ? 1 : 0;
                    }
                }
            }
        }

        public void SmoothMap()
        {
            int[,] auxMap = _map;
            // Se aplica un suavizado al mapa para que haya una relativa
            // concordancia entre las casillas y sus vecinos
            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    int neighbourWallTiles = GetNumberOfSurroundingWalls(i, j);

                    if (neighbourWallTiles > _neighborInfluenceIndex)
                        auxMap[i, j] = 1;
                    else if (neighbourWallTiles < _neighborInfluenceIndex)
                        auxMap[i, j] = 0;
                }
            }

            _map = auxMap;
        }

        #region Coins

        public void FillMapWithCoins()
        {
            int[,] auxMap = _map;
            _coinsDictionary = new Dictionary<GameObject, Coord>();
            _spawnedCoinsCounter = 0;
            foreach (var item in _spawnedCoins)
            {
                Destroy(item);
            }
            _spawnedCoins = new GameObject[_coinsAmount];

            _availableCoords = new List<Coord>();
            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    int neighbourWallTiles = GetNumberOfSurroundingWalls(i, j);

                    if (neighbourWallTiles == 0 && auxMap[i, j] == 0)
                    {
                        auxMap[i, j] = 2;
                        Coord coinCoord = new Coord(i, j);
                        _availableCoords.Add(coinCoord);
                    }
                }
            }

            // Si hay menos coordenadas disponibles que monedas
            // a crear, se ajustan las monedas a crear
            if (_coinsAmount >= _availableCoords.Count) _coinsAmount = _availableCoords.Count - 1;

            var rnd = new System.Random(_seed.GetHashCode());
            _availableCoords.Shuffle(rnd);

            int[] pivotCoins = new int[_coinsAmount / _pivotCoinsPercentage];

            for (int i = 0; i < pivotCoins.Length; i++)
            {
                if (i >= _coinsAmount || i >= _availableCoords.Count) break;

                GameObject coin = CoinSpawner(_availableCoords[i]);
                _spawnedCoins[i] = coin;
            }

            int pivotCoinsIndex = 0;
            int pivotCoinsModifier = 0;
            for (int i = pivotCoins.Length; i < _coinsAmount; i++)
            {
                if (pivotCoinsIndex >= pivotCoins.Length)
                {
                    pivotCoinsIndex = 0 + pivotCoinsModifier;
                }

                _coinsDictionary.TryGetValue(_spawnedCoins[pivotCoinsIndex], out Coord pivotCoinCoord);

                Coord coinPosition = GetAvailableCoordNearPivot(pivotCoinCoord);

                if (coinPosition.HasDefaultValue())
                {
                    _availableCoords.Remove(coinPosition);

                    continue;
                }

                GameObject coin = CoinSpawner(coinPosition);
                _spawnedCoins[i] = coin;
                pivotCoinsIndex++;
            }


            _map = auxMap;
        }

        public GameObject CoinSpawner(Coord position)
        {
            GameObject coin = Instantiate(_coinPrefab);
            coin.transform.position = CoordToWorldPoint(position);
            _coinsDictionary.Add(coin, position);
            _availableCoords.Remove(position);

            _spawnedCoinsCounter++;

            return coin;
        }

        public Coord GetAvailableCoordNearPivot(Coord pivotCoinCoord)
        {
            Coord availablePosition = new Coord();
            bool coordFound = false;
            for (int i = pivotCoinCoord.tileX + 1; i < _mapWidth && !coordFound; i++)
            {
                availablePosition = new Coord(i, pivotCoinCoord.tileY);
                if (_availableCoords.Contains(availablePosition))
                {
                    coordFound = true;
                }
                
            }

            if (coordFound) return availablePosition;

            for (int i = pivotCoinCoord.tileY + 1; i < _mapWidth && !coordFound; i++)
            {
                availablePosition = new Coord(pivotCoinCoord.tileX, i);
                if (_availableCoords.Contains(availablePosition)) coordFound = true;
            }

            if (coordFound) return availablePosition;

            for (int i = pivotCoinCoord.tileX - 1; i > 0 && !coordFound; i--)
            {
                availablePosition = new Coord(i, pivotCoinCoord.tileY);
                if (_availableCoords.Contains(availablePosition)) coordFound = true;
            }

            if (coordFound) return availablePosition;

            for (int i = pivotCoinCoord.tileY - 1; i > 0 && !coordFound; i--)
            {
                availablePosition = new Coord(pivotCoinCoord.tileX, i);
                if (_availableCoords.Contains(availablePosition)) coordFound = true;
            }

            return availablePosition;
        }

        #endregion

        public int GetNumberOfSurroundingWalls(int posX, int posY)
        {
            int numberOfSurroundingWalls = 0;
            // Se comprueba lo siguiente:
            //  |x - 1, y + 1,| x, y + 1   | x + 1, y + 1
            //  |x - 1, y     | posX, posY | x + 1, y
            //  |x - 1, y - 1 | x, y - 1   | x + 1, y - 1
            for (int rowIndex = posX - 1; rowIndex <= posX + 1; rowIndex++)
            {
                for (int columnIndex = posY - 1; columnIndex <= posY + 1; columnIndex++)
                {
                    // Si la posición está fuera del mapa se considera un muro
                    if (rowIndex < 0 || rowIndex >= _mapWidth || columnIndex < 0 || columnIndex >= _mapHeight)
                    {
                        numberOfSurroundingWalls++;
                        continue;
                    }
                    else
                    {
                        // Excluimos la comprobación de la casilla propia
                        if (rowIndex == posX && columnIndex == posY) continue;
                        //if(_map[rowIndex, columnIndex] == 1)
                            numberOfSurroundingWalls += _map[rowIndex, columnIndex];
                    }
                }
            }

            return numberOfSurroundingWalls;
        }

        // ehe
        public Vector2 CoordToWorldPoint(Coord tile)
        {
            Vector2 worldPosition = new Vector2(-_mapWidth / 2 + .5f + tile.tileX, -_mapHeight / 2 + .5f + tile.tileY);
            worldPosition *= _mapSquareSize;
            //Debug.Log($"[WorldPos]: {worldPosition}");
            return worldPosition;
        }

    }

}