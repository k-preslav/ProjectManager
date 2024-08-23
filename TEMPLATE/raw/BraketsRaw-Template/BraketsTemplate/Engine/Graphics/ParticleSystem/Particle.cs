using System;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public struct ParticleData
{
    public string textureName = "particle_default";
    public float lifeSpan = 2f;

    public Color colorStart = Color.Yellow;
    public Color colorEnd = Color.Red;

    public float opacityStart = 1f;
    public float opacityEnd = 0f;

    public float sizeStart = 4f;
    public float sizeEnd = 4f;

    public float speed = 100f;
    public float angle = 0f;

    public ParticleData() {}
}

public class Particle : Sprite
{
    public bool IsFinished = false;

    private readonly ParticleData _particleData;
    private float _lifeSpanLeft;
    private float _lifeSpanAmmount;
    private Vector2 _direction;

    public Particle(Vector2 pos, ParticleData particleData, int layer) : base("particle", pos, "none", layer, false)
    {
        _particleData = particleData;
        _lifeSpanLeft = particleData.lifeSpan;
        _lifeSpanAmmount = 1f;

        if (_particleData.speed != 0)
        {
            _particleData.angle = MathHelper.ToRadians(_particleData.angle);
            _direction = new Vector2((float)Math.Sin(_particleData.angle), -(float)Math.Cos(_particleData.angle));
        }
        else _direction = Vector2.Zero;

        this.textureName = particleData.textureName;
        this.Load();
    }

    public override void Update(float dt)
    {
        _lifeSpanLeft -= dt;

        _lifeSpanAmmount = MathHelper.Clamp(_lifeSpanLeft / _particleData.lifeSpan, 0, 1);
        this.Scale = MathHelper.Lerp(_particleData.sizeEnd, _particleData.sizeStart, _lifeSpanAmmount) / this.texture.Width;
        this.Tint = Color.Lerp(_particleData.colorEnd, _particleData.colorStart, _lifeSpanAmmount);
        this.Opacity = MathHelper.Clamp(MathHelper.Lerp(_particleData.opacityEnd, _particleData.opacityStart, _lifeSpanAmmount), 0, 1);   
        this.Position += _direction * _particleData.speed * dt;

        if (_lifeSpanLeft <= 0f)
        {
            IsFinished = true;
            this.DestroySelf();     
            return;
        }
        
        base.Update(dt);
    }
}