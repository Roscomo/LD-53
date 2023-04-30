using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Car : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] [Range(0, 1)] private float slowSpeed = 0.05f; 
    
    private Tilemap _tilemap;
    private RoadTile _currentRoadTile;
    private Vector3Int _currentTilePosition;
    private CarDirection _direction;
    private float _speed = 0.0f;
    
    public CarDirection Direction
    {
        get => _direction;
        set => _direction = value;
    }

    public Tilemap Tilemap
    {
        get => _tilemap;
        set => _tilemap = value;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _currentTilePosition = _tilemap.WorldToCell(transform.position);
        _currentRoadTile = _tilemap.GetTile<RoadTile>(_currentTilePosition);

        print(_tilemap.CellToWorld(_currentTilePosition));
    }

    // Update is called once per frame
    private void Update()
    {

        if (_currentRoadTile is null || GameLogicManager.Instance.Phase is not GamePhase.PlayOut)
        {
            _speed = Mathf.Lerp(_speed, 0.0f, slowSpeed);
            transform.Translate(Vector2.up * (Time.deltaTime * _speed));
            return;
        }

        _speed = maxSpeed;
        transform.Translate(Vector2.up * (Time.deltaTime * _speed));

        switch (_currentRoadTile.tileType)
        {
            case RoadTileType.StraightVertical:
                break;
            case RoadTileType.StraightHorizontal:
                break;
            case RoadTileType.CornerUpLeft:
                if (IsCloseToTileCenter())
                {
                    if (_direction is CarDirection.NorthBound)
                    {
                        ChangeDirection(CarDirection.WestBound);
                    }
                    
                    else if (_direction is CarDirection.EastBound)
                    {
                        ChangeDirection(CarDirection.SouthBound);
                    }
                }
                break;
            case RoadTileType.CornerUpRight:
                if (IsCloseToTileCenter())
                {
                    if (_direction is CarDirection.NorthBound)
                    {
                        ChangeDirection(CarDirection.EastBound);
                    }
                    
                    else if (_direction is CarDirection.WestBound)
                    {
                        ChangeDirection(CarDirection.SouthBound);
                    }
                }
                break;
            case RoadTileType.CornerDownLeft:
                if (IsCloseToTileCenter())
                {
                    if (_direction is CarDirection.SouthBound)
                    {
                        ChangeDirection(CarDirection.WestBound);
                    }
                    
                    else if (_direction is CarDirection.EastBound)
                    {
                        ChangeDirection(CarDirection.NorthBound);
                    }
                }
                break;
            case RoadTileType.CornerDownRight:
                if (IsCloseToTileCenter())
                {
                    if (_direction is CarDirection.SouthBound)
                    {
                        ChangeDirection(CarDirection.EastBound);
                    }
                    
                    else if (_direction is CarDirection.WestBound)
                    {
                        ChangeDirection(CarDirection.NorthBound);
                    }
                }
                break;
            case RoadTileType.FourWayJunction:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var currentPosOnGrid = _tilemap.WorldToCell(transform.position);
        if (currentPosOnGrid != _currentTilePosition)
        {
            
            if (_currentRoadTile.currentCar == this)
            {
                _currentRoadTile.currentCar = null;
            }

            _currentTilePosition = currentPosOnGrid;
            _currentRoadTile = _tilemap.GetTile<RoadTile>(_currentTilePosition);

            if (_currentRoadTile && !_currentRoadTile.currentCar)
            {
                _currentRoadTile.currentCar = this;
            }
        }
    }

    private bool IsCloseToTileCenter()
    {
        var tileWorldPos = _tilemap.CellToWorld(_currentTilePosition) + _tilemap.tileAnchor;
        return Vector2.Distance(transform.position, tileWorldPos) < 0.01f;
    }

    private void ChangeDirection(CarDirection direction)
    {
        _direction = direction;

        transform.up = direction switch
        {
            CarDirection.NorthBound => Vector2.up,
            CarDirection.SouthBound => Vector2.down,
            CarDirection.WestBound => Vector2.left,
            CarDirection.EastBound => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public void ReverseDirection()
    {
        switch (_direction)
        {
            case CarDirection.NorthBound:
                ChangeDirection(CarDirection.SouthBound);
                break;
            case CarDirection.SouthBound:
                ChangeDirection(CarDirection.NorthBound);
                break;
            case CarDirection.WestBound:
                ChangeDirection(CarDirection.EastBound);
                break;
            case CarDirection.EastBound:
                ChangeDirection(CarDirection.WestBound);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum CarDirection
{
    NorthBound,
    SouthBound,
    WestBound,
    EastBound,
}
