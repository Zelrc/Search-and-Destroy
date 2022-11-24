using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [Header("Status")]
    [Tooltip("This set the Character HP")]
    public int maxHealth = 100;

    public float _currentHeath;

    private Animator _animator;

    private bool _hasAnimator;

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Reset the Character's Health
        _currentHeath = maxHealth;
        _hasAnimator = TryGetComponent<Animator>(out _animator);
    }

    private void Update()
    {
        OnDeath();
    }

    private void OnDeath()
    {
        if (_currentHeath <= 0)
        {
            if (_hasAnimator)
            {
                Debug.Log("Awaiting animation to get implement here");
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
