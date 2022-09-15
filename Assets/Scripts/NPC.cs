using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    /*
     * Parent class for basically any character that would exists in the gameworld
     * 
     */

    protected SpriteRenderer sprite;
    protected BoxCollider2D baseCollider;
    protected Rigidbody2D rb2d;
    protected AudioSource audioSource;

    // Find some more similar stuff
    //public Sprite[] spriteArray;
    public float hp;
    public GameObject healthBar;
    public float maxHp;
    public float speed;
    public GameObject thePlayer;

    protected void Init()
    {
        //maxHp = hp;
        sprite = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        baseCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // For animation change (simple sprites)
    //protected void SwapSprite(int spriteIndex)
    //{
    //    if (!(spriteIndex > spriteArray.Length))
    //    {
    //        sprite.sprite = spriteArray[spriteIndex];
    //    }
    //}

    // Flip sprite in the X axis (Horizontal)
    protected void FlipSprite()
    {
        sprite.flipX = !sprite.flipX;
    }


    // TODO move this somewhere, this is related to bosses
    protected void UpdateHealthBar()
    {
        healthBar.GetComponent<HPBossBar>().UpdateHealthBar();
    }

    // Assuming npcs either teleports to a location
    //protected void MoveTowards(Vector2 position)
    //{
    //    transform.position = position;
    //}

    // Assuming npcs walks towards a location with a sprite animation
    // Probably better way to animate but leave for now
    //protected void MoveTowards(Vector2 position, int[] animations, float animationTime)
    //{

    //}
}
