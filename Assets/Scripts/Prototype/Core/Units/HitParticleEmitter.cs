using System.Collections;
using UnityEngine;


public abstract class HitParticleEmitter : Unit
{
    [SerializeField] protected GameObject FXHit;

    public override bool AttackAt(Vector2Int coords)
    {
        Debug.Log("Drone.attack()");
        GameObject particle = Instantiate(FXHit);
        particle.transform.position = transform.position;
        Destroy(particle, 5.0f);
        return base.AttackAt(coords);
    }
}
