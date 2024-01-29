using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCollisionDetector : MonoBehaviour
{
    public Material shoppingatlas;
    private GameObject depthsurface;
    private GameObject currentHit;
    private int ignoreLayer;
    private int playerLayer;
    private int groundLayer;

    private Collider myCollider;


    private void Start()
    {
        myCollider = GetComponent<Collider>();
        ignoreLayer = LayerMask.NameToLayer("Ignore");
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
        depthsurface = GameObject.Find("Cube");

    }

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, myCollider.bounds.extents.x);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == ignoreLayer || hitCollider.gameObject.layer == playerLayer ||  hitCollider.gameObject.layer == groundLayer)
            {
                continue; // Skip the ignored layers
            }

            if (currentHit != hitCollider.gameObject)
            {
                if (currentHit != null)
                {
                    Debug.Log("Cursor exited the collider of: " + currentHit.name);
                    currentHit.GetComponent<Outline>().OutlineColor = Color.white;
                    currentHit.GetComponent<Outline>().enabled = false;

                    
                }

                currentHit = hitCollider.gameObject;
                Debug.Log("Cursor entered the collider of: " + currentHit.name);
                currentHit.GetComponent<Outline>().OutlineColor = Color.yellow;
                currentHit.GetComponent<Outline>().enabled = true;
                 
                depthsurface.GetComponent<CollisionDetection>().SetCursorHit(hitCollider);



            }
        }

        // If no colliders are hit, currentHit is set to null
        if (hitColliders.Length == 0 && currentHit != null)
        {
            Debug.Log("Cursor exited the collider of: " + currentHit.name);
           currentHit.GetComponent<Outline>().OutlineColor = Color.white;
            currentHit.GetComponent<Outline>().enabled = false;
            currentHit = null;
        }
    }

      
    //}
    public GameObject GetCurrentHit()
    {
        return currentHit;
    }
}
