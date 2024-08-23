using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class ParticleEmitterData
{
    public ParticleData particleData = new();
    
    public float angle = 0f;
    public float angleVariance = 45f;
    
    public float lifeSpanMin = 0.1f;
    public float lifeSpanMax = 2f;
    
    public float speedMin = 100f;
    public float speedMax = 100f;

    public float sizeStartMin = 1;
    public float sizeStartMax = 4;
    public float sizeEndMin = 24;
    public float sizeEndMax = 36;

    public Color colorStart = Color.Yellow;
    public Color colorEnd = Color.Red;

    public float opacityStart = 1f;
    public float opacityEnd = 0f;

    public float interval = 1f;
    public int emitCount = 1;
    public bool visible = true;

    public ParticleEmitterData() {}
}

public class ParticleEmitter
{
    public string Name;
    public Vector2 Position;
    public int Layer;

    private readonly ParticleEmitterData _emitterData;
    private float _intervalLeft;

    List<Particle> particles = new List<Particle>();
    private bool enabled = true;
    private bool drawOnLoading = false;

    public ParticleEmitter(string name, Vector2 pos, ParticleEmitterData particleEmitterData, int layer, bool drawOnLoading=false)
    {
        this.Name = name;
        this.Position = pos;
        this.Layer = layer;
        this.enabled = true;
        this.drawOnLoading = drawOnLoading;
     
        _emitterData = particleEmitterData;
        _intervalLeft = particleEmitterData.interval;

        ParticleManager.AddParticleEmitter(this);
    }

    private void Emit()
    {
        ParticleData particleData = _emitterData.particleData;
        particleData.sizeStart = Randomize.FloatInRange(_emitterData.sizeStartMin, _emitterData.sizeStartMax);
        particleData.sizeEnd = Randomize.FloatInRange(_emitterData.sizeEndMin, _emitterData.sizeEndMax);
        particleData.lifeSpan = Randomize.FloatInRange(_emitterData.lifeSpanMin, _emitterData.lifeSpanMax);
        particleData.speed = Randomize.FloatInRange(_emitterData.speedMin, _emitterData.speedMax);
        particleData.colorStart = _emitterData.colorStart;
        particleData.colorEnd = _emitterData.colorEnd;
        particleData.opacityStart = _emitterData.opacityStart;
        particleData.opacityEnd = _emitterData.opacityEnd;

        float r = (float)(new Random().NextDouble() * 2) - 1;
        particleData.angle = _emitterData.angleVariance * r;

        Particle p = new Particle(Position, particleData, this.Layer);
        p.drawOnLoading = drawOnLoading;
        particles.Add(p);
    }

    public void SetVisible(bool value)
    {
        _emitterData.visible = value;
    }

    public void SetEnable(bool value)
    {
        this.enabled = value;
    }
    public async void Burst(int burstTime)
    {
        if (enabled) SetVisible(true);
        await Task.Delay(burstTime);
        SetVisible(false);
    }

    public void Unload()
    {
        foreach (var p in particles.ToList())
        {
            p.DestroySelf();
        }
    }

    public void Update()
    {
        _intervalLeft -= Globals.DEBUG_DT;
        while (_intervalLeft <= 0f)
        {
            _intervalLeft += _emitterData.interval;
            for (int i = 0; i < _emitterData.emitCount; i++)
            {
                if (_emitterData.visible) 
                    Emit();
            }
        }
    }
}