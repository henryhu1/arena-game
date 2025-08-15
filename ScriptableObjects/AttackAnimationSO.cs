using UnityEngine;

[CreateAssetMenu(fileName = "AttackAnimationSO", menuName = "Attack/Animation")]
public class AttackAnimationSO : ScriptableObject
{
    public float attackStartTime = 0.4f;
    public float attackEndTime = 0.65f;
    public PlayerAnimations animationName;
}
