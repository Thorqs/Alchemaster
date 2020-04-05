using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    public float range;

    public Spell spell;
    
    public GameObject trashCan;
    public bool isPrePlaced;
    
    public GameObject splatterEffects;

    private List<Collider2D> monstersInRange = new List<Collider2D>();

    public float fireRate;
    private float lastFireTime = -999f;
    
    private GameObject newSpell;

    [HideInInspector]
    public GameObject pathingGrid;

    

    // Use this for initialization
    void Start()
    {
        GetComponent<CircleCollider2D>().radius *= range;
        if (isPrePlaced)
        {
            return;
        }
        pathingGrid = GameObject.FindGameObjectWithTag("PathingGrid");
        
        if (pathingGrid.GetComponent<DijkstraMap>().CheckPlaced(transform.position.x, transform.position.y)){
            return;
        }
        else //if there is already a tower at this location
        {
            GetComponent<Sellable>().isInvalid = true;
            Debug.Log("Already a tower here!");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Time.time - lastFireTime > 1 / fireRate && monstersInRange.Count > 0)
        {
            bool flag = true;
            while(flag){
                if (monstersInRange.Count > 0 && !monstersInRange[0])
                {
                    monstersInRange.RemoveAt(0);
                }
                else
                {
                    flag = false;
                }
            }
            if (monstersInRange.Count > 0)
            {
                if (spell.hasProjectile)
                {
                    spell.spellProjectile.GetComponent<SpellProjectile>().target = monstersInRange[0].gameObject;
                    newSpell = Instantiate(spell.spellProjectile, transform);
                    newSpell.GetComponent<SpellProjectile>().trashCan = trashCan;
                    newSpell.GetComponent<SpellProjectile>().splatterEffects = splatterEffects;
                }
                else if(spell.areaOfEffect.GetComponent<AreaOfEffect>().shape == EffectShape.Circle)
                {
                    newSpell = Instantiate(spell.areaOfEffect, transform.position, Quaternion.identity);
                    newSpell.GetComponent<AreaOfEffect>().trashCan = trashCan;
                    newSpell.GetComponent<AreaOfEffect>().splatterEffects = splatterEffects;
                    
                }
                else // line attack
                {
                    Vector3 direction = monstersInRange[0].transform.position - transform.position;
                    float angle = 90 + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    direction = Vector3.Normalize(direction) * spell.areaOfEffect.GetComponent<AreaOfEffect>().size / 4;
                    
                    newSpell = Instantiate(spell.areaOfEffect, transform.position + direction, Quaternion.AngleAxis(angle, Vector3.forward));
                    newSpell.GetComponent<AreaOfEffect>().splatterEffects = splatterEffects;
                }
            }
            

            lastFireTime = Time.time;

        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster" && !monstersInRange.Contains(other))
        {
            monstersInRange.Add(other);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (monstersInRange.Contains(other))
        {
            monstersInRange.Remove(other);
        }

    }
}
