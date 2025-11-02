using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float initialSpeed = 18f;
    public float lifeSeconds = 3.5f;

    [Header("Visuals")]
    public TextMeshPro valueLabel;
    public Renderer rend;
    public Material mat1, mat2, mat3;

    Rigidbody rb;
    int value;

    void Awake()
    {
        if (!valueLabel) valueLabel = GetComponentInChildren<TextMeshPro>(true);
        var mr = valueLabel ? valueLabel.GetComponent<MeshRenderer>() : null;
        if (mr) mr.sortingOrder = 5;

        if (valueLabel && !valueLabel.GetComponent<BillboardUpright>())
            valueLabel.gameObject.AddComponent<BillboardUpright>();
    }

    public void Init(int v, Vector3 velocity)
    {
        value = v;
        if (valueLabel) valueLabel.text = v.ToString();

        if (rend)
        {
            if (v == 1 && mat1) rend.material = mat1;
            else if (v == 2 && mat2) rend.material = mat2;
            else if (v == 3 && mat3) rend.material = mat3;
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.velocity = velocity;

        CancelInvoke();
        Invoke(nameof(Die), lifeSeconds);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            var crate = other.GetComponent<Crate>();
            if (crate != null)
            {
                bool applied = crate.TryApplyBullet(value, transform.position);
                if (applied)
                {
                    Die();
                }
            }
        }
    }

    void Die() => Destroy(gameObject);
}
