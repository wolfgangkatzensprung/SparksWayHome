using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child of Player that contains Player's ParticleSystems and plays and pauses them according to situation.
/// </summary>
public class PlayerParticles : MonoBehaviour
{
    Player player;
    ParticleSystem[] dustParticleSystems;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        dustParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (var dust in dustParticleSystems)
        {
            dust.Stop();
        }
    }

    void OnEnable()
    {
        return;
        player.Movement.OnStartMoving += StartDustParticles;
        player.Movement.OnStopMoving += StopDustParticles;
    }

    void OnDisable()
    {
        player.Movement.OnStartMoving -= StartDustParticles;
        player.Movement.OnStopMoving -= StopDustParticles;
    }

    private void StartDustParticles()
    {
        foreach (var dust in dustParticleSystems)
        {
            dust.Play();
        }
    }

    private void StopDustParticles()
    {
        foreach (var dust in dustParticleSystems)
        {
            dust.Stop();
        }
    }
}
