#region
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion

public class InputManager : MonoBehaviour
{
    [SerializeField, ReadOnly] Vector2 moveInput;

    public Vector2 MoveInput => moveInput;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    // public void OnInteract(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         Ray ray = Helpers.CameraMain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    //         float maxDistance = 3f;
    //         float radius = 0.5f;
    //
    //         if (Physics.SphereCast(ray, radius, out RaycastHit hitInfo, maxDistance, LayerMask.GetMask("Hit")))
    //         {
    //             if (hitInfo.collider.TryGetComponent(out IInteractable interactable))
    //             {
    //                 interactable.Interact();
    //                 Logger.Log($"Interacted with {hitInfo.collider.name}", this, "Interact");
    //             }
    //         }
    //
    //         Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 1f);
    //     }
    // }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }
}
