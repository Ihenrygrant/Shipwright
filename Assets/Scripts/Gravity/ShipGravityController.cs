using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGravityController : MonoBehaviour {

    List<GameObject> gravityTargets = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gravityTargets.Count > 0) {
            //for () {
            //    ApplyGravity();
            //}
        }

	}

    void OnTriggerEnter(Collider other)
    {
       

    }

    void ApplyGravity()
    {

    }
}
