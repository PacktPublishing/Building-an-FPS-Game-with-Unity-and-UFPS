using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class DamageHandler_Zombie : vp_DamageHandler
{

    private Animator animator;

	// Use this for initialization
	protected  override void Awake ()
	{
	    base.Awake();

	    animator = GetComponent<Animator>();
	}

    public override void Damage(vp_DamageInfo damageInfo)
    {
        base.Damage(damageInfo);

        if (CurrentHealth > 0)
        {
            animator.Play("hit", 0, 0f);
        }
        
    }

    public override void Die()
    {
        if (!enabled || !vp_Utility.IsActive(gameObject))
            return;

        if (m_Audio != null)
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot(DeathSound);
        }

        foreach (GameObject o in DeathSpawnObjects)
        {
            if (o != null)
            {
                GameObject g = (GameObject)vp_Utility.Instantiate(o, Transform.position, Transform.rotation);
                if ((Source != null) && (g != null))
                    vp_TargetEvent<Transform>.Send(g.transform, "SetSource", OriginalSource);
            }
        }

        animator.Play("back_fall");

        Destroy(GetComponent<ZombieBehaviour>());
        Destroy(GetComponent<NavMeshAgent>());
    }
}
