using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    CompanionAI _companionAI;
    CharacterStatus _characterStatus;
    Rigidbody _rb;
    bool _statusFound;


    private void Start()
    {
        _companionAI = GameObject.FindWithTag("Bot").GetComponent<CompanionAI>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rb.velocity = transform.forward * 5f;
        Destroy(this.gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            _statusFound = other.TryGetComponent(out _characterStatus);
            if(_statusFound)
            {
                _characterStatus.TakeDamage(_companionAI.botDamage);
                Destroy(this.gameObject);
            }
        }
    }
}
