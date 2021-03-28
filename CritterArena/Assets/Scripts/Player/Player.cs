using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MovementHandler))]
public class Player : MonoBehaviour
{
    private MovementHandler movement;
    private PlayerInputHandler inputHandler;
    private GameObject playerCharacter;

    [Header("Movement")]
    public float moveSpeed = 5;


    [Header("Dash")]
    public float dashTime = 0.1f;
    public float dashSpeed = 20;

    private void Awake()
    {
        movement = GetComponent<MovementHandler>();
    }

    public void InitializePlayer(GameObject playerCharacter, PlayerInputHandler inputHandler)
    {
        this.playerCharacter = Instantiate(playerCharacter, this.transform);
        RegisterInputCommands(inputHandler);
    }

    private void RegisterInputCommands(PlayerInputHandler inputHandler)
    {
        inputHandler.RegisterOnMoveAction(Move);
        inputHandler.RegisterOnDashAction(Dash);
    }

    public void Move(Vector3 direction)
    {
        movement.SetMoveDirection(direction);
    }

    public void Dash()
    {
        movement.StartDash();
    }

    public float GetCurrentSpeed()
    {
        return movement.CurrentSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.layer != LayerMask.NameToLayer("Ground")) && other.gameObject != this.gameObject)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Players"))
            {
                Vector3 selfPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 colPos = new Vector3(other.transform.position.x, 0, other.transform.position.z);
                Vector3 forceDir = (selfPos - colPos).normalized;
                Vector3 selfVelocity = GetComponent<CharacterController>().velocity;
                Vector3 otherVelocity = other.gameObject.GetComponent<CharacterController>().velocity;

                float force = otherVelocity.magnitude - selfVelocity.magnitude;

                force = force > 0 ? force : force / 4;

                Debug.Log(gameObject.name + " self:" + selfVelocity);
                Debug.Log(gameObject.name + " other:" + otherVelocity);
                Debug.Log(gameObject.name + " force:" + force);
                movement.AddKnockback(forceDir, force);
            }
        }
    }
}
