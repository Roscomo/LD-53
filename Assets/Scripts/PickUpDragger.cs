using System.Collections.Generic;
using System.Linq;
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

    private bool _canPlace;
    private GameObject _ghost;
    private Collider2D _ghostCollider;
    private SpriteRenderer _ghostRenderer;

    private void Start()
    {
        countText.text = $"{count}";
        _pickupSprite = pickupPrefab.GetComponent<SpriteRenderer>().sprite;
        
        _tilemap = FindObjectOfType<Tilemap>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canPlace = false;
        if(count <= 0) return;
        
        if (!_ghost)
        {
            _ghost = new GameObject();
            _ghostRenderer = _ghost.AddComponent<SpriteRenderer>();
            var ghostCollider = _ghost.AddComponent<BoxCollider2D>();
            ghostCollider.size = Vector2.one * 0.95f;

            _ghostCollider = ghostCollider;
            
            _ghostRenderer.sprite = _pickupSprite;
            _ghostRenderer.sortingOrder = 2;
            
            var halfAlpha = _ghostRenderer.color;
            halfAlpha.a = 0.5f;
            _ghostRenderer.color = halfAlpha;
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

                        var contacts = new List<Collider2D>();
                        _canPlace = _ghostCollider.GetContacts(contacts) == 0 || contacts.All(x => x.GetComponent<Goal>());
                    }
                    else
                    {
                        _ghost.transform.position = mousePosition;
                        _canPlace = false;
                    }
                }
                else
                {
                    _ghost.transform.position = _tilemap.CellToWorld(mouseToGridPos) + _tilemap.tileAnchor;
                    
                    var contacts = new List<Collider2D>();
                    _canPlace = _ghostCollider.GetContacts(contacts) == 0 || contacts.All(x => x.GetComponent<Goal>()); ;
                }
            }
            else
            {
                _ghost.transform.position = mousePosition;
                _canPlace = false;
            }
            
            if(!_canPlace) ChangeGhostColor(Color.red);
            else ChangeGhostColor(Color.white);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(count <= 0) return;

        if (_canPlace)
        {
            var pickUp = Instantiate(pickupPrefab);
            var pickUpPosition = _ghost.transform.position;
            pickUpPosition.z = 0.0f;
            pickUp.transform.position = pickUpPosition;
            count -= 1;
            countText.text = $"{count}";
            pickUp.OnPickUp += OnSignPickUp;
        }
        
        Destroy(_ghost);
    }

    private void OnSignPickUp()
    {
        count += 1;
        countText.text = $"{count}";
    }
    
    private void ChangeGhostColor(Color color)
    {
        _ghostRenderer.color = new Color(color.r, color.g, color.b, _ghostRenderer.color.a);
    }
}
