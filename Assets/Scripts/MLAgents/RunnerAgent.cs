using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Unity.MLAgents.DecisionRequester;

public class RunnerAgent : Agent
{

    [Header("Agent")]

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // Agent
    private float animationBlend;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    // timeout deltatime
    private float fallTimeoutDelta;

    public Transform objective;
    private float maxDistance = 1;
    public Transform[] spawnPoints;
    public bool inTraining;
    private Rigidbody rb;
    private Animator animator;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private bool hasAnimator;

    [SerializeField]
    private RuntimeAnimatorController animatorController;

    //Decision Requester
    [Header("Decision Requester")]
    public bool isReady = true;
    [Range(1, 20)]
    public int decisionPeriod = 5;
    public bool takeActionsBetweenDecisions = true;

    protected override void Awake()
    {
        base.Awake();
        Academy.Instance.AgentPreStep += OnAgentReady;
    }

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        rb = GetComponent<Rigidbody>();
        if(inTraining)
            AssignAnimationIDs();
    }

    private void OnDestroy()
    {
        if (Academy.IsInitialized)
            Academy.Instance.AgentPreStep -= OnAgentReady;
    }

    public override void OnEpisodeBegin()
    {
        if (objective != null && spawnPoints.Length > 1 && inTraining)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            int idxObjective = Random.Range(0, spawnPoints.Length);
            transform.localPosition = spawnPoints[idx].localPosition;
            if (idxObjective == idx)
                objective.localPosition = idxObjective == spawnPoints.Length - 1 ?
                    new Vector3(spawnPoints[0].localPosition.x, objective.localPosition.y, spawnPoints[0].localPosition.z) :
                    new Vector3(spawnPoints[idx + 1].localPosition.x, objective.localPosition.y, spawnPoints[idx + 1].localPosition.z);
            else
                objective.localPosition = new Vector3(spawnPoints[idxObjective].localPosition.x, objective.localPosition.y, spawnPoints[idxObjective].localPosition.z);

            //Vir a mudar
            maxDistance = Vector3.Distance(transform.localPosition, objective.localPosition);
        }
        Physics.SyncTransforms();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ca = actionsOut.ContinuousActions;
        ca[0] = Input.GetAxis("Vertical");
        ca[1] = Input.GetAxis("Horizontal");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveSpeed = 15f;
        float rotateSpeed = 150f;
        float move = (actions.ContinuousActions[0] + 1) / 2;// entre 0 e 1
        float rotate = actions.ContinuousActions[1];

        transform.Rotate(new Vector3(0, rotate * Time.fixedDeltaTime * rotateSpeed, 0));
        rb.velocity = (transform.forward * move * moveSpeed) + (transform.up * verticalVelocity);

        animationBlend = Mathf.Lerp(animationBlend, move * moveSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        if (hasAnimator)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, move);
        }

        JumpAndGravity();
        GroundedCheck();

        if (inTraining)
        {
            if (Vector3.Distance(transform.localPosition, objective.localPosition) >= maxDistance)
                AddReward(-1f / MaxStep);
            else
                AddReward((-1) * (Vector3.Distance(transform.localPosition, objective.localPosition) / maxDistance) / MaxStep);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (objective != null)
        {
            Vector3 direction = (objective.position - transform.position).normalized;
            sensor.AddObservation(direction.x);
            sensor.AddObservation(direction.z);
            sensor.AddObservation(Mathf.Clamp01(Vector3.Distance(transform.localPosition, objective.localPosition)/maxDistance));
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Objective")) && inTraining)
        {
            AddReward(collision.gameObject.CompareTag("Objective") ? 1f : -1f);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Objective") && !inTraining)
            gameObject.SetActive(false);
        
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void OnAgentReady(int academyStepCount)
    {
        if (isReady)
        {
            var context = new DecisionRequestContext
            {
                AcademyStepCount = academyStepCount
            };

            if (ShouldRequestDecision(context))
            {
                RequestDecision();
            }

            if (ShouldRequestAction(context))
            {
                RequestAction();
            }
        }
    }

    private bool ShouldRequestDecision(DecisionRequestContext context)
    {
        return context.AcademyStepCount % decisionPeriod == 0;
    }

    private bool ShouldRequestAction(DecisionRequestContext context)
    {
        return takeActionsBetweenDecisions;
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (hasAnimator)
                animator.SetBool(animIDFreeFall, false);

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
                verticalVelocity = -2f;
        }
        else
        {
            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else if (hasAnimator)
            {
                // update animator if using character
                animator.SetBool(animIDFreeFall, true);
            }
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
            verticalVelocity += Gravity * Time.deltaTime;
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        if (hasAnimator)
            animator.SetBool(animIDGrounded, Grounded);
    }

    public void ReadyAgent()
    {
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        animator.runtimeAnimatorController = animatorController as RuntimeAnimatorController;
        AssignAnimationIDs();
        inTraining = false;
        isReady = true;
    }
}
