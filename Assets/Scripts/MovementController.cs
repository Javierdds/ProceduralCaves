using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;

        private Rigidbody2D _characterController;
        private float _hInput;
        private float _vInput;

        private delegate void MovePlayerDelegate(Vector2 input);
        private MovePlayerDelegate _doMove;

        private void Awake()
        {
            _characterController = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _doMove = MoveObject;
        }

        private void FixedUpdate()
        {
            Vector2 moveInput = new Vector2(_hInput, _vInput);

            _doMove(moveInput);
        }

        private void Update()
        {
            _hInput = Input.GetAxis("Horizontal");
            _vInput = Input.GetAxis("Vertical");
        }

        //Funcion del cliente
        private void MoveObject(Vector2 input)
        {
            Vector2 moveInput = new Vector2(input.x, input.y);

            _characterController.MovePosition(_characterController.position 
                + (moveInput * _movementSpeed * Time.deltaTime));
        }
    }
}

