using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{

    public Spell ownSpell;
    public float refundRate;

    private AlchemyManager manager;

    [HideInInspector]
    public bool isInvalid = false; // Used to check if was placed incorrectly. Ensures full refund.

    [HideInInspector]
    public bool isTower = false;

    // Use this for initialization
    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("AlchemyManager").GetComponent<AlchemyManager>();
        //Debug.Log("started and found manager.");
    }

    void OnMouseOver()
    {
       if(Input.GetMouseButtonDown(1))
       {
            if (isTower)
            {
                GetComponent<Tower>().pathingGrid.GetComponent<DijkstraMap>().RemoveStructureReference(transform.position.x, transform.position.y);
            }
            else
            {
                DijkstraMap map = GetComponent<Wall>().pathingGrid.GetComponent<DijkstraMap>();
                map.RemoveStructureReference(transform.position.x, transform.position.y);
                GetComponent<Wall>().GetComponent<Collider2D>().enabled = false;
                map.CalcMap();
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (isInvalid)
        {
            manager.UpdateEssence(ownSpell.cost);
        }
        else
        {
            manager.UpdateEssence(ownSpell.cost * refundRate);
        }
    }
}
