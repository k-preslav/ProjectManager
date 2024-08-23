using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class LoadingScreen
{
    public static UIScreen loadingScreen;
    public static bool isLoading = false;

    private static TimeSpan minimumLoadingTime;
    private static DateTime loadStartTime;

    private static bool isInitialized = false;

    public static void Initialize()
    {
        if (isInitialized)
            return;

        loadingScreen = new UIScreen();

        UIText loadingText = new UIText("Loading...");
        loadingText.Tag = "__loading__ui_element";
        loadingText.SetAlign(UIAllign.Center, new Vector2(0));
        loadingText.SetFontSize(36);
        loadingText.Animate(UIAnimation.SmoothFlashing, 750, min: 0.35f);

        loadingScreen.AddElement(loadingText);

        // var particles = new ParticleEmitter("main_menu_particles", new Vector2(Globals.APP_Width / 2, 132), new ParticleEmitterData{
        //     angleVariance = 360,
        //     lifeSpanMin = 1.4f,
        //     lifeSpanMax = 2.6f,
        //     emitCount = 4,
        //     sizeStartMin = 22,
        //     sizeStartMax = 26,
        //     sizeEndMin = 12,
        //     sizeEndMax = 14,
        //     interval = 0.15f,
        //     speedMin=25,
        //     speedMax=65,
        //     colorStart = Color.DarkRed,
        //     colorEnd = Color.Red,
        //     visible = true,
        // }, 2, drawOnLoading: true);

        isInitialized = true;
    }

    public static void Show()
    {
        minimumLoadingTime = new TimeSpan(0, 0, 0, 0, Randomize.IntInRange(975, 2345));

        loadingScreen.Show();

        loadStartTime = DateTime.Now;
        isLoading = true;

        Globals.Camera.BackgroundColor = Color.Black;
    }

    public static async Task Hide()
    {
        TimeSpan realLoadingTime = DateTime.Now - loadStartTime;
        TimeSpan waitTime = minimumLoadingTime - realLoadingTime;

        if (waitTime > TimeSpan.Zero)
        {
            await Task.Delay(waitTime);
        }

        Debug.Log($" - Total loading time: {realLoadingTime.TotalMilliseconds + waitTime.TotalMilliseconds} ms");

        loadingScreen.Hide();
        isLoading = false;
    }
}