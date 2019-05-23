using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotableObject : vp_Grab {

	Vector3 origPos;
	Quaternion origRot;

	enum GrabState{
		Default,
		PickUp,
		Examine,
		PutBack

	};

	// FOr some reason this was  private:
	vp_FPPlayerEventHandler m_FPPlayer = null;
	vp_FPPlayerEventHandler FPPlayer
	{
		get
		{
			if (m_FPPlayer == null)
				m_FPPlayer = (m_Player as vp_FPPlayerEventHandler);
			return m_FPPlayer;
		}
	}

	private GrabState state;

	protected override void Start()
	{
		base.Start();
		state = GrabState.Default;
		origPos = transform.position;
		origRot = transform.rotation;
	}


	protected override void Update()
	{
		base.Update ();


		if (FPPlayer && FPPlayer.Crosshair.Get() == m_InteractCrosshair) 
		{
			if (FPPlayer.InputGetButtonDown.Send ("Attack"))
			{
				TryInteract (m_Player);
			}
		}
	}


	public override bool TryInteract(vp_PlayerEventHandler player)
	{
		return base.TryInteract(player);
	}


	protected override void StartGrab()
	{
		base.StartGrab ();

		var input = m_Player.GetComponent<vp_FPInput> ();
		input.MouseLookMutePitch = true;
		input.MouseLookMuteYaw = true;
	}


	/// <summary>
	/// Stops the grab state.
	/// </summary>
	protected override void StopGrab()
	{
		
		base.StopGrab();

		// Check if throwing or putting down
		if (!(FPPlayer.InputGetButtonDown.Send ("Attack"))) 
		{
			// Returns object to original spot
			transform.position = origPos;
			transform.rotation = origRot;

			if (Rigidbody != null) {
				Rigidbody.velocity = Vector3.zero;
			}
		}

		var input = m_Player.GetComponent<vp_FPInput> ();
		input.MouseLookMutePitch = false;
		input.MouseLookMuteYaw = false;

	}

	/// <summary>
	/// Optionally slows the player down while carrying this grabbable.
	/// </summary>
	protected override void UpdateBurden()
	{

		base.UpdateBurden ();

		// Also stop the player from moving while holding
		m_Player.Attack.Stop ();
		m_Controller.Stop ();



	}

	protected override void UpdateRotation()
	{
		var movement = new Vector3 (Input.GetAxis ("Mouse X"), 0, Input.GetAxis ("Mouse Y"));
		m_Transform.Rotate (movement);
	}

	protected override void UpdatePosition()
	{
		base.UpdatePosition ();

		//m_Transform.Rotate (0, 0, 1);
	}
}
