using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Crate : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro valueLabel;
    public float bottomY = -6.5f;

    [Header("Visuals")]
    public Renderer rend;
    public Color normalColor = Color.white;
    public Color successFlash = Color.green;
    public Color failFlash = Color.red;

    Rigidbody rb;
    Collider myCol;
    int currentValue;
    float driftPhase;

    bool isDying = false;
    bool hasEnteredView = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        myCol = GetComponent<Collider>();

        var lvl = GameManager.I.ActiveLevel;
        currentValue = Random.Range(lvl.minCrateValue, lvl.maxCrateValue + 1);
        if (valueLabel) valueLabel.text = currentValue.ToString();

        driftPhase = Random.Range(0f, Mathf.PI * 2f);

        if (valueLabel && !valueLabel.GetComponent<BillboardUpright>())
            valueLabel.gameObject.AddComponent<BillboardUpright>();
        var mr = valueLabel ? valueLabel.GetComponent<MeshRenderer>() : null;
        if (mr) mr.sortingOrder = 5;
    }

    void FixedUpdate()
    {
        if (isDying) return;

        if (!hasEnteredView && rend && rend.isVisible) hasEnteredView = true;

        var lvl = GameManager.I.ActiveLevel;

        Vector3 pos = rb.position;
        pos.y -= lvl.fallSpeed * Time.fixedDeltaTime;
        float drift = Mathf.Sin(Time.time * 1.2f + driftPhase) * lvl.horizontalDriftMax;
        pos.x += drift * Time.fixedDeltaTime * 2f;

        rb.MovePosition(pos);

        if (pos.y < bottomY)
        {
            BeginGroundDeath();
        }
    }

    public bool TryApplyBullet(int bulletValue, Vector3 hitPoint)
    {
        if (isDying) return false;

        if (!hasEnteredView) return false;

        currentValue -= bulletValue;

        if (currentValue > 0)
        {
            if (valueLabel) valueLabel.text = currentValue.ToString();
            Flash(normalColor, 0.05f);
        }
        else if (currentValue == 0)
        {
            GameManager.I.OnCrateDestroyedExact();
            Flash(successFlash, 0.1f);
            Destroy(gameObject, 0.05f);
        }
        else
        {
            GameManager.I.OnCrateDestroyedBelow();
            Flash(failFlash, 0.1f);
            Destroy(gameObject, 0.05f);
        }

        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDying) return;

        if (other.CompareTag("Ground"))
        {
            BeginGroundDeath();
        }
    }

    void BeginGroundDeath()
    {
        isDying = true;
        if (myCol) myCol.enabled = false;
        GameManager.I.OnCrateMissed();
        StopAllCoroutines();
        StartCoroutine(GroundFlashAndDie());
    }

    System.Collections.IEnumerator GroundFlashAndDie()
    {
        if (rend) rend.material.color = failFlash;
        yield return new WaitForSecondsRealtime(0.15f);
        Destroy(gameObject);
    }

    void Flash(Color c, float t)
    {
        if (!rend) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(c, t));
    }

    System.Collections.IEnumerator FlashRoutine(Color c, float t)
    {
        var original = rend.material.color;
        rend.material.color = c;
        yield return new WaitForSeconds(t);
        if (rend) rend.material.color = original;
    }
}
