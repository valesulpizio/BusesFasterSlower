using System.Collections.Generic;
using System.IO;
using UnityEngine;

// This class manages saving a series of trials into a CSV file.
// Usage:
//     ResultsFile f = new ResultsFile();
//     f.Name = "Name";
//     f.Save(trial1);
//     f.Save(trial2);
//     f.Close();

[System.Serializable]
public class ResultsFile
{
    [Tooltip("Name of the results file.")]
    public string Name = "Results";

    private StreamWriter streamWriter = null;

    public void Save(Trial trial)
    {
        if (trial == null)
            return;
        if (streamWriter == null)
        {
            string fileName = Application.dataPath + "/Resources/Results Files/" + Name + ".csv";
            streamWriter = File.AppendText(fileName);
            SaveLine(trial.ResultsHeader);
        }
        SaveLine(trial.ResultsLine);
    }

    private void SaveLine(object[] line)
    {
        for (int i = 0; i < line.Length; i++)
        {
            streamWriter.Write(line[i]);
            if (i < line.Length - 1)
                streamWriter.Write("\t");
        }
        streamWriter.WriteLine();
    }

    public void Close()
    {
        if (streamWriter != null)
        {
            Debug.Log("Saving results...\n");
            streamWriter.WriteLine("");
            streamWriter.Close();
            streamWriter = null;
        }
    }
}