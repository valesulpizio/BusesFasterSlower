[System.Serializable]
public class Buses_trial : Trial
{
    public int Break;
    //public string trial_type;
    //public string trial_code;
    public float SM_velocity;
    public float OM_velocity;
    //public string Condition_name;
    //public float onset;
    public int duration;
    //public float ITI_duration;

    public float StartTime = 0;
    public float ChangeTime = 0;
    public float EndTime;

    // Define the header of the trial list file here
    private static readonly string[] trialListHeader = { "Break","SM_velocity", "OM_velocity", "duration"};
    override public string[] TrialListHeader => trialListHeader;

    // Define the header of the results file here
    private static readonly string[] resultsFileHeader = { "Break", "SM_velocity", "OM_velocity", "duration", "StartTime", "ChangeTime" };
    override protected string[] TrialSpecificResultsHeader => resultsFileHeader;

    // Read takes an array of strings read from a row of the trial list and
    // parses it into values of trial properties
    override public void Read(string[] values)
    {
        Break = int.Parse(values[0]);
        SM_velocity = float.Parse(values[1]);
        OM_velocity = float.Parse(values[2]);
        //onset = float.Parse(values[3]);
        duration = int.Parse(values[3]);
        //ITI_duration = float.Parse(values[4]);
    }

    // Save must returns an array of values to be saved to the results file, one
    // for each column of the results file as defined in ResultsFileHeader
    override protected object[] Save()
    {
        ResponseTime = Response != 0 ? (ResponseTime - ChangeTime) * 1000 : 0;
        return new object[] { SM_velocity, OM_velocity, duration, StartTime, ChangeTime };
    }
}