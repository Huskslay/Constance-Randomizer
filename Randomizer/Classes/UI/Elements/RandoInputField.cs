using Randomizer.Patches.Menu;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Randomizer.Classes.UI.Elements;

public class RandoInputField
{
    private static readonly int maxLength = 9;
    private static readonly float blinkTime = 0.5f;

    public RandoButton button;

    private readonly string text;
    private string input;
    private readonly Action<string> onUpdate;

    private float nextTime = 0;
    private bool showing = false;

    public RandoInputField(Transform transform, string text, string input, Action<string> onUpdate)
    {
        button = CConStartMenu_Patch.CreateButton(text, transform, OnClick, OnUpdate);

        this.text = text;
        this.input = input;
        this.onUpdate = onUpdate;
    }

    public void OnClick(RandoButton _) { }

    private void OnUpdate()
    {
        if (Time.time > nextTime)
        {
            nextTime = Time.time + blinkTime;
            showing = !showing;
        }
        button.text = text + input.ToString() + (showing ? "<color=#FFFFFFFF>" : "<color=#FFFFFF00>") + "|</color>";

        if (Keyboard.current.backspaceKey.wasPressedThisFrame && input.Length > 0)
        { input = input[..^1]; onUpdate?.Invoke(input); }

        if (input.Length > maxLength) input = input[..maxLength];
        if (input.Length >= maxLength) return;

        if (Keyboard.current.digit0Key.wasPressedThisFrame || Keyboard.current.numpad0Key.wasPressedThisFrame)
        { input += "0"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
        { input += "1"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
        { input += "2"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame)
        { input += "3"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame)
        { input += "4"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame)
        { input += "5"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit6Key.wasPressedThisFrame || Keyboard.current.numpad6Key.wasPressedThisFrame)
        { input += "6"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit7Key.wasPressedThisFrame || Keyboard.current.numpad7Key.wasPressedThisFrame)
        { input += "7"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit8Key.wasPressedThisFrame || Keyboard.current.numpad8Key.wasPressedThisFrame)
        { input += "8"; onUpdate?.Invoke(input); }

        if (Keyboard.current.digit9Key.wasPressedThisFrame || Keyboard.current.numpad9Key.wasPressedThisFrame)
        { input += "9"; onUpdate?.Invoke(input); }
    }

    public void SetInput(string newInput)
    {
        if (newInput.Length >= maxLength) return;

        input = newInput;
        onUpdate?.Invoke(input);
    }
}