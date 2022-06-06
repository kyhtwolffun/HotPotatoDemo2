using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;
using Photon.Deterministic;

public class BombExplodeDisplay : MonoBehaviour
{
    [SerializeField] private Material explodedMaterial;
    [SerializeField] private Text textBox;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private EntityView entityView;
    
    private void Start()
    {
        QuantumEvent.Subscribe<EventExplode>(this, (eventData) => StartCoroutine(ChangeColor(eventData)), once: true);
        QuantumEvent.Subscribe<EventBombMark>(this, (eventData) => StartCoroutine(BombMarkDisplay(eventData)));
    }

    private void OnDestroy()
    {
        QuantumEvent.UnsubscribeListener<EventExplode>(this);
        QuantumEvent.UnsubscribeListener<EventBombMark>(this);
    }

    private IEnumerator BombMarkDisplay(EventBombMark e)
    {
        yield return new WaitUntil(() => QuantumRunner.Default.Session != null && QuantumRunner.Default.Session.FrameVerified != null);
        var f = QuantumRunner.Default.Game.Frames.Verified;

        if (f.TryGet<PlayerLink>(entityView.EntityRef, out var pl) && pl.Player == e.player)
        {
            if (e.display)
            {
                textBox.text = e.time.AsInt.ToString();
            }
            else
            {
                textBox.text = "";
            }
        }  
    }

    private IEnumerator ChangeColor(EventExplode e)
    {
        yield return new WaitUntil(() => QuantumRunner.Default.Session != null && QuantumRunner.Default.Session.FrameVerified != null);
        var f = QuantumRunner.Default.Game.Frames.Verified;

        if (f.TryGet<PlayerLink>(entityView.EntityRef, out var pl) && pl.Player == e.player)
        {
            meshRenderer.material = explodedMaterial;
        }
    }

}
