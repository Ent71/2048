using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ParticleSystem))]
public class Effect : MonoBehaviour, IPoolable<IMemoryPool>
{
    protected ParticleSystem CollisionParticleSystem;
    private IMemoryPool _pool;

    private void Awake()
    {
        CollisionParticleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    public virtual void Play()
    {
        CollisionParticleSystem.Play();
    }

    public void OnParticleSystemStopped()
    {
        _pool.Despawn(this);
    }

    public void OnDespawned()
    {
        _pool = null;
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
        Play();
    }

    public class Factory : PlaceholderFactory<Effect>
    {

    }
}
