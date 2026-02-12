using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class ControllerHandsManager : MonoBehaviour
{
    public InputActionReference triggerActionRef;
    public InputActionReference gripActionRef;

    public Animator handAnimator;


    private void Awake()
    {
        handAnimator = GetComponent<Animator>();

        SetupInputActions();
    }

    private void SetupInputActions()
    {
        if(triggerActionRef != null && gripActionRef != null)
        {
            triggerActionRef.action.performed += ctx => UpdateAnimator("Trigger", ctx.ReadValue<float>());
            triggerActionRef.action.canceled += ctx => UpdateAnimator("Trigger", 0);

            gripActionRef.action.performed += ctx => UpdateAnimator("Grip", ctx.ReadValue<float>());
            gripActionRef.action.canceled += ctx => UpdateAnimator("Grip", 0);
        }
        else
        {
            Debug.LogWarning("TriggerRef and GripRef are not assigned in the inspector");
        }



    }


    private void UpdateAnimator(string parameter, float value)
    {
        if(handAnimator != null)
        {
            handAnimator.SetFloat(parameter, value);
        }
    }


    private void OnEnable()
    {
        triggerActionRef?.action.Enable();
        gripActionRef?.action.Enable();
    }

    private void OnDisable()
    {
        triggerActionRef?.action.Disable();
        gripActionRef?.action.Disable();
    }

}
