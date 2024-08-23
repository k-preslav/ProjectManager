using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class UIText : UIElement
{
    public UIText(string text="New UI Text") : base(text, UIAllign.TopLeft)
    {
        this.BackgroundColor = Color.Transparent;
        this.ForegroundColor = Color.White;
    }
}