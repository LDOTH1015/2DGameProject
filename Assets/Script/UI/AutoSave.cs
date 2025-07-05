using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
public abstract class AutoSave<T> where T : new()
{
    private string filePath;

    public T Data { get; private set; }

    protected AutoSave(string fileName)
    {
        filePath = Path.Combine(Application.dataPath, $"Resources/JsonData/{fileName}.json");

        try
        {
            var json = Resources.Load($"JsonData/{fileName}") as TextAsset;
            Data = JsonConvert.DeserializeObject<T>(json.ToString());
            if (Data == null)
            {
                Data = new T();
            }
        }
        catch (Exception ex)
        {
            Data = new T();
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
        }
    }

    public void Modify(Action<T> modifyAction)
    {
        try
        {
            modifyAction(Data);
            Save();
        }
        catch (Exception ex)
        {
        }
    }
}
