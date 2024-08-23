using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BraketsEngine;

public class Tile : Sprite
{
    public Tile(Vector2 pos, string texName) : base("s_tile", pos, texName, 0, true) { }

    public void ApplyPosition(float scale)
    {
        float posX = this.Position.X * this.Scale * this.texture.Width;
        float posY = this.Position.Y * this.Scale * this.texture.Height;

        this.Scale = scale;
        this.Position = new Vector2(
            posX - (this.Scale * this.texture.Width) / 2,
            posY - (this.Scale * this.texture.Height) / 2
        );
    }

    private float setRandomColorTimer = 0.75f;
    public override void Update(float dt) //TODO: This code shoud not be here. Make it so the tilemap gives a Tile class that can have this code or other instead
    {
        setRandomColorTimer -= dt;
        if (setRandomColorTimer <= 0)
        {
            setRandomColorTimer = 0.75f;

            int randInt = new Random().Next(0, 10);
            if (randInt == 7)
            {
                this.Tint = Color.WhiteSmoke;
            }
            else if (randInt == 4)
            {
                this.Tint = Color.Wheat;
            }
            else
                this.Tint = Color.White;
        }
    }
}