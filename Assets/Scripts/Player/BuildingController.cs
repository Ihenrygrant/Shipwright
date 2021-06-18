using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{

    public float AtlasElementSize = 0.333f;
    public float AtlasOffsetRow = 0.333f;
    public float AtlasOffsetCol = 0.333f;

    private bool isPlacingObject = false;
    private GameObject PlayerObjectMarker;
    private GameObject PlayerObject;

    private Vector3 rotationOffset = new Vector3(0, 0, 0);
    private ChunkGameObject chunkSelection;

    void Awake()
    {
        PlayerInfo.blockSelection = 0;
    }

    public void Update()
    {
        if (isPlacingObject)
        {
            UpdateObject();
        }
    }

    public void PlaceBlock()
    {
        if (isPlacingObject)
        {
            return;
        }


        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            chunkSelection = hit.collider.transform.GetComponent<ChunkGameObject>();
            Vector3 positionSelection = getBlockIndex(hit);

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                return;
            }

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<int> foundVertices = new List<int>();
            foundVertices.Add(triangles[hit.triangleIndex * 3 + 0]);
            foundVertices.Add(triangles[hit.triangleIndex * 3 + 1]);
            foundVertices.Add(triangles[hit.triangleIndex * 3 + 2]);

            //Debug.Log(" position: " + positionSelection + ", triIndex: " + hit.triangleIndex + ", firstTri: " + firstTri + "  " + (int)positionSelection.z  + "  " + positionSelection.z);

            Vector3 chunkoffset = new Vector3(0, 0, 0);

            //min
            if ((int)positionSelection.x == 0 && hit.normal.x == -1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[0] = (ushort)(newChunkSelection[0] - 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.x = ChunkSettings.xSize;
            }
            else if ((int)positionSelection.y == 0 && hit.normal.y == -1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[1] = (ushort)(newChunkSelection[1] - 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.y = ChunkSettings.ySize;
            }
            else if ((int)positionSelection.z == 0 && hit.normal.z == -1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[2] = (ushort)(newChunkSelection[2] - 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.z = ChunkSettings.zSize;
            }

            //max
            if ((int)positionSelection.x >= ChunkSettings.xSize - 1 && hit.normal.x == 1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[0] = (ushort)(newChunkSelection[0] + 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.x = -1.0f;
            }
            else if ((int)positionSelection.y >= ChunkSettings.ySize - 1 && hit.normal.y == 1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[1] = (ushort)(newChunkSelection[1] + 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.y = -1.0f;
            }
            else if ((int)positionSelection.z >= ChunkSettings.zSize - 1 && hit.normal.z == 1)
            {
                ushort[] newChunkSelection = (ushort[])chunkSelection.chunkInfo.index.Clone();
                newChunkSelection[2] = (ushort)(newChunkSelection[2] + 1);
                chunkSelection = transform.parent.GetComponent<ShipChunkHandler>().chunks[newChunkSelection[0], newChunkSelection[1], newChunkSelection[2]].GetComponent<ChunkGameObject>();
                positionSelection.z = -1.0f;
            }

            chunkSelection.SetBlock((int)positionSelection.x + (int)hit.normal.x, (int)positionSelection.y + (int)hit.normal.y, (int)positionSelection.z + (int)hit.normal.z, (byte)PlayerInfo.blockSelection, BlockStatus.Occupied_Block);
        }
        Debug.DrawRay(ray.origin, Camera.main.transform.forward * 10f, Color.red);
    }

    public void RemoveBlock()
    {
        if (isPlacingObject)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            chunkSelection = hit.collider.transform.GetComponent<ChunkGameObject>();
            Vector3 positionSelection = getBlockIndex(hit);


            MeshCollider meshCollider = hit.collider.transform.GetComponent<MeshCollider>();
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                return;
            }

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            //Debug.Log(triangles.Length + "  " + hit.triangleIndex*3);

            //int offset = getBlockTriIndexOffset(positionSelection, hit.normal);
            //int firstTri = (hit.triangleIndex - (hit.triangleIndex % 2)) - offset;
            //Debug.Log(" position: " + positionSelection + ", triIndex: " + hit.triangleIndex + ", firstTri: " + firstTri + "  " + (int)positionSelection.z  + "  " + positionSelection.z + "  " + chunkSelection.blockTriIndex[(int)positionSelection.x, (int)positionSelection.y, (int)positionSelection.z]);
            // Debug.Log((int)positionSelection.x + "  " + (int)positionSelection.y + "  " + (int)positionSelection.z);

            chunkSelection.SetBlock((int)positionSelection.x, (int)positionSelection.y, (int)positionSelection.z, 0, BlockStatus.Empty);

        }
        Debug.DrawRay(ray.origin, Camera.main.transform.forward * 10f, Color.red);
    }

    public void StartPlaceObject(GameObject item)
    {
        PlayerObjectMarker = Instantiate(item);
        PlayerObject = item;

        //PlayerObjectMarker.GetComponent<Renderer>().material.color = Color.green;
        foreach (Renderer render in PlayerObjectMarker.GetComponentsInChildren<Renderer>())
        {
            render.material.color = Color.green;
        }

        isPlacingObject = true;
    }

    void UpdateObject()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            chunkSelection = hit.collider.transform.GetComponent<ChunkGameObject>();

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                return;
            }

            PlayerObjectMarker.SetActive(true);
            PlayerObjectMarker.transform.position = hit.point;

            if (Input.GetMouseButtonDown(1))
            {
                PlaceObject();
            }
        }
        else
        {
            PlayerObjectMarker.SetActive(false);
        }
    }

    void PlaceObject()
    {
        Debug.Log(PlayerObject);

        foreach (Renderer render in PlayerObjectMarker.GetComponentsInChildren<Renderer>())
        {
            render.material.color = Color.white;
        }


        Instantiate(PlayerObject, PlayerObjectMarker.transform) ;
        //Destroy(PlayerObjectMarker);

        isPlacingObject = false;
    }

    private void blockDetection()
    {

    }

    Vector3 getBlockIndex(RaycastHit hit)
    {
        Vector3 positionSelection = (hit.point - hit.collider.transform.position);
        if (hit.normal.x != 0)
        {
            if (hit.normal.x == -1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x + 0.51f);
                positionSelection.y = Mathf.Floor(positionSelection.y + 0.51f);
                positionSelection.z = Mathf.Floor(positionSelection.z + 0.51f);
            }
            else if (hit.normal.x == 1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x);
                positionSelection.y = Mathf.Floor(positionSelection.y + 0.51f);
                positionSelection.z = Mathf.Floor(positionSelection.z + 0.51f);
            }

        }

        if (hit.normal.y != 0)
        {
            if (hit.normal.y == -1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x + 0.51f);
                positionSelection.y = Mathf.Floor(positionSelection.y + 0.51f);
                positionSelection.z = Mathf.Floor(positionSelection.z + 0.51f);
            }
            else if (hit.normal.y == 1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x + 0.51f);
                positionSelection.y = Mathf.Floor(positionSelection.y);
                positionSelection.z = Mathf.Floor(positionSelection.z + 0.51f);
            }

        }

        if (hit.normal.z != 0)
        {
            if (hit.normal.z == -1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x + 0.51f);
                positionSelection.y = Mathf.Floor(positionSelection.y + 0.51f);
                positionSelection.z = Mathf.Floor(positionSelection.z + 0.51f);
            }
            else if (hit.normal.z == 1)
            {
                positionSelection.x = Mathf.Floor(positionSelection.x + 0.51f);
                positionSelection.y = Mathf.Floor(positionSelection.y + 0.51f);
                positionSelection.z = Mathf.Floor(positionSelection.z);
            }

        }
        return positionSelection;
    }

}
