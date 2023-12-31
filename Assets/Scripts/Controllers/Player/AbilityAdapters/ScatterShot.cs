﻿using Model;
using Model.Player;
using UnityEngine;

namespace Controllers.Player.AbilityAdapters
{
    public class ScatterShot : AbilityBase
    {
        private readonly Transform _transform;
        private readonly GameObject _prefab;
        private readonly Animator _animator;
        private static readonly int Shot = Animator.StringToHash("scatterShot");

        public ScatterShot(Transform transform, GameObject prefab, Animator animator) :
            base(cooldown: 2.5f, globalCooldown: 1.8f, resourceCost: 20f, blockMovementFor: 1.8f, baseDamage: 50f)
        {
            _transform = transform;
            _prefab = prefab;
            _animator = animator;
        }

        public override void PerformAbility(PlayerModel playerModel)
        {
            var obj1 = Instantiate(_prefab, _transform.position + (_transform.forward * 0.5f) + _transform.up*1.2f,
                _transform.rotation);
            var obj2 = Instantiate(
                _prefab,
                _transform.position + (_transform.forward * 0.7f) + (_transform.right * -1f) + _transform.up*1.2f,
                _transform.rotation * Quaternion.Euler(0f, -10f, 0f)
            );
            var obj3 = Instantiate(
                _prefab,
                _transform.position + (_transform.forward * 0.7f) + (_transform.right * 1f) + _transform.up*1.2f,
                _transform.rotation * Quaternion.Euler(0f, 10f, 0f)
            );

            var controller1 = obj1.GetComponent<PlayerAttackControllerBase>();
            controller1.Damage = Damage(playerModel);

            var controller2 = obj2.GetComponent<PlayerAttackControllerBase>();
            controller2.Damage = Damage(playerModel);

            var controller3 = obj3.GetComponent<PlayerAttackControllerBase>();
            controller3.Damage = Damage(playerModel);
        }

        protected override bool TriggerAnimation(PlayerModel player)
        {
            _animator.SetTrigger(Shot);
            return true;
        }

        protected override Resource GetResource(PlayerModel player) => player.Stamina;

        protected override float DamageModifier(PlayerModel playerModel) => playerModel.PhysicalDamageModificator();
    }
}