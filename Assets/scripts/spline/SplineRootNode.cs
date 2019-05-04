using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineRootNode : MonoBehaviour {

    #region FIELDS
    public SplineRootNode Prev;
    public SplineRootNode Next;

    public int Index = -1;

    [HideInInspector]
    public float GizmoSize = 0.1f;
    #endregion

    void Start () {

    }

    private void OnDrawGizmos () {
        Gizmos.DrawSphere(transform.position, GizmoSize);
    }
}
