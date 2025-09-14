using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
  public static Vector3 GetPositionOnNavMesh(Vector3 requestedPos, float maxDistance = 200f)
  {
    Vector3 roughPos = new(requestedPos.x, 100f, requestedPos.z);
    if (NavMesh.SamplePosition(roughPos, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
    {
      return hit.position;
    }
    return Vector3.negativeInfinity;
  }

  public static Vector3 GetRandomPositionInNavSphere(Vector3 origin, float distance)
  {
    Vector3 randomDirection = Random.insideUnitSphere * distance;
    randomDirection += origin;
    return GetPositionOnNavMesh(randomDirection);
  }
}