using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ChunkBinarySaver : MonoBehaviour {

    const string folderName = "BinaryChunkData";
    const string fileExtension = ".dat";

    public void SaveChunks(List<ChunkData> data, string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, data);
        }
    }

    public List<ChunkData> LoadChunks(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (List<ChunkData>) binaryFormatter.Deserialize(fileStream);
        }
    }

    public string[] GetFilePaths()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);

        return Directory.GetFiles(folderPath, fileExtension);
    }
}
