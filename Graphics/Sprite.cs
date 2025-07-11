using Mario;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class Sprite
{
    public TextureRegion TextureRegion { get; set; }
    
    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0.0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float LayerDepth { get; set; } = 0.0f;
    public float Width => TextureRegion.Width * Scale.X;
    public float Height => TextureRegion.Height * Scale.Y;

  
    
    public Sprite() {}

    public static Sprite FromFile(string filename)
    {
       
       
        var texture =  Core.Content.Load<Texture2D>(filename);
        var region = new TextureRegion(texture, 0,0, texture.Width, texture.Height);
        
        return new Sprite(region);
    }

    public Sprite(TextureRegion textureRegion)
    {
        TextureRegion = textureRegion;
        
        
    }

    public void CenterOrigin()
    {
        Origin = new Vector2(TextureRegion.Width * 0.5f, TextureRegion.Height * 0.5f);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
      
        var actualSpriteEffects = SpriteEffects.None;
        var newScale = Scale;
        if (Scale.X < 0f)
        {
           actualSpriteEffects = SpriteEffects.FlipHorizontally;
           newScale.X = -Scale.X;
        }

        if (Scale.Y < 0f)
        {
            actualSpriteEffects = SpriteEffects.FlipVertically;
            newScale.Y = -Scale.Y;
        }

        if (Effects != SpriteEffects.None)
        {
            actualSpriteEffects = Effects;
        }
        
        TextureRegion.Draw(spriteBatch, position, Color, Rotation, Origin, newScale, actualSpriteEffects, LayerDepth);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
       
        Draw(spriteBatch, Position);
    }

  


}