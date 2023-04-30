using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PickUpDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private PickUp pickupPrefab;

    private Sprite _pickupSprite;
    private Tilemap _tilemap;
    private RoadTile _roadTile;
    
    private GameObject _ghost;

    private void Start()
    {
        _pickupSprite = pickupPrefab.GetComponent<SpriteRenderer>().sprite;
        
        _tilemap = FindObjectOfType<Tilemap>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        if (_ghost)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            mousePosition.z = 1;

            var mouseToGridPos = _tilemap.WorldToCell(mousePosition);
            _roadTile = _tilemap.GetTile<RoadTile>(mouseToGridPos);

            if (_roadTile)
            {
                _ghost.transform.position = _tilemap.CellToWorld(mouseToGridPos) + _tilemap.tileAnchor;
            }
            else
            {
                _ghost.transform.position = mousePosition;
            }

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_roadTile)
        {
            var pickUp = Instantiate(pickupPrefab);
            var pickUpPosition = _ghost.transform.position;
            pickUpPosition.z = 0.0f;
            pickUp.transform.position = pickUpPosition;
        }
        
        Destroy(_ghost);
    }
}
