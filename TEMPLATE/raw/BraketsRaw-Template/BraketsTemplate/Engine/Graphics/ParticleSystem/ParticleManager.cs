using System.Collections.Generic;

namespace BraketsEngine;

public static class ParticleManager
{
    private static List<ParticleEmitter> _particleEmitters = new List<ParticleEmitter>();

    public static void AddParticleEmitter(ParticleEmitter emitter)
    {
        _particleEmitters.Add(emitter);
    }

    public static void Burst(string name, int burstTime)
    {
        GetEmitter(name).Burst(burstTime);
    }

    public static void Update()
    {
        foreach (var emitter in _particleEmitters) 
            emitter.Update();
    }

    public static ParticleEmitter GetEmitter(string name)
    {
        foreach (var emitter in _particleEmitters)
        {
            if (emitter.Name == name)
                return emitter;
        }

        Debug.Warning($"No particle emitter of name '{name}' was found!");
        return null;
    }

    public static void UnloadAll()
    {
        foreach (var particleEmitter in _particleEmitters)
        {
            particleEmitter.Unload();
        }
        _particleEmitters.Clear();
    }

    public static void Unload(string name)
    {
        foreach (var particleEmitter in _particleEmitters)
        {
            if (particleEmitter.Name == name)
            {
                particleEmitter.Unload();
                _particleEmitters.Remove(particleEmitter);
                break;
            }
        }
    }
    public static void Unload(ParticleEmitter emitter)
    {
        emitter.Unload();
        _particleEmitters.Remove(emitter);
    }
}