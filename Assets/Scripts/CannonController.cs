using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform barrelPivot;
    public Transform bulletSpawnPoint;

    [Header("Model axis")]
    public float axisOffsetDeg = 90f;

    [Header("Clamp (degrees)")]
    public float centerAngle = 0f;
    public float halfRange = 90f;

    void Update()
    {
        if (GameManager.I == null || GameManager.I.state != GameState.Playing)
            return;

        AimToMouseExactClamped();
        HandleFire();
    }

    void AimToMouseExactClamped()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        if (!plane.Raycast(ray, out float dist)) return;

        Vector3 hit = ray.GetPoint(dist);
        Vector3 dir = hit - barrelPivot.position; dir.z = 0f;
        if (dir.sqrMagnitude < 1e-8f) return;

        float target = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        target += axisOffsetDeg;

        float delta = Mathf.DeltaAngle(centerAngle, target);
        delta = Mathf.Clamp(delta, -halfRange, halfRange);
        float clamped = centerAngle + delta;

        barrelPivot.rotation = Quaternion.Euler(0f, 0f, clamped);
    }

    void HandleFire()
    {
        if (Input.GetKeyDown(KeyCode.S)) Shoot(1);
        if (Input.GetKeyDown(KeyCode.D)) Shoot(2);
        if (Input.GetKeyDown(KeyCode.F)) Shoot(3);
    }

    void Shoot(int value)
    {
        if (!GameManager.I.TryConsumeBullet(value)) return;

        Vector3 dir = bulletSpawnPoint.up.normalized;

        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        var go = Instantiate(GameManager.I.bulletPrefab, bulletSpawnPoint.position, rot);
        var b = go.GetComponent<Bullet>();
        b.Init(value, dir * b.initialSpeed);

        AudioManager.I?.PlayShoot();
    }
}
