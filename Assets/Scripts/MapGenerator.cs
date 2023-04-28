//using UnityEngine;
//using System.Collections;
//using System;

//public class MapGenerator : MonoBehaviour
//{

//	public int width;
//	public int height;

//	public string seed;
//	public bool useRandomSeed;

//	[Range(0, 100)]
//	public int randomFillPercent;

//	int[,] map;

//	void Start()
//	{
//		GenerateMap();
//	}

//	void Update()
//	{
//		if (Input.GetMouseButtonDown(0))
//		{
//			GenerateMap();
//		}
//	}

//	void GenerateMap()
//	{
//		map = new int[width, height];
//		RandomFillMap();

//		for (int i = 0; i < 5; i++)
//		{
//			SmoothMap();
//		}
//	}


//	void RandomFillMap()
//	{
//		if (useRandomSeed)
//		{
//			seed = Time.time.ToString();
//		}

//		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

//		for (int x = 0; x < width; x++)
//		{
//			for (int y = 0; y < height; y++)
//			{
//				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
//				{
//					map[x, y] = 1;
//				}
//				else
//				{
//					map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
//				}
//			}
//		}
//	}

//	void SmoothMap()
//	{
//		for (int x = 0; x < width; x++)
//		{
//			for (int y = 0; y < height; y++)
//			{
//				int neighbourWallTiles = GetSurroundingWallCount(x, y);

//				if (neighbourWallTiles > 4)
//					map[x, y] = 1;
//				else if (neighbourWallTiles < 4)
//					map[x, y] = 0;

//			}
//		}
//	}

//	int GetSurroundingWallCount(int gridX, int gridY)
//	{
//		int wallCount = 0;
//		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
//		{
//			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
//			{
//				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
//				{
//					if (neighbourX != gridX || neighbourY != gridY)
//					{
//						wallCount += map[neighbourX, neighbourY];
//					}
//				}
//				else
//				{
//					wallCount++;
//				}
//			}
//		}

//		return wallCount;
//	}


//void OnDrawGizmos()
//{
//	if (map != null)
//	{
//		for (int x = 0; x < width; x++)
//		{
//			for (int y = 0; y < height; y++)
//			{
//				Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
//				Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
//				Gizmos.DrawCube(pos, Vector3.one);
//			}
//		}
//	}
//}

//	private void OnDrawGizmosSelected()
//	{
//		if (map == null) return;

//		for (int i = 0; i < width; i++)
//		{
//			for (int j = 0; j < height; j++)
//			{
//				//_map[i, j] = Random.Range(0, 2);
//				Gizmos.color = map[i, j] == 1 ? Color.black : Color.white;
//				Gizmos.DrawCube(new Vector2(i, j), Vector2.one);
//			}
//		}
//	}
//}

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
        int wallCount = 0;
        for (int neighbourX = posX - 1; neighbourX <= posX + 1; neighbourX++)
        {
            for (int neighbourY = posY - 1; neighbourY <= posY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < _mapWidth && neighbourY >= 0 && neighbourY < _mapHeight)
                {
                    if (neighbourX != posX || neighbourY != posY)
                    {
                        wallCount += _map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
        //int numberOfSurroundingWalls = 0;
        //// Se comprueba lo siguiente:
        ////  |x - 1, y + 1,| x, y + 1   | x + 1, y + 1
        ////  |x - 1, y     | posX, posY | x + 1, y
        ////  |x - 1, y - 1 | x, y - 1   | x + 1, y - 1
        //for (int rowIndex = posX - 1; rowIndex <= posX + 1; rowIndex++)
        //{
        //    for (int columnIndex = posY - 1; columnIndex < posY + 1; columnIndex++)
        //    {
        //        // Si la posición está fuera del mapa o es la coordenada de la que queremos
        //        // contar sus vecinos, ignoramos la iteración y pasamos a la siguiente
        //        if (rowIndex < 0 || rowIndex >= _mapWidth || columnIndex < 0 || columnIndex >= _mapHeight
        //            || (rowIndex == posX && columnIndex == posY))
        //        {
        //            //Debug.Log($"[Posición]: {rowIndex} {columnIndex} está fuera del mapa");
        //            continue;
        //        }

        //        numberOfSurroundingWalls += _map[rowIndex, columnIndex] == 1 ? 1 : 0;
        //    }
        //}
        //Debug.Log($"[Number of surrounding walls for {posX} {posY}]: {numberOfSurroundingWalls}");

        //return numberOfSurroundingWalls;
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
