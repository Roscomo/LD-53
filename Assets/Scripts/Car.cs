using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Car : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float slowSpeed = 0.05f;

    private Tilemap _tilemap;
    private RoadTile _currentRoadTile;
    private Vector3Int _currentTilePosition;
    private CarDirection _direction;
    private float _speed = 0.0f;
    private bool _stopped;

    private Rigidbody2D _rb;

    private bool hasReset = false;
    private bool hasFlipped = false;
    
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
        _rb = GetComponent<Rigidbody2D>();
        _currentTilePosition = _tilemap.WorldToCell(transform.position);
        _currentRoadTile = _tilemap.GetTile<RoadTile>(_currentTilePosition);
        hasFlipped = true;
    }

    private void Update()
    {
        if(!_currentRoadTile) return;

        var close = IsCloseToTileCenter();


        switch (_currentRoadTile.tileType)
        {
            case RoadTileType.StraightHorizontal:
            case RoadTileType.StraightVertical:
            case RoadTileType.FourWayJunction:
            case RoadTileType.ThreeWayLeft:
            case RoadTileType.ThreeWayRight:
            case RoadTileType.ThreeWayUp:
            case RoadTileType.ThreeWayDown:
                break;
            case RoadTileType.CornerUpLeft:
            case RoadTileType.CornerUpRight:
            case RoadTileType.CornerDownLeft:
            case RoadTileType.CornerDownRight:
            case RoadTileType.DeadEnd:
                if(close && !hasReset)
                {
                    var newPos = _tilemap.CellToWorld(_currentTilePosition) + _tilemap.tileAnchor;
                    newPos.z = 0.0f;
                    transform.position = newPos;
                    hasReset = true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        
        
        switch (_currentRoadTile.tileType)
        {
            case RoadTileType.StraightVertical:
                break;
            case RoadTileType.StraightHorizontal:
                break;
            case RoadTileType.CornerUpLeft:
                if (close)
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
                if (close)
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
                if (close)
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
                if (close)
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
            case RoadTileType.DeadEnd:
                if (close && !hasFlipped)
                {
                    hasFlipped = true;
                    ReverseDirection();
                }
                break;
            case RoadTileType.ThreeWayLeft:
            case RoadTileType.ThreeWayRight:
            case RoadTileType.ThreeWayUp:
            case RoadTileType.ThreeWayDown:
            case RoadTileType.FourWayJunction:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var currentPosOnGrid = _tilemap.WorldToCell(transform.position);
        if (currentPosOnGrid != _currentTilePosition)
        {
            hasReset = false;
            hasFlipped = false;
            _currentTilePosition = currentPosOnGrid;
            _currentRoadTile = _tilemap.GetTile<RoadTile>(_currentTilePosition);

            if (!_currentRoadTile && gameObject.CompareTag("Player"))
            {
                print("No Tile");
                GameLogicManager.Instance.GameFailed();
            }
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_stopped)
        {
            return;
        }

        if (_currentRoadTile is null || GameLogicManager.Instance.Phase is not GamePhase.PlayOut)
        {
            _speed = Mathf.Lerp(_speed, 0.0f, slowSpeed * Time.deltaTime);
            _rb.MovePosition(transform.position + transform.up * (Time.fixedDeltaTime * _speed));
            return;
        }

        _speed = maxSpeed;
        _rb.MovePosition(transform.position + transform.up * (Time.fixedDeltaTime * _speed));
    }

    private bool IsCloseToTileCenter()
    {
        var tileWorldPos = _tilemap.CellToWorld(_currentTilePosition) + _tilemap.tileAnchor;
        return Vector2.Distance(transform.position, tileWorldPos) < 0.1f;
    }

    public void ChangeDirection(CarDirection direction)
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

    public void Stop()
    {
        StartCoroutine(StopCoroutine());
    }

    private IEnumerator StopCoroutine()
    {
        _stopped = true;

        yield return new WaitForSeconds(0.5f);

        _stopped = false;
    }

    public void ResetPosition()
    {
        var newPos = _tilemap.CellToWorld(_currentTilePosition) + _tilemap.tileAnchor;
        newPos.z = 0.0f;
        transform.position = newPos;
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Car>() && GameLogicManager.Instance.Phase is GamePhase.PlayOut)
        {
            print("Failed");
            GameLogicManager.Instance.GameFailed();
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

