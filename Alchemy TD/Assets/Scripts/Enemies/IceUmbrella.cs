using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceUmbrella : Monster {

    public GameObject parentHUD;
    public int numPoints = 64;
    public CircleCollider2D hitCirc;
    /*
    new void Start()
    {
        currentHealth = maxHealth;
    }
    */

    override public void UpdateHealth(float healthDifference)
    {
        Debug.Log(currentHealth);
        currentHealth += healthDifference;
        // healthBar.fillAmount = ((float)currentHealth / (float)maxHealth);
        if(currentHealth <= 0)
        {

            parentHUD.SetActive(true);
            transform.parent.GetComponentInParent<Monster>().shielded = false;
            transform.parent.GetComponent<Animator>().SetTrigger("Melt");
            gameObject.SetActive(false);
        }
    }

    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            other.gameObject.GetComponentInParent<Monster>().shielded = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            other.gameObject.GetComponentInParent<Monster>().shielded = false;
        }
    }
    */

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            other.gameObject.GetComponentInParent<Monster>().shielded = true;
        }
    }

    /*void OnDestroy()
    {
        // simple colldier shrinking
        float rad_o = hitCirc.radius;
        for(int i = 100; i > 0; i--)
        {
            hitCirc.radius = i*rad_o/100f;
        }

        /*
        raycast tech for getting people out
        // Compute the angle between two triangles in the circle
		float delta = 2f*Mathf.PI/(float)(numPoints-1);
		// Start with angle of 0
		float alpha = 0f;

        for(int i=1; i<=numPoints; i++)
        {
            // get polar x and y from spherical
            float x = hitCirc.radius*Mathf.Cos(alpha);
            float y = hitCirc.radius*Mathf.Sin(alpha);

            // Create a ray
            Vector2 ray = new Vector2(x, y);
            ray.x *= transform.lossyScale.x;
            ray.y *= transform.lossyScale.y;

            // Cast the ray
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, ray.magnitude);
            // Check if ray has hit something, if yes, check how far from the ray's origin point
            // and adjust the distance of where the mesh point is going to be located.
            if(hit.collider.tag == "Monster")
            {
                hit.collider.gameObject.GetComponentInParent<Monster>().shielded = false;
            }
            else
            {
                alpha += delta;
            }
        }
    }*/
}
