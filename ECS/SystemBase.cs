namespace MonoGameLibrary.ECS;

using Arch.Core;
using Arch.System;


public abstract class SystemBase : BaseSystem<World, float>
{
    public SystemBase(World world) : base(world) {}

    public virtual void PhysicsUpdate() {}
}