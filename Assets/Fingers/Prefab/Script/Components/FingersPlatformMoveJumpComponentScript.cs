//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 


using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Allow controlling a rigid body in 2D with jump, move and drop through platform ability
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Fingers Gestures/Component/Platform jump, move and drop down", 7)]
    public class FingersPlatformMoveJumpComponentScript : MonoBehaviour
    {
        private Rigidbody2D playerBody;

        [Tooltip("Max velocity in x and y direction. The x and y component will be clamped to this speed.")]
        [Range(1.0f, 128.0f)]
        public float MaxSpeed = 32.0f;

        [Tooltip("Force of a jump")]
        [Range(1.0f, 128.0f)]
        public float JumpForce = 16.0f;

        [Tooltip("How far the tap can move to still count as a jump")]
        [Range(0.3f, 5.0f)]
        public float JumpThresholdUnits = 3.0f;

        [Tooltip("The jump must happen with this seconds or it fails")]
        [Range(0.1f, 0.5f)]
        public float JumpThresholdSeconds = 0.3f;

        [Tooltip("Move speed multiplier")]
        [Range(0.01f, 10.0f)]
        public float MoveSpeed = 0.1f;

        [Tooltip("How far the pan must move before movements starts")]
        [Range(0.1f, 1.0f)]
        public float MoveThresholdUnits = 0.35f;

        private TapGestureRecognizer jumpTap;
        private PanGestureRecognizer movePan;
        private SwipeGestureRecognizer swipeDown;
        private readonly Collider2D[] overlapArray = new Collider2D[4];

        private void Start()
        {
            playerBody = GetComponent<Rigidbody2D>();

            jumpTap = new TapGestureRecognizer();
            jumpTap.StateUpdated += JumpTap_StateUpdated;
            jumpTap.ClearTrackedTouchesOnEndOrFail = true;

            // require fast taps
            jumpTap.ThresholdSeconds = JumpThresholdSeconds;

            // allow a little more slide than a normal tap
            jumpTap.ThresholdUnits = JumpThresholdUnits;

            movePan = new PanGestureRecognizer();
            movePan.StateUpdated += MovePan_StateUpdated;
            movePan.ThresholdUnits = 0.35f; // require a little more slide before panning starts

            // jump up and move sideways is allowed
            movePan.AllowSimultaneousExecution(jumpTap);

            // swipe down requires no other gestures to be executing
            swipeDown = new SwipeGestureRecognizer
            {
                Direction = SwipeGestureRecognizerDirection.Down
            };
            swipeDown.StateUpdated += SwipeDown_StateUpdated;
            swipeDown.AllowSimultaneousExecution(movePan);

            FingersScript.Instance.AddGesture(jumpTap);
            FingersScript.Instance.AddGesture(movePan);
            FingersScript.Instance.AddGesture(swipeDown);
        }

        private IEnumerator StopFallThrough(PlatformEffector2D effector)
        {
            yield return new WaitForSeconds(0.35f);
            effector.rotationalOffset = 0.0f;
        }

        private Collider2D FindIntersectingPlatform()
        {
            // find a platform intersecting the player - you could also tag the platform object
            //  or put it in a different layer, I have chosen to look at the object name
            ContactFilter2D filter = new ContactFilter2D();
            int count = playerBody.OverlapCollider(filter, overlapArray);
            for (int i = 0; i < count; i++)
            {
                if (overlapArray[i].name.EndsWith("Platform", System.StringComparison.OrdinalIgnoreCase))
                {
                    return overlapArray[i];
                }
            }
            return null;
        }

        private void SwipeDown_StateUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                // if on a platform, drop down
                Collider2D platform = FindIntersectingPlatform();
                if (platform != null)
                {
                    PlatformEffector2D effector = platform.GetComponent<PlatformEffector2D>();
                    if (effector != null)
                    {
                        // allow fall through
                        effector.rotationalOffset = -180.0f;

                        StartCoroutine(StopFallThrough(effector));
                    }
                }
            }
        }

        private void MovePan_StateUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                Vector2 velocity = playerBody.velocity;
                velocity.x += (gesture.VelocityX * Time.deltaTime * MoveSpeed);
                playerBody.velocity = velocity;
            }
        }

        private void JumpTap_StateUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                // if on a platform, jump
                if (FindIntersectingPlatform() != null)
                {
                    // jump, touching a platform
                    Vector2 velocity = playerBody.velocity;
                    velocity.y = JumpForce;
                    playerBody.velocity = velocity;
                }
            }
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            Vector2 velocity = playerBody.velocity;
            velocity.x = Mathf.Clamp(velocity.x, -MaxSpeed, MaxSpeed);
            velocity.y = Mathf.Clamp(velocity.y, -MaxSpeed, MaxSpeed);
            playerBody.velocity = velocity;
        }
    }
}
