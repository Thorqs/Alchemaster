using UnityEngine;

/// <summary>
/// Contains data for castable spells.
/// </summary>
[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject {

    public new string name;
    public string description;

    public float damage;
    public int cost = 10;

    [Header("Projectile Settings")]
    public bool hasProjectile;
    public GameObject spellProjectile = null;

    [Header("Area of Effect Settings")]
    public bool hasAreaOfEffect;
    public bool isTower;
    public bool isWall;
    public GameObject areaOfEffect = null;
    
    [Header("Reagent Settings")]
    public bool isCraftable = true;
    // The number of reagents the spell must require (default 2).
    private const int NUMREAGENTS = 2;
    // The list of reagents that make up the spell.
    public ReagentEnum[] reagents = new ReagentEnum[NUMREAGENTS];

    // Controls how the spell will snap to the game grid.
    [Header("Cast Settings")]
    public SnapMode snapMode;

    private void OnEnable()
    {
        if (!hasAreaOfEffect) {
            if (hasProjectile)
            {
                spellProjectile.GetComponent<SpellProjectile>().damage = damage;
            }
        }
        else if (areaOfEffect.GetComponent<AreaOfEffect>())
        {
            areaOfEffect.GetComponent<AreaOfEffect>().damage = damage;
        }

        if (spellProjectile)
        {
            spellProjectile.GetComponent<SpellProjectile>().hasAreaOfEffect = hasAreaOfEffect;
        }

        if (isCraftable)
        {
            // Forces the number of reagents to be NUMREAGENTS (default 2).
            System.Array.Resize(ref reagents, NUMREAGENTS);
        }
        // Set the projectile's area of effect. Allows the projectile to instantiate AoE
        // when it reaches target location.
        if (hasProjectile)
        {
            spellProjectile.GetComponent<SpellProjectile>().areaOfEffect = areaOfEffect;
        }   
        
    }

}
