using UnityEngine;

public interface IInteractable
{
    public void TakeHit(int damage, UnitType unit, float stun);
}
