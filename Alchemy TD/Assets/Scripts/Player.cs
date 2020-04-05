using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{

    private CursorController cursorController;

    int reagentCount = 0;
    ReagentEnum[] reagents = new ReagentEnum[2];

    public Spell[] spellList;

    private void Start()
    {
        cursorController = GetComponent<CursorController>();
    }


    /// <summary>
    /// Adds a reagent to the alchemy table.
    /// </summary>
    /// <param name="reagent">The reagent to add to the table</param>
    public void AddReagent(Reagent reagent)
    {
        reagents[reagentCount] = reagent.reagentEnum;
        Debug.Log("Added " + reagent);
        reagentCount++;
        if (reagentCount >= 2)
        {
            Spell spell = MakeSpell(reagents);
            if (spell != null)
            {

                CastSpell(spell);
            }
            reagentCount = 0;
        }
    }


    /// <summary>
    /// Creates a spell from two reagents.
    /// </summary>
    /// <param name="reagents">An array of two reagents.</param>
    /// <returns>The spell to cast (null if can't find).</returns>
    public Spell MakeSpell(ReagentEnum[] reagents)
    {
        foreach (Spell spell in spellList)
        {
            // Check if the spell's reagent list contains the two reagents.
            if (Array.IndexOf(reagents, spell.reagents[0]) > -1 && Array.IndexOf(reagents, spell.reagents[1]) > -1)
            {
                if (Array.IndexOf(spell.reagents, reagents[0]) > -1 && Array.IndexOf(spell.reagents, reagents[1]) > -1)
                {
                    return spell;
                }
            }
        }
        Debug.Log("No Spell found!");
        return null;
    }


    /// <summary>
    /// Starts process of casting a spell.
    /// </summary>
    /// <param name="spell">The spell to cast</param>
    public void CastSpell(Spell spell)
    {
        StopCoroutine("GetCursorPosition");
        StartCoroutine("GetCursorPosition", spell);
    }


    /// <summary>
    /// Prompts the user to select the target location for their spell.
    /// </summary>
    /// <param name="spell">The spell to cast.</param>
    /// <returns>N/A</returns>
    IEnumerator GetCursorPosition(Spell spell)
    {
        float size = 1;
        if (spell.areaOfEffect.GetComponent<AreaOfEffect>())
        {
            size = spell.areaOfEffect.GetComponent<AreaOfEffect>().size;
        }
        //cursorController.SetCursor(spell.areaOfEffect.GetComponent<SpriteRenderer>(), size, spell.snapMode);

        // Pause until user clicks somewhere.
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        // Turn the clicked spot into a Vector3.
        //Vector3 shootDirection = Input.mousePosition;
        //shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
        //shootDirection.z = 0.0f;

        Transform target = cursorController.transform;


        if (spell.hasProjectile)
        {
            //spell.spellProjectile.GetComponent<SpellProjectile>().shootDirection = shootDirection;
            //Instantiate(spell.spellProjectile, new Vector3(0, -4, 0), Quaternion.identity);
            spell.spellProjectile.GetComponent<SpellProjectile>().shootDirection = target.position;
            Instantiate(spell.spellProjectile, new Vector3(0, -4, 0), target.rotation);
        }
        else
        {
            //Instantiate(spell.areaOfEffect, shootDirection, Quaternion.identity);
            Instantiate(spell.areaOfEffect, target.position, target.rotation);
        }
        cursorController.SetCursor(null, 1 / size, SnapMode.None);
        yield return null;

    }

}
