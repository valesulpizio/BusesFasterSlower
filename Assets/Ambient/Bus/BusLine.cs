using UnityEngine;

public class BusLine : MonoBehaviour
{
    [Tooltip("Number of Buses to create.")]
    public int NumberOfBuses = 15;

    [Tooltip("Distance between Buses.")]
    public float DistanceBetweenBuses = 15.0f;

    private bool created = false;
    private GameObject[] createdBuses;

    void Start()
    {
        if (gameObject.transform.childCount != 1)
        {
            Debug.Log("The Bus line object must have one and only one child: the bus to be replicated.\n");
            return;
        }
        if (NumberOfBuses <= 0)
        {
            Debug.Log("Number Of Buses is " + NumberOfBuses + ": no Bus will be created.\n");
            return;
        }

        Transform BusTransform = gameObject.transform.GetChild(0);
        createdBuses = new GameObject[NumberOfBuses - 1];
        BusTransform.localPosition = Vector3.zero;
        for (int i = 0; i < NumberOfBuses - 1; i++)
        {
            GameObject newBus = Object.Instantiate(BusTransform.gameObject, gameObject.transform, false);
            newBus.name = BusTransform.gameObject.name + " " + (i + 2);
            newBus.transform.localPosition = new Vector3(0, 0, - DistanceBetweenBuses * (i + 1));
            createdBuses[i] = newBus;
        }
        created = true;
    }

    void OnDestroy()
    {
        if (created)
        {
            for (int i = 0; i < createdBuses.Length; i++)
            {
                Object.Destroy(createdBuses[i]);
            }
        }
    }
}
