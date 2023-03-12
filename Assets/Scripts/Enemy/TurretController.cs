using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField] private float sightRange;
    [SerializeField] private float timeBetweenAttacks = 1.5f;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameObject projectile;

    [Header("General settings")]
    [SerializeField] Transform rotatePoint;
    [SerializeField] Transform firePoint;

    PlayerController player;
    float shotCounter;

    void Start()
    {
        shotCounter = timeBetweenAttacks;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < sightRange)
        {
            Quaternion lookRotation = Quaternion.LookRotation(player.transform.position + Vector3.up - rotatePoint.transform.position);

            rotatePoint.transform.rotation = Quaternion.Lerp(rotatePoint.transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

            shotCounter -= Time.deltaTime;

            if (shotCounter <= 0)
            {
                Debug.Log("Instantiate Projectile");
                shotCounter = timeBetweenAttacks;
            }
        }
        else
        {
            shotCounter = timeBetweenAttacks;
            rotatePoint.transform.rotation = Quaternion.Lerp(rotatePoint.rotation, Quaternion.Euler(0f, rotatePoint.transform.rotation.eulerAngles.y + 10f, 0f), rotateSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
