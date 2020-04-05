using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public Phase[] phases;

    [HideInInspector]
    public int phasesSpawned = 0;

    //true once all phases have been sent to spawners
    [HideInInspector]
    public bool waveEnded = false;
}

[System.Serializable]
public class Phase
{
    public float progressRequired; //Progress in the wave before phase spawns
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnFreq;
    public int spawnPoint; //numbered from left-right starting 0.
    public float progressAwarded; //progress awarded for the whole phase.
    //time delay between reaching the progressRequired and to start spawning.
    public float spawnDelay;

    [HideInInspector]
    public bool spawned = false;
    [HideInInspector]
    public float spawnTimer = 0;
}

public class LevelMaster : MonoBehaviour
{
    public Slider progressBar;
    public Slider healthBar;
    public Slider essence;
    public Slider waveDelay;
    public GameObject splatterEffects;
    public GameObject waveIntro;
    public int totalEnemiesKilled = 0;
    public int totalEnemiesSpawned = 0;
    public Text waveCounter;
    public GameObject dijkstraMap;
    public Wave[] waves;
    public GameObject[] spawnPoints;

    Wave activeWave;
    int waveIndex;

    void Start ()
    {
        waveIndex = 0;
        SetupWave(waves[0]);
    }

    void Update ()
    {
        if(waveIndex < waves.Length)
        {
            activeWave = waves[waveIndex];
            if(activeWave.phasesSpawned < activeWave.phases.Length)
            {
                if(waveDelay.value <= 0)
                {
                    foreach(Phase phase in activeWave.phases)
                    {
                        //The phases may not be in order
                        if (progressBar.value >= phase.progressRequired && phase.spawned == false)
                        {
                            if (phase.spawnTimer >= phase.spawnDelay)
                            {
                                spawnPhase(phase);
                                phase.spawned = true;
                                phase.spawnTimer = 0;
                            }

                            else
                            {
                                phase.spawnTimer += Time.deltaTime;
                            }
                        }
                    }
                }
            }
            else if(!activeWave.waveEnded) //if all phases have been spawned
            {
                activeWave.waveEnded = true;
            }
            else if(waveDelay.value <= 0)
            {
                waveIndex++;
                if(waveIndex < waves.Length)
                {
                    progressBar.value = 0;
                    SetupWave(waves[waveIndex]);
                }
                else
                {
                    SceneManager.LoadScene("MainMenu");
                }

            } else if(totalEnemiesSpawned == totalEnemiesKilled)
            {
                waveIntro.GetComponent<Text>().text = "Wave Complete,\n Well Done!";
                waveIntro.SetActive(true);
            }
        }
        else if (progressBar.value >= progressBar.maxValue)
        {
            //if all waves spawned and all enemies despawned
            SceneManager.LoadScene("MainMenu");
        }
        if(healthBar.value == 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    void FixedUpdate()
    {
        if(waveDelay.value > 0f)
        {
            waveDelay.value -= 0.1f;
        }
    }

    public void spawnPhase(Phase phase)
    {
        //Sends the information for the now to spawn phase to the spawner it was assigned.
        SpawnController spawner = spawnPoints[phase.spawnPoint].GetComponent<SpawnController>();
        spawner.addSpawn(phase.enemyPrefab, phase.enemyCount, phase.spawnFreq, phase.progressAwarded);
        if(activeWave.phasesSpawned <= 0) {
            waveIntro.GetComponent<Text>().text = "Wave " + (waveIndex+1);
            waveIntro.SetActive(true);
        } else {
            waveIntro.SetActive(false);
        }
    }

    public void spawnEnemy(float cost)
    {
        //Called by spawner objects when they spawn an enemy to update information in this class
        totalEnemiesSpawned++;
        progressBar.value += cost;
    }

    public void despawnEnemy(float damage, float score, bool reachedTarget)
    {
        essence.value += score;
        totalEnemiesKilled++;

        //Called by the target object when an enemy reaches them to update information in this class
        if (reachedTarget)
        {
            healthBar.value -= damage;
        }
    }

    public void finishedPhase()
    {
        //Called by spawner to indicate it has finished spawning all of a wave.
        activeWave.phasesSpawned++;
    }

    public void SetupWave(Wave wave)
    {

        progressBar.maxValue = 0;
        progressBar.gameObject.transform.GetChild(2).GetComponent<Slider>().value = progressBar.gameObject.transform.GetChild(2).GetComponent<Slider>().maxValue;

        foreach(Phase phase in wave.phases)
        {
            progressBar.maxValue += phase.progressAwarded;
        }

        waveCounter.text = "Wave " + (waveIndex + 1);
    }

    public void returnToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
