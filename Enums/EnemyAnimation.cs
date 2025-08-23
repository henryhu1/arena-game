using System;

public enum EnemyAnimation
{
    Idle,
    Walk,
    Attack,
    Damage,
    Death,
}

[Serializable]
public struct EnemyAnimationName
{
    public EnemyAnimation key;
    public string value;
}
