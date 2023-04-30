using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PickUp : MonoBehaviour
{
    [SerializeField] private PickUpType type;
    
    private Tilemap _tilemap;
    private RoadTile _tile;
    private void Start()
    {
        _tilemap = FindObjectOfType<Tilemap>();

        if (_tilemap)
        {
            var gridPosition = _tilemap.WorldToCell(transform.position);
            _tile = _tilemap.GetTile<RoadTile>(gridPosition);
            var newPosition = _tilemap.CellToWorld(gridPosition) + _tilemap.tileAnchor;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }

    private void Update()
    {
        if(!_tile.currentCar) return;

        if (Vector2.Distance(_tile.currentCar.transform.position, transform.position) < 0.01f)
        {
            switch (type)
            {
                case PickUpType.UTurn:
                    _tile.currentCar.ReverseDirection();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Destroy(gameObject);
        }
    }
    
    private enum PickUpType
    {
        UTurn
    }
}
