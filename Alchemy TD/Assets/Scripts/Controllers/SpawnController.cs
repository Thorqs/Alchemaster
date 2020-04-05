using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnInfo
{
    //The information for each of the phases sent to the spawner.

    public GameObject enemy;
    public int spawnAmount;
    public int spawnIndex = 0;
    public float spawnPeriod;//Calculated from the spawn frequency
    public float spawnTimer = 0;
    public float progressCost;
    //public GameObject dijkMap;
}

public class SpawnController : MonoBehaviour {

    public GameObject levelController;
    public GameObject splatterEffects;

    LevelMaster levelScript;
    List<spawnInfo> spawningLines = new List<spawnInfo>();
    spawnInfo currLine;

    private void Start()
    {
        levelScript = levelController.GetComponent<LevelMaster>();
    }

    void Update () {
        for(int i = 0; i < spawningLines.Count; i++)
        {
            currLine = spawningLines[i];
            if(currLine.spawnIndex >= currLine.spawnAmount)
            {
                spawningLines.RemoveAt(i);
                levelScript.finishedPhase();
                i--;//to compensate for the list reducing in size.
                continue;
            }

            if(currLine.spawnTimer >= currLine.spawnPeriod)
            {
                var enemy = Instantiate(currLine.enemy, transform.position, Quaternion.identity, transform);
                if(transform.rotation.z != 0) {
                    enemy.transform.localScale = new Vector3(enemy.transform.localScale.x, (enemy.transform.localScale.y/3), enemy.transform.localScale.z);
                    enemy.transform.position = new Vector3(enemy.transform.position.x, (enemy.transform.position.y + Random.Range(-1f, 1f)), enemy.transform.position.z);
                } else {
                    enemy.transform.localScale = new Vector3((enemy.transform.localScale.x/3), enemy.transform.localScale.y, enemy.transform.localScale.z);
                    enemy.transform.position = new Vector3((enemy.transform.position.x + Random.Range(-1f, 1f)), enemy.transform.position.y, enemy.transform.position.z);
                }
                enemy.GetComponent<DijkstraPath>().mapObject = levelScript.dijkstraMap;
                enemy.GetComponent<Monster>().LevelController = levelController;
                enemy.GetComponent<Monster>().splatterEffects = splatterEffects;
                levelScript.spawnEnemy(currLine.progressCost/currLine.spawnAmount);
                currLine.spawnTimer = 0;
                currLine.spawnIndex++;
            }
            currLine.spawnTimer += Time.deltaTime;
        }
	}

    public void addSpawn(GameObject enemy, int amount, float freq, float cost)
    {
        spawningLines.Add(new spawnInfo() {enemy = enemy, spawnAmount = amount, spawnPeriod = 1f/ freq, progressCost = cost});
    }
}
