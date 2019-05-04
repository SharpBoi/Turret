using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocateOnSpline : MonoBehaviour {

    #region FIELDS
    public List<Transform> WhichObjects = new List<Transform>();
    public BezierSpline WhatSpline;

    public bool UpdateInGameMode = true;
    #endregion

    void Start () {

    }

    private void FixedUpdate () {
        if(UpdateInGameMode) {
            for(int i = 0; i < WhichObjects.Count; i++) {
                WhichObjects[i].position = WhatSpline.PosOnSpline((float)i / WhichObjects.Count);
            }
        }
    }
}
