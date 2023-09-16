using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class RoadGeneration : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int dimension;
    [SerializeField]
    [Min(0)]
    private int tileSize = 20;
    [SerializeField]
    [Min(0)]
    private int numberOfNPC;

    [SerializeField]
    private Vector2 startPos;
    [SerializeField]
    private ScriptableObjectCityTile[] startTiles;

    [SerializeField]
    private Vector2 endPos;
    [SerializeField]
    private ScriptableObjectCityTile[] endTiles;

    private Vector2 exitPos;
    [SerializeField]
    private ScriptableObjectCityTile[] exitTiles;

    [SerializeField]
    private ScriptableObjectCityTile[] limitTiles;
    [SerializeField]
    private ScriptableObjectCityTile[] sideTiles;
    [SerializeField]
    private ScriptableObjectCityTile[] tiles;

    [SerializeField]
    private GameObject[] npcs;

    List<Vector2> unexploredTiles;

    private Dictionary<Vector2, GeneratorCell> city;
    private Dictionary<Vector2, GeneratorCell> edgeCity;
    private Queue<Vector2> cellQueue;

    private GeneratorCell currentCell;
    private Vector2 currentPos;
    private Vector2 currentPropagatePos;

    private Vector2[] cellNeighbors = new Vector2[4];

    private List<Vector2> propagatedCells;

    private List<Vector2> roadPos;

    private bool isDone = false;

    long nPrint = 0;

    public bool IsDone
    {
        get { return isDone; }
    }

    public Vector3 StartPos { get { return transform.localPosition + new Vector3(startPos.x * tileSize, 0, startPos.y * tileSize); } }

    public Vector3 EndPos { get { return transform.localPosition + new Vector3(endPos.x * tileSize, 0, endPos.y * tileSize); } }

    public Vector3 ExitPos { get { return transform.localPosition + new Vector3(exitPos.x * tileSize, 0, exitPos.y * tileSize); } }

    public int TileSize { get { return tileSize; } }


    // Start is called before the first frame update
    void Start()
    {
        if (startPos.x >= dimension || startPos.y >= dimension || startPos.x < 0 || startPos.y < 0)
            startPos = new Vector2(startPos.x % dimension, startPos.y % dimension);

        if (dimension >= 3 && ((endPos.x == 0 && endPos.y == 0) || (endPos.x == dimension - 1 && endPos.y == 0) ||
            (endPos.x == 0 && endPos.y == dimension - 1) || (endPos.x == dimension - 1 && endPos.y == dimension - 1) ||
            endPos.x != dimension - 1 && endPos.y != dimension - 1 && endPos.x != 0 && endPos.y != 0))
        {
            int rand = Random.Range(0, 1);
            endPos = new Vector2(rand == 0 ? (startPos.x == 0 ? dimension - 1 : 0) : Random.Range(1, dimension - 2),
                                  rand == 1 ? (startPos.y == 0 ? dimension - 1 : 0) : Random.Range(1, dimension - 2));
        }
        else
            endPos = new Vector2(endPos.x % dimension, endPos.y % dimension);


        unexploredTiles = new List<Vector2> { startPos, endPos };

        System.Array directions = System.Enum.GetValues(typeof(ScriptableObjectCityTile.Direction));
        System.Array types = System.Enum.GetValues(typeof(ScriptableObjectCityTile.SideType));

        city = new Dictionary<Vector2, GeneratorCell>();
        edgeCity = new Dictionary<Vector2, GeneratorCell>();
        cellQueue = new Queue<Vector2>();
        propagatedCells = new List<Vector2>();
        roadPos = new List<Vector2>();

        for (int x = 0; x < dimension; x++)
            for (int y = 0; y < dimension; y++)
                if (x == endPos.x && y == endPos.y)
                    city.Add(new Vector2(x, y), new GeneratorCell(endTiles.Length > 0 ? endTiles : sideTiles,
                        directions.Length, types.Length, new Vector3(x * tileSize, 0, y * tileSize), this.transform));

                else if (x == 0 || y == 0 || x == dimension - 1 || y == dimension - 1)
                {
                    city.Add(new Vector2(x, y), new GeneratorCell(x == startPos.x && y == startPos.y && startTiles.Length > 0 ? startTiles : sideTiles,
                        directions.Length, types.Length, new Vector3(x * tileSize, 0, y * tileSize), this.transform,
                        y == dimension - 1 ? ScriptableObjectCityTile.SideType.SIDEWALK : ScriptableObjectCityTile.SideType.ANY,
                        x == 0 ? ScriptableObjectCityTile.SideType.SIDEWALK : ScriptableObjectCityTile.SideType.ANY,
                        y == 0 ? ScriptableObjectCityTile.SideType.SIDEWALK : ScriptableObjectCityTile.SideType.ANY,
                        x == dimension - 1 ? ScriptableObjectCityTile.SideType.SIDEWALK : ScriptableObjectCityTile.SideType.ANY));

                    edgeCity.Add(new Vector2(x, y), new GeneratorCell(limitTiles, directions.Length, types.Length, new Vector3(x * tileSize, 0, y * tileSize), this.transform,
                        y == dimension - 1 ? ScriptableObjectCityTile.SideType.PORT : ScriptableObjectCityTile.SideType.SIDEWALK,
                        x == 0 ? ScriptableObjectCityTile.SideType.PORT : ScriptableObjectCityTile.SideType.SIDEWALK,
                        y == 0 ? ScriptableObjectCityTile.SideType.PORT : ScriptableObjectCityTile.SideType.SIDEWALK,
                        x == dimension - 1 ? ScriptableObjectCityTile.SideType.PORT : ScriptableObjectCityTile.SideType.SIDEWALK));
                }

                else
                    city.Add(new Vector2(x, y), new GeneratorCell(x == startPos.x && y == startPos.y && startTiles.Length > 0 ? startTiles : tiles,
                        directions.Length, types.Length, new Vector3(x * tileSize, 0, y * tileSize), this.transform));

        if (exitTiles.Length > 0)
        {
            exitPos = new Vector2(endPos.x == 0 || endPos.x == dimension - 1 ? (endPos.x == 0 ? -1 : dimension) : endPos.x,
            endPos.y == 0 || endPos.y == dimension - 1 ? (endPos.y == 0 ? -1 : dimension) : endPos.y);

            city.Add(exitPos, new GeneratorCell(exitTiles, directions.Length, types.Length, new Vector3(exitPos.x * tileSize, 0, exitPos.y * tileSize), this.transform));
        }

        StartCoroutine(WaveFunctionCollapse());
    }

    private void GetCellNeighbors(Vector2 cellPos)
    {
        cellNeighbors[0] = city.ContainsKey(cellPos + Vector2.up) ? cellPos + Vector2.up : Vector2.negativeInfinity;
        cellNeighbors[1] = city.ContainsKey(cellPos + Vector2.left) ? cellPos + Vector2.left : Vector2.negativeInfinity;
        cellNeighbors[2] = city.ContainsKey(cellPos + Vector2.down) ? cellPos + Vector2.down : Vector2.negativeInfinity;
        cellNeighbors[3] = city.ContainsKey(cellPos + Vector2.right) ? cellPos + Vector2.right : Vector2.negativeInfinity;
    }

    private void SetRoadNeighbors(Vector2 cellPos)
    {
        ScriptableObjectCityTile cellTile = city[cellPos].CurrentTile;
        if (cellNeighbors[0].x > 0 && !city[cellNeighbors[0]].IsClosed && cellTile.North.Contains(ScriptableObjectCityTile.SideType.ROAD))
            city[cellNeighbors[0]].isRoad = true;  

        if (cellNeighbors[1].x > 0 && !city[cellNeighbors[1]].IsClosed && cellTile.West.Contains(ScriptableObjectCityTile.SideType.ROAD))
            city[cellNeighbors[1]].isRoad = true;

        if (cellNeighbors[2].x > 0 && !city[cellNeighbors[2]].IsClosed && cellTile.South.Contains(ScriptableObjectCityTile.SideType.ROAD))
            city[cellNeighbors[2]].isRoad = true;

        if (cellNeighbors[3].x > 0 && !city[cellNeighbors[3]].IsClosed && cellTile.East.Contains(ScriptableObjectCityTile.SideType.ROAD))
            city[cellNeighbors[3]].isRoad = true;
    }

    private Vector2[] LabyrinthNeighbors(Vector2 pos)
    {
        List<Vector2> neighbors = new List<Vector2>();

        if (roadPos.Contains(pos + Vector2.up))
            neighbors.Add(pos + Vector2.up);

        if (roadPos.Contains(pos + Vector2.left))
            neighbors.Add(pos + Vector2.left);

        if (roadPos.Contains(pos + Vector2.right))
            neighbors.Add(pos + Vector2.right);

        if (roadPos.Contains(pos + Vector2.down))
            neighbors.Add(pos + Vector2.down);

        return neighbors.ToArray();
    }

    private bool Propagate()
    {
        cellQueue.Enqueue(currentPos);
        while (cellQueue.Count > 0)
        {
            currentPropagatePos = cellQueue.Dequeue();
            currentCell = city[currentPropagatePos];

            GetCellNeighbors(currentPropagatePos);

            int tilesDropped = 0;
            for (int i = 0; i < cellNeighbors.Length; i++)
            {
                if (cellNeighbors[i].x >= 0 && city[cellNeighbors[i]].CurrentTile == null)
                {
                    switch (i)
                    {
                        case 0:
                            tilesDropped = city[cellNeighbors[i]].Propagate(currentCell.GetAllowedTypes(ScriptableObjectCityTile.Direction.NORTH), ScriptableObjectCityTile.Direction.SOUTH, currentCell.CurrentTile?.RestrictionNorth, currentCell.CurrentTile);
                            break;
                        case 1:
                            tilesDropped = city[cellNeighbors[i]].Propagate(currentCell.GetAllowedTypes(ScriptableObjectCityTile.Direction.WEST), ScriptableObjectCityTile.Direction.EAST, currentCell.CurrentTile?.RestrictionWest, currentCell.CurrentTile);
                            break;
                        case 2:
                            tilesDropped = city[cellNeighbors[i]].Propagate(currentCell.GetAllowedTypes(ScriptableObjectCityTile.Direction.SOUTH), ScriptableObjectCityTile.Direction.NORTH, currentCell.CurrentTile?.RestrictionSouth, currentCell.CurrentTile);
                            break;
                        case 3:
                            tilesDropped = city[cellNeighbors[i]].Propagate(currentCell.GetAllowedTypes(ScriptableObjectCityTile.Direction.EAST), ScriptableObjectCityTile.Direction.WEST, currentCell.CurrentTile?.RestrictionEast, currentCell.CurrentTile);
                            break;
                    }

                    if (!propagatedCells.Contains(cellNeighbors[i]))
                        propagatedCells.Add(cellNeighbors[i]);

                    if (city[cellNeighbors[i]].GetNumberOfPossibleTiles() == 0)
                    {
                        cellQueue.Clear();
                        return false;
                    }

                    if (tilesDropped > 0)
                        cellQueue.Enqueue(cellNeighbors[i]);
                }
            }
        }
        return true;
    }

    private void RecursiveBacktracker()
    {
        Stack<Vector2> positionStack = new Stack<Vector2>();
        positionStack.Push(startPos);
        while (positionStack.Count > 0)
        {
            currentPos = positionStack.Pop();

            if (roadPos.Contains(currentPos))
                roadPos.Remove(currentPos);

            Vector2[] neighbors = LabyrinthNeighbors(currentPos);

            if (neighbors.Length > 0)
            {

                foreach (Vector2 neighbor in neighbors)
                {
                    positionStack.Push(neighbor);
                    roadPos.Remove(neighbor);
                }
            }
            else
            {
                Debug.Log(currentPos * tileSize);
                city[currentPos].SetObstacle();
            }
                
        }
    }

    private IEnumerator WaveFunctionCollapse()
    {
        while (unexploredTiles.Count > 0)
        {
            //Obter o tile
            currentPos = unexploredTiles.FirstOrDefault();
            unexploredTiles.RemoveAt(0);

            //Selecionar o tile
            city[currentPos].SelectTile();
            //DebugPrint("wwwwww");
            //DebugPrint(currentPos + "");
            //DebugPrint(city[currentPos].CurrentTile + "");
            //DebugPrint(city[currentPos].currentDirection + "");

            if (city[currentPos].isRoad && !roadPos.Contains(currentPos))
                roadPos.Add(currentPos);

            if (city[currentPos].CurrentTile == null)
            {
                //DebugPrint("Here");
                /*DebugPrint(currentPos + "");
                foreach (GeneratorCell cell in city.Values)
                    cell.SpawnTile();
                break;*/
                foreach (GeneratorCell cell in city.Values)
                    cell.Reset();
                unexploredTiles.Clear();
                roadPos.Clear();
                unexploredTiles.Add(startPos);
                continue;
            }

            //Propagação
            if (!Propagate())
            {
                //DebugPrint("dddddddd");
                foreach (Vector2 pos in propagatedCells)
                    city[pos].Unpropagate();
                propagatedCells.Clear();
                city[currentPos].RemoveTileVariation(city[currentPos].CurrentTile, city[currentPos].currentDirection);
                unexploredTiles.Insert(0, currentPos);
                continue;
            }

            foreach (Vector2 pos in propagatedCells)
                city[pos].SetPropagation();
            propagatedCells.Clear();

            //Obter visinhos
            GetCellNeighbors(currentPos);
            //Definir os visinhos que são estradas.
            SetRoadNeighbors(currentPos);

            //Adicionar tiles não selecionados
            for (int i = 0; i < cellNeighbors.Length; i++)
                if (cellNeighbors[i].x >= 0 && !city[cellNeighbors[i]].IsClosed && !unexploredTiles.Contains(cellNeighbors[i]))
                    unexploredTiles.Add(cellNeighbors[i]);


            unexploredTiles.Sort(delegate (Vector2 pos1, Vector2 pos2)
            {
                if ((city[pos1].isRoad && city[pos2].isRoad) || (!city[pos1].isRoad && !city[pos2].isRoad))
                {
                    if (city[pos1].GetNumberOfPossibleTiles() > city[pos2].GetNumberOfPossibleTiles())
                        return 1;
                    else if (city[pos1].GetNumberOfPossibleTiles() < city[pos2].GetNumberOfPossibleTiles())
                        return -1;
                    else return 0;
                }
                else if (city[pos1].isRoad && !city[pos2].isRoad)
                    return -1;
                else if (!city[pos1].isRoad && city[pos2].isRoad)
                    return 1;
                else return 0;
            });

            yield return null;
        }

        RecursiveBacktracker();

        foreach (GeneratorCell cell in city.Values)
            cell.SpawnTile();

        foreach (GeneratorCell cell in edgeCity.Values)
        {
            cell.SelectTile();
            cell.SpawnTile();
        }

        if (npcs.Length > 0)
            for (int x = 0; x < numberOfNPC; x++)
            {
                Vector3 npcPos = new Vector3(Random.Range(-tileSize / 2, (dimension - 1) * tileSize), 0,
                    Random.Range(-tileSize / 2, (dimension - 1) * tileSize));

                if (Physics.CheckSphere(npcPos, 5, LayerMask.GetMask("Obstacles")))
                {
                    x--;
                    continue;
                }

                Instantiate(npcs[Random.Range(0, npcs.Length)], npcPos + transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)), transform);
            }

        yield return new WaitForSeconds(0.5f);

        isDone = true;
    }  

    private void DebugPrint(string text)
    {
        nPrint += 1;
        Debug.Log(text + " " + nPrint);
    }
}
