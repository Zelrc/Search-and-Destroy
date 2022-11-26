using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CompanionAI : MonoBehaviour
{
    // Search the Player by Tag
    // First, check HP of Player 
    // Is the healing on Cooldown?
    // No
    // If player's HP less than 10%, heal the player
    // Yes
    // Is there enemy?
    // Shoot the enemy and try protecting the player
    // No
    // Move towards to the player left or right

    [Header("Bot Status")]
    [Tooltip("Bot current speed")]
    public float currentSpeed;
    [Tooltip("Bot current rotation speed")]
    public float rotateSpeed = 3f;

    [Tooltip("Bot maximum speed")]
    public int maxSpeed = 8;
    [Tooltip("Bot maximum rotate speed")]
    public int maxRotateSpeed = 4;

    [Tooltip("The maximum time for bot to reach max Speed")]
    public float timeToReachMaxSpeed = 2f;

    [Tooltip("Bot steering force")]
    public float steerForce = 1f;

    [Tooltip("Bot radius wander around the player")]
    public float aroundPlayerRadius = 3f;

    [Tooltip("Time for bot to heal the player again")]
    public int healCoolDownTime = 10;

    [Tooltip("The Bot Detect Range with the enemy")]
    public float detectRange = 5f;
    [Tooltip("Enemy in which layer to detect")]
    public LayerMask detectLayer;

    [Tooltip("The Bot damage towards the target")]
    public float botDamage = 10f;
    [Tooltip("Time for Bot to do next attack")]
    public int attackCoolDownTime = 5;

    [Tooltip("Bullet Prefab")]
    public GameObject bulletPrefab;
    [Tooltip("Maximum bullet per shooting")]
    public int maxBullet = 50;

    [Tooltip("Bot's Target Transform")]
    [SerializeField] Transform _target;

    [SerializeField] GameObject _playerGameObject;
    private CharacterStatus _characterStatus;
    private Rigidbody _botBody;

    private bool _healOnCoolDown = false;
    private float _healTimer;

    private bool _attackOnCoolDown = false;
    private float _attackTimer;
    [SerializeField] private int _bulletCount = 0;

    [SerializeField] private float _distance;

    private Vector3 _aroundThePlayer;
    private Vector3 _distanceBetweenBotAndTarget;
    private Vector3 _botCurrentDirection;
    private Vector3 _steeringForward;


    private void Awake()
    {
        _playerGameObject = GameObject.FindWithTag("Player");
        _characterStatus = _playerGameObject.GetComponent<CharacterStatus>();
        _botBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        AllTimer();
        DistanceBetween();
        BotLogic();
    }

    private void AllTimer()
    {
        if (_healOnCoolDown)
        {
            _healTimer -= Time.deltaTime;
        }

        if (_healTimer <= 0)
        {
            _healTimer = healCoolDownTime;
            _healOnCoolDown = false;
        }

        if (_attackOnCoolDown)
        {
            _attackTimer -= Time.deltaTime;
        }

        if (_attackTimer <= 0)
        {
            _attackTimer = attackCoolDownTime;
            _attackOnCoolDown = false;
            _bulletCount = 0;
        }
    }

    private void MoveTowardsToTarget()
    {
        // Calculate the distance between bot and target
        _distanceBetweenBotAndTarget =
            new Vector3
            (_target.position.x - transform.position.x,
            0,
            _target.position.z - transform.position.z).normalized;

        // Make the bot stay on top of player
        _distanceBetweenBotAndTarget.y = 0;

        // Calculate the bot towards the target
        _botCurrentDirection = Vector3.RotateTowards(transform.forward, _distanceBetweenBotAndTarget, rotateSpeed * Time.deltaTime * 0.2f, 0.0f);

        // Make Bot look at the target
        transform.rotation = Quaternion.LookRotation(_botCurrentDirection);

        if (currentSpeed < maxSpeed)
            currentSpeed += (maxSpeed / timeToReachMaxSpeed) * Time.deltaTime;
        else
            currentSpeed = maxSpeed;

        if (rotateSpeed < maxRotateSpeed)
            rotateSpeed += (maxRotateSpeed / timeToReachMaxSpeed) * Time.deltaTime;
        else
            rotateSpeed = maxRotateSpeed;

        // Calculate the resultant Vector3
        _steeringForward = _distanceBetweenBotAndTarget - _botCurrentDirection;

        // Move the bot towards to the player
        _botBody.velocity = (currentSpeed * _botCurrentDirection) + (_steeringForward * steerForce);

    }

    private void HealPlayer()
    {
        _characterStatus._currentHeath += _characterStatus.maxHealth * 30 / 100;
    }

    private void ShootEnemy()
    {
        Transform nearestEnemy = null;
        Collider[] physicsDetector = Physics.OverlapSphere(transform.position, detectRange, detectLayer, QueryTriggerInteraction.Ignore);
        float minimumDistance = Mathf.Infinity;
        foreach (Collider enemyCollider in physicsDetector)
        {
            if (enemyCollider.gameObject.CompareTag("enemy"))
            {
                Vector3 botVector = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 enemyVector = new Vector3(enemyCollider.transform.position.x, 0, enemyCollider.transform.position.z);
                float distanceBetweenBotAndEnemies = Vector3.Distance(botVector, enemyVector);
                // The Bot will go to another enemy and another if it keep detect new enemy, while attacking one enemy
                // In order to avoid that, normally I would limit a distance between bot and player
                // But the another fix is, when the bot has no attack left will return to the player.
                // I will continue develop this part if needed, if else, will focus on bulleting and stuff first
                if (distanceBetweenBotAndEnemies < minimumDistance)
                {
                    minimumDistance = distanceBetweenBotAndEnemies;
                    nearestEnemy = enemyCollider.transform;
                    if (distanceBetweenBotAndEnemies <= 2f && _bulletCount < maxBullet)
                    {

                        // Shoot the enemy limited time
                        Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(_botCurrentDirection));
                        _bulletCount++;
                        if (_bulletCount >= maxBullet)
                        {
                            _attackOnCoolDown = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Moving to the target enemy");
                        _target = nearestEnemy;
                        MoveTowardsToTarget();
                    }
                }
            }
        }

        if (nearestEnemy != null)
        {
            Debug.Log("The nearest enemy is" + nearestEnemy.gameObject.name);
        }
        else
        {
            _target = _playerGameObject.transform;
            MoveTowardsToTarget();
            Debug.Log("There is no enemy found, so target set to player");
        }

    }

    private void BotLogic()
    {
        // Piority will be when player HP is low, it will fly over to heal
        if (!_healOnCoolDown)
        {
            if (_characterStatus._currentHeath <= _characterStatus._currentHeath * 10 / 100)
            {
                // We need the bot move towards to the player before he could heal the player
                if (_distance >= 4f)
                {
                    _target = gameObject.transform;
                    MoveTowardsToTarget();
                }
                else
                {
                    HealPlayer();
                }
            }
        }

        // Then if the player HP is not low, it will continue onward here, move towards to the enemy if Attack is not on cooldown
        // If attack on cooldown, it will go back to the player
        if (!_attackOnCoolDown)
        {
            ShootEnemy();
        }
        else
        {
            _target = _playerGameObject.transform;
            MoveTowardsToTarget();
        }
    }

    private void DistanceBetween()
    {
        _distance = Vector3.Distance(transform.position, _playerGameObject.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, detectRange);        
    }
}
