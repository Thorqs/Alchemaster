using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectile : MonoBehaviour {

    public int speed;
    public GameObject trashCan;
    public GameObject marking;
    public GameObject splatterEffects;

    [HideInInspector]
    public float damage = 0;

    [HideInInspector]
    public GameObject areaOfEffect = null;
    public bool hasAreaOfEffect;

    [HideInInspector]
    public Vector3 shootDirection;

    [HideInInspector]
    public GameObject target = null;

    public AudioClip[] sounds;
    private AudioSource audioSource;
    private bool isEnabled;
    private float startTime;
    
    private GameObject tempObj;


    void Start()
    {
        startTime = Time.time;
        Vector3 direction;
        if (target == null)
        {
            direction = shootDirection - transform.position;
        }
        else
        {
            direction = target.transform.position - transform.position;
        }
        
        float angle = 90 - Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        isEnabled = true;
        if (sounds.Length > 0)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0.05f;
            
            int i = Random.Range(0, sounds.Length);
            audioSource.clip = sounds[i];
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update () {
        if (target == null)
        {
            if (transform.position == shootDirection || shootDirection == new Vector3(0, 0, 0))
            {
                if (transform.childCount > 0)
                {
                    var particles = transform.GetChild(0);
                    transform.DetachChildren();
                    var pMain = particles.GetComponent<ParticleSystem>().main;
                    pMain.loop = false;
                    particles.GetComponent<ParticleSystem>().Stop();
                    trashCan.GetComponent<TrashcanScript>().trashParticle(particles.gameObject);
                }
                Destroy(gameObject);
            }
            transform.position = Vector3.MoveTowards(transform.position, shootDirection, speed * Time.deltaTime);
            
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (isEnabled && transform.position == target.transform.position && sounds.Length > 0)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                startTime = Time.time;
                isEnabled = false;
            }
            else if (!isEnabled && Time.time - startTime > audioSource.clip.length || (!isEnabled && sounds.Length == 0))
            {
                Destroy(gameObject);
            }
        }
        
    }


    private void OnDestroy()
    {
        if (hasAreaOfEffect)
        {
            GameObject spellAoe = Instantiate(areaOfEffect, transform.position, transform.rotation);
            spellAoe.GetComponent<AreaOfEffect>().trashCan = trashCan;
            spellAoe.GetComponent<AreaOfEffect>().splatterEffects = splatterEffects;
        }
        else if (target)
        {
            target.GetComponentInParent<Monster>().UpdateHealth(damage * -1);
        }
        
        if(marking != null) {
            tempObj = Instantiate(marking, transform.position, transform.rotation, splatterEffects.transform);
            splatterEffects.GetComponent<SplatterCleanup>().addSplatter(tempObj);
        }
    }

}
