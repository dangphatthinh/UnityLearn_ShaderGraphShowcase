using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ObservingHandler : MonoBehaviour
{
    public enum InteractionState
    {
        Moving,
        Observing,
        Returning,
        Looking,
    }
    
    public UIHandler UIHandler;
    public float ObjectRotationSpeed = 5.0f;
    
    private FirstPersonController m_FpsController;
    private PlayerInput m_PlayerInput;
    private GameObject m_MainCamera;

    private InteractableObject m_HighlightedObject = null;
    private StarterAssetsInputs m_StarterAssetInput;
    private InputAction m_InteractAction;
    private InputAction m_ShakeAction;

    private Vector3 m_ObservedStartingPoint;
    private Quaternion m_ObservedStartingRotation;
    
    private Vector3 m_ObservedObjectTargetPosition;
    private Quaternion m_ObservedObjectTargetRotation;
    
    private InteractionState m_CurrentState = InteractionState.Moving;
    private const float ShakeTime = 0.5f;
    private bool m_IsShaking = false;
    private float m_ShakingTime = 0.0f;
    
    private void OnEnable()
    {
        m_FpsController = GetComponent<FirstPersonController>();
        m_PlayerInput = GetComponent<PlayerInput>();
        m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        m_InteractAction = m_PlayerInput.currentActionMap.FindAction("Interact");
        m_InteractAction.Enable();
        
        m_ShakeAction = m_PlayerInput.currentActionMap.FindAction("Shake");
        m_ShakeAction.Enable();
        
        m_StarterAssetInput = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentState == InteractionState.Moving)
        {
            // Addition for the shadergraph
            if (Physics.Raycast(m_MainCamera.transform.position, m_MainCamera.transform.forward, out var hit, 2.0f))
            {
                var observable = hit.collider.GetComponentInParent<InteractableObject>();
                if (observable != null)
                {
                    m_HighlightedObject?.HideSelection();
                    m_HighlightedObject = observable;
                    m_HighlightedObject.ShowSelection();
                    UIHandler.SetText($"Interact with {m_InteractAction.GetBindingDisplayString()}");
                }
                else
                {
                    m_HighlightedObject?.HideSelection();
                    m_HighlightedObject = null;
                    UIHandler.HideText();
                }
            }
            else
            {
                m_HighlightedObject?.HideSelection();
                m_HighlightedObject = null;
                UIHandler.HideText();
            }
            
            if (m_InteractAction.WasPressedThisFrame() && m_HighlightedObject != null)
            {
                m_HighlightedObject.Interact(this);
            }
        }
        else if(m_CurrentState == InteractionState.Observing)
        {
            m_HighlightedObject.transform.position = Vector3.MoveTowards(m_HighlightedObject.transform.position, m_ObservedObjectTargetPosition, Time.deltaTime * 2.0f);
            m_HighlightedObject.transform.Rotate(m_MainCamera.transform.up, m_StarterAssetInput.look.x * ObjectRotationSpeed);
            m_HighlightedObject.transform.Rotate(m_MainCamera.transform.right, m_StarterAssetInput.look.y * ObjectRotationSpeed);
            
            if (m_InteractAction.WasPressedThisFrame())
            {
                m_CurrentState = InteractionState.Returning;
                m_ObservedObjectTargetPosition = m_ObservedStartingPoint;
                m_ObservedObjectTargetRotation = m_ObservedStartingRotation;
            }
            else if(m_ShakeAction.WasPressedThisFrame())
            {
                m_ShakingTime = 0.0f;
                m_IsShaking = true;
            }
            
            if (m_IsShaking)
            {
                m_ShakingTime += Time.deltaTime;
                m_HighlightedObject.transform.position = m_ObservedObjectTargetPosition +
                                                         transform.right * (Mathf.PingPong(m_ShakingTime * 4.0f, 0.5f) - 0.25f);
                if(m_ShakingTime >= ShakeTime)
                    m_IsShaking = false;
            }
        }
        else if(m_CurrentState == InteractionState.Looking)
        {
            if (m_InteractAction.WasPressedThisFrame() && m_HighlightedObject != null)
            {
                m_HighlightedObject.Interact(this);
            }
        }
        else if (m_CurrentState == InteractionState.Returning)
        {
            m_HighlightedObject.transform.position = Vector3.MoveTowards(m_HighlightedObject.transform.position, m_ObservedObjectTargetPosition, Time.deltaTime * 2.0f);
            m_HighlightedObject.transform.rotation = Quaternion.RotateTowards(m_HighlightedObject.transform.rotation, m_ObservedObjectTargetRotation, Time.deltaTime * 360.0f);
            
            if (m_HighlightedObject.transform.position == m_ObservedObjectTargetPosition && m_HighlightedObject.transform.rotation == m_ObservedObjectTargetRotation)
            {
                m_CurrentState = InteractionState.Moving;
                m_FpsController.enabled = true;
            }
        }
    }

    public void ObserveObject(ObservableObject observableObject)
    {
        m_CurrentState = InteractionState.Observing;
            
        m_HighlightedObject.HideSelection();
        m_ObservedStartingPoint = m_HighlightedObject.transform.position;
        m_ObservedStartingRotation = m_HighlightedObject.transform.rotation;
        m_ObservedObjectTargetPosition = m_MainCamera.transform.position + m_MainCamera.transform.forward * 1.0f;
                
        UIHandler.SetText($"Press {m_InteractAction.GetBindingDisplayString()} to return");
            
        m_FpsController.enabled = false;
    }

    public void EquipGlasses(ScriptableRendererFeature feature)
    {
        feature?.SetActive(true);
        
        m_HighlightedObject.HideSelection();
        m_CurrentState = InteractionState.Looking;
        
        UIHandler.SetText($"Press {m_InteractAction.GetBindingDisplayString()} to remove the glasses");
    }

    public void RemoveGlasses(ScriptableRendererFeature feature)
    {
        feature?.SetActive(false);
        m_CurrentState = InteractionState.Moving;
        UIHandler.HideText();
    }
}
