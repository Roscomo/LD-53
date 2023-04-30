using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class House : MonoBehaviour
{
    [SerializeField] private GameObject GoalPrefab;
    [SerializeField] private float spawnOffset;
    [SerializeField] private SpawnPosition GoalPosition = SpawnPosition.Right;
    
    private Tilemap _tileMap;

    private RoadTile _nearestRoadTile;
    
    // Start is called before the first frame update
    private void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();

        var goalObject = Instantiate(GoalPrefab);
        switch (GoalPosition)
        {
            case SpawnPosition.Up:
                goalObject.transform.position = transform.position + (Vector3)Vector2.up * spawnOffset;
                break;
            case SpawnPosition.Down:
                goalObject.transform.position = transform.position + (Vector3)Vector2.down * spawnOffset;
                break;
            case SpawnPosition.Left:
                goalObject.transform.position = transform.position + (Vector3)Vector2.left * spawnOffset;
                break;
            case SpawnPosition.Right:
                goalObject.transform.position = transform.position + (Vector3)Vector2.right * spawnOffset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.green;
        
        switch (GoalPosition)
        {
            case SpawnPosition.Up:
                Handles.DrawWireCube(transform.position + (Vector3)Vector2.up, Vector3.one);
                break;
            case SpawnPosition.Down:
                Handles.DrawWireCube(transform.position + (Vector3)Vector2.down, Vector3.one);
                break;
            case SpawnPosition.Left:
                Handles.DrawWireCube(transform.position + (Vector3)Vector2.left, Vector3.one);
                break;
            case SpawnPosition.Right:
                Handles.DrawWireCube(transform.position + (Vector3)Vector2.right, Vector3.one);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
#endif

    private enum SpawnPosition
    {
        Up,
        Down,
        Left,
        Right
    }
}
