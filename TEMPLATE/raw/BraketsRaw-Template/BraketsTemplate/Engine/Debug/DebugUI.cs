using System;
using System.Collections.Generic;
using FontStashSharp;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;

namespace BraketsEngine;

public class DebugWindow
{
    public Action OnDraw;
    private string _name;
    private Vector2 _size;
    private bool _visible;

    public DebugWindow(string name, int width=320, int height=180, bool visible=true)
    {
        this._name = name;
        this._size = new Vector2(width, height);

        this._visible = visible;
    }

    public virtual void Draw()
    {
        if (OnDraw is not null)
        {
            ImGui.SetNextWindowSize(this._size.ToNumerics());
            ImGui.Begin(this._name);
            OnDraw.Invoke();
            ImGui.End();
        }
    }
}

public class DebugUI
{
    private List<DebugWindow> _windows = new List<DebugWindow>();
    private ImGuiRenderer _renderer;

    public void Initialize(Game owner)
    {
        _renderer = new ImGuiRenderer(owner);
        _renderer.RebuildFontAtlas();   

        Globals.DEBUG_UI = this;
    }
    public void DrawWindows(GameTime gameTime)
    {
        if (_renderer is null)
            return;

        _renderer.BeginLayout(gameTime);
        foreach (var window in _windows)
            window.Draw();
        _renderer.EndLayout();
    }
    
    public void AddWindow(DebugWindow win)
    {
        _windows.Add(win);
    }

    // ----- OVERLAY -------
    private float _refreshRate;
    private float _refreshTimer;
    private float _currentFps;

    public void DrawOverlay(SpriteBatch sp, float refreshRate)
    {
        if (!Globals.DEBUG_Overlay)
            return;

        _refreshRate = refreshRate;

        _refreshTimer -= Globals.DEBUG_DT;
        if (_refreshTimer <= 0)
        {
            _currentFps = Globals.DEBUG_FPS;
            _refreshTimer = refreshRate;
        }

        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 32), $"{_currentFps.ToString("0.0")} fps", 
            new Vector2(10, 10), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 3
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"Version: {Globals.APP_Version}", 
            new Vector2(10,45), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"VSync: {Globals.APP_VSync}", 
            new Vector2(10, 70), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
        sp.DrawString(
            ResourceManager.GetFont("NeorisMedium", 24), $"Sprites: {Globals.ENGINE_Main.Sprites.Count}", 
            new Vector2(10, 95), Color.White * 0.75f, effect: FontSystemEffect.Stroked, effectAmount: 2
        );
    }
}