using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] GameObject _playerGameObject;
    private CharacterStatus _characterStatus;
    private Rigidbody _botBody;

    private bool _healOnCoolDown = false;
    private float _healTimer;

    private bool _attackOnCoolDown = false;
    private float _attackTimer;

    private float _distance;

    private Vector3 _aroundThePlayer;
    private Vector3 _distanceBetweenBotAndPlayer;
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
    }

    private void MoveTowardsToPlayer()
    {
        // Calculate the distance between bot and target
        _distanceBetweenBotAndPlayer =
            new Vector3
            (_playerGameObject.transform.position.x - transform.position.x,
            0,
            _playerGameObject.transform.position.z - transform.position.z).normalized;

        // Make the bot stay on top of player
        _distanceBetweenBotAndPlayer.y = _playerGameObject.transform.position.y + 4f;

        // Calculate the bot towards the target
        _botCurrentDirection = Vector3.RotateTowards(transform.forward, _distanceBetweenBotAndPlayer, rotateSpeed * Time.deltaTime * 0.2f, 0.0f);

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
        _steeringForward = _distanceBetweenBotAndPlayer - _botCurrentDirection;

        // Move the bot towards to the player
        _botBody.velocity = (currentSpeed * _botCurrentDirection) + (_steeringForward * steerForce);

    }

    private void HealPlayer()
    {
        _characterStatus._currentHeath += _characterStatus.maxHealth * 30 / 100;
    }

    private void ShootEnemy()
    {
        Collider[] physicsDetector = Physics.OverlapSphere(transform.position, detectRange, detectLayer, QueryTriggerInteraction.Ignore);
        foreach (Collider enemy in physicsDetector)
        {
            if(enemy.CompareTag("Enemy"))
            {
                Vector3 _distancebetweenBotandEnemy = enemy.transform.position - transform.position;
                CharacterStatus enemyStatus = enemy.GetComponent<CharacterStatus>();
                // Need to code cooldown and rapid check
                // Need to check which enemy has the lowest health/closest, make the bot fly over
                // attack it when it's not on cooldown
                
                enemyStatus.TakeDamage(botDamage);
            }
        }

    }

    private void BotLogic()
    {
        // If Bot see enemy, it need to get close to the enemy and shoot the enemy
        // Piority will be when player HP is low, it will fly over to heal
        if (!_healOnCoolDown)
        {
            if (_characterStatus._currentHeath <= _characterStatus._currentHeath * 10 / 100)
            {
                // We need the bot move towards to the player before he could heal the player
                if (_distance >= 4f)
                {
                    MoveTowardsToPlayer();
                }
                else
                {
                    HealPlayer();
                }

            }
        }

        // Then if the player HP is not low, it will continue onward here
        ShootEnemy();
    }

    private void DistanceBetween()
    {
        _distance = Vector3.Distance(transform.position, _playerGameObject.transform.position);
    }
}
