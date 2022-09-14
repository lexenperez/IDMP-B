using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;

[RequireComponent(typeof(PathCreator), typeof(TrailRendererLocal))]
public class PlayerAttack : MonoBehaviour
{
    // References
    private SpriteRenderer spriteRenderer;
    private PathCreator pathCreator;
    [SerializeField] private PolygonCollider2D polyCollider;
    private TrailRendererLocal trailRendererLocalScript;
    [SerializeField] private GameObject flipCollider;

    [Header("Attack Configurations")]
    [SerializeField, Range(0.01f, 5f)] private float attackTime = 1f;
    [SerializeField, Range(0.01f, 5f)] private float attackCooldown = 1f;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackSfxs;
    private int currSfx = 0;

    // Attack
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathCreator = GetComponent<PathCreator>();
        trailRendererLocalScript = GetComponent<TrailRendererLocal>();
        audioSource = GetComponent<AudioSource>();
        
        polyCollider.enabled = false;

        // Set trail to beginning
        trailRendererLocalScript.objToFollow.transform.position = pathCreator.path.GetPointAtDistance(0);
        trailRendererLocalScript.objToFollow.transform.rotation = pathCreator.path.GetRotationAtDistance(0);

        if (flipCollider == null)
            Debug.LogError("Missing child object to flip. Please assign.");
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && canAttack && !PauseMenu.gameIsPaused)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        trailRendererLocalScript.SetEmit(true);
        polyCollider.enabled = true;
        bool attackLeft = spriteRenderer.flipX;

        // Flip attack collider
        if (attackLeft)
            flipCollider.transform.rotation = Quaternion.Euler(0, 180f, 0);
        else
            flipCollider.transform.rotation = Quaternion.identity;

        Transform trailTransform = trailRendererLocalScript.objToFollow.transform;

        float attackTimeCounter = 0;
        float distanceTravelled = 0;

        // Start Attack
        while (attackTimeCounter < attackTime)
        {
            attackTimeCounter += Time.deltaTime;
            distanceTravelled += (pathCreator.path.length / attackTime) * Time.deltaTime;

            Vector3 trailPosition = pathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);

            if (attackLeft)
                trailTransform.position = trailPosition + (new Vector3(trailPosition.x - transform.position.x, 0, 0) * -2f);   
            else
                trailTransform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);

            trailTransform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
            yield return null; // Continue to run until next frame
        }

        if (attackSfxs.Length > 0)
        {
            audioSource.PlayOneShot(attackSfxs[currSfx], 0.2f);
            currSfx++;
            if (currSfx >= attackSfxs.Length) currSfx = 0;
        }

        // Finshed Attacking
        polyCollider.enabled = false;
        trailRendererLocalScript.SetEmit(false);

        // Reset trail to beginning
        trailRendererLocalScript.objToFollow.transform.position = pathCreator.path.GetPointAtDistance(0);
        trailRendererLocalScript.objToFollow.transform.rotation = pathCreator.path.GetRotationAtDistance(0);

        // Clear trail
        trailRendererLocalScript.Reset();

        yield return new WaitForSeconds(attackCooldown - attackTimeCounter);

        canAttack = true;
    }
}
