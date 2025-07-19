using UnityEngine;

public class PlayerEquipables : MonoBehaviour, IPlayerComponent
{
    private PlayerManager manager;

    public void Initialize(PlayerManager manager)
    {
        this.manager = manager;
    }
}
