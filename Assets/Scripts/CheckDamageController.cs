using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDamageController : MonoBehaviour
{
    [SerializeField] private bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.tag == "Enemy")
            {
                //Debug.Log("Attacking enemy");
                TakeDamage(other.gameObject, 10, other.tag);
            }
        }
        else
        {
            if (other.tag == "Player")
            {
                //Debug.Log("Attacking player");
                if (other.gameObject.GetComponentInParent<PlayerController>().playerState != PlayerController.MovementStates.Dash)
                    TakeDamage(other.gameObject, 10, other.tag);
            }
        }
    }

    private void TakeDamage(GameObject entity, int damage, string tag)
    {
        entity.gameObject.GetComponentInParent<HealthController>().TakeDamage(damage, tag);
    }
}
