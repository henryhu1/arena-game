using System;
using System.Linq;

public class AnimationNameAttribute : Attribute
{
    public string Name { get; set; }
    public AnimationNameAttribute(string name)
    {
        Name = name;
    }
} 

public static class PlayerAnimationsExtensions
{
    public static T GetAttribute<T>(this Enum value) where T : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name)
            .GetCustomAttributes(false)
            .OfType<T>()
            .SingleOrDefault();
    }

    public static string GetAnimationName(this Enum value)
    {
        var animationName = GetAttribute<AnimationNameAttribute>(value);
        return animationName?.Name;
    }
}

public enum PlayerAnimations
{
    [AnimationName("Idle")] IDLE,
    [AnimationName("RunForward")] RUN_FORWARD,
    [AnimationName("Sprint")] SPRINT,
    [AnimationName("RunBackward")] RUN_BACKWARD,
    [AnimationName("RunBackwardRight")] RUN_BACKWARD_RIGHT,
    [AnimationName("RunBackwardLeft")] RUN_BACKWARD_LEFT,
    [AnimationName("RunLeft")] RUN_LEFT,
    [AnimationName("StrafeLeft")] STRAFE_LEFT,
    [AnimationName("RunRight")] RUN_RIGHT,
    [AnimationName("StrafeRight")] STRAFE_RIGHT,
    [AnimationName("Jump")] JUMP,
    [AnimationName("Jump_Down")] JUMP_DOWN,
    [AnimationName("Jump_Up")] JUMP_UP,
    [AnimationName("JumpWhileRunning")] JUMP_WHILE_RUNNING,
    [AnimationName("FallingLoop")] FALLING_LOOP,

    [AnimationName("PunchRight")] PUNCH_RIGHT,
    [AnimationName("PunchLeft")] PUNCH_LEFT,
    [AnimationName("MeleeAttack_OneHanded")] MELEE_ATTACK_ONE_HANDED,
}
