using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBanner : MonoBehaviour
{
    [Header("UI Refs")]
    public Image fill;
    public TextMeshProUGUI label;

    [Header("Animation")]
    public float smoothSpeed = 6f;

    float targetFill = 0f;

    public void ResetBar(int target)
    {
        targetFill = 0f;
        if (fill) fill.fillAmount = 0f;
        if (label) label.text = $"0 / {Mathf.Max(target, 0)} points";
    }

    public void SetProgress(int current, int target)
    {
        targetFill = (target > 0) ? Mathf.Clamp01((float)current / target) : 0f;
        if (label) label.text = $"{Mathf.Max(current, 0)} / {Mathf.Max(target, 0)} points";
    }

    void Update()
    {
        if (!fill) return;
        fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, targetFill, smoothSpeed * Time.unscaledDeltaTime);
    }
}

