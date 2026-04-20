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
    public float successThreshold = 0.9f;
    public string fileName;

    private int trainingIndex;    
    private int windowSize;
    private Queue<float> resultHistory = new Queue<float>();

    void Awake()
    {   //tworzenie folderu do zapisu, od teraz customFolderPath zawiera pe³n¹ œcie¿kê do pliku, tylko brakuje imienia pliku
        customFolderPath = Path.Combine(customFolderPath, folderName);
        if (!Directory.Exists(customFolderPath))
        {
            Directory.CreateDirectory(customFolderPath);
        }
        //successThreshold = 0.9f;
        windowSize = 100;
      //  float r = 0.5f;
        //int trainingIndex = LoadProgress();
       // SaveProgress(fileName, trainingIndex);
        //AddResult(0.5f ,ref trainingIndex);
    }
    /// <summary>
    /// Funkcja do pobrania zawartoœci pliku, aka na którym poziomie nauczania cl agent jest
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>zwraca int, który jest ostatnim osi¹gniêtym poziomem, przy braku pliku, zwraca 0, nie powstaje plik jeœli jest poziom 0</returns>
    public int LoadProgress()
    {
        string fullPath = Path.Combine(customFolderPath, this.fileName + ".txt");

        if (File.Exists(fullPath))
        {
            string content = File.ReadAllText(fullPath);
            if (int.TryParse(content, out int savedStage))
            {
                Debug.Log($"<color=cyan>[Curriculum]</color> Wczytano etap {savedStage} dla modelu: {this.fileName}");
                return savedStage;
            }
        }

        Debug.Log($"<color=yellow>[Curriculum]</color> Brak zapisu dla {fileName}. Start od etapu 0.");
        return 0;
    }
    /// <summary>
    /// zapisuje poziom nauki gdy agent awansuje, jeœli agent by³ na poziomie 0 i przeszed³ na poziom 1 to dopiero plik jest tworzony
    /// </summary>
    /// <param name="fileName">nazwa pliku do którego bêdzie postêp zapisany</param>
    /// <param name="currentStage">wartoœæ poziomu, minimalnie to bêdzie 1</param>
    private void SaveProgress(string fileName, int currentStage)
    {
        string fullPath = Path.Combine(customFolderPath, fileName + ".txt");
        File.WriteAllText(fullPath, currentStage.ToString());
    }
    /// <summary>
    /// AddResult zarz¹dza zawartoœci¹ kolejki oraz inkrementacj¹ etapu nauki cl, 
    /// 
    /// </summary>
    /// <param name="reward"> wartoœæ nagrody/kary jak¹ dostaje agent</param>
    /// <param name="fileName">wybrana nazwa pliku do której jest zapisany indeks nauki w przypadku inkrementacji</param>
    /// <param name="trainingIndex">indeks nauki</param>
    public void AddResult(float reward, ref int trainingIndex)
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

                SaveProgress(this.fileName, trainingIndex);

                Debug.Log($"<color=green><b>[AWANS]</b></color> Model {this.fileName} wskoczy³ na etap: {trainingIndex} (Œrednia: {averageReward:F2})");
            }
        }
    }





}
