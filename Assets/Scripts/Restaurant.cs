using System;
using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    private void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();
        
        var carObject = Instantiate(carPrefab);
        switch (playerStartPosition)
        {
            case SpawnPosition.Up:
                carObject.transform.position = transform.position + (Vector3)Vector2.up * spawnOffset;
                break;
            case SpawnPosition.Down:
                carObject.transform.position = transform.position + (Vector3)Vector2.down * spawnOffset;
                break;
            case SpawnPosition.Left:
                carObject.transform.position = transform.position + (Vector3)Vector2.left * spawnOffset;
                break;
            case SpawnPosition.Right:
                carObject.transform.position = transform.position + (Vector3)Vector2.right * spawnOffset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (carDirection)
        {
            case CarDirection.NorthBound:
                carObject.transform.up = Vector2.up;
                break;
            case CarDirection.SouthBound:
                carObject.transform.up = Vector2.down;
                break;
            case CarDirection.WestBound:
                carObject.transform.up = Vector2.left;
                break;
            case CarDirection.EastBound:
                carObject.transform.up = Vector2.right;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        carObject.Direction = carDirection;
        carObject.Tilemap = _tileMap;
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
