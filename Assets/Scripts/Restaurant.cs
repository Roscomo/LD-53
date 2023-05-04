using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * Essentially just gives me a way to easily lay out the cars spawn position and direction in the editor
 */
public class Restaurant : MonoBehaviour
{
    [SerializeField] private Car carPrefab;
    [SerializeField] private float spawnOffset;
    [SerializeField] private SpawnPosition playerStartPosition = SpawnPosition.Right;
    [SerializeField] private CarDirection carDirection = CarDirection.NorthBound;
    
    private Tilemap _tileMap;
    private RoadTile _nearestRoadTile;

    private Car _carInstance;
    // Start is called before the first frame update
    private void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();
        
        GameLogicManager.Instance.OnLevelRestarted += OnLevelRestarted;
        SpawnCar();
    }

    private void OnLevelRestarted()
    {
        Destroy(_carInstance.gameObject);
        SpawnCar();
    }

    private void SpawnCar()
    {
        _carInstance = Instantiate(carPrefab);
        switch (playerStartPosition)
        {
            case SpawnPosition.Up:
                _carInstance.transform.position = transform.position + (Vector3)Vector2.up * spawnOffset;
                break;
            case SpawnPosition.Down:
                _carInstance.transform.position = transform.position + (Vector3)Vector2.down * spawnOffset;
                break;
            case SpawnPosition.Left:
                _carInstance.transform.position = transform.position + (Vector3)Vector2.left * spawnOffset;
                break;
            case SpawnPosition.Right:
                _carInstance.transform.position = transform.position + (Vector3)Vector2.right * spawnOffset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (carDirection)
        {
            case CarDirection.NorthBound:
                _carInstance.transform.up = Vector2.up;
                break;
            case CarDirection.SouthBound:
                _carInstance.transform.up = Vector2.down;
                break;
            case CarDirection.WestBound:
                _carInstance.transform.up = Vector2.left;
                break;
            case CarDirection.EastBound:
                _carInstance.transform.up = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _carInstance.Direction = carDirection;
        _carInstance.Tilemap = _tileMap;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPos;
        Vector3 direction;
        
        switch (playerStartPosition)
        {
            case SpawnPosition.Up:
                spawnPos = transform.position + (Vector3)Vector2.up;
                break;
            case SpawnPosition.Down:
                spawnPos = transform.position + (Vector3)Vector2.down;
                
                break;
            case SpawnPosition.Left:
                spawnPos = transform.position + (Vector3)Vector2.left;
                break;
            case SpawnPosition.Right:
                spawnPos = transform.position + (Vector3)Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (carDirection)
        {
            case CarDirection.NorthBound:
                direction = Vector2.up;
                break;
            case CarDirection.SouthBound:
                direction = Vector2.down;
                break;
            case CarDirection.WestBound:
                direction = Vector2.left;
                break;
            case CarDirection.EastBound:
                direction = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Handles.DrawWireCube(spawnPos, Vector3.one);
        Handles.DrawWireDisc(spawnPos + (direction * 0.25f), Vector3.forward, 0.1f);
    }
#endif

    private enum SpawnPosition
    {
        Up,
        Down,
        Left,
        Right,
    }
}
