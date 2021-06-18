using System.Collections.Generic;
using UnityEngine;

public class ChunkColliderController : MonoBehaviour {

    ChunkGameObject chunk;

    List<Collider> colliderTargets = new List<Collider>();

    List<BoxCollider> playerColliders = new List<BoxCollider>();

	// Use this for initialization
	void Start () {

        for (int i = 0; i < 3*3*3; i++) {
            BoxCollider m_collider = gameObject.AddComponent<BoxCollider>();
            m_collider.center = new Vector3(0,0,0);
            m_collider.size = new Vector3(1,1,1);

            playerColliders.Add(m_collider);
            m_collider.enabled = false;
        }

        chunk = GetComponent<ChunkGameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (colliderTargets.Count > 0)
        {
            for (int i = 0; i < colliderTargets.Count; i++) {
                //FindNearestColliders(colliderTargets[i]);
            }
        }
	}


    //TODO Offset to center of player
    void FindNearestColliders(Collider target)
    {
        Vector3 localPosition = (target.transform.position - transform.position);

        int x = Mathf.Min(ChunkSettings.xSize - 1, Mathf.Max(0, (int)localPosition.x));
        int y = Mathf.Min(ChunkSettings.ySize - 1, Mathf.Max(0, (int)localPosition.y));
        int z = Mathf.Min(ChunkSettings.zSize - 1, Mathf.Max(0, (int)localPosition.z));

        int colliderindex=0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int w = -1; w <= 1; w++)
                {
                    ToggleCollider(x+i, y+j, z+w, ref colliderindex);
                }
            }
        }
    }

    public int to1D(int x, int y, int z)
    {
        return (z * ChunkSettings.xSize * ChunkSettings.ySize) + (y * ChunkSettings.zSize) + x;
    }

    public int[] to3D(int idx)
    {
        int z = idx / (ChunkSettings.xSize * ChunkSettings.ySize);
        idx -= (z * ChunkSettings.xSize * ChunkSettings.ySize);
        int y = idx / ChunkSettings.xSize;
        int x = idx % ChunkSettings.xSize;
        return new int[] { x, y, z };
    }


    void ToggleCollider(int x, int y, int z,ref int index)
    {
        if (x < 0 || x > ChunkSettings.xSize - 1 || y < 0 || y > ChunkSettings.ySize - 1 || z < 0 || z > ChunkSettings.zSize - 1) {
            return;
        }

        if (chunk.chunkInfo.blocks[x, y, z].Status == (byte)BlockStatus.Occupied_Block || chunk.chunkInfo.blocks[x, y, z].Status == (byte)BlockStatus.Occupied_DestroyedBlock)
        {
            playerColliders[index].enabled = true;
            playerColliders[index].center = new Vector3(x, y, z);
            index++;
        }

        return;
    }

    void OnTriggerEnter(Collider other)
    {
        colliderTargets.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        colliderTargets.Remove(other);
    }
}
