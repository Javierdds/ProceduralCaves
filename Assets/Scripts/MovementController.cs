using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        private Rigidbody2D _rigidBody;
        private float _hInput;
        private float _vInput;

        private delegate void MovePlayerDelegate(Vector2 input);
        private MovePlayerDelegate _doMove;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _doMove = MoveObject;
        }

        private void FixedUpdate()
        {
            Vector2 moveInput = new Vector2(_hInput, _vInput).normalized;

            _doMove(moveInput);
        }

        private void Update()
        {
            _hInput = Input.GetAxisRaw("Horizontal");
            _vInput = Input.GetAxisRaw("Vertical");
        }

        //Funcion del cliente
        private void MoveObject(Vector2 input)
        {
            Vector2 moveInput = new Vector2(input.x, input.y);

            _rigidBody.MovePosition(_rigidBody.position 
                + (moveInput * _movementSpeed * Time.fixedDeltaTime));
        }
    }
}

