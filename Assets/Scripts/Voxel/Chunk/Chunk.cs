using System;

public static class ChunkSettings
{
    public static ushort xSize = 16;
    public static ushort ySize = 16;
    public static ushort zSize = 16;
}

[Serializable]
public class ChunkData
{
    public ushort[] index;
    public Block[,,] blocks;
}

