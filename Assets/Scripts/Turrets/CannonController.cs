using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public ShipController shipController;



    public GameObject targetShip;
    public GameObject barrel;
    public GameObject projectile;
    public ParticleSystem projectileEffect;
    public GameObject projectileStartLocation;

    public List<AudioClip> nearAudio;
    public List<AudioClip> midAudio;
    public List<AudioClip> farAudio;

    private float projectileVelocity = 1000.0f;

    bool isFiring = false;
    bool isReloading = false;

    float reloadTime = 1.0f;


    // Use this for initialization
    void Start()
    {
        //shipController = transform.parent.parent.GetComponent<ShipController>();


    }

    // Update is called once per frame
    void Update()
    {
        if (shipController == null) return;

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
        barrel.transform.LookAt(closestTargetBlock);


        if (!isFiring)
        {
            isFiring = true;
            reload();
        }
    }

    Transform GetClosestEnemy(Transform enemyShip)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        var enemyWeapons = enemyShip.Find("Weapons");

        foreach (Transform potentialTarget in enemyWeapons)
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

        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = midAudio[Random.Range(0, midAudio.Count)];
        audio.Play();

        projectileEffect.Play();
        GameObject bullet = Instantiate(projectile, projectileStartLocation.transform.position, transform.rotation);

        var direction = barrel.transform.forward * projectileVelocity;
        direction += AddNoiseOnAngle(-45, 45);

        bullet.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Force);

    }

    Vector3 AddNoiseOnAngle(float min, float max)
    {
        // Find random angle between min & max inclusive
        float xNoise = Random.Range(min, max);
        float yNoise = Random.Range(min, max);
        float zNoise = Random.Range(min, max);


        Vector3 noise = new Vector3(
 xNoise,
yNoise,
zNoise
);
        // Convert Angle to Vector3
        //Vector3 noise = new Vector3(
        //  Mathf.Sin(2 * Mathf.PI * xNoise / 360),
        //  Mathf.Sin(2 * Mathf.PI * yNoise / 360),
        //  Mathf.Sin(2 * Mathf.PI * zNoise / 360)
        //);
        return noise;
    }

    void reload()
    {
        isReloading = true;
        Invoke("setReload", Random.Range(5f, 8f) * reloadTime);
    }

    void setReload()
    {
        isReloading = false;
        fire();
        reload();
    }
}
