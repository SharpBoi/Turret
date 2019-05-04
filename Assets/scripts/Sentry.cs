using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour {

    #region FIELDS
    public Transform Pitch;
    public Vector3 PitchAxis = new Vector3(0, 1, 0);
    public Vector3 PitchOffset = new Vector3();
    public Transform Yaw;
    public Vector3 YawAxis = new Vector3(1, 0, 0);
    public Vector3 YawOffset = new Vector3();
    public Transform Target;


    private Vector3 proj0 = new Vector3();
    private Vector3 proj1 = new Vector3();
    private RaycastHit hit = new RaycastHit();
    private Plane plane = new Plane();
    #endregion

    void Start () {

    }

    private void FixedUpdate () {
        Vector3 mouseWorld = Input.mousePosition;
        mouseWorld.z = 1;
        mouseWorld = Camera.main.ScreenToWorldPoint(mouseWorld);

        Vector3 tgtPos = Vector3.zero;
        if (Target == null) {
            Physics.Raycast(Camera.main.transform.position, mouseWorld - Camera.main.transform.position, out hit);
            if (hit.collider != null)
                tgtPos = hit.point;
        }
        else {
            tgtPos = Target.position;
        }

        float angle = 0;

        proj0 = Vector3.ProjectOnPlane(tgtPos - transform.position, transform.up);
        proj0.Normalize();
        angle = Vector3.SignedAngle(transform.forward, proj0, transform.up);
        Pitch.localRotation = Quaternion.Euler(PitchAxis * angle + PitchOffset);

        plane.Set3Points(tgtPos, transform.position, transform.position + transform.up);
        proj1 = Vector3.ProjectOnPlane(tgtPos - Yaw.position, plane.normal);
        proj1.Normalize();
        angle = Vector3.SignedAngle(Pitch.forward, proj1, plane.normal);
        Yaw.localRotation = Quaternion.Euler(YawAxis * angle + YawOffset);
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Pitch.position, Pitch.position + proj0);
        Gizmos.DrawLine(Yaw.position, Yaw.position + proj1);

        Gizmos.DrawLine(Yaw.position, Yaw.position + Yaw.forward * 100);
    }
}
