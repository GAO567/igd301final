using System.Collections;
using System.Collections.Generic;
using KevinCastejon.ConeMesh;
using UnityEngine;
using TMPro;
public class MyTechnique : InteractionTechnique
{
    [SerializeField]
    int raycastMaxDistance = 1000;

    [SerializeField]
    private GameObject rightController;

    private LineRenderer lineRenderer;

    private bool isTriggering = false;

    [SerializeField]
    private GameObject cursorPrefab;
    private GameObject cursorInstance; // curosr
    private Renderer cursorRenderer;

    private Color normalColor = Color.gray;
    private Color activeColor = Color.blue;
    private float cursorDistance = 2.0f;


    public TextMeshProUGUI logtext;

    [SerializeField]
    private LayerMask raycastLayerMask; // Layer mask for raycasting

    public GameObject circle;
    public GameObject flashlight;
    public GameObject depthsurface;

    private bool handtrigger = false;


    private void Start()
    {
        lineRenderer = rightController.GetComponent<LineRenderer>();

        if (cursorPrefab != null)
        {
            cursorInstance = Instantiate(cursorPrefab);
            cursorInstance.SetActive(false);
            cursorInstance.GetComponent<SphereCollider>().enabled = true;

            cursorRenderer = cursorInstance.GetComponent<Renderer>();
            cursorRenderer.material.color = normalColor;
           
        }
        circle.SetActive(true);


    }

    private void FixedUpdate()
    {
        Transform rightControllerTransform = rightController.transform;


        // Set the beginning of the line renderer to the position of the controller
        lineRenderer.SetPosition(0, rightControllerTransform.position);

        // Creating a raycast and storing the first hit if existing
        RaycastHit hit;
        bool hasHit = Physics.Raycast(rightControllerTransform.position, rightControllerTransform.forward, out hit, Mathf.Infinity, raycastLayerMask);

        if (!hasHit) hit.point = raycastMaxDistance * rightControllerTransform.forward;
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) == 0.0f){
            //just raycast
            handtrigger = false;
            cursorRenderer.material.color = normalColor;

            circle.SetActive(false);
            depthsurface.transform.position = new Vector3(0, -2, 0);
            depthsurface.transform.localScale = new Vector3(1, 1, 0);

            if (hasHit)
            {
                //position curson at the hit point
                if (cursorInstance != null)
                {
                    cursorInstance.transform.position = hit.point;
                    cursorInstance.SetActive(true);
                    Vector3 directionToCursor = (hit.point - rightControllerTransform.position).normalized;
                    Vector3 adjustedPosition = hit.point - directionToCursor * cursorInstance.transform.localScale.x * 0.5f;
                 


                }
                cursorDistance = Vector3.Distance(rightControllerTransform.position, hit.point);


            }
            else
            {
                Vector3 cursorpos = rightControllerTransform.position + rightControllerTransform.forward * 2.0f;
                cursorInstance.transform.position = rightControllerTransform.position;
               



            }

            // Checking that the user pushed the trigger
            if (!isTriggering && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.1f && hasHit)
            {
                isTriggering = true;
                // Sending the selected object hit by the raycast
                currentSelectedObject = hit.collider.gameObject;
            }

        }
        else
        {
            //advanced raycast 
            cursorRenderer.material.color = activeColor;

            float cursormove = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
            float offset = 0;
            offset += cursormove;

            if (hasHit)
            {
                cursorDistance += 2 * cursormove * Time.deltaTime;
                Vector3 newCursorPos = rightControllerTransform.position + rightControllerTransform.forward * cursorDistance;


                if (cursorInstance != null)
                {
                    if (cursormove == 0f && !handtrigger) {
                        cursorInstance.transform.position = hit.point;
                    }
                    else { cursorInstance.transform.position = newCursorPos;
                        handtrigger = true;
                    }
                    cursorInstance.SetActive(true);
                    Vector3 directionToCursor = (newCursorPos - rightControllerTransform.position).normalized;
                    Vector3 adjustedPosition = newCursorPos - directionToCursor * cursorInstance.transform.localScale.x * 0.5f;
                    circle.SetActive(true);
                    circle.transform.position = adjustedPosition;
                    circle.transform.rotation = Quaternion.LookRotation(rightControllerTransform.forward) * Quaternion.Euler(0, 0, 90);
                    Vector3 middlePos = (hit.point + adjustedPosition) / 2.0f;
                    depthsurface.transform.position = middlePos;
                    depthsurface.transform.rotation = circle.transform.rotation;
                    if (offset < 0)
                    {
                        depthsurface.transform.localScale = new Vector3(1, 1, 0);

                    }
                    depthsurface.transform.localScale = new Vector3(1, 1, Vector3.Distance(hit.point, adjustedPosition));

                    Vector3 vec = newCursorPos - rightControllerTransform.position;
                    hit.point = newCursorPos + 10.0f * vec.magnitude * vec.normalized;

                    //flashlight.transform.position = rightControllerTransform.position;
                    //flashlight.transform.rotation = Quaternion.LookRotation(rightControllerTransform.forward) * Quaternion.Euler(0, 0, 90);
                    //float coneheight = Vector3.Distance(rightControllerTransform.position, adjustedPosition);
                    //if (coneheight > 10.0f)
                    //{
                    //    flashlight.GetComponent<Cone>().ConeHeight = 10.0f;

                    //}
                    //else
                    //{
                    //    flashlight.GetComponent<Cone>().ConeHeight = Vector3.Distance(rightControllerTransform.position, adjustedPosition);
                    //}
                    ////flashlight.GetComponent<Cone>().ConeRadius = 0.4f / flashlight.GetComponent<Cone>().ConeHeight;
                }



            }
            else
            {
                Vector3 cursorpos = rightControllerTransform.position + rightControllerTransform.forward * 2.0f;
                cursorInstance.transform.position = rightControllerTransform.position;
                circle.SetActive(false);
                //flashlight.transform.position = rightControllerTransform.position;
                //flashlight.transform.rotation = Quaternion.LookRotation(rightControllerTransform.forward) * Quaternion.Euler(0, 0, 90);
                //flashlight.GetComponent<Cone>().ConeHeight = Vector3.Distance(rightControllerTransform.position, cursorInstance.transform.position);
                depthsurface.transform.position = new Vector3(0, -2, 0);
                depthsurface.transform.localScale = new Vector3(1, 1, 0);

            }
            GameObject currenthit = cursorInstance.GetComponent<CursorCollisionDetector>().GetCurrentHit();
            if (!isTriggering && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.1f && currenthit)
            {
                isTriggering = true;
                currentSelectedObject = currenthit;

            }



        }
        lineRenderer.SetPosition(1, hit.point);


        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 0.0f)
        {
            isTriggering = false;
        }




        // DO NOT REMOVE
        // If currentSelectedObject is not null, this will send it to the TaskManager for handling
        if (currentSelectedObject)
        {
            logtext.text += "currentselect:" + currentSelectedObject.name;
        }
        base.CheckForSelection();
    }


}
