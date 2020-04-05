using UnityEngine;

public class EnemyWave : MonoBehaviour {
	// Public variables for inspector
	public Transform enemyPrefab;
	public float enemySpawnRate;
	public GameObject mapObject = null;

	// private variables
	private float lastSpawnTime;

	// Use this for initialization
	void Start ()
	{
		lastSpawnTime = Time.time;
	}

	// Update is called once per frame
	void Update ()
	{
		if(Time.time - lastSpawnTime > 1/enemySpawnRate)
		{
			spawnEnemy(enemyPrefab);
			lastSpawnTime = Time.time;
		}
	}

	void spawnEnemy(Transform enemyType)
	{
		Transform enemy = Instantiate(enemyType);
        enemy.SetParent(transform);
		enemy.localPosition = new Vector3(0, 0, 0);
		enemy.GetComponent<DijkstraPath>().mapObject = mapObject;
	}
}
