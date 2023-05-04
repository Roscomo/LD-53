using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpType type;
    
    private Tilemap _tilemap;
    private Car _car;

    public PickUpType Type => type;

    public event Action OnPickUp;

    private SpriteRenderer _renderer;
    private Collider2D _collider;


    private void OnEnable()
    {
        GameLogicManager.Instance.OnLevelRestarted += OnLevelRestarted;
    }

    private void OnDisable()
    {
        GameLogicManager.Instance.OnLevelRestarted -= OnLevelRestarted;
    }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        _tilemap = FindObjectOfType<Tilemap>();

        if (_tilemap)
        {
            var gridPosition = _tilemap.WorldToCell(transform.position);
            var newPosition = _tilemap.CellToWorld(gridPosition) + _tilemap.tileAnchor;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && GameLogicManager.Instance.Phase is GamePhase.Layout)
        {
            if (_tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) ==
                _tilemap.WorldToCell(transform.position))
            {
                Destroy(gameObject);
                OnPickUp?.Invoke();
            }
        }
        
        if(!_car) return;

        if (Vector2.Distance(_car.transform.position, transform.position) < 0.1f)
        {
            switch (type)
            {
                case PickUpType.UTurn:
                    _car.ReverseDirection();
                    break;
                case PickUpType.Stop:
                    _car.Stop();
                    break;
                case PickUpType.TurnUp:
                    if (_car.Direction is CarDirection.EastBound or CarDirection.WestBound)
                    {
                        _car.ChangeDirection(CarDirection.NorthBound);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case PickUpType.TurnDown:
                    if (_car.Direction is CarDirection.EastBound or CarDirection.WestBound)
                    {
                        _car.ChangeDirection(CarDirection.SouthBound);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case PickUpType.TurnLeft:
                    if (_car.Direction is CarDirection.NorthBound or CarDirection.SouthBound)
                    {
                        _car.ChangeDirection(CarDirection.WestBound);
                    }
                    else
                    {
                        return;
                    }
                    break;
                case PickUpType.TurnRight:
                    if (_car.Direction is CarDirection.NorthBound or CarDirection.SouthBound)
                    {
                        _car.ChangeDirection(CarDirection.EastBound);
                    }
                    else
                    {
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _car.ResetPosition();
            _renderer.enabled = false;
            _collider.enabled = false;
            _car = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var car = col.gameObject.GetComponent<Car>();

        if (car)
        {
            _car = car;
        }
    }


    private void OnLevelRestarted()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        _car = null;
    }
    
    public enum PickUpType
    {
        UTurn,
        Stop,
        TurnUp,
        TurnDown,
        TurnLeft,
        TurnRight
    }
    
    
    
}
