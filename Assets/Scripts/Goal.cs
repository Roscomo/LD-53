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

    private bool _isAccomplished;
    // Start is called before the first frame update
    private void Start()
    {
        GameLogicManager.Instance.RegisterGoal(this);
        _renderer = GetComponent<SpriteRenderer>();
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
        if (!deliveryVehicle)
        {
            deliveryVehicle = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        
        if (Vector2.Distance(deliveryVehicle.transform.position, transform.position) < 0.01f && !_isAccomplished)
        {
            OnGoalAccomplished?.Invoke(this);
            _tweenSequence.Kill();
            _isAccomplished = true;
            transform.DOScale(Vector3.one * 1.25f, 0.2f).SetEase(Ease.Linear);
            _renderer.DOFade(0.0f, 0.2f).OnComplete(() => Destroy(gameObject));
        }
    }
}
