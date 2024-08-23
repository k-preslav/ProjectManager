using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class UIButton : UIElement
{
    public UIButton(string text="New UI Button", string textureName="ui_default") : base(text: text, textureName: textureName)
    {
        base.CustomUpdate += OnUpdate;
        SetPaddding(new Vector2(10));
    }

    private void OnUpdate()
    {
        this.Size = base.GetTextSize() + base.padding;
    }
}