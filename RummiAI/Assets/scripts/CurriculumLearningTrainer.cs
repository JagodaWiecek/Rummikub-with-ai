using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class CurriculumLearningTrainer : MonoBehaviour
{

    [Header("Path to Settings")]
    public string customFolderPath = "results/CR_IL/";
    public string folderName = "TrainingProgress";
    public float successThreshold;


    private int trainingIndex;
    private string fileName;
    private string fullPath;
    
    
    private int windowSize;
    private Queue<float> resultHistory = new Queue<float>();

    void Awake()
    {
        customFolderPath = Path.Combine(customFolderPath, folderName);
        if (!Directory.Exists(customFolderPath))
        {
            Directory.CreateDirectory(customFolderPath);
        }
        successThreshold = 0.9f;
        windowSize = 100;
    }

    public int LoadProgress(string behaviorName)
    {
        string fullPath = Path.Combine(customFolderPath, behaviorName + ".txt");

        if (File.Exists(fullPath))
        {
            string content = File.ReadAllText(fullPath);
            if (int.TryParse(content, out int savedStage))
            {
                Debug.Log($"<color=cyan>[Curriculum]</color> Wczytano etap {savedStage} dla modelu: {behaviorName}");
                return savedStage;
            }
        }

        Debug.Log($"<color=yellow>[Curriculum]</color> Brak zapisu dla {behaviorName}. Start od etapu 0.");
        return 0;
    }
    private void SaveProgress(string behaviorName, int currentStage)
    {
        string fullPath = Path.Combine(customFolderPath, behaviorName + ".txt");
        File.WriteAllText(fullPath, currentStage.ToString());
    }

    public void AddResult(float reward, string behaviorName, ref int trainingIndex)
    {
        // 1. Dodajemy wynik (zak³adamy, ¿e reward jest znormalizowany lub u¿ywamy Twojej definicji sukcesu)
        resultHistory.Enqueue(reward);

        // 2. Usuwamy najstarszy, jeœli okno jest pe³ne
        if (resultHistory.Count > windowSize)
        {
            resultHistory.Dequeue();
        }

        // 3. Sprawdzamy œredni¹ tylko gdy mamy komplet danych
        if (resultHistory.Count == windowSize)
        {
            float averageReward = resultHistory.Average();

            if (averageReward >= successThreshold)
            {
                trainingIndex++;
                resultHistory.Clear(); // Resetujemy okno dla nowego wyzwania

                SaveProgress(behaviorName, trainingIndex);

                Debug.Log($"<color=green><b>[AWANS]</b></color> Model {behaviorName} wskoczy³ na etap: {trainingIndex} (Œrednia: {averageReward:F2})");
            }
        }
    }

}
