using System;
using System.Collections.Generic;
using Arch.Buffer;
using Arch.Core;
using Arch.System;
using Mario.Components;
using Mario.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.ECS;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;
using World = nkast.Aether.Physics2D.Dynamics.World;

namespace MonoGameLibrary;

public class Core : Game
{
    internal static Core instance;

    public static Core Instance => instance;
    // The scene that is currently active.
    private static Scene s_activeScene;

    // The next scene to switch to, if there is one.
    private static Scene s_nextScene;


    public static Arch.Core.World EntityWorld;

    public static Transform Camera { get; protected set; }
    public static CommandBuffer CommandBuffer { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    public static List<SystemBase> Systems;
    
    public static World PhysicsWorld { get; private set; }
    
    public static SpriteBatch SpriteBatch { get; private set; }
    
    public static new ContentManager Content { get; private set; }
    
    public static InputManager Input { get; private set; }
    public static bool ExitOnEscape { get; set; }
    
    public static AudioController Audio { get; private set; }

    public Core(string title, int width, int height, bool fullscreen)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }
        instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullscreen;
        
        Graphics.ApplyChanges();

        Window.Title = title;
        //tis a comment
        Content = base.Content;
        
        Content.RootDirectory = "Content";

        IsMouseVisible = true;
        CommandBuffer = new CommandBuffer();
        EntityWorld = Arch.Core.World.Create();
        PhysicsWorld = new World();
        Systems = new List<SystemBase>();
    }

   

    protected override void Initialize()
    {
        base.Initialize();
        
        GraphicsDevice =  base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Input = new InputManager();
        Audio = new AudioController();
        
        
        foreach (var system in Systems)
        {
            system.Initialize();
            
        }
        
    }

    private float _timeSinceLastPhysicsUpdateEnded = 0f;
    protected override void Update(GameTime gameTime)
    {
        
        foreach (var system in Systems)
        {
            system.BeforeUpdate(gameTime.ElapsedGameTime.Milliseconds/1000f);
        }
       
        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressedThisFrame(Keys.Escape))
        {
            Exit();
        }
        Audio.Update();
        foreach (var system in Systems)
        {
            system.Update(gameTime.ElapsedGameTime.Milliseconds/1000f);
        }
        // if there is a next scene waiting to be switch to, then transition
        // to that scene.
        if (s_nextScene != null)
        {
            TransitionScene();
        }

        // If there is an active scene, update it.
        if (s_activeScene != null)
        {
            s_activeScene.Update(gameTime);
        }

        _timeSinceLastPhysicsUpdateEnded += gameTime.ElapsedGameTime.Milliseconds;
        if (_timeSinceLastPhysicsUpdateEnded >= 20)
        {
            PhysicsUpdate();
        }
        
        base.Update(gameTime);
        foreach (var system in Systems)
        {
            system.AfterUpdate(gameTime.ElapsedGameTime.Milliseconds/1000f);
            
        }
        CommandBuffer.Playback(EntityWorld);
    }

    protected override void UnloadContent()
    {
        Audio.Dispose();
        base.UnloadContent();
    }
    protected override void Draw(GameTime gameTime)
    {
        // If there is an active scene, draw it.
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        base.Draw(gameTime);
    }

    public virtual void PhysicsUpdate()
    {
        Input.Update();
        
        PhysicsWorld.Step(0.02f);
        var query = new QueryDescription().WithAll<Transform, PhysicsComponent>();
        EntityWorld.Query(in query, (Entity entity, ref Transform transform, ref PhysicsComponent physics) =>
        {
            transform.Position = physics.Body.Position;
            transform.Rotation = physics.Body.Rotation;
        });
        
        foreach (var system in Systems)
        {
            system.PhysicsUpdate();

        }

        _timeSinceLastPhysicsUpdateEnded = 0;

    }

    public static void ChangeScene(Scene next)
    {
        // Only set the next scene value if it is not the same
        // instance as the currently active scene.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    private static void TransitionScene()
    {
        EntityWorld.Dispose();
        EntityWorld = Arch.Core.World.Create();
        PhysicsWorld.Clear();
        
        // If there is an active scene, dispose of it.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Force the garbage collector to collect to ensure memory is cleared.
        GC.Collect();

        // Change the currently active scene to the new scene.
        s_activeScene = s_nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        s_nextScene = null;

        // If the active scene now is not null, initialize it.
        // Remember, just like with Game, the Initialize call also calls the
        // Scene.LoadContent
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }
   
}