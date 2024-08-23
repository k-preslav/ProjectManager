using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace BraketsEngine;

public class Input
{
    static KeyboardState currentKeyState;
    static KeyboardState previousKeyState;

    static MouseState currentMouseState;
    static MouseState previousMouseState;

    public static KeyboardState GetKeyboardState()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
        return currentKeyState;
    }
    public static MouseState GetMouseState()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        return currentMouseState;
    }

    public static bool IsDown(Keys key)
    {
        return currentKeyState.IsKeyDown(key);
    }

    public static bool IsPressed(Keys key)
    {
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }

    public static Vector2 GetMousePosition()
    {
        Point p = currentMouseState.Position;
        return new Vector2(p.X, p.Y);
    }
    public static bool IsMouseDown(int index)
    {
        if (currentMouseState.LeftButton == ButtonState.Pressed && index == 0)
            return true;
        else if (currentMouseState.RightButton == ButtonState.Pressed && index == 1)
            return true;

        return false;
    }
    public static bool IsMouseClicked(int index)
    {
        if (index == 0)
            return previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
        else if (index == 1)
            return previousMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released;

        return false;
    }
}