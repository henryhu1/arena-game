using System.Collections;

public interface IEnemyAttackBehavior : IEnemyComponent
{
    void Setup();
    IEnumerator PerformAttack();
}