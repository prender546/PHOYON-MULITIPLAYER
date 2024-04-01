using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
    {
        public Transform center;
        
        public CinemachineVirtualCamera playerCamera;
        private float walkSpeed = 200;

        private CharacterController controller;
        public Animator animator;

        private Vector3 currentTargetRotation;
        private Vector3 timeToReachTargetRotation;
        private Vector3 dampedTargetRotationCurrentVelocity;
        private Vector3 dampedTargetRotationPassedTime;

        public bool canMove = true;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }

            timeToReachTargetRotation.y = 0.14f;

            if (playerCamera)
            {
                playerCamera.Follow = center;
                playerCamera.LookAt = center;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void FixedUpdate()
        {
            if (!canMove) return;

            Move();
        }
        private void Move()
        {
            var v = Input.GetAxis("Vertical");
            var h = Input.GetAxis("Horizontal");

            animator.SetBool("moving", v != 0 || h != 0);

            if (v == 0 && h == 0)
            {
                controller.SimpleMove(Vector3.zero);
                return;
            }

            var direction = GetDirection(h, v);
            var rotation = GetRotation();
            var speed = direction * walkSpeed * Time.deltaTime;

            if (rotation.HasValue)
            {
                transform.rotation = rotation.Value;
            }

            controller.SimpleMove(speed);
        }

        private Vector3 GetDirection(float x, float y)
        {
            var angle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;

            if (angle < 0)
            {
                angle += 360;
            }

            angle += playerCamera.transform.eulerAngles.y;

            if (angle > 360)
            {
                angle -= 360;
            }

            if (angle != currentTargetRotation.y)
            {
                currentTargetRotation.y = angle;
                dampedTargetRotationPassedTime.y = 0;
            }

            return Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }

        private Quaternion? GetRotation()
        {
            var currentYRotation = transform.eulerAngles.y;

            if (currentYRotation == currentTargetRotation.y) return null;

            var angle = Mathf.SmoothDampAngle(currentYRotation, currentTargetRotation.y,
                ref dampedTargetRotationCurrentVelocity.y,
                timeToReachTargetRotation.y - dampedTargetRotationPassedTime.y);

            dampedTargetRotationPassedTime.y += Time.deltaTime;

            return Quaternion.Euler(0, angle, 0);
        }
    }