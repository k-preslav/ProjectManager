using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BraketsEngine;

public class Main : Game
{
    public List<Sprite> Sprites;
    public List<UIElement> UI;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private GameManager _gameManager;
    private DebugUI _debugUi;
    private float sendDiagnosticCooldown = 0.5f;
    private float _sendDiagnosticsTimer = 0;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "PreloadContent";
        IsMouseVisible = true;

        this.Exiting += OnExit;
    }

    protected override async void Initialize()
    {
        Debug.Log("Calling Initialize()", this);

        _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
        _gameManager = new GameManager();

        Globals.LOAD_APP_P();
        IsFixedTimeStep = false;

        Debug.Log("Applying application properties...", this);
        Window.Title = Globals.APP_Title;
        _graphics.PreferredBackBufferWidth = Globals.APP_Width;
        _graphics.PreferredBackBufferHeight = Globals.APP_Height;
        _graphics.SynchronizeWithVerticalRetrace = Globals.APP_VSync;
        _graphics.ApplyChanges();

        Globals.ENGINE_Main = this;
        Globals.ENGINE_GraphicsDevice = _graphics.GraphicsDevice;
        Globals.ENGINE_SpriteBatch = _spriteBatch;

        _debugUi = new DebugUI();
        _debugUi.Initialize(this);

        if (Debugger.IsAttached)
            Globals.DEBUG_Overlay = true;

        new Camera(Vector2.Zero);

        this.Sprites = new List<Sprite>();
        this.UI = new List<UIElement>();

        if (Globals.BRIDGE_Run)
        {
            sendDiagnosticCooldown = Globals.BRIDGE_RefreshRate;
            if (sendDiagnosticCooldown == 0)
            {
                Globals.BRIDGE_Run = false;
            }
            else
            {
                BridgeClient bridgeClient = new BridgeClient();
                await bridgeClient.ConnectAsync(Globals.BRIDGE_Hostname, Globals.BRIDGE_Port);

                Globals.BRIDGE_Client = bridgeClient;
                Globals.BRIDGE_Client.OnReceive += (string msg) =>
                {
                    if (msg == "stop") Exit();
                };
            }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Debug.Log("Loading content...", this);
        _gameManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.GetKeyboardState();
        Input.GetMouseState();

        if (Input.IsPressed(Keys.F3))
            Globals.DEBUG_Overlay = !Globals.DEBUG_Overlay;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Globals.DEBUG_DT = dt;
        Globals.DEBUG_FPS = 1 / dt;

        Globals.Camera.CalculateMatrix();

        ParticleManager.Update();
        foreach (var elem in UI.ToList())
        {
            elem.Update(dt);
            elem.UpdateRect();
        }

        if (Globals.BRIDGE_Run)
        {
            _sendDiagnosticsTimer -= dt;
            DebugData.GetDiagnostics();

            if (_sendDiagnosticsTimer <= 0)
            {
                DebugBridge.SendData();
                _sendDiagnosticsTimer = sendDiagnosticCooldown;
            }
        }

        _gameManager?.Update(dt);
        foreach (var sp in Sprites.ToList())
        {
            if ((!Globals.STATUS_Loading && !LoadingScreen.isLoading) || sp.drawOnLoading)
            {
                sp.Update(dt);
                sp.UpdateRect();
            }
        }

        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Globals.Camera.BackgroundColor);

        // ------- Game Layer -------
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Globals.Camera.TranslationMatrix);

        var sortedSprites = Sprites.OrderBy(sp => sp.Layer).ToList();
        foreach (var sp in sortedSprites)
        {
            if ((!Globals.STATUS_Loading && !LoadingScreen.isLoading) || sp.drawOnLoading)
                sp.Draw();
        }
        _spriteBatch.End();

        // ------- UI Layer ------- 
        _spriteBatch.Begin();
        foreach (var elem in UI.ToList())
        {
            if (LoadingScreen.isLoading)
            {
                if (elem.Tag.Contains("__loading__"))
                {
                    elem.DrawUI();
                }

                _spriteBatch.End();
                return;
            }

            elem.DrawUI();
        }
        _debugUi.DrawOverlay(_spriteBatch, 0.25f);
        _debugUi.DrawWindows(gameTime);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void AddSprite(Sprite sp) => Sprites.Add(sp);
    public void RemoveSprite(Sprite sp) => Sprites.Remove(sp);

    public void AddUIElement(UIElement elem) => UI.Add(elem);
    public void RemoveUIElement(UIElement elem) => UI.Remove(elem);

    private void OnExit(object sender, EventArgs e)
    {
        Debug.Log("Calling OnExit()");

        Globals.BRIDGE_Client?.Disconnect();
        _gameManager.Stop();
    }
}
