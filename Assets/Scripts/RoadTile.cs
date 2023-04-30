using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "New Road Tile", menuName = "Tiles/RoadTile")]
public class RoadTile : Tile
{
    public RoadTileType tileType; 
    public Car currentCar;
}

public enum RoadTileType
{
    StraightVertical,
    StraightHorizontal,
    CornerUpLeft,
    CornerUpRight,
    CornerDownLeft,
    CornerDownRight,
    FourWayJunction,
}