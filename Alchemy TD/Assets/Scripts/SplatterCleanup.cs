using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterCleanup : MonoBehaviour {

    public int maxSplatters;
    private Queue<GameObject> splatters = new Queue<GameObject>();

    GameObject tempDelete;

    // Update is called once per frame
    void Update()
    {
        while(splatters.Count > maxSplatters)
        {
            tempDelete = splatters.Dequeue();
            Destroy(tempDelete);
        }
    }

    public void addSplatter(GameObject item)
    {
        //item should be a child of the splatterCleanup object.
        splatters.Enqueue(item);
    }
}
