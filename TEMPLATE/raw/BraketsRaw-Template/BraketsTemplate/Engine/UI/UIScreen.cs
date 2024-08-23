using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace BraketsEngine;

public class UIScreen
{
    private List<UIElement> _elements = new List<UIElement>();

    public void AddElement(UIElement element)
    {
        _elements.Add(element);
        Globals.ENGINE_Main.AddUIElement(element);
    }
    public void AddElements(UIElement[] elements)
    {
        _elements.AddRange(elements);
        foreach (var elem in elements)
            Globals.ENGINE_Main.AddUIElement(elem);
    }

    public void RemoveEement(UIElement element)
    {
        element.StopAnimation();
        _elements.Remove(element);
        Globals.ENGINE_Main.RemoveUIElement(element);
    }

    public void Show()
    {
        foreach (var elem in _elements)
            elem.visible = true;
    }

    public void Hide()
    {
        foreach (var elem in _elements)
            elem.visible = false;
    }

    public void Unload()
    {
        foreach (var elem in _elements.ToList())
            RemoveEement(elem);
    }
}
