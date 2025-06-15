using UnityEngine;

public interface IInteractable
{
    public void TakeHit(float damage, UnitRecipe unitRecipe, float stun);
}
