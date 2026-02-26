using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlugSnappable : MonoBehaviour
{
    public BreadboardManager breadboardManager;
    public SnapVisualizer snapVisualizer;
    public ComponentLeg leg;
    public JumperWireComponent parentWire;

    public bool isSnapped = false;
    private XRGrabInteractable xrGrab;

    private void Awake()
    {
        xrGrab = GetComponent<XRGrabInteractable>();
        xrGrab.selectEntered.AddListener(OnGrabbed);
        xrGrab.selectExited.AddListener(OnReleased);

    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isSnapped)
        {
            isSnapped = false;
            leg.ClearSnap();
            parentWire.OnPlugUnsnapped();
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

        if(breadboardManager.TryGetSnapPoint(leg.transform.position, out Vector3 snapPoint, out GridRegion gridRegion))
        {
            leg.transform.position = snapPoint;
            leg.snappedRegion = gridRegion;
            leg.isSnapped = true;
            isSnapped = true;

            parentWire.OnPlugSnapped();
        }
    }

    private void Update()
    {
        if(breadboardManager.TryGetSnapPoint(leg.transform.position, out Vector3 snapPoint, out _))
        {
            snapVisualizer.Show(snapPoint);
        }
        else
        {
            snapVisualizer.Hide();
        }
    }
}
