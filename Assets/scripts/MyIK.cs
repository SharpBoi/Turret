using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyIK : MonoBehaviour {

    #region FIELDS
    [Header("Structure")]
    public Transform Pole;
    public Transform Base;
    public List<Transform> Bones = new List<Transform>();
    public Transform Head;

    [Header("Settings")]
    [Range(0, 32)]
    public float MaxAngleScale = 1;
    [Range(1, 100)]
    public int Iterations = 1;
    public bool AimSrcPos = true;

    [Header("EditMode")]
    public bool EnableEditMode = false;

    private List<Vector3> srcPoses = new List<Vector3>();
    private List<float> srcDists = new List<float>();
    private float srcLen = 0;
    private Vector3 headProxy = new Vector3();
    #endregion


    private void Awake () {
        Debug.Log("awake");
    }

    void Start () {
        // fill src positions
        for (int i = 0; i < Bones.Count; i++) {
            srcPoses.Add(Base.InverseTransformPoint(Bones[i].position));
        }

        // fill src distances
        for (int i = 1; i < Bones.Count; i++) {
            float d = Vector3.Distance(Bones[i - 1].position, Bones[i].position);
            srcDists.Add(d);
            srcLen += d;
        }
    }

    private void FixedUpdate () {

        float proxyLen = Vector3.Distance(Bones[0].position, Head.position);
        proxyLen = Mathf.Clamp(proxyLen, 0, srcLen);
        headProxy = Bones[0].position + (Head.position - Bones[0].position).normalized * proxyLen;

        // replace on src positions
        if (AimSrcPos)
            for (int i = 0; i < Bones.Count; i++)
                Bones[i].position = Base.TransformPoint(srcPoses[i]);

        for (int its = 0; its < Iterations; its++) {
            Bones[Bones.Count - 1].position = headProxy;
            // backward
            for (int i = Bones.Count - 1; i > 0; i--) {
                Transform next = (i + 1 < Bones.Count ? next = Bones[i + 1] : null);
                Transform crnt = Bones[i];
                Transform prev = Bones[i - 1];
                float dist = srcDists[i - 1];

                Vector3 dir = (prev.position - crnt.position);
                dir.Normalize();
                #region try limit angle
                if (next != null) {
                    Vector3 dira = next.position - crnt.position;
                    Vector3 dirb = prev.position - crnt.position;

                    float a = Vector3.Angle(dira, dirb);

                    dir = Quaternion.AngleAxis(-(Mathf.PI - a * Mathf.Deg2Rad) * MaxAngleScale * (Vector3.Distance(Bones[0].position, headProxy) / srcLen),
                        new Plane(next.position, crnt.position, prev.position).normal) * dir;
                }
                #endregion

                prev.position = crnt.position + dir * dist * 1;
                //crnt.LookAt(prev);
            }

            // forward
            Bones[0].position = Base.TransformPoint(srcPoses[0]);
            for (int i = 1; i < Bones.Count; i++) {
                Transform prev = Bones[i - 1];
                Transform crnt = Bones[i];
                Transform next = (i + 1 < Bones.Count ? Bones[i + 1] : null);
                float dist = srcDists[i - 1];

                Vector3 dir = (crnt.position - prev.position);
                dir.Normalize();
                #region try limit angle
                if (next != null) {
                    Vector3 dira = next.position - crnt.position;
                    Vector3 dirb = prev.position - crnt.position;
                    float a = Vector3.Angle(dira, dirb);
                    dir = Quaternion.AngleAxis((Mathf.PI - a * Mathf.Deg2Rad) * (MaxAngleScale) * (Vector3.Distance(Bones[0].position, headProxy) / srcLen),
                        new Plane(next.position, crnt.position, prev.position).normal) * dir;
                }
                #endregion

                crnt.position = prev.position + dir * dist * 1;
                //prev.LookAt(next);
            }
        }
    }

    private void OnDrawGizmos () {
        if (Bones.Count >= 3) {
            Gizmos.DrawLine(Bones[0].position, Pole.position);
            Gizmos.DrawLine(Head.position, Pole.position);
        }

        for (int i = 1; i < Bones.Count; i++) {
            Gizmos.DrawLine(Bones[i - 1].position, Bones[i].position);
        }

        // draw proxy
        Gizmos.DrawSphere(headProxy, 0.03f);
    }
}
