using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnappableComponent : MonoBehaviour
{
    [Header("References")]
    public BreadboardManager breadboardManager;
    public SnapVisualizer snapVisualizer;

    [Header("Legs")]
    public ComponentLeg[] legs;

    public bool isPlaced = false;
    private XRGrabInteractable xrGrab;


    private void Awake()
    {
        xrGrab = GetComponent<XRGrabInteractable>();

        xrGrab.selectEntered.AddListener(OnGrabbed);
        xrGrab.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isPlaced)
        {
            isPlaced = false;
            xrGrab.enabled = true;

            foreach(ComponentLeg leg in legs)
            {
                leg.ClearSnap();
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        snapVisualizer.Hide();

        StartCoroutine(TrySnapNextFrame());
    }


    private IEnumerator TrySnapNextFrame()
    {

        yield return null;

        if (TrySnapAllLegs())
        {
            isPlaced = true;
            xrGrab.enabled = false;
        }
    }



    public bool TrySnapAllLegs()
    {
        Vector3[] pendingSnapPoints = new Vector3[legs.Length];
        GridRegion[] pendingRegions = new GridRegion[legs.Length];


        //checking and storing the leg data, if even one of them can't snap return false
        for (int i = 0;  i < legs.Length; i++)
        {
            if (!breadboardManager.TryGetSnapPoint(legs[i].transform.position, out pendingSnapPoints[i], out pendingRegions[i]))
            {
                return false;
            }
        }

        Vector3 midPoint = (legs[0].transform.position + legs[1].transform.position) / 2f;
        transform.position = midPoint;
        transform.localRotation = Quaternion.identity;


        for(int i = 0;i < legs.Length; i++)
        {
            legs[i].transform.position = pendingSnapPoints[i];
            legs[i].isSnapped = true;
            legs[i].snappedRegion = pendingRegions[i];
        }


        return true;
    }


    private void Update()
    {
        if (isPlaced) return;
        if (!xrGrab.isSelected) return;
        

        if(breadboardManager.TryGetSnapPoint(transform.position, out Vector3 snapPoint, out _))
        {
            snapVisualizer.Show(snapPoint);
        }
        else
        {
            snapVisualizer.Hide();
        }
    }


}
