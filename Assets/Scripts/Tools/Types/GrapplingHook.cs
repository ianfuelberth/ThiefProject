using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : Tool
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform grappleSource;
    public LayerMask grappleMask;
    public LineRenderer lineRenderer;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCooldown;
    private float grapplingCdTimer;
    
    private int count = 1;
    private bool grappling = false;

    public override ToolType ToolType { get { return ToolType.Grapple; } }

    public override string ToolName { get { return "Grappling Hook"; } }

    public override int Count { get { return count; } set { count = value; } }

    public override void Start()
    {
        base.Start();

        pm = player.GetComponent<PlayerMovement>();
        lineRenderer.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime; 
        }
    }

    public override void LateUpdate()
    {
        if (grappling)
        {
            lineRenderer.SetPosition(0, grappleSource.position);
        }
    }

    public override void PrimaryUse()
    {
        StartGrapple();
    }

    public override void SecondaryUse()
    {
        
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
        // pm.freeze = true;
        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, maxGrappleDistance, grappleMask))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = playerCam.transform.position + playerCam.transform.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
        
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowPoint = new Vector3(player.transform.position.x, player.transform.position.y - 1f, player.transform.position.z);

        float grapplePointRelYPos = grapplePoint.y - lowPoint.y;
        float highPointOnArc = grapplePointRelYPos + overshootYAxis;

        if (grapplePointRelYPos < 0) highPointOnArc = overshootYAxis;

        pm.GrappleJumpToPosition(grapplePoint, highPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCooldown;

        lineRenderer.enabled = false;
    }
}
