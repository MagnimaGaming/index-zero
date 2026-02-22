using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapVisualizer : MonoBehaviour
{
    private MeshRenderer mr;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        Hide();
    }

    public void Hide()
    {
        mr.enabled = false;
    }

    public void Show(Vector3 snapPointPos)
    {
        mr.enabled = true;
        transform.position = snapPointPos;
    }
}
