using System;
using System.Threading;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BraketsEngine;

public enum UIAllign
{
    TopLeft,
    MiddleLeft,
    BottomLeft,
    TopCenter,
    Center,
    BottomCenter,
    TopRigth,
    MiddleRight,
    BottomRight
}

public abstract class UIElement : Sprite
{
    public string Text;
    public Color BackgroundColor = Color.White;
    public Color ForegroundColor = Color.Black;
    public Action OnClick;
    public bool Clickable = true;

    public Vector2 Size;

    private SpriteFontBase _font;
    private string _curFontName;

    internal Action CustomUpdate;

    public UIAllign allign { get; private set; }
    public Vector2 margin { get; private set; }
    public Vector2 padding { get; private set; }

    private CancellationTokenSource animationCancellationToken;
    public bool animationRunning = false;

    public UIElement(string text = "UI Element", UIAllign allign = UIAllign.TopLeft, 
                    int marginX = 0, int marginY = 0, int padX = 0, int padY = 0,
                    string fontName = "NeorisMedium", int fontSize = 24, string textureName = "ui_default")
            : base("ui_element", new Vector2(0), textureName, 1, false)
    {
        this.Text = text;
        base.overrideDraw = true;
        base.textureName = textureName;

        this.allign = allign;
        this.margin = new Vector2(marginX, marginY);
        this.padding = new Vector2(padX, padY);

        this._font = ResourceManager.GetFont(fontName, fontSize);
        _curFontName = fontName;

        base.texture = ResourceManager.GetTexture(base.textureName);
        this.Size = new Vector2(base.texture.Width, base.texture.Height);
    }

    public override void Update(float dt)
    {
        if (this.allign == UIAllign.TopLeft)
        {
            this.Position = new Vector2(
                this.Rect.Width / 2, this.Rect.Height / 2
            ) + margin;
        }
        else if (this.allign == UIAllign.MiddleLeft)
        {
            this.Position = new Vector2(
                this.Rect.Width / 2, Globals.APP_Height / 2 - this.Rect.Height / 2
            ) + margin;
        }
        else if (this.allign == UIAllign.BottomLeft)
        {
            this.Position = new Vector2(
                this.Rect.Width / 2, Globals.APP_Height - this.Rect.Height / 2
            ) + new Vector2(margin.X, -margin.Y);
        }
        else if (this.allign == UIAllign.TopCenter)
        {
            this.Position = new Vector2(
                Globals.APP_Width / 2, this.Rect.Height / 2
            ) + margin;
        }
        else if (this.allign == UIAllign.Center)
        {
            this.Position = new Vector2(
                Globals.APP_Width / 2, Globals.APP_Height / 2 - this.Rect.Height / 2
            ) + margin;
        }
        else if (this.allign == UIAllign.BottomCenter)
        {
            this.Position = new Vector2(
                Globals.APP_Width / 2, Globals.APP_Height - this.Rect.Height / 2
            ) - margin;
        }
        else if (this.allign == UIAllign.TopRigth)
        {
            this.Position = new Vector2(
                Globals.APP_Width - this.Rect.Width / 2, this.Rect.Height / 2
            ) + new Vector2(-margin.X, margin.Y);
        }
        else if (this.allign == UIAllign.MiddleRight)
        {
            this.Position = new Vector2(
                Globals.APP_Width - this.Rect.Width / 2, Globals.APP_Height / 2 - this.Rect.Height / 2
            ) - margin;
        }
        else if (this.allign == UIAllign.BottomRight)
        {
            this.Position = new Vector2(
                Globals.APP_Width - this.Rect.Width / 2, Globals.APP_Height - this.Rect.Height / 2
            ) - margin;
        }

        if (this.Clickable)
        {
            if (this.Rect.Intersects(
                new Rectangle(Input.GetMousePosition().ToPoint(), new Point(1, 1))
            ))
            {
                if (Input.IsMouseClicked(0))
                    OnClick?.Invoke();
            }
        }

        CustomUpdate?.Invoke();
    }

    public override void UpdateRect()
    {
        this.Rect = new Rectangle(
            new Point((int)(this.Position.X - this.Size.X/ 2), (int)(this.Position.Y - this.Size.Y / 2)),
            this.Size.ToPoint()
        );
    }

    public void SetFont(string name)
    {
        int curSize = (int)_font.FontSize;
        _font = ResourceManager.GetFont(name, curSize);
        _curFontName = name;
    }
    public void SetFontSize(int size)
    {
        _font = ResourceManager.GetFont(_curFontName, size);
    }
    public void SetAlign(UIAllign allign, Vector2 margin)
    {
        this.allign = allign;
        this.margin = margin;
    }

    public void SetPaddding(Vector2 padding) => this.padding = padding;

    public void DrawUI()
    {
        if (!this.visible)
            return;

        Globals.ENGINE_SpriteBatch.Draw(
            texture, this.Rect, null,
            this.BackgroundColor * this.Opacity, this.Rotation,
            new Vector2(0), this._effects, 0
        );

        Vector2 textSize = GetTextSize();
        Vector2 textPosition = this.Position - new Vector2(textSize.X / 2, textSize.Y / 2);

        Globals.ENGINE_SpriteBatch.DrawString(
            _font, this.Text, textPosition, 
            this.ForegroundColor * this.Opacity, this.Rotation
        );
    }

    public void Animate(UIAnimation animation, int duration, float min=0f, float max=1f, bool once=false)
    {
        animationRunning = true;

        animationCancellationToken?.Cancel();
        animationCancellationToken = new CancellationTokenSource();
        var token = animationCancellationToken.Token;

        if (animation is UIAnimation.Flashing)
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        await UIAnimator.AnimateFlashing(duration, this);
                        if (once) StopAnimation();
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.Warning("Failed to animate UIElement!", this);
                }
            }, token);
        }
        else if (animation is UIAnimation.SmoothFlashing)
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        await UIAnimator.AnimateSmoothFlashing(duration, min, max, this);
                        if (once) StopAnimation();
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.Warning("Failed to animate UIElement!", this);
                }
            }, token);
        }
        else if (animation is UIAnimation.ZoomInOut)
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        await UIAnimator.AnimateZoomInOut(duration, min, max, this);
                        if (once) StopAnimation();
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.Warning("Failed to animate UIElement!", this);
                }
            }, token);
        }
    }
    public void StopAnimation()
    {
        animationCancellationToken?.Cancel();
        animationRunning = false;
    }

    public Vector2 GetTextSize()
    {
        return _font.MeasureString(Text);
    }
}