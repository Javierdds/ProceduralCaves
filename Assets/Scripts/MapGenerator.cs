using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;

    [SerializeField] private string _seed;
    [SerializeField] private bool _useRandomSeed;
    [SerializeField] private int _smoothIterations;
    [SerializeField] private int _neighborInfluenceIndex;

    [Range(0, 100)]
    [SerializeField] private int _fillPercent;

    private int[,] _map;


    private void Awake()
    {
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



    private void OnDrawGizmosSelected()
    {
        if (_map == null) return;

        for (int i = 0; i < _mapWidth; i++)
        {
            for (int j = 0; j < _mapHeight; j++)
            {
                //_map[i, j] = Random.Range(0, 2);
                Gizmos.color = _map[i, j] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(new Vector2(i, j), Vector2.one);
            }
        }
    }

}
