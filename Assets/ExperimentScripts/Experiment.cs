using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


abstract public class Experiment<T>: MonoBehaviour where T : Trial, new()
{
    [Header("Experiment setup")]
    [SerializeField]
    [Tooltip("Trial list.")]
    public TrialList TrialList = new TrialList(typeof(T));

    [SerializeField]
    [Tooltip("Current trial.")]
    private T CurrentTrial = null;

    [SerializeField]
    [Tooltip("Results file.")]
    public ResultsFile ResultsFile = new ResultsFile();

    [SerializeField]
    [Tooltip("Response key setup.")]
    public ResponseManager Response;

    private bool initialized = false;
    private bool destroyed = false;

    public Experiment()
    {
        Response = new ResponseManager(TrialList);
    }

    void Start()
    {
        StartCoroutine(MainCoroutine());
    }

    void Update()
    {
        Response.Check();
        UpdateExperiment();
    }

    void OnDestroy()
    {
        if (initialized && !destroyed)
        {
            DestroyExperiment();
            ResultsFile.Close();
        }
    }

    private IEnumerator MainCoroutine()
    {
        TrialList.Read();

        Debug.Log("Initializing experiment...\n");
        initialized = true;
        destroyed = false;

        yield return StartCoroutine(InitializeExperiment());
        yield return Response.WaitAccept();
        if (!TrialList.Aborted)
        {
            Debug.Log("Starting experiment...\n");
            yield return StartExperiment();
        }

        while (TrialList.NextTrial())
        {
            Debug.Log("Running trial " + TrialList.TrialIndex + "...\n");
            CurrentTrial = (T)TrialList.CurrentTrial;
            yield return RunTrial(CurrentTrial);
            ResultsFile.Save(CurrentTrial);
        }
        CurrentTrial = null;

        Debug.Log("Concluding experiment...\n");
        yield return StartCoroutine(ConcludeExperiment());
        DestroyExperiment();
        ResultsFile.Close();
        destroyed = true;
    }

    virtual protected IEnumerator InitializeExperiment()
    {
        yield return null;
    }

    virtual protected void DestroyExperiment()
    {
    }

    virtual protected IEnumerator StartExperiment()
    {
        yield return null;
    }

    virtual protected IEnumerator ConcludeExperiment()
    {
        yield return null;
    }

    virtual protected IEnumerator RunTrial(T currentTrial)
    {
        yield return null;
    }

    virtual protected void UpdateExperiment()
    {
    }
}
