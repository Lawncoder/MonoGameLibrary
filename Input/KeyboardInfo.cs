using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class KeyboardInfo
{
    public KeyboardState KeyboardState;
    public KeyboardState LastKeyboardState;

    public KeyboardInfo()
    {
        KeyboardState = Keyboard.GetState();
        LastKeyboardState = new KeyboardState();
    }

    public void Update()
    {
        LastKeyboardState = KeyboardState;
        KeyboardState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
    {
        return KeyboardState.IsKeyDown(key);
        
    }

    public bool IsKeyUp(Keys key)
    {
        return KeyboardState.IsKeyUp(key);
    }

    public bool WasKeyJustPressedThisFrame(Keys key)
    {
        return KeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key);
    }

    public bool WasKeyJustReleasedThisFrame(Keys key)
    {
        return KeyboardState.IsKeyUp(key) && LastKeyboardState.IsKeyDown(key);
    }
    
}