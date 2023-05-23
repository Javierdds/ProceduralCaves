using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [SerializeField] private Transform _coinTransform;
    [SerializeField] private float _initialMagnetSpeed;
    [SerializeField] private float _maxMagnetSpeed;
    [SerializeField] private float _increasedSpeedPerSecond;

    private bool _isChasingPlayer;
    private Transform _playerTransform;

    public bool IsChasingPlayer { get => _isChasingPlayer; set => _isChasingPlayer = value; }


    private void Awake()
    {
        _isChasingPlayer = false;
    }

    private void Update()
    {
        if (!_isChasingPlayer) return;

        Vector2 direction = _playerTransform.position - _coinTransform.position;

        _coinTransform.Translate(direction * _initialMagnetSpeed * Time.deltaTime);

        // Se incrementa la velocidad inicial cada segundo hasta llegar a un máximo
        if(_initialMagnetSpeed < _maxMagnetSpeed)
        {
            _initialMagnetSpeed += _increasedSpeedPerSecond * Time.deltaTime;
            if (_initialMagnetSpeed > _maxMagnetSpeed) _initialMagnetSpeed = _maxMagnetSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerTransform = collision.transform;
            _isChasingPlayer = true;
        }
    }

}
