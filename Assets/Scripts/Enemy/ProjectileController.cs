using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage;

    PlayerController player;

    bool stopFollowing;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        Vector3 target = player.transform.position + Vector3.up - transform.position;

        if (Vector3.Distance(transform.position, player.transform.position) > 1.75f && !stopFollowing)
        {
            Vector3 direction = Vector3.RotateTowards(transform.forward, target, rotateSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else stopFollowing = true;


        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
            DestroyObject();
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<HealthController>().TakeDamage(damage, other.tag);
        }

        DestroyObject();
    }
}
