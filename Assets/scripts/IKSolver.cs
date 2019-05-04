using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IKSolver)), CanEditMultipleObjects]
public class E_IKSolver : Editor {
    public override void OnInspectorGUI () {
        IKSolver solver = (IKSolver)target;
        if (solver.needResetOption) {
            GUI.enabled = false;
        }
        DrawDefaultInspector();
        if (solver.needResetOption) {
            GUI.enabled = true;
            if (GUILayout.Button("Reset Scene Hierarchy")) {
                solver.ResetHierarchy();
            }
        }
    }
}

[ExecuteInEditMode]
public class IKSolver : MonoBehaviour {
    
    private List<float> lengths = new List<float>();
    private List<Vector3> origPoses = new List<Vector3>(), origScales = new List<Vector3>();
    private List<Quaternion> origRots = new List<Quaternion>();

    [Header("Bones - Leaf to Root")]
    [Tooltip("Make sure you assign them in leaf to root order only...")]
    [SerializeField]
    public List<Transform> bones = new List<Transform>();
    [Tooltip("The end point of the leaf bone positioned at tip of the chain to get its orientation...")]
    public Transform endPointOfLastBone;

    [Header("Settings")]
    [Tooltip("Bend chain towards this target when target is closer than total chain length... Make sure you place it far than total chain length for better accuracy...")]
    public Transform poleTarget;
    [Tooltip("More precision...")]
    public int iterations;

    [Header("EditMode")]
    public bool enable;

    [HideInInspector]
    public bool needResetOption = false;

    private Vector3 lastTargetPosition;
    private bool editorInitialized = false;

    void Start () {
        lastTargetPosition = transform.position;
        if (Application.isPlaying && !editorInitialized) {
            Initialize();
        }
    }

    void Update () {
        if (Application.isEditor && enable && !editorInitialized) {
            if (enable) {
                if (bones.Count == 0) {
                    enable = false;
                    return;
                }
                for (int i = 0; i < bones.Count; i++) {
                    if (bones[i] == null) {
                        enable = false;
                        return;
                    }
                }
                if (endPointOfLastBone == null) {
                    enable = false;
                    return;
                }
                if (poleTarget == null) {
                    enable = false;
                    return;
                }
            }
            Initialize();
        }
        if (lastTargetPosition != transform.position) {
            if (Application.isPlaying || (Application.isEditor && enable)) {
                Solve();
            }
        }
    }

    void Initialize () {
        for(int i = 0; i < bones.Count; i++) {
            lengths.Add(0);
            origPoses.Add(new Vector3());
            origRots.Add(new Quaternion());
            origScales.Add(new Vector3());
        }

        origPoses[0] = bones[0].position;
        origScales[0] = bones[0].localScale;
        origRots[0] = bones[0].rotation;
        lengths[0] = Vector3.Distance(endPointOfLastBone.position, bones[0].position);
        GameObject g = new GameObject();
        g.name = bones[0].name;
        g.transform.position = bones[0].position;
        g.transform.up = -(endPointOfLastBone.position - bones[0].position);
        g.transform.parent = bones[0].parent;
        bones[0].parent = g.transform;
        bones[0] = g.transform;
        for (int i = 1; i < bones.Count; i++) {
            origPoses[i] = bones[i].position;
            origScales[i] = bones[i].localScale;
            origRots[i] = bones[i].rotation;
            lengths[i] = Vector3.Distance(bones[i - 1].position, bones[i].position);
            g = new GameObject();
            g.name = bones[i].name;
            g.transform.position = bones[i].position;
            g.transform.up = -(bones[i - 1].position - bones[i].position);
            g.transform.parent = bones[i].parent;
            bones[i].parent = g.transform;
            bones[i] = g.transform;
        }
        editorInitialized = true;
        needResetOption = true;
    }

    void Solve () {
        Vector3 rootPoint = bones[bones.Count - 1].position;
        bones[bones.Count - 1].up = -(poleTarget.position - bones[bones.Count - 1].position);
        for (int i = bones.Count - 2; i >= 0; i--) {
            bones[i].position = bones[i + 1].position + (-bones[i + 1].up * lengths[i + 1]);
            bones[i].up = -(poleTarget.position - bones[i].position);
        }
        for (int i = 0; i < iterations; i++) {
            bones[0].up = -(transform.position - bones[0].position);
            bones[0].position = transform.position - (-bones[0].up * lengths[0]);
            for (int j = 1; j < bones.Count; j++) {
                bones[j].up = -(bones[j - 1].position - bones[j].position);
                bones[j].position = bones[j - 1].position - (-bones[j].up * lengths[j]);
            }

            bones[bones.Count - 1].position = rootPoint;
            for (int j = bones.Count - 2; j >= 0; j--) {
                bones[j].position = bones[j + 1].position + (-bones[j + 1].up * lengths[j + 1]);
            }
        }
        lastTargetPosition = transform.position;
    }

    /// <summary>
    /// Do not ever call this in Play mode. It will mess up the IK system.
    /// </summary>
    public void ResetHierarchy () {
        for (int i = 0; i < bones.Count; i++) {
            Transform t = bones[i].GetChild(0);
            bones[i].GetChild(0).parent = bones[i].parent;
            if (Application.isPlaying) {
                Destroy(bones[i].gameObject);
            }
            else {
                DestroyImmediate(bones[i].gameObject);
            }
            bones[i] = t;
            t.position = origPoses[i];
            t.rotation = origRots[i];
            t.localScale = origScales[i];
        }
        lastTargetPosition = Vector3.zero;
        enable = false;
        editorInitialized = false;
        needResetOption = false;
    }
}

