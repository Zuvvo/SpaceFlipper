using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollider
{

}

public interface IRayCollider : ICollider
{
    void Raycast();
    List<IRayCollider> RayCollision(List<IRayCollider> collidersToSkip);
    void RegisterObject();
    void RegisterCollision(RaycastHit2D rayHit);
    void OnUpdate();
    void Unregister();
}