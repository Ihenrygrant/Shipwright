using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerRoundController : MonoBehaviour {

    public float timeout = 5.0f;
    public GameObject explosion;

    public List<AudioClip> nearAudio;
    public List<AudioClip> midAudio;
    public List<AudioClip> farAudio;

    // Use this for initialization
    void Start () {
        Invoke("DeactivateSelf", timeout);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DeactivateSelf()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = midAudio[Random.Range(0, midAudio.Count)];
        audio.Play();

        Instantiate(explosion, transform.position, transform.rotation);
        DeactivateSelf();
    }


}
