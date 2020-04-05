using UnityEngine;

//Create reagents in asset menu
[CreateAssetMenu(fileName = "New Reagent", menuName = "Reagent")]

/**
 * Container for Reagent Data
 */
public class Reagent : ScriptableObject {

    public new string name;
    public string description;

    public Sprite image;

    //Enum representation of the reagent.
    public ReagentEnum reagentEnum;

}

//An enum representation of the Reagent.
public enum ReagentEnum { Ruby, Emerald, Sapphire, Diamond, Amethyst };