using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerManager : MonoBehaviour
    {

        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _spinSpeed;

        private MovementController _movementController;
        private AnimationController _animationController;
        private PlayerPointsController _pointsController;

        public PlayerPointsController PointsController { get => _pointsController; set => _pointsController = value; }

        private void Awake()
        {
            _movementController = new MovementController(GetComponent<Rigidbody2D>(), _movementSpeed);
            _pointsController = new PlayerPointsController();
            _animationController = new AnimationController(transform, _spinSpeed);
        }

        private void FixedUpdate()
        {
            FixedUpdateControllers();
        }

        private void Update()
        {
            UpdateControllers();
        }

        private void FixedUpdateControllers()
        {
            _movementController.FixedUpdate();
        }

        private void UpdateControllers()
        {
            _movementController.Update();
            _animationController.Update();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Walls"))
            {
                _movementController.InvertRecordedInput();
            }
        }
    }

}
