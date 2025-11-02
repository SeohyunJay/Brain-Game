using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Ended }

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Levels")]
    public LevelConfig[] levels;
    [SerializeField] private int currentLevelIndex = 0;
    public bool resetBulletsEachLevel = true;

    [Header("Prefabs & Scene Refs")]
    public GameObject bulletPrefab;
    public GameObject cratePrefab;
    public Transform cannon;
    public Transform bulletSpawnPoint;

    [Header("Managers")]
    public UIHud hud;
    public Spawner spawner;

    [Header("Runtime (read-only)")]
    public int score = 0;
    public int levelScore = 0;
    public int bullets1 = 50, bullets2 = 50, bullets3 = 50;
    public float timeRemaining = 0f;
    public int cratesSpawnedThisLevel = 0;
    public int cratesDestroyedThisLevel = 0;
    public GameState state = GameState.Playing;

    [Header("Scenes")]
    public string endSceneName = "EndScene";

    const string KEY_HIGHSCORE = "HIGH_SCORE";

    public LevelConfig ActiveLevel => levels[Mathf.Clamp(currentLevelIndex, 0, Mathf.Max(0, levels.Length - 1))];
    public bool IsLastLevel => currentLevelIndex >= Mathf.Max(0, (levels?.Length ?? 1) - 1);

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("GameManager: No LevelConfig assets assigned.");
            enabled = false; return;
        }

        score = 0;
        currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, levels.Length - 1);
        StartLevel();
    }

    void Update()
    {
        if (state != GameState.Playing) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndRound(success: false, reason: "Time up");
        }

        hud?.RefreshTimer();
    }

    void StartLevel()
    {
        DestroyAllCrates();

        levelScore = 0;
        cratesSpawnedThisLevel = 0;
        cratesDestroyedThisLevel = 0;
        timeRemaining = ActiveLevel.levelTimeSeconds;

        if (resetBulletsEachLevel)
        {
            bullets1 = bullets2 = bullets3 = 50;
            hud?.RefreshBullets();
        }

        state = GameState.Playing;

        hud?.RefreshAll();
        hud?.RefreshProgress();

        if (ActiveLevel.musicClip) AudioManager.I?.PlayMusic(ActiveLevel.musicClip, loop: true, fadeIn: 0.75f);

        spawner?.BeginLevel();
    }

    void AdvanceToNextLevel()
    {
        currentLevelIndex = Mathf.Min(currentLevelIndex + 1, levels.Length - 1);
        StartLevel();
    }

    void MoveDownOneLevel()
    {
        currentLevelIndex = Mathf.Max(currentLevelIndex - 1, 0);
        StartLevel();
    }

    void DestroyAllCrates()
    {
        var crates = GameObject.FindGameObjectsWithTag("Crate");
        foreach (var c in crates) Destroy(c);
    }

    public bool TryConsumeBullet(int value)
    {
        switch (value)
        {
            case 1: if (bullets1 > 0) { bullets1--; hud?.RefreshBullets(); return true; } break;
            case 2: if (bullets2 > 0) { bullets2--; hud?.RefreshBullets(); return true; } break;
            case 3: if (bullets3 > 0) { bullets3--; hud?.RefreshBullets(); return true; } break;
        }
        AudioManager.I?.PlayNoAmmo();
        return false;
    }

    void AddScoreAndProgress(int delta)
    {
        score += delta;

        levelScore += delta;

        hud?.RefreshScore();
        hud?.RefreshProgress();

        int target = Mathf.Max(0, ActiveLevel.targetScoreToAdvance);

        bool reachedTarget = (target > 0 && levelScore >= target);
        bool destroyedAll = (cratesDestroyedThisLevel >= ActiveLevel.totalCrates);

        if (reachedTarget || destroyedAll)
        {
            EndRound(success: true, reason: reachedTarget ? "Reached target" : "No crates left");
            return;
        }

        if (levelScore < 0)
        {
            EndRound(success: false, reason: "Score below 0");
        }
    }

    public void OnCrateDestroyedExact()
    {
        cratesDestroyedThisLevel++;
        hud?.RefreshCrates();
        AudioManager.I?.PlayCrateSuccess();
        AddScoreAndProgress(ActiveLevel.pointsExactZero);
    }

    public void OnCrateDestroyedBelow()
    {
        cratesDestroyedThisLevel++;
        hud?.RefreshCrates();
        AudioManager.I?.PlayCrateFail();
        AddScoreAndProgress(ActiveLevel.pointsBelowZero);
    }

    public void OnCrateMissed()
    {
        hud?.BlinkMissed();
        AudioManager.I?.PlayCrateMiss();
        AddScoreAndProgress(ActiveLevel.pointsMissed);
    }

    public void OnCrateSpawned()
    {
        cratesSpawnedThisLevel++;
        hud?.RefreshCrates();
    }

    void EndRound(bool success, string reason = "")
    {
        if (state == GameState.Ended) return;
        state = GameState.Ended;

        int high = PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);
        if (score > high) { PlayerPrefs.SetInt(KEY_HIGHSCORE, score); PlayerPrefs.Save(); }

        if (success)
        {
            AudioManager.I?.PlayLevelUp();
            hud?.PlayLevelUpFX();

            if (!IsLastLevel)
            {
                hud?.ShowLevelBanner($"Level Up! → {currentLevelIndex + 2}");
                Invoke(nameof(AdvanceToNextLevel), 1.0f);
            }
            else
            {
                hud?.ShowLevelBanner("All Levels Complete!");
                // End run
                SceneLoader.FinalScore = score;
                SceneLoader.HighScore = PlayerPrefs.GetInt(KEY_HIGHSCORE, score);
                SceneLoader.FinalLevel = currentLevelIndex + 1;
                Invoke(nameof(GoEndScene), 1.1f);
            }
        }
        else
        {
            AudioManager.I?.PlayLevelDown();
            hud?.PlayLevelDownFX();

            int prevIndex = currentLevelIndex;
            currentLevelIndex = Mathf.Max(currentLevelIndex - 1, 0);

            if (prevIndex != currentLevelIndex)
                hud?.ShowLevelBanner($"Level Down → {currentLevelIndex + 1}");
            else
                hud?.ShowLevelBanner($"Retry Level {currentLevelIndex + 1}");

            Invoke(nameof(StartLevel), 1.0f);
        }
    }

    void GoEndScene() => SceneManager.LoadScene(endSceneName);

    public void TogglePause(bool isPaused)
    {
        if (state == GameState.Ended) return;

        state = isPaused ? GameState.Paused : GameState.Playing;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void RefreshHudAll() => hud?.RefreshAll();
}
