using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {

    #region FIELDS
    private Animator animtr;
    #endregion

    void Start () {
        animtr = GetComponent<Animator>();
    }

    private void FixedUpdate () {
        animtr.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.identity);
    }

    private void OnAnimatorIK (int layerIndex) {
        Debug.Log("ik " + layerIndex);
    }

    private void OnStateIK() {
        Debug.Log("ik " );

    }
}
