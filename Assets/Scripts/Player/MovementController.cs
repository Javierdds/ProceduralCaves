using UnityEngine;

namespace Player
{
    public class MovementController
    {

        private Rigidbody2D _rigidBody;
        private float _movementSpeed;
        private float _hInput;
        private float _vInput;
        private Vector2 _lastInputRecorded;

        private delegate void MovePlayerDelegate(Vector2 input);
        private MovePlayerDelegate _doMove;

        public MovementController(Rigidbody2D rb, float movementSpeed)
        {
            _rigidBody = rb;
            _movementSpeed = movementSpeed;

            _hInput = 0;
            _vInput = 0;
            _lastInputRecorded = new Vector2();


            _doMove = MoveObject;
        }

        public void FixedUpdate()
        {
            Vector2 moveInput = new Vector2(_hInput, _vInput);

            Debug.Log($"[Move input]: {moveInput}");

            if (Mathf.Abs(moveInput.sqrMagnitude) != 1)
            {
                _doMove(_lastInputRecorded);
                return;
            }

            moveInput = CalculatePriorityAxis(moveInput);
            _lastInputRecorded = moveInput;

            _doMove(_lastInputRecorded);


            Debug.Log($"[Input recorded]: {_lastInputRecorded}");
        }

        public void Update()
        {
            _hInput = Input.GetAxisRaw("Horizontal");
            _vInput = Input.GetAxisRaw("Vertical");
        }

        private void MoveObject(Vector2 input)
        {
            Vector2 moveInput = new Vector2(input.x, input.y);

            _rigidBody.MovePosition(_rigidBody.position 
                + (moveInput * _movementSpeed * Time.fixedDeltaTime));
        }

        public Vector2 CalculatePriorityAxis(Vector2 input)
        {
            if (_lastInputRecorded == input) return input;

            if (_lastInputRecorded.x != input.x)
                input = new Vector2(input.x, 0);

            if (_lastInputRecorded.y != input.y)
                input = new Vector2(0, input.y);

            return input;
        }

        public void InvertRecordedInput()
        {
            _lastInputRecorded *= -1;
        }
    }
}

