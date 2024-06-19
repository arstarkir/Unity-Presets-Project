using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    PlayerInputActions playerInput;
    InputAction move, jump;

    float yVelocity = 0;

    [SerializeField] float jumpHeight = 1;
    [SerializeField] float speed = 2;
    [SerializeField] Transform groundChecker;
    public bool isGrounded = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = new PlayerInputActions();
    }
    private void OnEnable()
    {
        move = playerInput.Player.Move;
        jump = playerInput.Player.Jump;
        move.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }
    void Update()
    {
        isGrounded = IsGrounded();
        if (isGrounded && yVelocity < 0)
            yVelocity = 0f;

        yVelocity -= 9.81f * Time.deltaTime;

        if (isGrounded && jump.ReadValue<float>() > 0)
            Jump();

        Vector3 moveDirection = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
        moveDirection *= speed;

        moveDirection.y = yVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }
    void Jump()
    {
        yVelocity += Mathf.Sqrt(jumpHeight * 2 * 9.81f);
    }
    bool IsGrounded()
    {
        return Physics.Raycast(groundChecker.position, Vector3.down, 0.2f);
    }
}