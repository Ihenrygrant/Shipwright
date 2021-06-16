using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ShipController : MonoBehaviour
{


    public List<GameObject> targets = new List<GameObject>();
    public GameObject LoadingScreen;

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(LoadChunks());
        //Load("PlayerShip");
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.GetComponent<ShipInfo>() != null)
        {
            if (other.transform.GetComponent<ShipInfo>().shipFaction != transform.GetComponent<ShipInfo>().shipFaction)
            {
                targets.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.gameObject);
    }

    void OnDisable()
    {
        //TODO setup disable broadcast to other ships.
    }

}



