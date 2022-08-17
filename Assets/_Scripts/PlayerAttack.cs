using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;

[RequireComponent(typeof(PathCreator), typeof(TrailRendererLocal))]
public class PlayerAttack : MonoBehaviour
{
    // References
    private PathCreator pathCreator;
    [SerializeField] private PolygonCollider2D polyCollider;
    private TrailRendererLocal trailRendererLocalScript;

    [Header("Attack Configurations")]
    [SerializeField, Range(0.01f, 5f)] private float attackTime = 1f;
    [SerializeField, Range(0.01f, 5f)] private float attackCooldown = 1f;

    // Attack
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        pathCreator = GetComponent<PathCreator>();
        trailRendererLocalScript = GetComponent<TrailRendererLocal>();
        
        polyCollider.enabled = false;

        // Set trail to beginning
        trailRendererLocalScript.objToFollow.transform.position = pathCreator.path.GetPointAtDistance(0);
        trailRendererLocalScript.objToFollow.transform.rotation = pathCreator.path.GetRotationAtDistance(0);
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && canAttack)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        trailRendererLocalScript.SetEmit(true);
        polyCollider.enabled = true;

        float attackTimeCounter = 0;
        float distanceTravelled = 0;

        // Start Attack
        while (attackTimeCounter < attackTime)
        {
            attackTimeCounter += Time.deltaTime;
            distanceTravelled += (pathCreator.path.length / attackTime) * Time.deltaTime;
            trailRendererLocalScript.objToFollow.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
            trailRendererLocalScript.objToFollow.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
            yield return null; // Continue to run until next frame
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
