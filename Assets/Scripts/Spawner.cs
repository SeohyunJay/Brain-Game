using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnParent;

    bool running;
    Coroutine loopCo;

    public void BeginLevel()
    {
        if (loopCo != null) StopCoroutine(loopCo);
        running = true;
        loopCo = StartCoroutine(SpawnLoop());
    }

    public void StopAll()
    {
        running = false;
        if (loopCo != null) { StopCoroutine(loopCo); loopCo = null; }
    }

    IEnumerator SpawnLoop()
    {
        var gm = GameManager.I;
        if (gm == null || gm.levels == null || gm.levels.Length == 0)
            yield break;

        LevelConfig lvl = gm.ActiveLevel;

        int spawned = 0;
        while (running && spawned < lvl.totalCrates)
        {
            while (running && CountActiveCrates() >= lvl.maxConcurrentCrates)
                yield return null;

            if (!running) break;

            float x = Random.Range(lvl.spawnXRange.x, lvl.spawnXRange.y);
            Vector3 pos = new Vector3(x, lvl.spawnY, 0f);

            GameObject prefab = gm.cratePrefab;
            if (prefab == null)
            {
                Debug.LogError("Spawner: GameManager.cratePrefab is not assigned.");
                yield break;
            }

            var go = Instantiate(prefab, pos, Quaternion.identity, spawnParent);
            if (!go.CompareTag("Crate")) go.tag = "Crate";

            spawned++;
            gm.OnCrateSpawned();

            float t = 0f;
            while (t < lvl.spawnInterval)
            {
                if (!running) break;
                t += Time.deltaTime;
                yield return null;
            }
        }

        loopCo = null;
    }

    int CountActiveCrates()
    {
        return GameObject.FindGameObjectsWithTag("Crate").Length;
    }
}
