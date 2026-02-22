using UnityEngine;

public class ComponentLeg : MonoBehaviour
{
    public string legName;

    [HideInInspector] public Node node = null;
    [HideInInspector] public bool isSnapped = false;
    [HideInInspector] public GridRegion snappedRegion = null;

    public void Clear()
    {
        node = null;
        snappedRegion = null;
        isSnapped = false;
    }
}
