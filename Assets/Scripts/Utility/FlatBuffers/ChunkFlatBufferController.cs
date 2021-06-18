using UnityEngine;
using FlatBuffers;
using ChunkFlatBufferStructures;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveThreadJob{
    public List<ChunkData> chunks;
    public string shipName;
}

public class ChunkFlatBufferController
{
    public bool _threadRunning;
    public Thread _thread;
    ShipChunkHandler handler;

    List<SaveThreadJob> saveJobs;

    public void Init(ShipChunkHandler handler)
    {
        saveJobs = new List<SaveThreadJob>();
        _thread = new Thread(SaveThreadedWork);
        this.handler = handler;
    }

    public void SetData(List<ChunkData> chunkInfos, string filename)
    {
        SaveThreadJob threadJob = new SaveThreadJob();
        threadJob.chunks = chunkInfos;
        threadJob.shipName = filename;
        saveJobs.Add(threadJob);
    }


    public void SaveThreadedWork()
    {
        bool workDone = false;
        _threadRunning = true;



        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
            for (int i = 0; i < saveJobs.Count; i++)
            {
                SaveShip(saveJobs[i].chunks, saveJobs[i].shipName);
                Thread.Sleep(5);
            }

            workDone = true;
        }
        handler.NotifyThreadFinished();

        _threadRunning = false;
    }

    public bool SaveShip(List<ChunkData> chunkInfos, string filename)
    {

        ChunkFlatBufferController chunkFlatBufferController = new ChunkFlatBufferController();
        byte[] fbbChunks = chunkFlatBufferController.PackChunk(chunkInfos);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(filename)) File.Delete(filename);
        using (FileStream fileStream = File.Open(filename, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, fbbChunks);
        }

        return true;
    }

    public byte[] PackChunk(List<ChunkData> chunks)
    {
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);

        var count_ChunkBlocks = ChunkSettings.xSize * ChunkSettings.ySize * ChunkSettings.zSize;

        #region ChunkBlockInfo
        VectorOffset[] offset_ChunkBlocksCollection = new VectorOffset[chunks.Count];

        for (int i_chunks = 0; i_chunks < chunks.Count; i_chunks++)
        {
            Offset<ChunkBlockInfo>[] offset_ChunkBlocks = new Offset<ChunkBlockInfo>[count_ChunkBlocks];

            for (int i_blocks = 0; i_blocks < chunks[i_chunks].blocks.Length; i_blocks++)
            {
                int z = i_blocks % ChunkSettings.zSize;
                int y = (i_blocks / ChunkSettings.zSize) % ChunkSettings.ySize;
                int x = i_blocks / (ChunkSettings.ySize * ChunkSettings.zSize);

                Block blockData = chunks[i_chunks].blocks[z, y, x];
                var bitPack = (blockData.Status << 5) | blockData.Orientation;

                var offset_block = ChunkBlockInfo.CreateChunkBlockInfo(fbb, (sbyte)blockData.Type, (sbyte)blockData.Health, (sbyte)bitPack);
                offset_ChunkBlocks[i_blocks] = offset_block;

            }
            offset_ChunkBlocksCollection[i_chunks] = ChunkInfoBuffer.CreateBlocksVector(fbb, offset_ChunkBlocks);
        }
        #endregion


        #region ChunkInfoBuffer
        Offset<ChunkInfoBuffer>[] offset_ChunkInfoBuffers = new Offset<ChunkInfoBuffer>[chunks.Count];
        for (int i_chunks = 0; i_chunks < chunks.Count; i_chunks++)
        {
            VectorOffset chunkIndexOffset = ChunkInfoBuffer.CreateChunkIndexVector(fbb, chunks[i_chunks].index);
            offset_ChunkInfoBuffers[i_chunks] = ChunkInfoBuffer.CreateChunkInfoBuffer(fbb, chunkIndexOffset, offset_ChunkBlocksCollection[i_chunks]);
        }
        #endregion

        var chunksVector = ChunkBuffer.CreateChunksVector(fbb, offset_ChunkInfoBuffers);

        var chunksOffset = ChunkBuffer.CreateChunkBuffer(fbb, chunksVector);

        fbb.Finish(chunksOffset.Value);
        Debug.Log(fbb.DataBuffer.ToSizedArray().Length);
        return fbb.DataBuffer.ToSizedArray();
    }

    public List<ChunkData> UnpackChunk(byte[] DataBuffer)
    {
        List<ChunkData> chunks = new List<ChunkData>();

        ByteBuffer bb = new ByteBuffer(DataBuffer);
        var data = ChunkFlatBufferStructures.ChunkBuffer.GetRootAsChunkBuffer(bb);



        for (int i = 0; i < data.ChunksLength; i++)
        {
            ChunkInfoBuffer? chunkInfo = data.Chunks(i);

            ChunkData unpackedInfo = new ChunkData();
            unpackedInfo.index = new ushort[3];
            unpackedInfo.blocks = new Block[ChunkSettings.xSize, ChunkSettings.ySize, ChunkSettings.zSize];

            unpackedInfo.index[0] = chunkInfo.Value.ChunkIndex(0);
            unpackedInfo.index[1] = chunkInfo.Value.ChunkIndex(1);
            unpackedInfo.index[2] = chunkInfo.Value.ChunkIndex(2);

            // Debug.Log(unpackedInfo.chunkIndex[0] + "  " + unpackedInfo.chunkIndex[1] + "  " + unpackedInfo.chunkIndex[2]);

            for (int z = 0; z < ChunkSettings.zSize; z++)
            {
                for (int y = 0; y < ChunkSettings.ySize; y++)
                {
                    for (int x = 0; x < ChunkSettings.xSize; x++)
                    {
                        unpackedInfo.blocks[x, y, z] = new Block();
                        var blockData = unpackedInfo.blocks[x, y, z];
                        var index = (z * ChunkSettings.xSize * ChunkSettings.ySize) + (y * ChunkSettings.xSize) + x;

                        blockData.Type = (byte)chunkInfo.Value.Blocks(index).Value.BlockTypes;
                        blockData.Health = (byte)chunkInfo.Value.Blocks(index).Value.BlockHealth;

                        byte bitpack = (byte)chunkInfo.Value.Blocks(index).Value.BitPackStatus3Orientations5;
                        blockData.Status = (byte)(bitpack >> 5);
                        blockData.Orientation = (byte)(bitpack & 31);
                    }
                }
            }
            chunks.Add(unpackedInfo);
        }


        return chunks;
    }
}
