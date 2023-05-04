using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PickUpDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private PickUp pickupPrefab;
    [SerializeField] private int count;
    [SerializeField] private TMP_Text countText;
    
    private Sprite _pickupSprite;
    private Tilemap _tilemap;
    private RoadTile _roadTile;
    
    private GameObject _ghost;

    private void Start()
    {
        countText.text = $"{count}";
        _pickupSprite = pickupPrefab.GetComponent<SpriteRenderer>().sprite;
        
        _tilemap = FindObjectOfType<Tilemap>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(count <= 0) return;
        
        if (!_ghost)
        {
            _ghost = new GameObject();
            var ghostRenderer = _ghost.AddComponent<SpriteRenderer>();
            
            ghostRenderer.sprite = _pickupSprite;
            ghostRenderer.sortingOrder = 2;
            
            var halfAlpha = ghostRenderer.color;
            halfAlpha.a = 0.5f;
            ghostRenderer.color = halfAlpha;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(count <= 0) return;
        
        if (_ghost)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 1;

            var mouseToGridPos = _tilemap.WorldToCell(mousePosition);
            _roadTile = _tilemap.GetTile<RoadTile>(mouseToGridPos);

            if (_roadTile)
            {
                if (pickupPrefab.Type is PickUp.PickUpType.TurnDown or PickUp.PickUpType.TurnUp
                    or PickUp.PickUpType.TurnLeft or PickUp.PickUpType.TurnRight)
                {
                    if (_roadTile.tileType is RoadTileType.FourWayJunction or RoadTileType.ThreeWayDown
                        or RoadTileType.ThreeWayLeft or RoadTileType.ThreeWayRight or RoadTileType.ThreeWayUp)
                    {
                        _ghost.transform.position = _tilemap.CellToWorld(mouseToGridPos) + _tilemap.tileAnchor;
                    }
                    else
                    {
                        _ghost.transform.position = mousePosition;
                    }
                }
                else
                {
                    _ghost.transform.position = _tilemap.CellToWorld(mouseToGridPos) + _tilemap.tileAnchor;
                }
                
                
            }
            else
            {
                _ghost.transform.position = mousePosition;
            }

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(count <= 0) return;

        if (pickupPrefab.Type is PickUp.PickUpType.TurnDown or PickUp.PickUpType.TurnUp
                or PickUp.PickUpType.TurnLeft or PickUp.PickUpType.TurnRight && (!_roadTile ||
                _roadTile.tileType is not RoadTileType.FourWayJunction  
                && _roadTile.tileType is not RoadTileType.ThreeWayDown
                && _roadTile.tileType is not RoadTileType.ThreeWayRight 
                && _roadTile.tileType is not RoadTileType.ThreeWayLeft 
                && _roadTile.tileType is not RoadTileType.ThreeWayUp))
        {
            Destroy(_ghost);
            return;
        }

        if (_roadTile)
        {
            var pickUp = Instantiate(pickupPrefab);
            var pickUpPosition = _ghost.transform.position;
            pickUpPosition.z = 0.0f;
            pickUp.transform.position = pickUpPosition;
            count -= 1;
            countText.text = $"{count}";
        }
        
        Destroy(_ghost);
    }
}
