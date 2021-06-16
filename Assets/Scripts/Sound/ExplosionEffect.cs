using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {

    public List<AudioClip> nearAudio;
    public List<AudioClip> midAudio;
    public List<AudioClip> farAudio;

    // Use this for initialization
    void Start () {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = midAudio[Random.Range(0, midAudio.Count)];
        audio.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
