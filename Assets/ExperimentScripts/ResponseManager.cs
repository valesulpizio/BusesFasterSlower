using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponseManager
{
    [Tooltip("Physical key or button to be used to start the experiment")]
    public KeyCode AcceptKey = KeyCode.Space;

    [Tooltip("Physical key or button to be used to abort the experiment")]
    public KeyCode AbortKey = KeyCode.Escape;

    [Tooltip("Physical key or button to be used as response button 1")]
    public KeyCode ResponseButton1 = KeyCode.Mouse0;

    [Tooltip("Physical key or button to be used as response button 2")]
    public KeyCode ResponseButton2 = KeyCode.Mouse1;

    [Tooltip("Physical key or button to be used as response button 3")]
    public KeyCode ResponseButton3 = KeyCode.A;

    [Tooltip("Physical key or button to be used as response button 4")]
    public KeyCode ResponseButton4 = KeyCode.L;

    [SerializeField]
    [Tooltip("Current status of the four response keys")]
    private bool[] responseButtonStatus = new bool[4];

    public bool[] Status { get => responseButtonStatus; }

    private readonly TrialList trialList;

    public ResponseManager(TrialList list)
    {
        trialList = list;
    }

    public void Check()
    {
        if (Input.GetKeyDown(AbortKey))
        {
            Debug.Log("Abort key pressed: will exit after current trial\n");
            trialList.Abort();
        }
        RecordResponse(0, ResponseButton1);
        RecordResponse(1, ResponseButton2);
        RecordResponse(2, ResponseButton3);
        RecordResponse(3, ResponseButton4);
    }

    private void RecordResponse(int buttonIndex, KeyCode buttonCode)
    {
        responseButtonStatus[buttonIndex] = Input.GetKey(buttonCode);
        if (trialList.CurrentTrial != null && Input.GetKeyDown(buttonCode))
        {
            if (trialList.CurrentTrial.Response > 0)
                trialList.CurrentTrial.Response = -1;
            else
                trialList.CurrentTrial.Response = buttonIndex + 1;
            trialList.CurrentTrial.ResponseTime = Time.time;
            Debug.Log("Pressed response button " + buttonIndex + 1 + "\n");
        }
    }

    public CustomYieldInstruction Wait(float maxTime = 100000.0f)
    {
        float startTime = Time.time;
        return new WaitWhile(() => MustContinueWaitingForResponse(startTime + maxTime));
    }

    private bool MustContinueWaitingForResponse(float untilTime)
    {
        if (trialList.Aborted || trialList.CurrentTrial == null)
            return false;
        if (trialList.CurrentTrial.Response > 0)
            return false;
        if (Time.time >= untilTime)
        {
            Debug.Log("Response timed out\n");
            return false;
        }
        return true;
    }

    public CustomYieldInstruction WaitAccept()
    {
        Debug.Log("Waiting for accept key...\n");
        return new WaitUntil(MustContinueWaitingForAccept);
    }

    private bool MustContinueWaitingForAccept()
    {
        if (Input.GetKeyDown(AcceptKey))
            Debug.Log("Accept key pressed\n");
        return Input.GetKeyDown(AcceptKey) || trialList.Aborted;
    }

    public CustomYieldInstruction WaitRelease()
    {
        if (IsAnyResponseKeyDown())
        {
            Debug.Log("Waiting for all keys released...\n");
            return new WaitWhile(IsAnyResponseKeyDown);
        }
        else
        {
            return null;
        }
    }

    private bool IsAnyResponseKeyDown()
    {
        return Input.GetKey(ResponseButton1)
        || Input.GetKey(ResponseButton2)
        || Input.GetKey(ResponseButton3)
        || Input.GetKey(ResponseButton4);
    }
}
