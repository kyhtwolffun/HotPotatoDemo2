using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;

public class BombExplodeDisplay : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;

    
    private void Start()
    {
        QuantumEvent.Subscribe<EventExplode>(this, ChangeColor, once: true);
    }

    private void ChangeColor(EventExplode e)
    {
        meshRenderer.material = material;
    }

}
