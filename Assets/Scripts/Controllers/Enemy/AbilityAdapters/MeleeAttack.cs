﻿using System;
using Controllers.Player;
using Model.Enemy;

namespace Controllers.Enemy.AbilityAdapters
{
    public class MeleeAttack : AbilityBase
    {
        private readonly Func<float> _distanceToPlayer;
        private readonly float _range;
        private readonly PlayerController _player;

        public MeleeAttack(Func<float> distanceToPlayer, PlayerController player, float damage = 10f, float range = 3f)
            : base(1f, 1f, damage)
        {
            _player = player;
            _distanceToPlayer = distanceToPlayer;
            _range = range;
        }

        protected override bool PerformAbility(Model.Enemy.Enemy enemy)
        {
            if (!(_distanceToPlayer() < _range)) return false;
            _player.Damage(Damage(enemy));
            return true;
        }
    }
}