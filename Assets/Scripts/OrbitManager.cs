using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    [SerializeField] private float orbitRadius = 1.5f;
    public List<Transform> orbitobjects = new List<Transform>();
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.W))
        {
            RepositionOrbit();
        }
    }

    private void RepositionOrbit()
    {
        int count = 0;
        foreach (var o in orbitobjects)
        {
            if (o.gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // 1. 위치 계산 (원형으로 배치)
            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * orbitRadius;
            orbitobjects[i].localPosition = offset;

            // 2. 중앙을 향하도록 회전 (방향 벡터 → Quaternion)
            Vector3 dirToCenter = -offset.normalized; // 중심을 향하는 방향
            float zRotation = Mathf.Atan2(dirToCenter.y, dirToCenter.x) * Mathf.Rad2Deg;
            orbitobjects[i].localRotation = Quaternion.Euler(0f, 0f, zRotation + 90f);
        }
    }
}
