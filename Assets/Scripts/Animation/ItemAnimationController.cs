using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimationController : MonoBehaviour
{
    [Header("General settings")]
    [SerializeField, Range(0f, 180.0f)] private float degreesPerSecond = 75f;
    [SerializeField, Range(0f, 5f)] private float verticalLength = 0.75f;
    [SerializeField, Range(0f, 5f)] private float verticalSpeed = 0.5f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        transform.Rotate(new Vector3(0, degreesPerSecond * Time.deltaTime, 0), Space.World);

        float verticalY = Mathf.PingPong(verticalSpeed * Time.time, verticalLength);
        transform.position = new Vector3(transform.position.x, startPosition.y + verticalY, transform.position.z);
    }
}
