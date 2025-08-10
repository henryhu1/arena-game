using UnityEngine;
using UnityEngine.AI;

public static class EnemyPositionUtils
{
  public static Vector3 GetPositionOnNavMesh(Vector3 requestedPos)
  {
    Vector3 roughPos = new(requestedPos.x, 100f, requestedPos.z);
    float maxDistance = 200f;
    if (NavMesh.SamplePosition(roughPos, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
    {
      return hit.position;
    }
    return Vector3.negativeInfinity;
  }
}