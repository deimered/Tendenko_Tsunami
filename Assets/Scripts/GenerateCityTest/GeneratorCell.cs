using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneratorCell
{
    //Dicionário direções
    private Dictionary<int, ScriptableObjectCityTile.Direction> possibleValues;
    private Dictionary<int, ScriptableObjectCityTile.Direction> yetToRemoveValues;

    //Scriptable objects
    private ScriptableObjectCityTile[] tiles;

    //For the propagation
    private bool matchType;
    private int nValuesRemoved = 0;
    private int nDirections;
    private int nTypes;
    private bool closed = false;

    private int[] allowedTypeCount;

    private Vector3 coord;

    private ScriptableObjectCityTile.SideType northRestriction = ScriptableObjectCityTile.SideType.ANY;
    private ScriptableObjectCityTile.SideType westRestriction = ScriptableObjectCityTile.SideType.ANY;
    private ScriptableObjectCityTile.SideType southRestriction = ScriptableObjectCityTile.SideType.ANY;
    private ScriptableObjectCityTile.SideType eastRestriction = ScriptableObjectCityTile.SideType.ANY;

#nullable enable
    //Current Tile

    ScriptableObjectCityTile? currentTile = null;
    public ScriptableObjectCityTile.Direction currentDirection;
    public bool isRoad;

    Transform father;

    public ScriptableObjectCityTile? CurrentTile
    {
        get
        {
            if (currentTile != null)
                currentTile.RotateTile(currentDirection);
            return currentTile;
        }
    }

    public Vector3 Coord
    {
        get { return coord; }
    }

    public int NDirections
    {
        get { return nDirections; }
    }

    public int NTypes
    {
        get { return nTypes; }
    }

    public bool IsClosed
    {
        get { return closed; }
    }

    public GeneratorCell(ScriptableObjectCityTile[] tiles, int nDirections, int nTypes, Vector3 coord, Transform father)
    {
        possibleValues = new Dictionary<int, ScriptableObjectCityTile.Direction>();
        yetToRemoveValues = new Dictionary<int, ScriptableObjectCityTile.Direction>();

        this.tiles = tiles;
        this.nDirections = nDirections;
        this.nTypes = nTypes;
        this.coord = coord;
        this.father = father;

        allowedTypeCount = new int[nDirections * nTypes];
        int idx = 0;

        foreach (ScriptableObjectCityTile tile in tiles)
        {
            for (int i = 0; i < nDirections; i++)
            {
                possibleValues.Add(idx, (ScriptableObjectCityTile.Direction)i);

                tiles[idx / nDirections].RotateTile((ScriptableObjectCityTile.Direction)i);

                foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].North)
                    allowedTypeCount[(int)ScriptableObjectCityTile.Direction.NORTH * nTypes + (int)type]++;

                foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].West)
                    allowedTypeCount[(int)ScriptableObjectCityTile.Direction.WEST * nTypes + (int)type]++;

                foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].South)
                    allowedTypeCount[(int)ScriptableObjectCityTile.Direction.SOUTH * nTypes + (int)type]++;

                foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].East)
                    allowedTypeCount[(int)ScriptableObjectCityTile.Direction.EAST * nTypes + (int)type]++;

                idx++;
            }
        }
    }

    public GeneratorCell(ScriptableObjectCityTile[] tiles, int nDirections, int nTypes, Vector3 coord, Transform father,
        ScriptableObjectCityTile.SideType northRestriction, ScriptableObjectCityTile.SideType westRestriction,
        ScriptableObjectCityTile.SideType southRestriction, ScriptableObjectCityTile.SideType eastRestriction)
    {
        possibleValues = new Dictionary<int, ScriptableObjectCityTile.Direction>();
        yetToRemoveValues = new Dictionary<int, ScriptableObjectCityTile.Direction>();

        this.tiles = tiles;
        this.nDirections = nDirections;
        this.nTypes = nTypes;
        this.coord = coord;
        this.father = father;

        this.northRestriction = northRestriction;
        this.westRestriction = westRestriction;
        this.southRestriction = southRestriction;
        this.eastRestriction = eastRestriction;

        Reset();
    }

   

    public int Propagate(ScriptableObjectCityTile.SideType[] types, ScriptableObjectCityTile.Direction direction,
        ScriptableObjectCityTile?[] restrictedTiles, ScriptableObjectCityTile? tile)
    {
        nValuesRemoved = 0;
        foreach (int key in possibleValues.Keys.ToArray())
        {

            if (restrictedTiles != null && restrictedTiles.Contains(tiles[key / nDirections]))
            {
                yetToRemoveValues.Add(key, possibleValues[key]);
                RemoveKey(key);
                nValuesRemoved++;
                continue;
            }

            tiles[key / nDirections].RotateTile(possibleValues[key]);

            switch (direction)
            {
                case ScriptableObjectCityTile.Direction.NORTH:
                    matchType = !tiles[key / nDirections].RestrictionNorth.Contains(tile) &&
                        tiles[key / nDirections].North.Where(x => types.Contains(x)).Count() > 0;
                    break;
                case ScriptableObjectCityTile.Direction.WEST:
                    matchType = !tiles[key / nDirections].RestrictionWest.Contains(tile) &&
                        tiles[key / nDirections].West.Where(x => types.Contains(x)).Count() > 0;
                    break;
                case ScriptableObjectCityTile.Direction.SOUTH:
                    matchType = !tiles[key / nDirections].RestrictionSouth.Contains(tile) &&
                        tiles[key / nDirections].South.Where(x => types.Contains(x)).Count() > 0;
                    break;
                case ScriptableObjectCityTile.Direction.EAST:
                    matchType = !tiles[key / nDirections].RestrictionEast.Contains(tile) &&
                        tiles[key / nDirections].East.Where(x => types.Contains(x)).Count() > 0;
                    break;
            }

            if (!matchType)
            {
                yetToRemoveValues.Add(key, possibleValues[key]);
                RemoveKey(key);
                nValuesRemoved++;
            }
        }

        return nValuesRemoved;
    }

    public void Unpropagate()
    {
        foreach (int key in yetToRemoveValues.Keys.ToArray())
        {
            possibleValues.Add(key, yetToRemoveValues[key]);
            tiles[key / nDirections].RotateTile(yetToRemoveValues[key]);

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].North)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.NORTH * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].West)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.WEST * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].South)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.SOUTH * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].East)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.EAST * nTypes + (int)type]++;
        }
        yetToRemoveValues.Clear();
    }

    public void SetPropagation()
    {
        yetToRemoveValues.Clear();
    }

    public void SelectTile()
    {
        closed = true;
        if (possibleValues.Count > 1)
        {
            float maxWeight = 0;
            foreach (int key in possibleValues.Keys)
                if (!tiles[key / nDirections].IsObstacle)
                    maxWeight += tiles[key / nDirections].Weight;

            float random = Random.Range(0, maxWeight);
            float weight = 0;
            foreach (int key in possibleValues.Keys)
            {
                if (tiles[key / nDirections].IsObstacle)
                    continue;

                weight += tiles[key / nDirections].Weight;

                if (weight >= random)
                {
                    currentDirection = possibleValues[key];
                    currentTile = tiles[key / nDirections];
                    break;
                }
            }
        }
        else if (possibleValues.Count == 1)
        {
            currentTile = tiles[possibleValues.First().Key / nDirections];
            currentDirection = possibleValues.First().Value;
        }

        allowedTypeCount = new int[nDirections * nTypes];

        if (currentTile != null)
        {
            currentTile.RotateTile(currentDirection);

            foreach (ScriptableObjectCityTile.SideType type in currentTile.North)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.NORTH * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in currentTile.West)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.WEST * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in currentTile.South)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.SOUTH * nTypes + (int)type]++;

            foreach (ScriptableObjectCityTile.SideType type in currentTile.East)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.EAST * nTypes + (int)type]++;
        }
    }

    private void RemoveKey(int key)
    {
        if (possibleValues.ContainsKey(key))
        {
            ScriptableObjectCityTile.Direction direction = possibleValues[key];
            tiles[key / nDirections].RotateTile(direction);
            possibleValues.Remove(key);

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].North)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.NORTH * nTypes + (int)type]--;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].West)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.WEST * nTypes + (int)type]--;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].South)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.SOUTH * nTypes + (int)type]--;

            foreach (ScriptableObjectCityTile.SideType type in tiles[key / nDirections].East)
                allowedTypeCount[(int)ScriptableObjectCityTile.Direction.EAST * nTypes + (int)type]--;

            if (currentTile == tiles[key / nDirections] && currentDirection == direction)
            {
                currentTile = null;
                closed = false;
            }
        }
    }

    public ScriptableObjectCityTile.SideType[] GetAllowedTypes(ScriptableObjectCityTile.Direction direction)
    {
        List<ScriptableObjectCityTile.SideType> allowed = new List<ScriptableObjectCityTile.SideType>();

        for (int i = 0; i < nTypes; i++)
        {
            if (allowedTypeCount[(int)direction * nTypes + i] > 0)
                allowed.Add((ScriptableObjectCityTile.SideType)i);


        }

        return allowed.ToArray();
    }
    public int GetNumberOfPossibleTiles()
    {
        return possibleValues.Count;
    }

    public void RemoveTile(ScriptableObjectCityTile tile)
    {
        int idx = IndexInTiles(tile);
        for (int i = 0; i < nDirections; i++)
            RemoveKey(idx * nDirections + i);
    }

    public void RemoveTileVariation(ScriptableObjectCityTile tile, ScriptableObjectCityTile.Direction direction)
    {
        int idx = IndexInTiles(tile);
        RemoveKey(idx * nDirections + (int)direction);
    }

    public void SpawnTile()
    {
        if (currentTile != null)
        {
            currentTile.RotateTile(currentDirection);
            var gm = MonoBehaviour.Instantiate(currentTile.GetRandomTile(), coord, Quaternion.Euler(currentTile.GetRotation()), father);
            gm.transform.localPosition = coord;
        }
    }

    public void Reset()
    {
        closed = false;
        possibleValues.Clear();
        yetToRemoveValues.Clear();
        currentTile = null;
        isRoad = false;
        allowedTypeCount = new int[nDirections * nTypes];
        int idx = 0;

        foreach (ScriptableObjectCityTile tile in tiles)
        {
            for (int i = 0; i < nDirections; i++)
            {
                tiles[idx / nDirections].RotateTile((ScriptableObjectCityTile.Direction)i);

                if ((northRestriction == ScriptableObjectCityTile.SideType.ANY || tiles[idx / nDirections].North.Any(x => northRestriction == x)) &&
                    (westRestriction == ScriptableObjectCityTile.SideType.ANY || tiles[idx / nDirections].West.Any(x => westRestriction == x)) &&
                    (southRestriction == ScriptableObjectCityTile.SideType.ANY || tiles[idx / nDirections].South.Any(x => southRestriction == x)) &&
                    (eastRestriction == ScriptableObjectCityTile.SideType.ANY || tiles[idx / nDirections].East.Any(x => eastRestriction == x)))
                {
                    possibleValues.Add(idx, (ScriptableObjectCityTile.Direction)i);

                    foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].North)
                    {
                        allowedTypeCount[(int)ScriptableObjectCityTile.Direction.NORTH * nTypes + (int)type]++;
                    }
                    foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].West)
                    {
                        allowedTypeCount[(int)ScriptableObjectCityTile.Direction.WEST * nTypes + (int)type]++;
                    }
                    foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].South)
                    {
                        allowedTypeCount[(int)ScriptableObjectCityTile.Direction.SOUTH * nTypes + (int)type]++;
                    }
                    foreach (ScriptableObjectCityTile.SideType type in tiles[idx / nDirections].East)
                    {
                        allowedTypeCount[(int)ScriptableObjectCityTile.Direction.EAST * nTypes + (int)type]++;
                    }

                }
                idx++;
            }
        }
    }

    public void SetObstacle() {
        closed = true;
        if (possibleValues.Count > 0 && currentTile != null)
        {
            currentTile.RotateTile(currentDirection);
            List<int> keys = new List<int>();
            foreach (int key in possibleValues.Keys)
            {
                if (!tiles[key / nDirections].IsObstacle)
                    continue;

                
                tiles[key / nDirections].RotateTile(possibleValues[key]);

                if (ContainsAll(key, currentTile.North, ScriptableObjectCityTile.Direction.NORTH) &&
                    ContainsAll(key, currentTile.West, ScriptableObjectCityTile.Direction.WEST) &&
                    ContainsAll(key, currentTile.South, ScriptableObjectCityTile.Direction.SOUTH) &&
                    ContainsAll(key, currentTile.East, ScriptableObjectCityTile.Direction.EAST))
                    keys.Add(key);
            }

            float maxWeight = 0;
            foreach (int key in keys)
                maxWeight += tiles[key / nDirections].Weight;

            float random = Random.Range(0, maxWeight);
            float weight = 0;
            foreach (int key in keys)
            {
                weight += tiles[key / nDirections].Weight;

                if (weight >= random)
                {
                    currentTile = tiles[key / nDirections];
                    currentDirection = possibleValues[key];
                    break;
                }
            }
        }
    }

    private int IndexInTiles(ScriptableObjectCityTile tile)
    {
        int idx = 0;
        foreach (ScriptableObjectCityTile t in tiles)
        {
            if (t == tile)
                break;
            idx++;
        }
        return idx;
    }

    private bool ContainsAll(int key, ScriptableObjectCityTile.SideType[] cont, ScriptableObjectCityTile.Direction direction)
    {
        switch (direction)
        {
            case ScriptableObjectCityTile.Direction.NORTH:
                foreach (ScriptableObjectCityTile.SideType type in cont)
                    if (!tiles[key / nDirections].North.Contains(type))
                        return false;
                    
                        
                break;
            case ScriptableObjectCityTile.Direction.WEST:
                foreach (ScriptableObjectCityTile.SideType type in cont)
                    if (!tiles[key / nDirections].West.Contains(type))
                        return false;
                    
                        
                break;
            case ScriptableObjectCityTile.Direction.SOUTH:
                foreach (ScriptableObjectCityTile.SideType type in cont)
                    if (!tiles[key / nDirections].South.Contains(type))
                        return false;
                    
                        
                break;
            case ScriptableObjectCityTile.Direction.EAST:
                foreach (ScriptableObjectCityTile.SideType type in cont)
                    if (!tiles[key / nDirections].East.Contains(type))
                        return false;      
                break;
        }
 
        return true;
    }
}
