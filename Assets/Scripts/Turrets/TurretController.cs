using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {

    private ShipController shipController;

    public GameObject targetShip;
    public GameObject pylon;
    public GameObject projectile;

    public GameObject projectileStartLocation1;
    public GameObject projectileStartLocation2;
    bool barrelToggle = false;

    public float projectileVelocity = 100.0f;

    bool isFiring = false;
    bool isReloading = false;

    float reloadTime = 1.0f;
    float fireTime = 0.5f;
    

	// Use this for initialization
	void Start () {
        shipController = transform.parent.GetComponent<ShipController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (shipController.targets.Count != 0)
        {
            targetShip = shipController.targets[0];
        }
        else
        {
            targetShip = null;
            CancelInvoke();
        }

        if (targetShip != null)
        {
            aim();
        }
        else
        {
            isFiring = false;

        }

    }

    void aim()
    {
        Transform closestTargetBlock = GetClosestEnemy(targetShip.transform);
        //pylon.transform.LookAt(targetShip.transform);
        pylon.transform.LookAt(closestTargetBlock);


        if (!isFiring)
        {
            InvokeRepeating("fire", 0.0f, 0.1f);
            //InvokeRepeating("reload", reloadTime, reloadTime);
            Invoke("reload", reloadTime);
            isFiring = true;
        }
    }

    Transform GetClosestEnemy(Transform enemyShip)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        //foreach (Transform potentialTarget in enemies)
        foreach (Transform potentialTarget in enemyShip)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    void fire()
    {
        if (!isFiring || isReloading) return;


        if (barrelToggle)
        {
            GameObject bullet = Instantiate(projectile, projectileStartLocation1.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(pylon.transform.forward * projectileVelocity, ForceMode.Force);
            barrelToggle = false;
        }
        else
        {
            GameObject bullet = Instantiate(projectile, projectileStartLocation2.transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(pylon.transform.forward * projectileVelocity, ForceMode.Force);
            barrelToggle = true;
        }





    }

    void reload()
    {
        isReloading = true;
        Invoke("setReload", Random.Range(0.8f, 1.05f) * reloadTime);
    }

    void setReload()
    {
        isReloading = false;
        Invoke("reload", fireTime);
    }

}
