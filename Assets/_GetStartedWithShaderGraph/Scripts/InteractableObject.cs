using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private GameObject m_Selector;
    private Vector3 m_SelectorPosition;
    
    private void OnEnable()
    {
        //Ensure there is a collider on the object when it start
        var collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            //if there is no colliders, we find all mesh filter and add a mesh collider on them
            var filters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var f in filters)
            {
                f.gameObject.AddComponent<MeshCollider>();
            }
        }
    }

    private void Update()
    {
        if (m_Selector.activeSelf)
        {
            m_Selector.transform.position =
                m_SelectorPosition + new Vector3(0, Mathf.PingPong(Time.time * 0.02f, 0.1f) - 0.05f, 0);
            m_Selector.transform.Rotate(m_Selector.transform.up, 25.0f * Time.deltaTime);
        }
    }
    
    private void Start()
    {
        m_Selector = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_Selector.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        m_Selector.transform.SetParent(transform);
        m_Selector.SetActive(false);
        
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        m_SelectorPosition = new Vector3(renderer.bounds.center.x, renderer.bounds.max.y + 0.1f, renderer.bounds.center.z);
    }

    public virtual void Interact(ObservingHandler handler)
    {
        
    }
    
    public void ShowSelection()
    {
        m_Selector.SetActive(true);
    }

    public void HideSelection()
    {
        m_Selector.SetActive(false);
    }
}
