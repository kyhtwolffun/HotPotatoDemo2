using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;

public class BombExplodeDisplay : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private EntityView entityView;
    
    private void Start()
    {
        QuantumEvent.Subscribe<EventExplode>(this, (eventData) => StartCoroutine(ChangeColor(eventData)), once: true);
    }

    private void OnDestroy()
    {
        QuantumEvent.UnsubscribeListener<EventExplode>(this);
    }

    private IEnumerator ChangeColor(EventExplode e)
    {
        yield return new WaitUntil(() => QuantumRunner.Default.Session != null && QuantumRunner.Default.Session.FrameVerified != null);
        var f = QuantumRunner.Default.Game.Frames.Verified;

        if (f.TryGet<PlayerLink>(entityView.EntityRef, out var pl) && pl.Player == e.player)
        {
            meshRenderer.material = material;
        }
    }

}
