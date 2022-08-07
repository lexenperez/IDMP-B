using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : NPC
{
    public BoxCollider2D[] colliders;
    public SpriteRenderer[] sprites;
    public Transform[] projectileSpawnPlacements;
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

    //protected void PerformAttack(int animationIndex, int colliderIndex, float duration)
    //{

    //}

    //protected void ToggleHitbox(int colliderIndex)
    //{
    //    Debug.Log("Toggling Hitbox");
    //    colliders[colliderIndex].enabled = !colliders[colliderIndex].enabled;
    //}

    //protected void ToggleSprite(int spriteIndex)
    //{
    //    Debug.Log("Toggling Sprite");
    //    sprites[spriteIndex].enabled = !sprites[spriteIndex].enabled;
    //}

    protected GameObject SpawnProjectile(int projectileIndex, GameObject prefab)
    {
        Debug.Log("Spawning a projectile");
        GameObject go = Instantiate(prefab);
        go.transform.position = projectileSpawnPlacements[projectileIndex].position;
        return go;
        
    }

}
