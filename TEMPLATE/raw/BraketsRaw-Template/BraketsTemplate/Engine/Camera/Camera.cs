using System;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class Camera
{
    public Vector2 TargetPosition;
    public Color BackgroundColor = Color.Cyan;
    public Matrix TranslationMatrix;

    // Camera shake variables
    private float shakeIntensity;
    private float shakeDuration;
    private float shakeTimer;

    public Camera(Vector2 startPos)
    {
        this.TargetPosition = startPos;
        Globals.Camera = this;

        shakeIntensity = 0f;
        shakeDuration = 0f;
        shakeTimer = 0f;
    }

    public void CalculateMatrix()
    {
        var dx = (Globals.APP_Width / 2) - TargetPosition.X;
        var dy = (Globals.APP_Height / 2) - TargetPosition.Y;

        if (shakeTimer > 0)
        {
            float shakeX = (float)(new Random().NextDouble() * 2 - 1) * shakeIntensity;
            float shakeY = (float)(new Random().NextDouble() * 2 - 1) * shakeIntensity;

            dx += shakeX;
            dy += shakeY;

            shakeTimer -= Globals.DEBUG_DT;
            if (shakeTimer <= 0)
            {
                shakeTimer = 0;
                shakeIntensity = 0f;
            }
        }

        TranslationMatrix = Matrix.CreateTranslation(dx, dy, 0f);
    }

    public void Follow(Sprite target, float smoothStep)
    {
        TargetPosition = Vector2.Lerp(Globals.Camera.TargetPosition, target.Position, smoothStep * Globals.DEBUG_DT);
    }

    public void GoTo(Vector2 target, float smoothStep)
    {
        TargetPosition = Vector2.Lerp(Globals.Camera.TargetPosition, target, smoothStep * Globals.DEBUG_DT);
    }

    public void Teleport(Vector2 target)
    {
        TargetPosition = target;
    }

    public void Center()
    {
        TargetPosition = new Vector2(Globals.APP_Width / 2, Globals.APP_Height / 2);
    }

    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }
}
