using System.Globalization;
using System.IO;
using UnityEngine;

public class Evaluation : MonoBehaviour
{
    [Header("Evaluated model")]
    [SerializeField] private int chosenLevel = 11;

    // Indeksy 0–9 dla dziesiêciu agentów
    [SerializeField] private int agentIndex = 0;

    [Header("Evaluation settings")]
    [SerializeField] private int numberOfGames = 1000;

    [Header("CSV settings")]
    [SerializeField]
    private string fileName = "baseline_evaluation_results.csv";

    private int completedGames;
    private int wins;
    private int losses;

    private bool evaluationFinished;

    public int ChosenLevel => chosenLevel;
    public int AgentIndex => agentIndex;

    private void Start()
    {
        completedGames = 0;
        wins = 0;
        losses = 0;
        evaluationFinished = false;

        Debug.Log(
            $"Evaluation started: agent {agentIndex}, " +
            $"level {chosenLevel}, games {numberOfGames}"
        );
    }

    public int levelEv()
    {
        return chosenLevel;
    }
    public void RegisterGameResult(bool agentWon)
    {
        if (evaluationFinished)
            return;

        completedGames++;

        if (agentWon)
            wins++;
        else
            losses++;

        Debug.Log(
            $"Game {completedGames}/{numberOfGames} finished. " +
            $"Agent won: {agentWon}. " +
            $"Wins: {wins}, losses: {losses}"
        );

        if (completedGames >= numberOfGames)
        {
            FinishEvaluation();
        }
    }

    private void FinishEvaluation()
    {
        evaluationFinished = true;

        float winRate = completedGames > 0
            ? (float)wins / completedGames * 100f
            : 0f;

        Debug.Log(
            $"Evaluation finished.\n" +
            $"Agent index: {agentIndex}\n" +
            $"Level: {chosenLevel}\n" +
            $"Games: {completedGames}\n" +
            $"Wins: {wins}\n" +
            $"Losses: {losses}\n" +
            $"Win rate: {winRate:F2}%"
        );

        bool resultSaved = SaveResultToCsv();

        if (!resultSaved)
        {
            Debug.LogError(
                "The evaluation result could not be saved. " +
                "The application will not be closed."
            );

            return;
        }

        Debug.Log("Evaluation finished and saved. Closing application.");

        QuitApplication();
    }

    private bool SaveResultToCsv()
    {
        string filePath = Path.Combine(
            Application.persistentDataPath,
            fileName
        );

        try
        {
            bool addHeader =
                !File.Exists(filePath) ||
                new FileInfo(filePath).Length == 0;

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if (addHeader)
                {
                    writer.WriteLine(
                        "AgentIndex,Level,Games,Wins,Losses,WinRate"
                    );
                }

                float winRate = completedGames > 0
                    ? (float)wins / completedGames * 100f
                    : 0f;

                string winRateText = winRate.ToString(
                    "F2",
                    CultureInfo.InvariantCulture
                );

                writer.WriteLine(
                    $"{agentIndex}," +
                    $"{chosenLevel}," +
                    $"{completedGames}," +
                    $"{wins}," +
                    $"{losses}," +
                    $"{winRateText}"
                );
            }

            Debug.Log($"Results saved to: {filePath}");
            return true;
        }
        catch (IOException exception)
        {
            Debug.LogError($"CSV saving error: {exception.Message}");
            return false;
        }
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}