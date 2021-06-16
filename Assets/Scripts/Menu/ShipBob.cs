using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBob : MonoBehaviour
{
    Vector3 startMarker;
    Vector3 endMarker;
    Vector3 bobAmount = new Vector3(0, 10, 0);
    bool toggle = true;

    public float speed = 0.0001f;

    private float startTime;

    private float journeyLength;



    void Start()
    {
        startMarker = transform.position;
        endMarker = transform.position + bobAmount;

        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, endMarker);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, endMarker) < 1.0f)
        {
            if (toggle)
            {
                startMarker = transform.position;
                endMarker = startMarker - bobAmount;

                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker, endMarker);

                toggle = false;
            }
            else
            {
                startMarker = transform.position;
                endMarker = startMarker + bobAmount;

                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker, endMarker);

                toggle = true;
            }
        }

        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Slerp(startMarker, endMarker, fracJourney);
    }
}
