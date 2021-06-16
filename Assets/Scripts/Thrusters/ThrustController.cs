using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustController : MonoBehaviour {

    public KeyCode key;
    public static float force = 1000.0f;
    ParticleSystem thrusterEffect;

	// Use this for initialization
	void Start () {
		//thrusterEffect = GetComponent<ParticleSystem>()
	}
	
	// Update is called once per frame
	void Update () {
        ThrustKey();
	}

    void ThrustKey()
    {
        if (Input.GetKey(key))
        {
            gameObject.GetComponentInParent<Rigidbody>().AddForceAtPosition(-transform.forward*force, transform.position);// (Vector3.forward, ForceMode.Acceleration);
        }
    }

}
