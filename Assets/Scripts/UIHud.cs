using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    [Header("Text HUD")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bulletsText;
    public TextMeshProUGUI cratesText;

    [Header("FX")]
    public LevelFX levelFX;

    [Header("Controls")]
    public Button pauseButton;
    public Toggle audioToggle;

    [Header("Level Banner")]
    public GameObject levelBanner;
    public TextMeshProUGUI levelBannerText;

    [Header("Score Banner (Progress Bar)")]
    public ScoreBanner scoreBanner;

    void Start()
    {
        if (pauseButton)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() =>
            {
                bool pause = GameManager.I && GameManager.I.state != GameState.Paused;
                GameManager.I?.TogglePause(pause);
                var label = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (label) label.text = pause ? "Resume" : "Pause";
            });

            var labelInit = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (labelInit) labelInit.text = "Pause";
        }

        if (audioToggle)
        {
            audioToggle.onValueChanged.RemoveAllListeners();
            audioToggle.onValueChanged.AddListener(on =>
            {
                AudioManager.I?.SetMuted(!on);
            });

            audioToggle.isOn = true;
        }

        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshLevel();
        RefreshScore();
        RefreshTimer();
        RefreshBullets();
        RefreshCrates();
        RefreshProgress();
    }

    public void RefreshLevel()
    {
        if (!GameManager.I) return;
        int n = Mathf.Clamp(GameManager.I != null ? (GameManager.I.ActiveLevel != null ? (IndexOfActiveLevel() + 1) : 1) : 1, 1, 999);
        if (levelText) levelText.text = $"Level: {n}";
    }

    public void RefreshScore()
    {
        if (scoreText && GameManager.I != null)
            scoreText.text = $"Total Score: {GameManager.I.score}";
    }

    public void RefreshTimer()
    {
        if (!timerText || GameManager.I == null) return;
        float t = Mathf.Max(0f, GameManager.I.timeRemaining);
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        timerText.text = $"Time: {m:00}:{s:00}";
    }

    public void RefreshBullets()
    {
        if (!bulletsText || GameManager.I == null) return;
        bulletsText.text = $"1s: {GameManager.I.bullets1} | 2s: {GameManager.I.bullets2} | 3s: {GameManager.I.bullets3}";
    }

    public void RefreshCrates()
    {
        if (!cratesText || GameManager.I == null) return;
        cratesText.text = $"Crates: {GameManager.I.cratesDestroyedThisLevel}";
    }

    public void ResetProgress()
    {
        if (!scoreBanner || GameManager.I == null) return;
        scoreBanner.ResetBar(GameManager.I.ActiveLevel.targetScoreToAdvance);
    }

    public void RefreshProgress()
    {
        if (!scoreBanner || GameManager.I == null) return;
        scoreBanner.SetProgress(GameManager.I.levelScore, GameManager.I.ActiveLevel.targetScoreToAdvance);
    }

    public void ShowLevelBanner(string msg)
    {
        if (!levelBanner || !levelBannerText) return;
        StopAllCoroutines();
        StartCoroutine(BannerRoutine(msg));
    }

    IEnumerator BannerRoutine(string msg)
    {
        levelBannerText.text = msg;
        levelBanner.SetActive(true);
        float t = 0f;
        float dur = 1.0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        levelBanner.SetActive(false);
    }

    public void BlinkMissed()
    {
        if (!scoreText) return;
        StopCoroutine(nameof(BlinkRoutine));
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        var c = scoreText.color;
        scoreText.color = Color.red;
        yield return new WaitForSecondsRealtime(0.2f);
        scoreText.color = c;
    }

    int IndexOfActiveLevel()
    {
        if (GameManager.I == null || GameManager.I.levels == null) return 0;
        var active = GameManager.I.ActiveLevel;
        for (int i = 0; i < GameManager.I.levels.Length; i++)
        {
            if (GameManager.I.levels[i] == active) return i;
        }
        return 0;
    }

    public void PlayLevelUpFX()
    {
        levelFX?.PlayLevelUpFX();
    }

    public void PlayLevelDownFX()
    {
        levelFX?.PlayLevelDownFX();
    }
}
