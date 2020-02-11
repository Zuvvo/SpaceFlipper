using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollisionDetector : MonoBehaviour
{
    public PlayerShip Ship;

    public List<ShipCollisionRaycaster> Raycasters;
    public List<ShipCollisionRaycaster> RaycastersForFrames;

    private ShipCollisionRaycaster currentraycaster;
    private RaycastHit2D rayHit;

    public void RaycastForCollisionsWithFrame()
    {
        for (int i = 0; i < RaycastersForFrames.Count; i++)
        {
            currentraycaster = RaycastersForFrames[i];
            BlockMovmentOnRaycastHit();
        }
    }

    private void BlockMovmentOnRaycastHit()
    {
        switch (currentraycaster.RaycastDirection)
        {
            case CollisionSide.Top:
                rayHit = Physics2D.Raycast(currentraycaster.transform.position, Vector2.up, PhysicsConstants.MinDistanceToFrames, LayerConstants.Frame);
                //Debug.DrawRay(currentraycaster.transform.position, Vector2.up);
                Ship.ShipController.MovingUpBlockedByCollisionDetector = rayHit.collider != null;
                break;
            case CollisionSide.Bottom:
                rayHit = Physics2D.Raycast(currentraycaster.transform.position, Vector2.down, PhysicsConstants.MinDistanceToFrames, LayerConstants.Frame);
                Ship.ShipController.MovingDownBlockedByCollisionDetector = rayHit.collider != null;
                break;
            case CollisionSide.Left:
                rayHit = Physics2D.Raycast(currentraycaster.transform.position, Vector2.left, PhysicsConstants.MinDistanceToFrames, LayerConstants.Frame);
                Ship.ShipController.MovingLeftBlockedByCollisionDetector = rayHit.collider != null;
                break;
            case CollisionSide.Right:
                rayHit = Physics2D.Raycast(currentraycaster.transform.position, Vector2.right, PhysicsConstants.MinDistanceToFrames, LayerConstants.Frame);
                Ship.ShipController.MovingRightBlockedByCollisionDetector = rayHit.collider != null;
                break;
        }
    }
}
