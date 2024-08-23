using System;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public enum UIAnimation
{
    Flashing,
    SmoothFlashing,
    ZoomInOut
}

public class UIAnimator
{
    public static async Task AnimateFlashing(int duration, UIElement element)
    {
        element.Opacity = 1;
        await Task.Delay(duration);
        element.Opacity = 0;
        await Task.Delay(duration);
    }
    public static async Task AnimateSmoothFlashing(int duration, float min, float max, UIElement element)
    {
        float initialOpacity = element.Opacity;
        float maxOpacity = max;
        float minOpacity = min;
        int steps = duration / 5;
        float stepDuration = duration / (steps * 2);

        // Flashing in
        for (int i = 0; i < steps; i++)
        {
            element.Opacity = MathHelper.Lerp(initialOpacity, maxOpacity, (float)i / steps);
            await Task.Delay((int)stepDuration);
        }

        // Flashing out
        for (int i = 0; i < steps; i++)
        {
            element.Opacity = MathHelper.Lerp(maxOpacity, minOpacity, (float)i / steps);
            await Task.Delay((int)stepDuration);
        }

        element.Opacity = minOpacity;
    }

    public static async Task AnimateZoomInOut(int duration, float min, float max, UIElement element)
    {
        Vector2 initialSize = element.Size;
        Vector2 maxSize = initialSize * max;
        Vector2 minSize = initialSize * min;
        int steps = duration / 5;
        float stepDuration = duration / (steps * 2);

        // Zoom in
        for (int i = 0; i < steps; i++)
        {
            element.Size = Vector2.Lerp(initialSize, maxSize, (float)i / steps);
            await Task.Delay((int)stepDuration);
        }

        // Zoom out
        for (int i = 0; i < steps; i++)
        {
            element.Size = Vector2.Lerp(maxSize, minSize, (float)i / steps);
            await Task.Delay((int)stepDuration);
        }

        element.Size = minSize;
    }
}