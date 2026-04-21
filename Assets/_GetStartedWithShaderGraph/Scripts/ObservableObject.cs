using System;
using UnityEngine;

public class ObservableObject : InteractableObject
{
    public override void Interact(ObservingHandler handler)
    {
        handler.ObserveObject(this);
    }
}
