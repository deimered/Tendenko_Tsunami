using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScriptableObjectCityTile", menuName = "ScriptableObjects/City Tile")]
public class ScriptableObjectCityTile : ScriptableObject
{
    public enum SideType { SIDEWALK, ROAD, ANY, BUILDING, BUILDINGROAD, PORT};
    public enum Direction { NORTH, WEST, SOUTH, EAST};

    [SerializeField]
    private GameObject[] tileVariations;

    [SerializeField]
    private float[] weights;

    /// <summary>
    /// /////////////////
    /// </summary>
    [SerializeField]
    private bool isObstacle;
    /// <summary>
    /// /////////////////
    /// </summary>

    [SerializeField]
    [Range(0, 1)]
    private float tileWeight = 1;

    [SerializeField]
    private Direction direction;

    [SerializeField]
    private SideType[] northSide;
    [SerializeField]
    private SideType[] westSide;
    [SerializeField]
    private SideType[] eastSide;
    [SerializeField]
    private SideType[] southSide;

    [SerializeField]
    private ScriptableObjectCityTile[] restrictionNorthSide;
    [SerializeField]
    private ScriptableObjectCityTile[] restrictionWestSide;
    [SerializeField]
    private ScriptableObjectCityTile[] restrictionEastSide;
    [SerializeField]
    private ScriptableObjectCityTile[] restrictionSouthSide;

    public SideType[] North
    {
        get {
            return direction switch
            {
                Direction.NORTH => northSide,
                Direction.WEST => eastSide,
                Direction.SOUTH => southSide,
                Direction.EAST => westSide,
                _ => northSide,
            };
        }
    }

    public SideType[] West
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => westSide,
                Direction.WEST => northSide,
                Direction.SOUTH => eastSide,
                Direction.EAST => southSide,
                _ => westSide,
            };
        }
    }

    public SideType[] East
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => eastSide,
                Direction.WEST => southSide,
                Direction.SOUTH => westSide,
                Direction.EAST => northSide,
                _ => eastSide,
            };
        }
    }

    public SideType[] South
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => southSide,
                Direction.WEST => westSide,
                Direction.SOUTH => northSide,
                Direction.EAST => eastSide,
                _ => southSide,
            };
        }
    }

    public ScriptableObjectCityTile[] RestrictionNorth
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => restrictionNorthSide,
                Direction.WEST => restrictionEastSide,
                Direction.SOUTH => restrictionSouthSide,
                Direction.EAST => restrictionWestSide,
                _ => restrictionNorthSide,
            };
        }
    }

    public ScriptableObjectCityTile[] RestrictionWest
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => restrictionWestSide,
                Direction.WEST => restrictionNorthSide,
                Direction.SOUTH => restrictionEastSide,
                Direction.EAST => restrictionSouthSide,
                _ => restrictionWestSide,
            };
        }
    }

    public ScriptableObjectCityTile[] RestrictionEast
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => restrictionEastSide,
                Direction.WEST => restrictionSouthSide,
                Direction.SOUTH => restrictionWestSide,
                Direction.EAST => restrictionNorthSide,
                _ => restrictionEastSide,
            };
        }
    }

    public ScriptableObjectCityTile[] RestrictionSouth
    {
        get
        {
            return direction switch
            {
                Direction.NORTH => restrictionSouthSide,
                Direction.WEST => restrictionWestSide,
                Direction.SOUTH => restrictionNorthSide,
                Direction.EAST => restrictionEastSide,
                _ => restrictionSouthSide,
            };
        }
    }

    public float Weight
    {
        get
        {
            return tileWeight;
        }
    }

    public float[] Weights
    {
        get { return weights; }
    }

    public bool IsObstacle
    {
        get { return isObstacle; }
    }

    public void RotateTile(Direction direction)
    {
        this.direction = direction;
    }

    public GameObject GetRandomTile()
    {
        float weightSum = 0;
        float random = Random.Range(0f, 1f);
        for (int i = 0; i < weights.Length; i++)
        {
            weightSum += weights[i];
            if (weightSum >= random)
                return tileVariations[i];
        }
        return tileVariations[0];
    }

    public Vector3 GetRotation()
    {
        return direction switch
        {
            Direction.NORTH => Vector3.zero,
            Direction.WEST => Vector3.up * -90,
            Direction.SOUTH => Vector3.up * -180,
            Direction.EAST => Vector3.up * -270,
            _ => Vector3.zero,
        };
    }

    public Direction GetRotatedDirection(Direction direction)
    {
        return (Direction)(((int)direction + (int)this.direction)%4);
    }
}
