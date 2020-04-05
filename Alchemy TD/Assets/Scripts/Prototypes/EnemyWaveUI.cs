using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveUI : MonoBehaviour {
	// Public variables for inspector
	public Transform enemyPrefab;
	public float enemySpawnRate;
	public GameObject mapObject = null;
	public Slider waveProgress;

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
		if(Time.time - lastSpawnTime > 1/enemySpawnRate && waveProgress.value < 100f)
		{
			spawnEnemy(enemyPrefab);
			lastSpawnTime = Time.time;
			waveProgress.value += 1;
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
