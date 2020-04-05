using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour {

    public List<Attributes> weaknesses = new List<Attributes>();
    public float maxHealth;
    protected float currentHealth;
    public GameObject splatterEffects;

    public float damage;
    public float points;
    public bool shielded = false;
    public bool slowed = false;
    public GameObject LevelController;
    public GameObject splatter;

    public Image healthBar;
    public Text text;

    private bool noPath = false;
    
    

    GameObject tempObject;

    protected void Start()
    {
        /*
        if(GetComponent<DijkstraPath>() == null)
        {
            noPath = true;
        }
        */
        currentHealth = maxHealth;
        text.text = "HP: " + currentHealth + "/" + maxHealth + "\nWeak: " + weaknesses[0];
    }

    protected void Update()
    {
        if (noPath)
        {
            transform.Translate(Vector3.down * 2 * Time.deltaTime);
        }
    }


    public virtual void UpdateHealth(float healthDifference)
    {
        if(!shielded)
        {
            currentHealth += healthDifference;
            healthBar.fillAmount = (currentHealth / maxHealth);
            if(currentHealth <= 0)
            {
                LevelController.GetComponent<LevelMaster>().despawnEnemy(damage, points, false);
                Destroy(gameObject);
                tempObject = Instantiate(splatter, transform.position, transform.rotation, splatterEffects.transform);
                splatterEffects.GetComponent<SplatterCleanup>().addSplatter(tempObject);
            }
            text.text = "HP: " + Mathf.Round(currentHealth * 10) / 10 + "/" + maxHealth + "\nWeak: " + weaknesses[0];
        }
        else
        {
            shielded = false;
        }
    }

}
