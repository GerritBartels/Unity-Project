using System.Collections;
using Controllers.Player.AbilityAdapters;
using Controllers.UI.Menu;
using Model;
using Model.Player;
using UnityEngine;

namespace Controllers.Player
{
    /// <summary>
    /// <c>PlayerController</c> defines control for the player game object within unity.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private Vector3 _moveDirection;
        private float _mouseX;
        public float rotationSpeed;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject meleePrefab;
        [SerializeField] private GameObject fireBallPrefab;

        [SerializeField] private float baseSpeed = 5f;

        [SerializeField] private GameObject canvas;

        private Rigidbody _rigidbody;
        private Animator _animator;

        public PlayerModel PlayerModel { get; }

        private const float RegenerationDelay = 0.1f;

        private RangedAttack _rangedAttack;
        private ScatterShot _scatterShot;
        private MeleeAttack _meleeAttack;
        private FireBall _fireBall;

        private BulletTime _bulletTime;
        private static readonly int Dead = Animator.StringToHash("dead");
        private static readonly int PlayerSpeed = Animator.StringToHash("playerSpeed");

        private IAbility<PlayerModel>[] _abilities;
        public IAbility<PlayerModel>[] Abilities => _abilities;

        /// <summary>
        /// Constructor that initializes a <c>PlayerController</c> by instantiating a new <see cref="Model.Player.PlayerModel"/> with a given <c>baseSpeed</c>.
        /// </summary>
        private PlayerController()
        {
            PlayerModel = new PlayerModel(baseSpeed);
        }

        private void Start()
        {
            // Load mouse sensitivity and player stats from PlayerPrefs
            rotationSpeed = PlayerPrefs.GetFloat("mouseSensitivity", 2000);
            PlayerModel.Strength = PlayerPrefs.GetInt("Strength", 0);
            PlayerModel.Agility = PlayerPrefs.GetInt("Agility", 0);
            PlayerModel.Intelligence = PlayerPrefs.GetInt("Intelligence", 0);
            PlayerModel.Reset();

            _animator = GetComponent<Animator>();

            // Instantiate abilities
            _rangedAttack = new RangedAttack(transform, bulletPrefab, _animator);
            _scatterShot = new ScatterShot(transform, bulletPrefab, _animator);
            _meleeAttack = new MeleeAttack(transform, meleePrefab, _animator);
            _fireBall = new FireBall(transform, fireBallPrefab, _animator);
            _abilities = new IAbility<PlayerModel>[] { _rangedAttack, _meleeAttack, _scatterShot, _fireBall };

            _bulletTime = new BulletTime(this);

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            StartCoroutine(Regeneration());
        }

        public void OnCollisionEnter(Collision other)
        {
            // Prevents the player from falling through the planet
            if (other.gameObject.CompareTag("Bedrock"))
            {
                transform.position += (transform.up * 0.25f);
            }
        }

        public void Update()
        {
            if (!PlayerModel.IsAlive) return;
            // Attack
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                PlayerModel.UseAbility(_rangedAttack);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                PlayerModel.UseAbility(_meleeAttack);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayerModel.UseAbility(_scatterShot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayerModel.UseAbility(_fireBall);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _bulletTime.Use(PlayerModel);
            }

            // Sprint or walk
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PlayerModel.Sprint();
            }
            else
            {
                PlayerModel.Walk();
            }

            // Pass the player's movement direction to the animator
            // Might make sense to inlcude a sideways movement animation as well for a and d
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                _animator.SetFloat(PlayerSpeed, PlayerModel.Speed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _animator.SetFloat(PlayerSpeed, -PlayerModel.Speed);
            }
            else
            {
                _animator.SetFloat(PlayerSpeed, 0);
            }
        }

        public void LateUpdate()
        {
            // Rotate player
            if (!PlayerModel.IsAlive) return;
            _mouseX = Input.GetAxis("Mouse X");
            transform.RotateAround(transform.position, transform.up, Time.deltaTime * _mouseX * rotationSpeed);
        }

        public void FixedUpdate()
        {
            // Move player
            if (!PlayerModel.IsAlive) return;
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            _rigidbody.MovePosition(_rigidbody.position +
                                    transform.TransformDirection(_moveDirection) *
                                    (PlayerModel.Speed * Time.deltaTime));
        }

        /// <summary>
        /// <c>Shot</c> is called from an event in the player's <c>Quick Shooting</c> animation and
        /// performs the actual ability by triggering its <see cref="RangedAttack.PerformAbility(Model.Player.PlayerModel)"/> method.
        /// </summary>
        private void Shot()
        {
            _rangedAttack.PerformAbility(PlayerModel);
        }

        /// <summary>
        /// <c>ScatterShot</c> is called from an event in the player's <c>Scatter Shot</c> animation and
        /// performs the actual ability by triggering its <see cref="ScatterShot.PerformAbility(Model.Player.PlayerModel)"/> method.
        /// </summary>
        private void ScatterShot()
        {
            _scatterShot.PerformAbility(PlayerModel);
        }

        /// <summary>
        /// <c>Melee</c> is called from an event in the player's <c>Melee Attack</c> animation and
        /// performs the actual ability by triggering its <see cref="MeleeAttack.PerformAbility(Model.Player.PlayerModel)"/> method.
        /// </summary>
        private void Melee()
        {
            _meleeAttack.PerformAbility(PlayerModel);
        }

        /// <summary>
        /// <c>FireBall</c> is called from an event in the player's <c>Fireball</c> animation and
        /// performs the actual ability by triggering its <see cref="FireBall.PerformAbility(Model.Player.PlayerModel)"/> method.
        /// </summary>
        private void FireBall()
        {
            _fireBall.PerformAbility(PlayerModel);
        }

        /// <summary>
        /// <c>Damage</c> calls the <see cref="PlayerModel.TakeDamage"/> method and upon death triggers the player's death animation.
        /// Also calls the <see cref="GameMenu.GameOver"/> method to trigger the UI's game over menu and destroys all remaining enemies.
        /// </summary>
        /// <param name="damage">the amount of damage taken</param>
        public void Damage(float damage)
        {
            if (PlayerModel.TakeDamage(damage)) return;
            // Death
            _animator.SetTrigger(Dead);
            canvas.GetComponent<GameMenu>().GameOver();
        }

        /// <summary>
        /// <c>Regeneration</c> coroutine calls the <see cref="PlayerModel.Regenerate(float)"/> method with a given <c>RegenerationDelay</c> while the Player is alive.
        /// </summary>
        /// <returns>
        /// <see cref="WaitForSeconds"/> delay if player is alive; otherwise, <c>null</c>.
        /// </returns>
        protected virtual IEnumerator Regeneration()
        {
            while (PlayerModel.IsAlive)
            {
                PlayerModel.Regenerate(RegenerationDelay);
                yield return new WaitForSeconds(RegenerationDelay);
            }

            yield return null;
        }
    }
}