using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [System.Serializable]
    class WayPointPath
    {
        public Transform wayPoint;

        public Transform GetWayPoint(int wayPointIndex)
        {
            return wayPoint.GetChild(wayPointIndex);
        }

        public int GetNextWayPointIndex(int currentWayPointIndex)
        {
            int nextWayPointIndex = currentWayPointIndex + 1;

            if (nextWayPointIndex == wayPoint.transform.childCount)
                nextWayPointIndex = 0;

            return nextWayPointIndex;
        }
    }

    [SerializeField] private WayPointPath wayPointPath;
    [SerializeField] private float speed = 1;

    private Transform previousWayPoint;
    private Transform targetWayPoint;
    private int currentWayPointIndex;
    private float timeToWayPoint;
    private float progressTime;

    void Start()
    {
        TargetNextWayPoint();
    }

    private void FixedUpdate()
    {
        progressTime += Time.deltaTime;

        float progressPercentage = progressTime / timeToWayPoint;

        progressPercentage = Mathf.SmoothStep(0, 1, progressPercentage);

        transform.position = Vector3.Lerp(previousWayPoint.position, targetWayPoint.position, progressPercentage);
        transform.rotation = Quaternion.Lerp(previousWayPoint.rotation, targetWayPoint.rotation, progressPercentage);

        if (progressPercentage >= 1)
            TargetNextWayPoint();
    }

    private void TargetNextWayPoint()
    {
        previousWayPoint = wayPointPath.GetWayPoint(currentWayPointIndex);
        currentWayPointIndex = wayPointPath.GetNextWayPointIndex(currentWayPointIndex);
        targetWayPoint = wayPointPath.GetWayPoint(currentWayPointIndex);

        progressTime = 0;

        float distanceToWayPoint = Vector3.Distance(previousWayPoint.position, targetWayPoint.position);

        timeToWayPoint = distanceToWayPoint / speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
