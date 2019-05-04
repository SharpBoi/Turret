using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BezierSpline : MonoBehaviour {

    #region FIELDS
    [Header("Nodes")]
    public List<SplineRootNode> Nodes = new List<SplineRootNode>();

    [Header("Settings")]
    [Range(0, 1000)]
    public int Interpolate = 10;
    public bool UpdateRebuildEditMode = true;
    public bool UpdateRebuildGameMode = false;
    public float GizmoSize = 0.1f;

    private Vector3[] points = new Vector3[0];
    private Vector3[] subPnts = new Vector3[0];
    #endregion

    void Start () {
        FillNodes();
    }

    // Update is called once per frame
    void Update () {
        if (UpdateRebuildEditMode && Application.isPlaying == false) {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].GizmoSize = GizmoSize;
            Build();
        }
    }
    private void FixedUpdate () {
        if (UpdateRebuildGameMode && Application.isPlaying) {
            Build();
        }
    }

    public void Build() {
        if (points.Length != Interpolate) points = new Vector3[Interpolate + 1];
        for (int i = 0; i < Interpolate; i++) {
            Vector3 p = solve((float)i / Interpolate);
            points[i] = p;
        }
        points[points.Length - 1] = Nodes[Nodes.Count - 1].transform.position;
    }

    public Vector3 PosOnSpline(float t) {
        return solve(t);
    }

    private Vector3 solve (float t) {
        int offset = 0;
        int its = 0;
        for (int i = 0; i < subPnts.Length; i++) {
            int prevI = i - (Nodes.Count - 1 - offset) - 1;

            its++;
            if (its == Nodes.Count - 1 - offset) {
                its = 0;
                offset++;
            }

            if (prevI < 0) {
                subPnts[i] = Vector3.Lerp(Nodes[i].transform.position, Nodes[i + 1].transform.position, t);
            }
            else {
                subPnts[i] = Vector3.Lerp(subPnts[prevI], subPnts[prevI + 1], t);
            }
        }

        return Vector3.Lerp(subPnts[subPnts.Length - 2], subPnts[subPnts.Length - 1], t);
    }

    public void FillNodes () {
        for (int i = 0; i < Nodes.Count; i++) {
            if (Nodes[i] == null) {
                Nodes.RemoveAt(i);
                i = 0;
                continue;
            }

            SplineRootNode root = Nodes[i];

            root.Index = i;
            root.name = "root (" + i + ")";

            if (i + 1 < Nodes.Count) root.Next = Nodes[i + 1];
            else root.Next = Nodes[0];

            if (i - 1 > -1) root.Prev = Nodes[i - 1];
            else root.Prev = Nodes[Nodes.Count - 1];
        }

        subPnts = new Vector3[getSubPointsCnt()];
        points = new Vector3[Interpolate + 1];
    }
    public int getSubPointsCnt () {
        int ret = 0;
        int rslt = 0;
        for (int i = 0; i < Nodes.Count; i++) {
            rslt = Nodes.Count - (i + 1);
            ret += rslt;
            if (rslt == 2) return ret;
        }

        return ret;
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.blue;
        for (int i = 1; i < Nodes.Count; i++)
            Gizmos.DrawLine(Nodes[i - 1].transform.position, Nodes[i].transform.position);

        Gizmos.color = Color.white;
        for (int i = 1; i < points.Length; i++)
            Gizmos.DrawLine(points[i - 1], points[i]);
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor {
    public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        BezierSpline tgt = target as BezierSpline;

        if (GUILayout.Button("Fill nodes")) {
            tgt.FillNodes();
        }
        if(GUILayout.Button("Force rebuild")) {
            tgt.FillNodes();
            tgt.Build();
        }
    }
}