using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : NPC
{
    public BoxCollider2D[] colliders;
    public bool allowSelfHitbox;
    // Abstract class should handle all general animation changes and hitboxes to enable (at least, for general melee attacks)
    
    protected void Init()
    {
        base.Init();
        if (allowSelfHitbox)
        {
            base.baseCollider.enabled = true;
        }
    }

    // For any melee attack, it's simply just an animation followed by a collider enabled somewhere near the unit for a certain duration
    // For ranged or any other special attacks, probably need to be its own gameobject that handles itself

    private void performAttack(int animationIndex, int colliderIndex, float duration)
    {

    }

    private void toggleHitbox(int colliderIndex)
    {
        colliders[colliderIndex].enabled = !colliders[colliderIndex].enabled;
    }

}
