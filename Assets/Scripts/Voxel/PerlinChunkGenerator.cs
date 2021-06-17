using UnityEngine;

public class PerlinChunkGenerator : MonoBehaviour
{

    public int seed;

    private Chunk chunk;

    void Start()
    {
        //chunk = GetComponent<Chunk>();
        //Generate(chunk, seed);
    }

    private void Generate(Chunk chunk, int seed)
    {
        Random.InitState(seed);
        byte[,,] blocks = new byte[ChunkSettings.xSize, ChunkSettings.ySize, ChunkSettings.zSize];

        for (var z = 0; z < ChunkSettings.xSize; ++z)
        {
            for (var y = 0; y < ChunkSettings.ySize; ++y)
            {
                for (var x = 0; x < ChunkSettings.zSize; ++x)
                {
                    //// TODO: This needs work.
                    //var gen = (
                    //    Mathf.PerlinNoise(x / (float)chunk.width, y / (float)chunk.width) +
                    //    Mathf.PerlinNoise(x / (float)chunk.width, z / (float)chunk.width) +
                    //    Mathf.PerlinNoise(z / (float)chunk.width, y / (float)chunk.width)
                    //    ) / 3.0f;


                    var gen = Random.Range(0.0f, 1.0f);
                    var filled = false;
                    if (gen > 0.01)
                    {
                        filled = true;
                    }
                    blocks[x, y, z] = (byte)BlockStatus.Occupied_Block;


                }
            }
        }
        chunk.FillBlocks(blocks);
    }
}