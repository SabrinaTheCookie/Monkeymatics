using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float limbMovementSpeed;
    public float limbMaxSpeed;
    [Tooltip("The radius (units) from which limbs can grab objects.")]
    public float grabRadius;


    [Header("Limbs")] 
    public Rigidbody2D body2D;
    [Header("Left Upper")]
    public bool leftUpperActive;
    public bool leftUpperAttached;
    public Rigidbody2D leftUpper;
    public Rigidbody2D leftUpperHand;
    public static event Action<bool> OnLeftUpperAttached; 

    [Header("Left Lower")]
    public bool leftLowerActive;
    public bool leftLowerAttached;
    public Rigidbody2D leftLower;
    public Rigidbody2D leftLowerHand;
    public static event Action<bool> OnLeftLowerAttached; 
    
    [Header("Right Lower")]
    public bool rightLowerActive;
    public bool rightLowerAttached;
    public Rigidbody2D rightLower;
    public Rigidbody2D rightLowerHand;
    public static event Action<bool> OnRightLowerAttached; 

    [Header("Right Upper")]
    public bool rightUpperActive;
    public bool rightUpperAttached;
    public Rigidbody2D rightUpper;
    public Rigidbody2D rightUpperHand;
    public static event Action<bool> OnRightUpperAttached; 

    [Header("Tail")]
    public bool tailActive;
    public bool tailAttached;
    public Rigidbody2D tail;
    public Rigidbody2D tailHand;
    public static event Action<bool> OnTailAttached; 


    //Event Subscriptions
    void OnEnable()
    {
        InputManager.onLeftUpper += ToggleLeftUpper;
        InputManager.onLeftLower += ToggleLeftLower;
        InputManager.onRightUpper += ToggleRightUpper;
        InputManager.onRightLower += ToggleRightLower;
        InputManager.onTail += ToggleTail;
        InputManager.OnLookUpdated += MoveLimbs;
        InputManager.OnPrimaryPressed += AttachLimbs;
        InputManager.OnSecondaryPressed += DetachLimbs;
    }

    void OnDisable()
    {
        InputManager.onLeftUpper -= ToggleLeftUpper;
        InputManager.onLeftLower -= ToggleLeftLower;
        InputManager.onRightUpper -= ToggleRightUpper;
        InputManager.onRightLower -= ToggleRightLower;
        InputManager.onTail -= ToggleTail;
        InputManager.OnLookUpdated -= MoveLimbs;
        InputManager.OnPrimaryPressed -= AttachLimbs;
        InputManager.OnSecondaryPressed -= DetachLimbs;
    }

    private void ToggleLeftUpper(bool toggle)
    {
        leftUpperActive = toggle;
    }
    private void ToggleLeftLower(bool toggle)
    {
        leftLowerActive = toggle;
    }
    private void ToggleRightLower(bool toggle)
    {
        rightLowerActive = toggle;
    }
    private void ToggleRightUpper(bool toggle)
    {
        rightUpperActive = toggle;
    }
    private void ToggleTail(bool toggle)
    {
        tailActive = toggle;
    }

    private void MoveLimbs(Vector2 movementDelta)
    {
        
        float limbSpeed = Mathf.Min(limbMovementSpeed * movementDelta.magnitude, limbMaxSpeed);
        Vector2 limbMovement = limbSpeed * Time.fixedDeltaTime * movementDelta.normalized;
        
        if (leftUpperActive && !leftUpperAttached)
        {
            leftUpper.MovePosition(leftUpper.transform.position + (Vector3)limbMovement);
        }

        if (leftLowerActive && !leftLowerAttached)
        {
            leftLower.MovePosition((Vector2)leftLower.transform.position + limbMovement);
        }

        if (rightLowerActive && !rightLowerAttached)
        {
            rightLower.MovePosition((Vector2)rightLower.transform.position + limbMovement);
        }

        if (rightUpperActive && !rightUpperAttached)
        {
            rightUpper.MovePosition((Vector2)rightUpper.transform.position + limbMovement);
        }

        if (tailActive && !tailAttached)
        {
            tail.MovePosition((Vector2)tail.transform.position + limbMovement);
        }

        //If limb is attached AND active, move the body.
        if (leftUpperActive && leftUpperAttached || leftLowerActive &&  leftLowerAttached 
            || rightUpperActive && rightUpperAttached || rightLowerActive && rightLowerAttached
            || tailActive && tailAttached)
        {
            body2D.MovePosition((Vector2)transform.position + limbMovement);
        }
    }

    void AttachLimbs()
    {
        if (leftUpperActive)
        {
            if (TryGrabHand(leftUpperHand))
            {
                leftUpperAttached = true;
                OnLeftUpperAttached?.Invoke(true);
            }
        }

        if (leftLowerActive)
        {
            if (TryGrabHand(leftLowerHand))
            {
                leftLowerAttached = true;
                OnLeftLowerAttached?.Invoke(true);
            }


        }

        if (rightLowerActive)
        {
            if (TryGrabHand(rightLowerHand))
            {
                rightLowerAttached = true;
                OnRightLowerAttached?.Invoke(true);
            }
        }

        if (rightUpperActive)
        {
            if(TryGrabHand(rightUpperHand))
            {
                rightUpperAttached = true;
                OnRightUpperAttached?.Invoke(true);
            }
            
        }

        if (tailActive)
        {
            if (TryGrabHand(tailHand))
            {
                tailAttached = true;
                OnTailAttached?.Invoke(true);
            }
        }
        
    }

    private bool TryGrabHand(Rigidbody2D hand)
    {
        List<RaycastHit2D> grabHits;
        
        grabHits = Physics2D.CircleCastAll(hand.transform.position, grabRadius,
            Vector2.zero, 0, LayerMask.GetMask("Grabable")).ToList();
        
        foreach (var hit in grabHits)
        {
            //Test for hazards
            if (hit.transform.CompareTag("Hazard"))
            {
                //Grabbed a hazard, what happens?
                return true;
            }
            //Test for props
            if (hit.transform.CompareTag("Prop"))
            {
                //Grabbed a prop
                hit.transform.parent = transform;
                return true;
            }
            //Else grabbable surface
            hand.constraints = RigidbodyConstraints2D.FreezeAll;
            return true;
        }

        return false;
    }

    void DetachLimbs()
    {
        if (leftUpperActive)
        {
            leftUpperAttached = false;
            leftUpperHand.constraints = RigidbodyConstraints2D.None;
            OnLeftUpperAttached?.Invoke(false);
        }

        if (leftLowerActive)
        {
            leftLowerAttached = false;
            leftLowerHand.constraints = RigidbodyConstraints2D.None;
            OnLeftLowerAttached?.Invoke(false);

        }

        if (rightLowerActive)
        {
            rightLowerAttached = false;
            rightLowerHand.constraints = RigidbodyConstraints2D.None;
            OnRightLowerAttached?.Invoke(false);
        }

        if (rightUpperActive)
        {
            rightUpperAttached = false;
            rightUpperHand.constraints = RigidbodyConstraints2D.None;
            OnRightUpperAttached?.Invoke(false);
        }

        if (tailActive)
        {            
            tailAttached = false;
            tailHand.constraints = RigidbodyConstraints2D.None;
            OnTailAttached?.Invoke(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawWireSphere(leftUpper.transform.GetChild(0).position, grabRadius);
        Gizmos.DrawWireSphere(leftLower.transform.GetChild(0).position, grabRadius);
        Gizmos.DrawWireSphere(rightUpper.transform.GetChild(0).position, grabRadius);
        Gizmos.DrawWireSphere(rightLower.transform.GetChild(0).position, grabRadius);
        Gizmos.DrawWireSphere(tail.transform.GetChild(0).position, grabRadius);
    }
}