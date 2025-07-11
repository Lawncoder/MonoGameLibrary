using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private TimeSpan _elapsedTime;
    private int _currentFrame;
    private Animation _animation;
    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            TextureRegion = _animation.Frames[0];
        }
    }

    public AnimatedSprite(){}

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public void Update(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime;
        if (_elapsedTime >= _animation.Delay)
        {
            _elapsedTime -= _animation.Delay;
            _currentFrame++;
            _currentFrame %= _animation.Frames.Count;
            TextureRegion = _animation.Frames[_currentFrame];
        }
    }
}