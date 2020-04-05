using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [HideInInspector]
    public float damage; //-1 if non damaging ability.


    public Attributes element;

    public float duration;
    public float size;
    public GameObject marking;
    public GameObject splatterEffects;
    public bool markOnDeath;//if false, mark appears on start, true = mark appears on death.
    public bool hasParticles;

    public GameObject trashCan;

    private float startTime;

    public AudioClip[] sounds;
    private AudioSource audioSource;
    private bool isEnabled;
    
    private GameObject tempObj;

    public EffectShape shape = EffectShape.Circle;


    private List<Collider2D> monstersInRange = new List<Collider2D>();
    // Is used solely for the tornado spell. Checks if the monster has reached the centre.
    private List<bool> hasReachedCentre = new List<bool>();
    private List<Vector3> trajectory = new List<Vector3>();

    [Header("DoT Settings")]
    public bool isDOT;
    public float tickRate; //damage ticks per second
    private float lastTickTime = 0;
    private float damagePerTick;

    [Header("Crowd Control Settings")]
    public bool isCC;
    public float slowFactor;

    [Header("Displacement Settings")]
    public bool isDisplacement;
    public bool isTornado;


    void Start()
    {
        transform.localScale *= size;
        startTime = Time.time;
        if(hasParticles) {
            transform.GetChild(0).GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Background";
            transform.GetChild(0).GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 30;
        } else {
            transform.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
            transform.GetComponent<SpriteRenderer>().sortingOrder = 30;
        }


        if (isDOT)
        {
            damagePerTick = damage / (duration * tickRate);
        }

        isEnabled = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.05f;
        if (sounds.Length > 0)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0.05f;
            int i = Random.Range(0, sounds.Length);
            audioSource.clip = sounds[i];
            audioSource.Play();
        }

        if (marking != null && markOnDeath == false)
        {
            tempObj = Instantiate(marking, transform.position, transform.rotation, splatterEffects.transform);
            splatterEffects.GetComponent<SplatterCleanup>().addSplatter(tempObj);
        }
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < monstersInRange.Count; i++)
        {
            //If the monster at i is dead, remove reference.
            if (monstersInRange[i] == null)
            {
                monstersInRange.RemoveAt(i);
                if (isTornado)
                {
                    hasReachedCentre.RemoveAt(i);
                    trajectory.RemoveAt(i);
                }
                --i;
            }
        }

        if (isEnabled && Time.time - startTime > duration)
        {
            if (isDisplacement)
            {
                for (int m = 0; m < monstersInRange.Count; m++)
                {
                    
                        monstersInRange[m].transform.parent.position = transform.position + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    
                }
            }
            if (transform.childCount > 0)
            {
                var particles = transform.GetChild(0);
                transform.DetachChildren();
                var pMain = particles.GetComponent<ParticleSystem>().main;
                pMain.loop = false;
                particles.GetComponent<ParticleSystem>().Stop();
                particles.localScale /= size;
                trashCan.GetComponent<TrashcanScript>().trashParticle(particles.gameObject);
            }
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            startTime = Time.time;
            isEnabled = false;

            if (marking != null && markOnDeath == true)
            {
                tempObj = Instantiate(marking, transform.position, transform.rotation, splatterEffects.transform);
                splatterEffects.GetComponent<SplatterCleanup>().addSplatter(tempObj);
            }
        }

        if (!isEnabled && (sounds.Length == 0 || Time.time - startTime > audioSource.clip.length))
        {
            Destroy(gameObject);
        }


        if (isDOT)
        {
            if (Time.time - lastTickTime > 1 / tickRate && monstersInRange.Count > 0)
            {
                for (int m = 0; m < monstersInRange.Count; m++)
                {

                    Monster monster = monstersInRange[m].GetComponentInParent<Monster>();
                    float damageDealt = damagePerTick;
                    if (!monster.weaknesses.Contains(Attributes.None) && monster.weaknesses.Contains(element))
                    {
                        damageDealt *= 2;
                    }
                    monster.UpdateHealth(-1 * damageDealt);

                }
                lastTickTime = Time.time;
            }
        }

        if (isDisplacement)
        {
            for (int m = 0; m < monstersInRange.Count; m++)
            {
                if (isTornado && hasReachedCentre[m])
                {
                    monstersInRange[m].transform.parent.position = Vector3.MoveTowards(monstersInRange[m].transform.parent.position, trajectory[m], 4 * Time.deltaTime);
                }
                else
                {
                    monstersInRange[m].transform.parent.position = Vector3.MoveTowards(monstersInRange[m].transform.parent.position, transform.position, 5 * Time.deltaTime);
                    if (isTornado && monstersInRange[m].transform.parent.position == transform.position)
                    {
                        hasReachedCentre[m] = true;
                        trajectory[m] = monstersInRange[m].transform.parent.position + Vector3.Normalize( new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * 10;
                    }
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCC)
        {
            if (other.tag == "Monster" && !other.gameObject.GetComponentInParent<Monster>().slowed)
            {
                if (!other.isTrigger)
                {
                    AnimationController monster = other.gameObject.GetComponent<AnimationController>();
                    monster.UpdateSpeed(slowFactor);
                    other.gameObject.GetComponentInParent<Monster>().slowed = true;
                }
            }
        }

        if (isDOT || isDisplacement)
        {
            if(isDisplacement && other.tag == "Monster" && (other.GetComponent<IceUmbrella>() || other.GetComponent<IceTable>())){
                Debug.Log("Icy-bois are cc immune");
                return;
            }
            else if (other.tag == "Monster" && !monstersInRange.Contains(other))
            {
                monstersInRange.Add(other);
                if (isTornado)
                {
                    hasReachedCentre.Add(false);
                    trajectory.Add(Vector3.zero);
                }
            }

        }

        else
        {
            if (other.tag == "Monster")
            {
                float damageDealt = damage;
                //Check to see if its an effective type.
                Monster monster = other.gameObject.GetComponentInParent<Monster>();
                if (!monster.weaknesses.Contains(Attributes.None) &&  monster.weaknesses.Contains(element))
                {
                    damageDealt *= 2;
                }
                monster.UpdateHealth(-1 * damageDealt);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (monstersInRange.Contains(other))
        {
            int index = monstersInRange.IndexOf(other);
            monstersInRange.Remove(other);
            if (isTornado)
            {
                hasReachedCentre.RemoveAt(index);
                trajectory.RemoveAt(index);
            }
        }
        if (isCC && other.tag == "Monster" && other.gameObject.GetComponentInParent<Monster>().slowed)
        {
            AnimationController monster = other.gameObject.GetComponent<AnimationController>();
            monster.UpdateSpeed(1 / slowFactor);
            other.gameObject.GetComponentInParent<Monster>().slowed = false;
        }
    }

}

public enum EffectShape { Circle, Line }
