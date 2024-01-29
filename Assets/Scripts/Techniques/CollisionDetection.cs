

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Material shoppingatlas;
    public Material translucent;
    public GameObject Task;
    private HashSet<Collider> collidersInside = new HashSet<Collider>();
    private Collider myCollider;
    private int ignoreLayer;
    private int playerLayer;
    private int groundLayer;
    private Collider cursorhit=null;
    private GameObject tasktarget;


    private void Start()
    {
        myCollider = GetComponent<Collider>();
        ignoreLayer = LayerMask.NameToLayer("Ignore");
        playerLayer = LayerMask.NameToLayer("Player");
        groundLayer = LayerMask.NameToLayer("Ground");
    }


    private void Update()
    {
        tasktarget = Task.GetComponent<TaskManager>().GetCurrentObjectToSelect();
        Debug.Log("get current target" + tasktarget.GetInstanceID());
        Collider[] colliders = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, Quaternion.identity);
        HashSet<Collider> currentColliders = new HashSet<Collider>(colliders);

        foreach (var collider in currentColliders)
        {
            if (collider.gameObject.layer != ignoreLayer && collider.gameObject.layer != playerLayer && collider.gameObject.layer != groundLayer)
            {
                OnTriggerEnterManually(collider);
            }
            else
            {
                Debug.Log("something get"+collider.gameObject.layer);
            }
        }

        foreach (var collider in collidersInside)
        {
            if (!currentColliders.Contains(collider))
            {
                if (collider.gameObject.layer != ignoreLayer && collider.gameObject.layer != playerLayer && collider.gameObject.layer != groundLayer)
                {
                    OnTriggerExitManually(collider);

                }

            }
        }

        collidersInside = currentColliders;

    }




    private void OnTriggerExitManually(Collider other)
    {
        collidersInside.Remove(other);
        Debug.Log("Cone exited the collider of: " + other.gameObject.name);

        Outline outline = other.gameObject.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;

        }
        Debug.Log("other id " + other.gameObject.GetInstanceID());

        if(other.gameObject.GetInstanceID()!= tasktarget.GetInstanceID())
        {
            other.GetComponent<MeshRenderer>().material = shoppingatlas;

        }

        //if (cursorhit != other)
        //{
        //    other.GetComponent<MeshRenderer>().material = shoppingatlas;

        //}

        //Renderer renderer = other.gameObject.GetComponent<Renderer>();
        //Material[] currentMaterials = renderer.materials;
        //currentMaterials = System.Array.FindAll(currentMaterials, material =>
        //        material.name != "OutlineMask (Instance) (Instance)" &&
        //        material.name != "OutlineFill (Instance) (Instance)");
        //var newMaterialsList = new List<Material>(currentMaterials);
        //newMaterialsList.Add(shoppingatlas);
        //renderer.materials = newMaterialsList.ToArray();

    }

    void OnTriggerEnterManually(Collider other)
    {
        collidersInside.Add(other);
        Debug.Log("Cone entered the collider of: " + other.gameObject.name);

        Outline outline = other.gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = other.gameObject.AddComponent<Outline>();
            outline.OutlineColor = Color.white;
        }
        else
        {
            outline.enabled = true;

        }
        if (other.gameObject.GetInstanceID() != tasktarget.GetInstanceID())
        {
            other.GetComponent<MeshRenderer>().material = translucent;

        }
        Debug.Log("other id " + other.gameObject.GetInstanceID());
        //if (cursorhit != other)
        //{
        //    other.GetComponent<MeshRenderer>().material = translucent;

        //}


        //Renderer renderer = other.gameObject.GetComponent<Renderer>();
        //Material[] materials = renderer.materials;
        //int index = -1;

        //for (int i = 0; i < materials.Length; i++)
        //{
        //    if (materials[i].name.Equals("hypercasual_shopping_atlas" + " (Instance)", System.StringComparison.InvariantCulture))
        //    {
        //        index = i;
        //        break;
        //    }
        //}

        //if (index != -1)
        //{
        //    List<Material> newMaterialsList = new List<Material>(materials);
        //    newMaterialsList.RemoveAt(index);
        //    renderer.materials = newMaterialsList.ToArray();
        //}
    }

    public void SetCursorHit(Collider collider)
    {
        cursorhit = collider;
    }


}
