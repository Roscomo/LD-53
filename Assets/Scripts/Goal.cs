using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Goal : MonoBehaviour
{
    public event Action<Goal> OnGoalAccomplished;
    public GameObject deliveryVehicle;

    private Sequence _tweenSequence;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    
    private bool _isAccomplished;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        GameLogicManager.Instance.RegisterGoal(this);
        GameLogicManager.Instance.OnLevelRestarted += OnLevelRestarted;
        
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        
        _tweenSequence = DOTween.Sequence();
        _tweenSequence
            .Append(transform.DOScale(Vector3.one * 0.9f, 0.5f).SetEase(Ease.Linear))
            .Append(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear))
            .SetLoops(-1);
    }
    
    private void OnDisable()
    {
        _tweenSequence.Kill();
    }

    // Update is called once per frame
    private void Update()
    {
        if(!deliveryVehicle) return;
        
        if (Vector2.Distance(deliveryVehicle.transform.position, transform.position) < 0.1f && !_isAccomplished)
        {
            _collider.enabled = false;

            OnGoalAccomplished?.Invoke(this);
            _tweenSequence.Pause();
            _isAccomplished = true;
            transform.DOScale(Vector3.one * 1.25f, 0.2f).SetEase(Ease.Linear).OnComplete(() => transform.DORewind());
            _renderer.DOFade(0.0f, 0.2f).OnComplete(() =>
            {
                _renderer.enabled = false;
                _renderer.DORewind();
            });
        }
    }
    
    private void OnLevelRestarted()
    {
        transform.DORewind();
        _renderer.DORewind();
        
        _collider.enabled = true;
        _renderer.enabled = true;

        _isAccomplished = false;
        GameLogicManager.Instance.RegisterGoal(this);
        _tweenSequence.Play();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        var car = other.gameObject.GetComponent<Car>();
        if (car && other.gameObject.CompareTag("Player"))
        {
            deliveryVehicle = other.gameObject;
        }
    }
}
