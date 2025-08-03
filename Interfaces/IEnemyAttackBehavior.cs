using System.Collections;

public interface IEnemyAttackBehavior : IEnemyComponent
{
    IEnumerator PerformAttack();
}