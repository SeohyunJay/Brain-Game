using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BrainGame/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Spawning")]
    public int totalCrates = 20;
    public Vector2 spawnXRange = new Vector2(-7.5f, 7.5f);
    public float spawnY = 6.0f;
    public float spawnInterval = 1.2f;
    public int maxConcurrentCrates = 5;

    [Header("Crate Values")]
    public int minCrateValue = 3;
    public int maxCrateValue = 12;

    [Header("Motion")]
    public float fallSpeed = 1.6f;
    public float horizontalDriftMax = 1.2f;

    [Header("Win/Lose")]
    public float levelTimeSeconds = 180f;
    public int targetScoreToAdvance = 100;

    [Header("Scoring")]
    public int pointsExactZero = 5;
    public int pointsBelowZero = -3;
    public int pointsMissed = -2;

    [Header("Optional per-level music")]
    public AudioClip musicClip;
}
