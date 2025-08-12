using UnityEngine;

public interface IInteractable
{
    public void DefenseTakeHit(float damage, UnitRecipe unitRecipe, float stun);
    public void DungeonTakeHit(float damage);
}
