using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class EquippableGlasses : InteractableObject
{
    public ScriptableRendererFeature RendererFeature;
    public UnityEvent OnPutOnEvent;
    public UnityEvent OnRemoveEvent;

    private bool m_Equipped = false;
    
    public override void Interact(ObservingHandler handler)
    {
        if (!m_Equipped)
        {
            handler.EquipGlasses(RendererFeature);
            PutOn();
        }
        else
        {
            handler.RemoveGlasses(RendererFeature);
            Remove();
        }
    }

    private void OnDestroy()
    {
        RendererFeature?.SetActive(false);
    }

    void PutOn()
    {
        
        OnPutOnEvent.Invoke();
        m_Equipped = true;
        gameObject.SetActive(false);
    }

    void Remove()
    {
        m_Equipped = false;
        OnRemoveEvent.Invoke();
        gameObject.SetActive(true);
    }
}
