using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFX : MonoBehaviour
{
    [Header("UI")]
    public Image flash;
    public Image icon;

    [Header("Particles (optional)")]
    public ParticleSystem confettiUp;
    public ParticleSystem burstDown;

    [Header("Timings")]
    [Range(0.05f, 1.5f)] public float flashIn = 0.12f;
    [Range(0.05f, 1.5f)] public float hold = 0.25f;
    [Range(0.05f, 1.5f)] public float flashOut = 0.35f;

    [Header("Colors")]
    public Color upFlashColor = new Color(0.05f, 1f, 0.3f, 0.38f);
    public Color downFlashColor = new Color(1f, 0.1f, 0.1f, 0.38f);
    public Color upIconColor = new Color(0.2f, 1f, 0.5f, 1f);
    public Color downIconColor = new Color(1f, 0.3f, 0.3f, 1f);

    Coroutine co;

    public void PlayLevelUpFX()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(FlashRoutine(up: true));
    }

    public void PlayLevelDownFX()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(FlashRoutine(up: false));
    }

    IEnumerator FlashRoutine(bool up)
    {
        if (!flash) yield break;

        if (up && confettiUp) confettiUp.Play();
        if (!up && burstDown) burstDown.Play();

        Color startFlash = flash.color; startFlash.a = 0f;
        Color targetFlash = up ? upFlashColor : downFlashColor;

        if (icon)
        {
            icon.gameObject.SetActive(true);
            var c = up ? upIconColor : downIconColor;
            c.a = 0f;
            icon.color = c;
            icon.rectTransform.localScale = Vector3.one * 0.75f;
        }

        float t = 0f;
        while (t < flashIn)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / flashIn);
            flash.color = Color.Lerp(startFlash, targetFlash, k);

            if (icon)
            {
                var ic = icon.color; ic.a = Mathf.SmoothStep(0f, 1f, k);
                icon.color = ic;
                icon.rectTransform.localScale = Vector3.Lerp(Vector3.one * 0.75f, Vector3.one, k);
            }
            yield return null;
        }
        flash.color = targetFlash;
        if (icon)
        {
            var ic = icon.color; ic.a = 1f; icon.color = ic;
            icon.rectTransform.localScale = Vector3.one;
        }

        t = 0f;
        while (t < hold) { t += Time.unscaledDeltaTime; yield return null; }

        t = 0f;
        while (t < flashOut)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / flashOut);
            flash.color = Color.Lerp(targetFlash, new Color(targetFlash.r, targetFlash.g, targetFlash.b, 0f), k);

            if (icon)
            {
                var ic = icon.color; ic.a = Mathf.Lerp(1f, 0f, k);
                icon.color = ic;
                icon.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.08f, k);
            }
            yield return null;
        }

        var f = flash.color; f.a = 0f; flash.color = f;
        if (icon)
        {
            var ic = icon.color; ic.a = 0f; icon.color = ic;
            icon.gameObject.SetActive(false);
        }

        co = null;
    }
}
