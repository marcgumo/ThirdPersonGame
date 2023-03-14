using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class SightRangeEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyController enemy = (EnemyController)target;

        Vector3 viewAngle1 = DirectionFromAngle(enemy.transform.eulerAngles.y, enemy.GetSightAngle() * -0.5f);
        Vector3 viewAngle2 = DirectionFromAngle(enemy.transform.eulerAngles.y, enemy.GetSightAngle() * 0.5f);

        Handles.color = Color.blue;
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngle1 * enemy.GetSightRange());
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngle2 * enemy.GetSightRange());

        Handles.DrawWireArc(enemy.transform.position, Vector3.up, viewAngle1, enemy.GetSightAngle(), enemy.GetSightRange());
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
