using KinematicCharacterController;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultMovementState : CharacterState
{
    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f; // Max speed when stable on ground
    public float StableMovementSharpness = 15; // Sharpness of the acceleration when stable on ground
    public float MaxStableDistanceFromLedge = 5f;
    [Range(0f, 180f)]
    public float MaxStableDenivelationAngle = 180f;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 10f; // Max speed for air movement
    public float AirAccelerationSpeed = 3f; // Acceleration when in air
    public float Drag = 0.1f; // Air drag

    [Header("Jumping")]
    public bool AllowJumpingWhenSliding = false; // Is jumping allowed when we are sliding down a surface, even if we are not "stable" on it?
    public bool AllowDoubleJump = false;
    public bool AllowWallJump = false;
    public float JumpSpeed = 9f;
    public float JumpPreGroundingGraceTime = 0f; // Time before landing that jump inputs will be remembered and applied at the moment of landing
    public float JumpPostGroundingGraceTime = 0f; // Time after getting un-grounded that jumping will still be allowed

    public bool IsCrouching { get; private set; }
    private Collider[] probedColliders = new Collider[8];
    private bool jumpRequested = false;
    private bool jumpConsumed = false;
    private bool doubleJumpConsumed = false;
    private bool jumpedThisFrame = false;
    private bool canWallJump = false;
    private Vector3 wallJumpNormal;
    private float timeSinceJumpRequested = Mathf.Infinity;
    private float timeSinceLastAbleToJump = 0f;
    private Vector3 internalVelocityAdd = Vector3.zero;
    private bool shouldBeCrouching = false;

    public override void OnStateEnter(CharacterState previousState)
    {
        if (HelpOverlayManager.GeneralHelpOverlayShouldBeDisplayed())
        {
            HelpOverlayManager.SetPanel(HelpPanel.General);
        }

        Motor.SetStabilitySolvingActivation(true);
    }

    public override void OnStateExit(CharacterState nextState)
    {
    }

    public override void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public override void AfterCharacterUpdate(float deltaTime)
    {
        // Handle jumping pre-ground grace period
        if (jumpRequested && timeSinceJumpRequested > JumpPreGroundingGraceTime)
        {
            jumpRequested = false;
        }

        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
        {
            // If we're on a ground surface, reset jumping values
            if (!jumpedThisFrame)
            {
                doubleJumpConsumed = false;
                jumpConsumed = false;
            }

            timeSinceLastAbleToJump = 0f;
        }
        else
        {
            // Keep track of time since we were last able to jump (for grace period)
            timeSinceLastAbleToJump += deltaTime;
        }

        // Handle landing and leaving ground
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }

        // Handle uncrouching
        if (IsCrouching && !shouldBeCrouching)
        {
            // Do an overlap test with the character's standing height to see if there are any obstructions
            Motor.SetCapsuleDimensions(C.BaseCharacterRadius, C.BaseCharacterHeight, 0f);
            if (Motor.CharacterOverlap(
                Motor.TransientPosition,
                Motor.TransientRotation,
                probedColliders,
                Motor.CollidableLayers,
                QueryTriggerInteraction.Ignore) > 0)
            {
                // If obstructions, just stick to crouching dimensions
                Motor.SetCapsuleDimensions(C.BaseCharacterRadius, C.BaseCharacterHeight, C.BaseCharacterHeight / 2f);
            }
            else
            {
                // If no obstructions, uncrouch
                C.MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                IsCrouching = false;
            }
        }
    }

    public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (C.TargetLookDirection != Vector3.zero && C.OrientationSharpness > 0f)
        {
            // Smoothly interpolate from current to target look direction
            C.SmoothedLookDirection = Vector3.Lerp(C.SmoothedLookDirection, C.TargetLookDirection, 1f - Mathf.Exp(-C.OrientationSharpness * deltaTime)).normalized;

            // Set the current rotation (which will be used by the KinematicCharacterMotor)
            currentRotation = Quaternion.LookRotation(C.SmoothedLookDirection, Motor.CharacterUp);
        }

        if (C.OrientTowardsGravity)
        {
            // Rotate from current up to invert gravity
            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -C.Gravity) * currentRotation;
        }
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 targetMovementVelocity = Vector3.zero;
        CharacterGroundingReport ground = Motor.GroundingStatus;

        // Ground movement
        if (ground.IsStableOnGround)
        {
            // Reorient current velocity on the ground slope before smoothing (important to avoid velocity loss in slope changes)
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, ground.GroundNormal) * currentVelocity.magnitude;

            // Calculate target velocity (still oriented on ground slope)
            Vector3 inputRight = Vector3.Cross(C.WorldspaceCharacterPlaneMoveInputVector, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(ground.GroundNormal, inputRight).normalized * C.WorldspaceCharacterPlaneMoveInputVector.magnitude;
            float maxSpeed = GInput.SlowModifier ? MaxStableMoveSpeed / C.SlowSpeedDivider : MaxStableMoveSpeed;
            targetMovementVelocity = reorientedInput * maxSpeed;

            // Smoothly interpolate to target velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));

            // Add some gravity if we're grounded but not snapping to ground
            // (this situation can happen when we want to "launch off" ledges or declining slopes)
            //if (Motor.GroundingStatus.IsPreventingSnapping)
            //{
                //currentVelocity += Gravity * deltaTime;
            //}
        }
        // Air movement
        else
        {
            if (C.WorldspaceCharacterPlaneMoveInputVector.sqrMagnitude > 0f)
            {
                // If we want to move, add an acceleration to the velocity
                float maxSpeed = GInput.SlowModifier ? MaxAirMoveSpeed / C.SlowSpeedDivider : MaxAirMoveSpeed;
                targetMovementVelocity = C.WorldspaceCharacterPlaneMoveInputVector * maxSpeed;

                // Prevent climbing on un-stable slopes with air movement
                if (Motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, C.Gravity);
                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
            }

            // Gravity
            currentVelocity += C.Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }

        // Handle jumping
        jumpedThisFrame = false;
        timeSinceJumpRequested += deltaTime;
        if (jumpRequested)
        {
            // Handle double jump
            if (AllowDoubleJump)
            {
                if (jumpConsumed && !doubleJumpConsumed && (AllowJumpingWhenSliding ? !ground.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                {
                    Motor.ForceUnground();

                    // Add to the return velocity and reset jump state
                    currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                    jumpRequested = false;
                    doubleJumpConsumed = true;
                    jumpedThisFrame = true;
                }
            }

            // See if we actually are allowed to jump
            if (canWallJump ||
                (!jumpConsumed && ((AllowJumpingWhenSliding ? ground.FoundAnyGround : ground.IsStableOnGround) || timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
            {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = Motor.CharacterUp;
                if (canWallJump)
                {
                    jumpDirection = wallJumpNormal;
                }
                else if (ground.FoundAnyGround && !ground.IsStableOnGround)
                {
                    jumpDirection = ground.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                Motor.ForceUnground();

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                jumpRequested = false;
                jumpConsumed = true;
                jumpedThisFrame = true;
            }
        }

        // Reset wall jump
        canWallJump = false;

        // Take into account additive velocity
        if (internalVelocityAdd.sqrMagnitude > 0f)
        {
            currentVelocity += internalVelocityAdd;
            internalVelocityAdd = Vector3.zero;
        }
    }

    public override void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        // Find all scenarios where stability should be canceled
        if (hitStabilityReport.IsStable)
        {
            // "Launching" off of slopes of a certain denivelation angle
            if (hitStabilityReport.InnerNormal.sqrMagnitude != 0f && hitStabilityReport.InnerNormal.sqrMagnitude != 0f)
            {
                float denivelationAngle = Vector3.Angle(hitStabilityReport.InnerNormal, hitStabilityReport.OuterNormal);
                if (denivelationAngle > MaxStableDenivelationAngle)
                {
                    hitStabilityReport.PreventGroundSnapping = true;
                }
            }

            // Ledge stability
            if (hitStabilityReport.LedgeDetected)
            {
                hitStabilityReport.PreventGroundSnapping = true;

                if (hitStabilityReport.IsOnEmptySideOfLedge && hitStabilityReport.DistanceFromLedge > MaxStableDistanceFromLedge)
                {
                    hitStabilityReport.IsStable = false;
                }
            }
        }
    }

    public override bool IsColliderValidForCollisions(Collider coll)
    {
        return !C.IgnoredColliders.Contains(coll);
    }

    public override void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public override void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // We can wall jump only if we are not stable on ground and are moving against an obstruction
        if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
        {
            canWallJump = true;
            wallJumpNormal = hitNormal;
        }
    }

    /// <summary>
    /// This is called by MyPlayer upon jump input press
    /// </summary>
    public void OnJump()
    {
        // Init jumping values
        timeSinceJumpRequested = 0f;
        jumpRequested = true;
    }

    /// <summary>
    /// This is called by MyPlayer upon crouch input press
    /// </summary>
    public void OnCrouch(bool crouch)
    {
        shouldBeCrouching = crouch;

        if (crouch)
        {
            IsCrouching = true;
            Motor.SetCapsuleDimensions(C.BaseCharacterRadius, C.BaseCharacterHeight, C.BaseCharacterHeight / 2f);
            C.MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
        }
    }

    protected void OnLanded()
    {
    }

    protected void OnLeaveStableGround()
    {
    }

    public void AddVelocity(Vector3 velocity)
    {
        internalVelocityAdd += velocity;
    }
}