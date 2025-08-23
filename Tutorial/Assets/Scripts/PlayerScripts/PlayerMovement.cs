using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player {
    public class PlayerMovement : MonoBehaviour {

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] GameInput gameInput;
        [SerializeField] LayerMask collidingMask;

        private bool isWalking = false;
        private Rigidbody rb;

        void Start() {
            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            Vector2 inputVector = gameInput.GetMovementVectorNormalized();
            Move(inputVector);
        }

        public void Move(Vector2 inputVector) {
            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
            isWalking = moveDir != Vector3.zero;


            if (!IsPlayerColliding(moveDir)) {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            } else {
                // Attempt to move on X or Z only
                Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);
                if (!IsPlayerColliding(moveDirX)) {
                    transform.position += moveDirX * moveSpeed * Time.deltaTime;
                } else if (!IsPlayerColliding(moveDirZ)) {
                    transform.position += moveDirZ * moveSpeed * Time.deltaTime;
                }
            }
        }

        private bool IsPlayerColliding(Vector3 moveDir) {
            float moveDistance = moveSpeed * Time.deltaTime;
            float playerRadius = 0.7f;
            float playerHeight = 2f;
            return Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance, collidingMask);
        }

        public bool IsWalking() {
            return isWalking;
        }
    }
}