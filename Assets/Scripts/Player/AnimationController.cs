using UnityEngine;

namespace Player
{
    public class AnimationController
    {

        private Transform _playerTransform;
        private float _spinSpeed;

        private delegate void DoAnimation();
        private DoAnimation _customAnimation;


        public AnimationController(Transform playerTransform, float spinSpeed)
        {
            this._playerTransform = playerTransform;
            this._spinSpeed = spinSpeed;

            this._customAnimation = IdleAnimation;
        }

        public void IdleAnimation()
        {
            _playerTransform.RotateAround(_playerTransform.position, Vector3.forward, _spinSpeed * Time.deltaTime);
        }

        public void Update()
        {
            _customAnimation();
        }
    }
}