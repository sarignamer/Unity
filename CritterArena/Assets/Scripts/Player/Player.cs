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

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer != LayerMask.NameToLayer("Ground")) && collision.gameObject != this.gameObject)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Players"))
            {
                Debug.Log(collision.gameObject.name);
                Vector3 selfPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 colPos = new Vector3(collision.transform.position.x, 0, collision.transform.position.z);
                Vector3 dir = (selfPos - colPos).normalized;
                float force = GetCurrentSpeed() + collision.gameObject.GetComponent<Player>().GetCurrentSpeed();
                movement.AddKnockback(dir, force);
            }
        }
    }
}
