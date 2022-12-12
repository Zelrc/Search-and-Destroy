using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float steerForce = 2f;
    public float rotateSpeed = 4f;

    public Transform _target;
    CompanionAI _companionAI;
    AIPatrolling _characterStatus;
    Rigidbody _rb;
    bool _statusFound;

    private Vector3 _distanceBetweenBulletAndTarget;
    private Vector3 _bulletCurrentDirection;
    private Vector3 _steeringForward;

    private void Start()
    {
        _companionAI = GameObject.FindWithTag("Bot").GetComponent<CompanionAI>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_target == null)
        {
            _rb.velocity = transform.forward * 5f;
        }
        else
        {
            if ((transform.position.y - _target.position.y) >= 1)
            {
                Vector3 targetTransform = new Vector3(transform.position.x, _target.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetTransform, bulletSpeed);
            }
            else
                TrackTarget();
        }
        
        Destroy(this.gameObject, 5f);
    }

    private void TrackTarget()
    {        
        // Calculate the distance between bot and target
        _distanceBetweenBulletAndTarget = new Vector3
            (_target.position.x - transform.position.x,
            _target.position.y - transform.position.y,
            _target.position.z - transform.position.z).normalized;

        // Calculate the bot towards the target
        _bulletCurrentDirection = Vector3.RotateTowards(transform.forward, _distanceBetweenBulletAndTarget, rotateSpeed * Time.deltaTime * 0.5f, 0.0f);
       

        // Make Bot look at the target
        transform.rotation = Quaternion.LookRotation(_bulletCurrentDirection);

        // Calculate the resultant Vector3
        _steeringForward = _distanceBetweenBulletAndTarget - _bulletCurrentDirection;



        // Move the bot towards to the player
        _rb.velocity = (bulletSpeed * _bulletCurrentDirection) + (_steeringForward * steerForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            _statusFound = other.TryGetComponent(out _characterStatus);
            if (_statusFound)
            {
                int randomDamage = Random.Range(_companionAI.botDamage - 4, _companionAI.botDamage + 4);
                _characterStatus.TakeDamage(randomDamage);
                Destroy(this.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}
