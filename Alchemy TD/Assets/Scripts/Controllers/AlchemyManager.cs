using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AlchemyManager : MonoBehaviour
{

    private CursorController cursorController;
    private Spell curSpell;
    public Sprite rangeIndicator;

    int reagentCount = 0;
    ReagentEnum[] reagents = new ReagentEnum[2];

    public Spell[] spellList;
    public GameObject diamondButton;
    public GameObject amethystButton;
    public GameObject sapphireButton;
    public GameObject rubyButton;
    public GameObject emeraldButton;
    public Slider essence;
    public Slider essenceAfter;
    public GameObject UI;
    public GameObject splatterEffects;
    public Reagent diamond;
    public Reagent amethyst;
    public Reagent sapphire;
    public Reagent ruby;
    public Reagent emerald;
    public float regenRate;
    public GameObject trashCan;
    public GameObject AlchemySigns;
    public GameObject Wizard;
    public GameObject BuildParticle;
    public GameObject CastingParticle;
    public Image spellImage;
    public Text spellDescription;
    public Text spellAttributes;

    bool canPlace = true;
    GameObject newSpell;
    //bool clearOnCast = false;


    void Start()
    {
        cursorController = GameObject.FindWithTag("CursorController").GetComponent<CursorController>();
        Time.fixedDeltaTime = 0.1f;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            cancelSpell();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            AddReagent(emerald);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            AddReagent(diamond);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            AddReagent(amethyst);
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            AddReagent(ruby);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            AddReagent(sapphire);
        }
    }

    public void UpdateEssence(float amount)
    {
        essence.value += amount;
    }

    void FixedUpdate()
    {
        essence.value += regenRate/10;
        if(curSpell)
        {
            essenceAfter.value = essence.value - curSpell.cost;
        }
        else
        {
            essenceAfter.value = essence.value;
        }
    }

    public void SetCanPlace(bool placeable)
    {
        canPlace = placeable;
    }

    /// <summary>
    /// Adds a reagent to the alchemy table.
    /// </summary>
    /// <param name="reagent">The reagent to add to the table.</param>
    public void AddReagent(Reagent reagent)
    {
        GameObject reagentButton = null;
        switch (reagent.reagentEnum)
        {
            case ReagentEnum.Diamond:
                reagentButton = diamondButton;
                break;
            case ReagentEnum.Amethyst:
                reagentButton = amethystButton;
                break;
            case ReagentEnum.Sapphire:
                reagentButton = sapphireButton;
                break;
            case ReagentEnum.Ruby:
                reagentButton = rubyButton;
                break;
            case ReagentEnum.Emerald:
                reagentButton = emeraldButton;
                break;
            default:
                Debug.Log("That doesn't match existing reagents.");
                break;
        }
        reagentButton.GetComponent<ButtonControl>().indicate(false);

        if(reagentButton.GetComponent<ButtonControl>().checkEnabled() == false) {
            return;
        }


        if(reagentCount >= 2)
        {
            cancelSpell();
            AlchemySigns.SetActive(false);
            clearAlchemyLines();
            reagentCount = 0;
            curSpell = null;
        }
        reagents[reagentCount] = reagent.reagentEnum;
        Debug.Log("Added " + reagent);
        reagentCount++;

        reagentButton.GetComponent<ElementCircleAnimation>().highlight();


        if (reagentCount == 2)
        {
            curSpell = MakeSpell(reagents);
            if (curSpell != null)
            {
                AlchemySigns.SetActive(true);
                setAlchemyLines(reagents[0], reagents[1]);
                CastSpell();
                spellImage.sprite = curSpell.areaOfEffect.GetComponent<SpriteRenderer>().sprite;
                spellImage.enabled = true;
                spellDescription.text = curSpell.name + ": " + curSpell.description;

                //Below is to do with changing the spell attribute text
                spellAttributes.text = "Cost: " + curSpell.cost;
                if (curSpell.isTower)
                {
                    Tower tower = curSpell.areaOfEffect.GetComponent<Tower>();
                    spellAttributes.text += "\nDmg: " + tower.spell.damage;
                    if (tower.spell.hasAreaOfEffect)
                    {
                        spellAttributes.text += "\nType: " + tower.spell.areaOfEffect.GetComponent<AreaOfEffect>().element;
                    }
                    else
                    {
                        spellAttributes.text += "\nType: None";
                    }


                }
                else
                {
                    spellAttributes.text += "\nDmg: " + curSpell.damage;
                }

                if (curSpell.hasAreaOfEffect)
                {
                    spellAttributes.text += "\nType: " + curSpell.areaOfEffect.GetComponent<AreaOfEffect>().element;
                }
                //end spellAttributes editing
            }

            if (curSpell.snapMode == SnapMode.None)
            {
                CastingParticle.SetActive(true);
            }
        }
        else
        {
            StopCoroutine("GetCursorPosition");
            cursorController.SetCursor(null, 1, SnapMode.None);
            CastingParticle.SetActive(false);
        }
    }

    /// <summary>
    /// Removes a reagent from the alchemical reaction.
    /// </summary>
    /// <param name="reagent">The reagent to be removed.</param>
    public void RemoveReagent(Reagent reagent)
    {



        // C# won't allow the assignment of null to a field, so we just move
        // the pointer back so that next add will overwrite.

        StopCoroutine("GetCursorPosition");
        cursorController.SetCursor(null, 1, SnapMode.None);
        // Check which one matches and move the other back if needed
        if (reagentCount > 1 && reagents[0] == reagent.reagentEnum)
        {
            reagents[0] = reagents[1];
        }

        reagentCount--;
        Debug.Log("Removed " + reagent);
        AlchemySigns.SetActive(false);
        clearAlchemyLines();
        CastingParticle.SetActive(false);
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
    public void CastSpell()
    {
        StopCoroutine("GetCursorPosition");
        StartCoroutine("GetCursorPosition", curSpell);
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
            //Hardcoding fixes to incorrect cursor sizes. No idea why they need these, they jsut do.
            if (spell.name == "Arcane Bolt")
            {
                size /= 2;
            }
            else if (spell.name == "Tornado" || spell.name == "Maelstrom")
            {
                size *= 2;
            }
            else if (spell.name == "Explosion")
            {
                size *= 3;
            }
        }
        else if (spell.isTower)
        {
            size = spell.areaOfEffect.GetComponent<Tower>().range;
        }


        if (spell.isWall)
        {
            cursorController.SetCursor(spell.areaOfEffect.GetComponent<SpriteRenderer>().sprite, size, spell.snapMode);
        }
        else
        {
            cursorController.SetCursor(rangeIndicator, size, spell.snapMode);
        }

        while (essence.value < curSpell.cost)
        {
            yield return null;
        }

        // Pause until user clicks somewhere
        while (!Input.GetMouseButtonDown(0) || !canPlace)
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
            spell.spellProjectile.GetComponent<SpellProjectile>().target = null;
            newSpell = Instantiate(spell.spellProjectile, new Vector3(0, -4, 0), target.rotation);
            newSpell.GetComponent<SpellProjectile>().trashCan = trashCan;
            newSpell.GetComponent<SpellProjectile>().splatterEffects = splatterEffects;
        }
        else if(spell.hasAreaOfEffect)
        {
            newSpell = Instantiate(spell.areaOfEffect, target.position, target.rotation);
            newSpell.GetComponent<AreaOfEffect>().trashCan = trashCan;
            newSpell.GetComponent<AreaOfEffect>().splatterEffects = splatterEffects;
        } else if(spell.isTower)
        {
            newSpell = Instantiate(spell.areaOfEffect, target.position, target.rotation);
            newSpell.GetComponent<Tower>().trashCan = trashCan;
            newSpell.GetComponent<Tower>().splatterEffects = splatterEffects;
        } else if(spell.isWall)
        {
            Instantiate(spell.areaOfEffect, target.position, target.rotation);
        }
        cursorController.SetCursor(null, 1 / size, SnapMode.None);
        essence.value -= curSpell.cost;
        
        tutorialSpell(spell);

        if(spell.snapMode == SnapMode.None)
        {
            Wizard.GetComponent<Animator>().SetTrigger("CastSpell");
        } else
        {
            Wizard.GetComponent<Animator>().SetTrigger("RaiseWall");
            BuildParticle.SetActive(true);
        }


        //Waiting prevents multiple spells being cast in same place
        yield return new WaitForSeconds(0.15f);
        CastSpell();

        yield return null;
    }

    public void clearAlchemyLines()
    {
        diamondButton.GetComponent<ElementCircleAnimation>().deactivateLines();
        amethystButton.GetComponent<ElementCircleAnimation>().deactivateLines();
        sapphireButton.GetComponent<ElementCircleAnimation>().deactivateLines();
        rubyButton.GetComponent<ElementCircleAnimation>().deactivateLines();
        emeraldButton.GetComponent<ElementCircleAnimation>().deactivateLines();
    }

    public void setAlchemyLines(ReagentEnum element1, ReagentEnum element2)
    {
        //Sorry for the disgusting code, if You have a better solution, please replace this.

        switch(element1)
        {
            case ReagentEnum.Diamond:
                switch(element2)
                {
                    case ReagentEnum.Diamond:
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectSelf();
                        break;

                    case ReagentEnum.Amethyst:
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        break;

                    case ReagentEnum.Sapphire:
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        break;

                    case ReagentEnum.Ruby:
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        break;

                    case ReagentEnum.Emerald:
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose ();
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        break;

                }
                break;

            case ReagentEnum.Amethyst:
                switch (element2)
                {
                    case ReagentEnum.Diamond:
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        break;

                    case ReagentEnum.Amethyst:
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectSelf();
                        break;

                    case ReagentEnum.Sapphire:
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        break;

                    case ReagentEnum.Ruby:
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        break;

                    case ReagentEnum.Emerald:
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        break;

                }
                break;

            case ReagentEnum.Sapphire:
                switch (element2)
                {
                    case ReagentEnum.Diamond:
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        break;

                    case ReagentEnum.Amethyst:
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        break;

                    case ReagentEnum.Sapphire:
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectSelf();
                        break;

                    case ReagentEnum.Ruby:
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        break;

                    case ReagentEnum.Emerald:
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        break;

                }
                break;

            case ReagentEnum.Ruby:
                switch (element2)
                {
                    case ReagentEnum.Diamond:
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        break;

                    case ReagentEnum.Amethyst:
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        break;

                    case ReagentEnum.Sapphire:
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        break;

                    case ReagentEnum.Ruby:
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectSelf();
                        break;

                    case ReagentEnum.Emerald:
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        break;

                }
                break;

            case ReagentEnum.Emerald:
                switch (element2)
                {
                    case ReagentEnum.Diamond:
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        diamondButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        break;

                    case ReagentEnum.Amethyst:
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        amethystButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        break;

                    case ReagentEnum.Sapphire:
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectLeftFar();
                        sapphireButton.GetComponent<ElementCircleAnimation>().ConnectRightFar();
                        break;

                    case ReagentEnum.Ruby:
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectLeftClose();
                        rubyButton.GetComponent<ElementCircleAnimation>().ConnectRightClose();
                        break;

                    case ReagentEnum.Emerald:
                        emeraldButton.GetComponent<ElementCircleAnimation>().ConnectSelf();
                        break;

                }
                break;
        }
    }

    public void setMaxEssence(float maxE, float addE) {
        essence.maxValue = maxE;
        essenceAfter.maxValue= maxE;
        essence.value += addE;
    }

    public void cancelSpell()
    {
        AlchemySigns.SetActive(false);
        clearAlchemyLines();
        reagentCount = 0;
        CastingParticle.SetActive(false);
        cursorController.SetCursor(null, 1, SnapMode.None);
        StopCoroutine("GetCursorPosition");
        curSpell = null;
        spellImage.enabled = false;
        spellDescription.text = "Use Q, W, E, A, and D to activate elements. Two elements makes a spell. Spacebar clears the spell.";
        spellAttributes.text = "";
    }
    
    public void tutorialSpell(Spell spell) {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial")) {
            UI.GetComponent<TutorialMaster>().checkSpell(spell);
        }
    }
}
