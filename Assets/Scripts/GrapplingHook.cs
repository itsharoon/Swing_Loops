using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GrapplingHook : MonoBehaviour {

    private Rigidbody RB;

    [HideInInspector]
    public bool draw_dev_line = true;
    public static bool FallAnim = false;

    [HideInInspector]
    public float break_tether_velo;
    private float node_distance;
    private float start_node_distance;
    [HideInInspector]
    public float ForceAmount;

    private GameObject hooked_node;
    private GameObject grappling_line;
    private GameObject[] HookPoints;

    private SpringJoint Joint;

    [HideInInspector]
    public Texture LR_Texture;

    [HideInInspector]
    public Transform Player_Hand;
    [HideInInspector]
    public Transform TurnPoint;


    #region DEFAULT FUNCTIONS
    void Start() 
    {
        RB = gameObject.GetComponent<Rigidbody>();
        InitializeLineRenderer();
    }

    private void Update()
    {
        if (PlayerController.GameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //If Gameobject is Null, Attach a Rope to the Hook
                AttachRope();
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Remove Rope From Hook
                BreakTether();
            }

            if (Input.GetMouseButton(0))
            {
                if (hooked_node.name == "Point4" || hooked_node.name == "Point5")
                {
                    //Smoothly Update Rotation on Curved HookPoints
                    UpdateRotation();
                }
            }
        }
    }

    private void LateUpdate()
    {
        //After all the Initialization/Updations, Create/Draw a rope
        CreateRope();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HookTrigger"))
        {
            ChangeHookColor();
        }
    }
    #endregion

    #region CUSTOM FUNCTIONS
    void InitializeLineRenderer()
    {
        if (draw_dev_line)
        {
            grappling_line = new GameObject("GrapplingLine");
            grappling_line.SetActive(false);
            LineRenderer line_renderer = grappling_line.AddComponent<LineRenderer>();
            line_renderer.materials[0].mainTexture = LR_Texture;
            line_renderer.endWidth = .3f;
            line_renderer.startWidth = .1f;
        }
        hooked_node = null;
        return;
    }

    void CreateRope()
    {
        if (hooked_node != null)
        {
            if (draw_dev_line)
            {
                grappling_line.GetComponent<LineRenderer>().SetPosition(0, Player_Hand.transform.position);
                grappling_line.GetComponent<LineRenderer>().SetPosition(1, hooked_node.transform.position);
            }
        }
    }

    //Attach rope through spring joint on a hook
    void AttachRope()
    {
        if (hooked_node == null)
        {
            FallAnim = false;
            hooked_node = FindClosestHook();

            node_distance = (Vector3.Distance(hooked_node.transform.position, gameObject.transform.position));

            if (draw_dev_line)
            {
                FindClosestHook().GetComponent<MeshRenderer>().material.color = Color.blue;

                RB.AddForce(Vector3.up, ForceMode.Impulse);

                if (hooked_node.name != "Point4" || hooked_node.name != "Point5")
                    RB.AddRelativeForce(Vector3.forward * 1.5f);

                Joint = this.gameObject.AddComponent<SpringJoint>();
                Joint.autoConfigureConnectedAnchor = false;
                Joint.connectedAnchor = hooked_node.transform.position;

                Joint.maxDistance = node_distance * 0.35f;
                Joint.minDistance = node_distance * 0.15f;

                Joint.spring = 4f;
                Joint.damper = 7f;
                Joint.massScale = 4.5f;
                grappling_line.SetActive(true);
            }
        }
    }

    //Remove rope from hook point
    public void BreakTether()
    {
        if (hooked_node != null)
        {
            hooked_node = null;

            if (draw_dev_line)
            {
                FallAnim = true;
                grappling_line.GetComponent<LineRenderer>().SetPosition(0, Vector3.zero);
                grappling_line.SetActive(false);
                Destroy(this.gameObject.GetComponent<SpringJoint>());
            }
        }
    }
    
    //Find Closest Hook Point to which the player can attach the rope
    GameObject FindClosestHook()
    {
        HookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
        GameObject Closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in HookPoints)
        {
            Vector3 diff = t.transform.position - currentPos;
            float dist = diff.sqrMagnitude;
            if (dist < minDist)
            {
                Closest = t;
                minDist = dist;
            }
        }
        return Closest;
    }

    //Rotation Update on Curved Points
    void UpdateRotation()
    {
        if (hooked_node.name == "Point4")
        {
            transform.RotateAround(hooked_node.transform.position, Vector3.up, Time.deltaTime * 15);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 215, 0), Time.deltaTime);
        }

        if (hooked_node.name == "Point5")
        {
            transform.RotateAround(TurnPoint.transform.position, Vector3.up, Time.deltaTime * 20);
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }  
    }

    //Highlight HookPoint based on distance from player
    void ChangeHookColor()
    {
        FindClosestHook().GetComponent<MeshRenderer>().material.color = Color.red;
        FindClosestHook().transform.parent.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    #endregion
}
