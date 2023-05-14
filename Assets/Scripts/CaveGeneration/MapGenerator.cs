using ProceduralCave.Generator.CaveMesh;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralCave.Generator
{

    public class MapGenerator : MonoBehaviour
    {

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
        [SerializeField] private float _coinsAmount;
        [SerializeField] private int _coinsInfluence;
        [SerializeField] private GameObject _coinPrefab;

        private int[,] _map;

        private List<Coord> _availableCoords;
        private int _spawnedCoins;

        private void Awake()
        {
            _availableCoords = new();
            _spawnedCoins = 0;

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

        public void FillMapWithCoins()
        {
            int[,] auxMap = _map;

            for (int i = 0; i < _mapWidth; i++)
            {
                for (int j = 0; j < _mapHeight; j++)
                {
                    int neighbourWallTiles = GetNumberOfSurroundingWalls(i, j);

                    if (neighbourWallTiles == 0 && auxMap[i,j] == 0) 
                    {
                        auxMap[i, j] = 2;
                        Coord coinCoord = new Coord(i, j);
                        _availableCoords.Add(coinCoord);
                    }
                }
            }

            var rnd = new System.Random();
            _availableCoords.Shuffle(rnd);

            for (int i = 0; i < _availableCoords.Count && _spawnedCoins < _coinsAmount; i++)
            {
                GameObject coin = Instantiate(_coinPrefab);
                coin.transform.position = CoordToWorldPoint(_availableCoords[i]);

                _spawnedCoins++;
            }

            _map = auxMap;
        }

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