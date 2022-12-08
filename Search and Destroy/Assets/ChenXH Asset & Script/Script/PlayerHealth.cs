using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    [SerializeField] Text healthText;
    public bool isDead;

    void Start()
    {
        StartCoroutine("checkPlayerHP");
    }


    void Update()
    {

    }

    IEnumerator checkPlayerHP()
    {
        while (isDead == false)
        {
            healthText.text = "Health: " + health.ToString("0");

            if (health <= 0)
            {
                health = 0;
                //Destroy(gameObject);
                isDead = true;
                GameManager.instance.losePanelOpen();
                Debug.Log("Player dead");
            }

            yield return null;
        }
    }
}
