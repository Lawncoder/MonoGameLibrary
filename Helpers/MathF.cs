using System;
using System.Numerics;
using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace Mario.Helpers;


public static class GameDevMath
{
    public static T Lerp<T>(T a, T b, float t)
    {
        return ((dynamic)b-a)*t + a;
    }

    public static float LerpWithClamp(float a, float b, float t)
    {
        var value =  Lerp(a, b, t);
        return Math.Clamp(value, Math.Min(a,b), Math.Max(a,b));
    }
    
    public static Vector2 Up = new Vector2(0, 1);

    public static Microsoft.Xna.Framework.Vector2 Physics2ScreenVector(Vector2 vector)
    {
        return new Microsoft.Xna.Framework.Vector2(vector.X, vector.Y) * 16f;
    }
 
    
}