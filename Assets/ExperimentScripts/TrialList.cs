using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

[System.Serializable]
abstract public class Trial
{
    public int Index;
    public float ResponseTime = 0;
    public int Response = 0;
    public bool Valid = true;

    private static readonly string[] resultsHeaderPrefix = { "Index" };
    private static readonly string[] resultsHeaderSuffix = { "Response", "ResponseTime", "Valid" };
    private static readonly string[] noString = { };

    public virtual string[] TrialListHeader => noString;

    public string[] ResultsHeader => resultsHeaderPrefix.Concat(TrialSpecificResultsHeader).Concat(resultsHeaderSuffix).ToArray();
    protected virtual string[] TrialSpecificResultsHeader => noString;

    public object[] ResultsLine
    {
        get
        {
            object[] linePrefix = { Index };
            object[] lineSuffix = { Response, ResponseTime, Valid };
            return linePrefix.Concat(Save()).Concat(lineSuffix).ToArray();
        }
    }

    abstract public void Read(string[] line);
    abstract protected object[] Save();
}


// This class manages a trial list: it reads the trial list from a CSV file,
// it iterates over the trial list and allows to abort the trial list.
// Usage:
//       TrialList<MyTrialType> trialList = new TrialList<MyTrialType>();
//       trialList.Name = "Name";
//       trialList.Read();
//       while (trialList.NextTrial()) {
//           // do something with the trial
//           trialList.TrialIndex;
//           trialList.CurrentTrial;
//       }

[System.Serializable]
public class TrialList<T> : TrialList
{
    public TrialList() : base(typeof(T))
    {
    }
}

[System.Serializable]
public class TrialList
{
    [Tooltip("Name of the trial list.")]
    public string Name = "TrialList";

    [Tooltip("Whether to repeat invalid trials.")]
    public bool RepeatInvalidTrials = false;

    [Tooltip("Whether the trial list has been aborted.")]
    public bool Aborted = false;

    [SerializeField]
    [Tooltip("Number of trials in the list.")]
    private int numberOfTrials = 0;
    public int NumberOfTrials { get => numberOfTrials; }

    [SerializeField]
    [Tooltip("Index of the current trial.")]
    private int trialIndex = 0;
    public int TrialIndex { get => trialIndex; }

    [SerializeField]
    [Tooltip("Explore properties of the current trial.")]
    private Trial currentTrial = null;
    public Trial CurrentTrial { get => currentTrial; }

    public readonly Type trialType;
    private readonly List<Trial> trialList = new List<Trial>();

    private static readonly string SPLIT_RE = @";|\t";
    private static readonly string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public TrialList(Type type)
    {
        trialType = type;
    }

    private Trial CreateTrial()
    {
        return (Trial)Activator.CreateInstance(trialType);
    }

    public void Read()
    {
        Debug.Log("Reading trial list...\n");
        TextAsset trialListFile = Resources.Load<TextAsset>("Trial Lists/" + Name);
        if (trialListFile == null)
        {
            Debug.Log("Trial list file not found\n");
            return;
        }

        string[] lines = Regex.Split(trialListFile.text, LINE_SPLIT_RE);
        if (lines.Length <= 1)
            return;

        string[] header = Regex.Split(lines[0], SPLIT_RE);
        string[] expectedHeader = CreateTrial().TrialListHeader;
        if (!Enumerable.SequenceEqual<string>(header, expectedHeader))
        {
            Debug.Log("The trial list has a wrong header: Please check it carefully.\n");
            return;
        }

        int index = 1;
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;
            if (values.Length != expectedHeader.Length)
            {
                Debug.Log("Invalid entry at line " + (i + 1) + " of the trial list: ignored.\n");
                continue;
            }

            Trial trial = CreateTrial();
            trial.Read(values);
            trial.Index = index++;
            trialList.Add(trial);
        }

        numberOfTrials = trialList.Count;
        trialIndex = 0;
        currentTrial = null;
        Aborted = false;
        Debug.Log("Successfully read " + numberOfTrials + " trials from trial list " + Name + "\n");
    }

    public void Abort()
    {
        Aborted = true;
    }

    public bool NextTrial()
    {
        // End current trial
        if (currentTrial != null)
        {
            if (currentTrial.Response <= 0)
                currentTrial.Valid = false;
            if (!currentTrial.Valid && RepeatInvalidTrials && !Aborted)
            {
                currentTrial.Valid = true;
                currentTrial.Response = 0;
                currentTrial.ResponseTime = 0;
                return true;
            }
        }

        // Switch to next trial, if any
        bool hasNextTrial = (!Aborted) && ++trialIndex <= numberOfTrials;
        currentTrial = hasNextTrial ? trialList[trialIndex - 1] : null;
        if (!hasNextTrial)
            trialIndex = 0;
        return hasNextTrial;
    }
}
