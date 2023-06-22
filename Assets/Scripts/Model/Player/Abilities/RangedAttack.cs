﻿using UnityEngine;

namespace Model.Player.Abilities
{
    public class RangedAttack : AbilityBase
    {
        private readonly Transform _transform;
        private readonly GameObject _prefab;

        public RangedAttack(Transform transform, GameObject prefab) : base(0f, 0.2f, 2f, 0.2f)
        {
            _transform = transform;
            _prefab = prefab;
        }

        protected override bool PerformAbility(PlayerModel player)
        {
            Instantiate(_prefab, _transform.position + (_transform.forward * 0.5f), _transform.rotation);
            return true;
        }

        protected override Resource GetResource(PlayerModel player)
        {
            return player.Mana;
        }
    }
}