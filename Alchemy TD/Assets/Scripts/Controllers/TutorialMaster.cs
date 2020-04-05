//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class TutorialMaster : MonoBehaviour
{
    //public Text dialogue;
    public Text instructions;
    public Button cont;
    public Button prev;
    public GameObject AlchemyMaster;
    public GameObject LevelMaster;

    public GameObject tutWall1;
    public GameObject tutWall2;
    public GameObject tutWall3;
    public GameObject tutWall4;

    public Button start;
    
    public Spell[] spells;
    public Text spellText;
    bool[] spellFlags;
    int discoveredSpells = 0;

    AlchemyManager alchemyScript;
    bool oneSpawned = false;
    bool twoSpawned = false;
    bool threeSpawned = false;
    bool fourSpawned = false;
    
    string unknown = "???";
    
    string[] spellDescription = 
    {
        "Wall",
        "Lightning Tower",
        "Arcane Tower",
        "Swamp Tower",
        "Fire Tower",
        "Whirlwind",
        "Wind Bolt",
        "Snow Storm",
        "Fire Storm",
        "Arcane Bolt",
        "Water Bolt",
        "Fire Bolt",
        "Whirlpool",
        "Steam",
        "Fireball"
    };

    string[] tutText =
    {
        "Excuse me master,\n It appears that your dastardly rival, 'Tim the Enchanter' is sending his minions to invade your tower!",
        "Thankfully we just finished installing the new alchemy-circle. With it's power you should be able to hold them off.",
        "The alchemy circle allows you to create magical spell by combining two gems. Each combination makes a completely new spell, so be sure to experiment once it's fully operational!",
        "Alrighty, let's test this thing out! Try creating the 'arcane bolt' spell. Make it by combining two amethysts.\n\n(Click the amethyst button twice to add the gems, then click an area to cast the spell there. You don't need to combine gems again until you want to change your current spell)",
        "Wowee! Your magical prowess knows no bounds, master!\n\n ..Oh No! Here come the first of Tim's minions! Use that arcane bolt to wipe them out!",
        "Whoever left our gates wide open shall be severely punished, I assure you.\n In the meantime, the other apprentices and I have managed to set up some magical defences!" +
            "\n Alas, we are but simple servants, and our setup isn't entirely useful at the moment. Why don't you try and improve it? Combine two emeralds to make the 'create wall' spell, and add some more walls to the courtyard. ",
        "Unforunately due to a recent OSHA visit, you are no longer allowed to completely wall off any path between the gates and your tower. So try to stall the monsters long enough so you have time to defeat them all before they reach us." +
            "\n\n(Try and create a maze to slow down the enemies. Right click placed walls to remove them.)",
        "Master, we've managed to find huge stock of mana!\nYou should have enough to create some towers to help defend us.\nEmerald combined with a separate gem will make one of a variety of towers that will attack any enemy that gets close to them." +
            "\nTry creating an Arcane Tower by combining an emerald with an amethyst. Then, place it on the courtyard",
        "The towers take a while to kill enemies, but if you can build enough of them, weak minions like goblins will stand no chance!",
        "It seems Tim is getting complacent - there will only be some goblins for a while. This could be a good chance to test out the capabilities of the alchemy circle! You should probably discover all its spells before continuing",
        "ALL ELEMENTS UNLOCKED, each individual element as well as each unique pair of elements produces a different spell, with a total of 15 to discover\nEmerald makes walls, with another element it makes a tower.\n amethyst combined with other elements make cheap bolt spells\n other combinations are more expensive but have unique effects."
    };
    int tutTextIndex = 0;

    /*string[] diaArray =
    {
        ""
    };*/

    // Initialization
    void Start ()
    {
        alchemyScript = AlchemyMaster.GetComponent<AlchemyManager>();

        Time.timeScale = 0f;
        instructions.text = tutText[tutTextIndex];
        disableElement(alchemyScript.diamondButton);
        disableElement(alchemyScript.emeraldButton);
        disableElement(alchemyScript.sapphireButton);
        disableElement(alchemyScript.rubyButton);
        disableElement(alchemyScript.amethystButton);
        alchemyScript.setMaxEssence(50, 0);
        enableWalls(false);
        spellFlags = new bool[16];
    }

    // Update is called once per frame
    void Update ()
    {
         if(tutTextIndex == 3 && spellFlags[9] == true) {
            enableNav(true);
        } else if(tutTextIndex == 4 && LevelMaster.GetComponent<LevelMaster>().totalEnemiesKilled >= 5) {
            enableNav(true);
        } else  if(tutTextIndex == 5 && spellFlags[0] == true) {
            enableNav(true);
        } else if(tutTextIndex == 6 && LevelMaster.GetComponent<LevelMaster>().totalEnemiesKilled >= 10) {
            enableNav(true);
        } else  if(tutTextIndex == 7 && spellFlags[2] == true) {
            enableNav(true);
        } else if(tutTextIndex == 8 && LevelMaster.GetComponent<LevelMaster>().totalEnemiesKilled >= 15) {
            enableNav(true);
        } else if(tutTextIndex == 10 && discoveredSpells >= 15)
        {
            start.gameObject.SetActive(true);
        }
    }

    public void nextItem()
    {
        if(tutTextIndex < tutText.Length-1) {
            instructions.text = tutText[++tutTextIndex];
            if(tutTextIndex == 3)
            {
                Time.timeScale = 1f;
                enableElement(alchemyScript.amethystButton);
                indicate(alchemyScript.amethystButton, true);
                if(spellFlags[9] == false) {
                    enableNav(false);
                }
            }
            if(tutTextIndex == 4) {
                if(!oneSpawned) {
                    enableNav(false);
                    LevelMaster.GetComponent<LevelMaster>().spawnPhase(LevelMaster.GetComponent<LevelMaster>().waves[0].phases[0]);
                    alchemyScript.setMaxEssence(50, 50);
                    oneSpawned = true;
                }
            }
            if(tutTextIndex == 5) {
                enableWalls(true);
                enableElement(alchemyScript.emeraldButton);
                indicate(alchemyScript.emeraldButton, true);
                if(spellFlags[0] == false) {
                    enableNav(false);
                }
            }
            if(tutTextIndex == 6)
            {
                if(!twoSpawned) {
                    enableNav(false);
                    LevelMaster.GetComponent<LevelMaster>().spawnPhase(LevelMaster.GetComponent<LevelMaster>().waves[0].phases[1]);
                    alchemyScript.setMaxEssence(50, 50);
                    twoSpawned = true;
                }
            }
            if(tutTextIndex == 7)
            {
                alchemyScript.setMaxEssence(100, 100);
                indicate(alchemyScript.emeraldButton, true);
                indicate(alchemyScript.amethystButton, true);
                if(spellFlags[2] == false) {
                    enableNav(false);
                }
            }
            if(tutTextIndex == 8) {
                if(!threeSpawned) {
                    enableNav(false);
                    LevelMaster.GetComponent<LevelMaster>().spawnPhase(LevelMaster.GetComponent<LevelMaster>().waves[0].phases[2]);
                    alchemyScript.setMaxEssence(100, 100);
                    threeSpawned = true;
                }
            }
            if(tutTextIndex == 10)
            {
                enableElement(alchemyScript.diamondButton);
                enableElement(alchemyScript.rubyButton);
                enableElement(alchemyScript.sapphireButton);
                alchemyScript.setMaxEssence(10000, 10000);
                spellText.gameObject.SetActive(true);
                enableNav(false);
                if (!fourSpawned)
                {
                    LevelMaster.GetComponent<LevelMaster>().spawnPhase(LevelMaster.GetComponent<LevelMaster>().waves[0].phases[3]);
                    fourSpawned = true;
                }
            }
        } 
    }

    public void prevItem()
    {
        if(tutTextIndex > 0) {
            instructions.text = tutText[--tutTextIndex];
            if(tutTextIndex == 2)
            {
                Time.timeScale = 0.0f;
                disableElement(alchemyScript.amethystButton);
                indicate(alchemyScript.amethystButton, false);
                alchemyScript.cancelSpell();
            }
            if(tutTextIndex == 4) {
                enableWalls(false);
                disableElement(alchemyScript.emeraldButton);
                indicate(alchemyScript.emeraldButton, false);
            }
            if(tutTextIndex == 6)
            {
                alchemyScript.setMaxEssence(50, 0);
                indicate(alchemyScript.emeraldButton, false);
                indicate(alchemyScript.amethystButton, false);
            }
            if(tutTextIndex == 9)
            {
                alchemyScript.setMaxEssence(100, 0);
                disableElement(alchemyScript.diamondButton);
                disableElement(alchemyScript.rubyButton);
                disableElement(alchemyScript.sapphireButton);
                spellText.gameObject.SetActive(false);
            }
        }
    }

    public void startGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void disableElement(GameObject button) {
        button.GetComponent<ButtonControl>().disableElement();
    }

    public void enableElement(GameObject button) {
        button.GetComponent<ButtonControl>().enableElement();
    }

    public void indicate(GameObject button, bool ind) {
        button.GetComponent<ButtonControl>().indicate(ind);
    }

    public void enableWalls(bool enWall) {
        tutWall1.SetActive(enWall);
        tutWall2.SetActive(enWall);
        tutWall3.SetActive(enWall);
        tutWall4.SetActive(enWall);
    }
    
    public void enableNav(bool enNav) {
        cont.gameObject.SetActive(enNav);
        prev.gameObject.SetActive(enNav);
    }
    
    public void checkSpell(Spell spell) {
        int spellIndex = Array.IndexOf(spells, spell);
        if(spellIndex != -1) {
            if(spellFlags[spellIndex] == false) {
                discoveredSpells++;
            }
            spellFlags[spellIndex] = true;
        }

        if(discoveredSpells >= 15)
        {
            spellText.text = "       All spells discovered";
            instructions.text = "well done, you are ready to face Tim's main forces";
        }
        spellText.text = "Spells Discovered:" + 
        "\nEmerald + Emerald =    " + (spellFlags[0]?spellDescription[0]:unknown) +
        "\nEmerald + Diamond =   " + (spellFlags[1]?spellDescription[1]:unknown) +
        "\nEmerald + Amethyst =  " + (spellFlags[2]?spellDescription[2]:unknown) +
        "\nEmerald + Sapphire =   " + (spellFlags[3]?spellDescription[3]:unknown) +
        "\nEmerald + Ruby =         " + (spellFlags[4]?spellDescription[4]:unknown) +
        "\nDiamond + Diamond =   " + (spellFlags[5]?spellDescription[5]:unknown) +
        "\nDiamond + Amethyst =  " + (spellFlags[6]?spellDescription[6]:unknown) +
        "\nDiamond + Sapphire =  " + (spellFlags[7]?spellDescription[7]:unknown) +
        "\nDiamond + Ruby =        " + (spellFlags[8]?spellDescription[8]:unknown) +
        "\nAmethyst + Amethyst = " + (spellFlags[9]?spellDescription[9]:unknown) +
        "\nAmethyst + Sapphire = " + (spellFlags[10]?spellDescription[10]:unknown) +
        "\nAmethyst + Ruby =      " + (spellFlags[11]?spellDescription[11]:unknown) +
        "\nSapphire + Sapphire = " + (spellFlags[12]?spellDescription[12]:unknown) +
        "\nSapphire + Ruby =       " + (spellFlags[13]?spellDescription[13]:unknown) +
        "\nRuby + Ruby =            "  + (spellFlags[14]?spellDescription[14]:unknown);
    }
}
