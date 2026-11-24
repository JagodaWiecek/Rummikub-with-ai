using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BoardSensor : ISensor
{
    private System.Func<Dictionary<Vector3Int, Tile>> boardProvider;
    private int minX, minZ, width, height;
    private int numChannels = 7;
    private string sensorName;
    private ObservationSpec observationSpec;

    public BoardSensor(string name, int height, int width, int minX, int minZ, System.Func<Dictionary<Vector3Int, Tile>> provider)
    {
        this.sensorName = name;
        this.width = width;
        this.height = height;
        this.minX = minX;
        this.minZ = minZ;
        this.boardProvider = provider;
        this.observationSpec = ObservationSpec.Visual(numChannels, height, width);
    }

    public int Write(ObservationWriter writer)
    {
        var boardReference = boardProvider?.Invoke();
        var spec = GetObservationSpec();
        int specHeight = spec.Shape[1]; // Pobiera wysokoœæ ze specyfikacji (10)
        int specWidth = spec.Shape[2];
        for (int h = 0; h < specHeight; h++) // h = 0 to 9
        {
            for (int w = 0; w < specWidth; w++) // w = 0 to 23
            {
                Vector3Int gamePos = new Vector3Int(w + minX, 0, h + minZ);


                if (boardReference != null && boardReference.ContainsKey(gamePos))
                {
                    var tile = boardReference[gamePos];

                    // KOLEJNOŒÆ MA ZNACZENIE: writer[wysokoœæ, szerokoœæ, kana³]
                    // h musi byæ tam, gdzie rozmiar jest 10, w tam gdzie 24.
                    writer[0, h, w] = tile.GetNumber() / 30f;

                    int colorId = GetColorIndex(tile.GetColor());
                    for (int i = 0; i < 5; i++)
                    {
                        writer[i+1, h, w] = (colorId == i) ? 1f : 0f;
                    }
                    writer[6, h, w] = 1f;
                }
                else
                {
                    for (int i = 0; i < numChannels; i++)
                    {
                        writer[i, h, w] = 0f;
                    }
                }
            }
        }
        Debug.Log("Sensor planszy pracuje...");
        return numChannels * width * height;
    }

    public ObservationSpec GetObservationSpec() => observationSpec;
    public string GetName() => sensorName;
    public void Update() { }
    public void Reset() { }
    public byte[] GetCompressedObservation() => null;
    public CompressionSpec GetCompressionSpec() => CompressionSpec.Default();

    private int GetColorIndex(UnityEngine.Color c)
    {
        
        if (c == Color.red) return 0;
        if (c == Color.blue) return 1;
        if (c == new Color(1f, 0.50f, 0f)) return 2;
        if (c == Color.black) return 3;
        if (c == Color.magenta || c == new Color(0.5f, 0f, 0.5f)) return 4;
        return -1;
    }
}