using System.Collections;
using UnityEngine;

public class Buses : Experiment<Buses_trial>
{
    // User-settable values
    [Header("Bus experiment setup")]

    [Tooltip("Minimum interval between two consecutive trials.")]
    public float MinInterTrialTime = 0.5f;

    [Tooltip("Maximum interval between two consecutive trials.")]
    public float MaxInterTrialTime = 5f; //5f

    [Tooltip("Position of the camera at the beginning of the trial.")]
    public float InitialX_CameraPosition = 0f;

    [Tooltip("Position of the first bus on the road at the beginning of the trial.")]
    public float InitialX_BusPosition = 355f;

    // References to game objects
    private GameObject BusLine;
    private Rigidbody BusLineRigidBody;
    private GameObject cameraObject;
    private Rigidbody cameraRigidBody;
    private GameObject FixationCross;
    //private GameObject instructionPanel;
    private GameObject startInstructions;
    private GameObject restInstructions;

    override protected IEnumerator InitializeExperiment()
    {
        // Get reference to bus
        BusLine = GameObject.Find("BusLine");
        BusLineRigidBody = BusLine.GetComponent<Rigidbody>();

        // Get reference to camera
        cameraObject = GameObject.Find("Camera");
        cameraRigidBody = cameraObject.GetComponent<Rigidbody>();

        // Get reference to fixation cross
        FixationCross = GameObject.Find("FixationCross");
        FixationCross.SetActive(true);

        // Get reference to panels
        //instructionPanel = GameObject.Find("InstructionsPanel");
        startInstructions = GameObject.Find("StartInstructions");
        restInstructions = GameObject.Find("RestInstructions");

        // Initialize visibility
        BusLine.SetActive(false);
        startInstructions.SetActive(true);
        restInstructions.SetActive(false);

        yield return null;
    }


    override protected void DestroyExperiment()
    {
        BusLine.SetActive(true);
    }


    override protected IEnumerator StartExperiment()
    {
        startInstructions.SetActive(false);
        restInstructions.SetActive(false);
        yield return null;
    }


    override protected IEnumerator ConcludeExperiment()
    {
        yield return null;
    }


    override protected IEnumerator RunTrial(Buses_trial currentTrial)
    {
        // FIRST TRIAL PERIOD

        // Reset camera position
        cameraObject.transform.position = new Vector3(InitialX_CameraPosition, cameraObject.transform.position.y, 0);

        BusLine.transform.position = new Vector3(InitialX_BusPosition, 0, 25);
        //BusLine.transform.position = new Vector3(BusLine.transform.position.x, 0, 25);

        // Set Structure        
        BusLine.SetActive(true);
        FixationCross.SetActive(true);

        // Set camera and bus speed
        cameraRigidBody.velocity = new Vector3(currentTrial.SM_velocity * 0.277778f, 0, 0);
        BusLineRigidBody.velocity = new Vector3(currentTrial.OM_velocity * 0.277778f,0, 0);

        // Save current time
        currentTrial.StartTime = Time.time;

        // Wait for TrialDuration seconds
        yield return new WaitForSeconds(currentTrial.duration);

        // END OF TRIAL

        // Stop camera and bus, hide bus
        BusLineRigidBody.velocity = new Vector3(0, 0, 0);
        cameraRigidBody.velocity = new Vector3(0, 0, 0);
        BusLine.SetActive(true);
       
        // Save current time
        currentTrial.EndTime = Time.time;

        // Wait a minimum intertrial time
        yield return new WaitForSeconds(MinInterTrialTime);

        // Wait for response within a timeout
        yield return Response.Wait(MaxInterTrialTime - MinInterTrialTime);

        // Wait for the subject to release response keys
        yield return Response.WaitRelease();

        // Check if valid response
        if (currentTrial.ResponseTime <= currentTrial.ChangeTime)
            currentTrial.Valid = false;

        // Show rest instructions if specified
        else if (currentTrial.Break > 0)
        {
            //instructionPanel.SetActive(true);
            restInstructions.SetActive(true);
            yield return Response.WaitAccept();
            //instructionPanel.SetActive(false);
            restInstructions.SetActive(false);

        }

    }
    override protected void UpdateExperiment()
    {
    }
}