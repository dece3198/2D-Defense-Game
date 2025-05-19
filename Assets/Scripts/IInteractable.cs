using UnityEngine;

public interface IInteractable
{
    public void TakeHit(float damage, UnitType unit, float stun);
}
