using System.Collections.Generic;
using UnityEngine;


public class CustomCallbacks : QuantumCallbacks
{
    public List<Quantum.RuntimePlayer> playerPrototype;

    public override void OnGameStart(Quantum.QuantumGame game)
    {
        // paused on Start means waiting for Snapshot
        if (game.Session.IsPaused) return;

        foreach (var lp in game.GetLocalPlayers())
        {
            Debug.Log("CustomCallbacks - sending player: " + lp);
            for (int i = 0; i < playerPrototype.Count ; i++)
            {
                game.SendPlayerData(lp, playerPrototype[i]);

            }
        }
    }

    public override void OnGameResync(Quantum.QuantumGame game)
    {
        Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
    }
}

