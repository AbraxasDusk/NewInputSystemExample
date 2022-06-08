// Script to control the players input
public class InputController : MonoBehavior
{
	// Reference to the Input Actions you created.
	private PlayerControls _playerControls;

	// Reference to the main camera in your scene.
	private Camera _mainCamera;

	// Private variables for use inside the script
	private bool _attackFlag
	private Vector2 _mousePosition;

	// Public variables for access outside the script
	// Setting it up like this will allow you to access the values outside this script, but now allow you to overwrite them.
	public bool AttackFlag
	{
		get => _attackFlag;
	}

	public Vector2 RelativeMousePosition
	{
		get => CalculateRelativeMousePosition();
	}

	// On Enable we set up the camera, and configure which 'Actions' and variables we want to lock together.
	private void OnEnable()
	{
		_mainCamera = Camera.main;

		_playerControls = new PlayerControls();

		// This line will update _attackFlag to true whenever the action is triggered
		_playerControls.Actions.Attack.performed += i => _attackFlag = true;

		// These 2 lines will keep _mousePosition updated whenever the player moves the mouse.
		// However this will return co-ordinates in screen space, not world space which can cause issues if used like this.
		_playerControls.Actions.MousePosition.performed += i => _mousePosition = i.ReadValue<Vector2>();
		_playerControls.Actions.MousePosition.canceled += i => _mousePosition = i.ReadValue<Vector2>();

		_playerControls.Enable();
	}

	// This will reset the _attackFlag at the end of every frame to prevent it from firing multiple times.
	private void LateUpdate()
	{
		_attackFlag = false;
	}

	// This will take the mouse position in screen space, and return a Vector2 relative to the player's position
	private Vector2 CalculateRelativeMousePosition()
	{
		Vector2 worldMousePosition = _mainCamera.ScreenToWorldPoint(_mousePosition);
		return worldMousePosition - (Vector2)this.transform.position;
	}
}



// Script to control the player character
public class PlayerController : MonoBehavior
{
	private InputController _input;

	private void Update()
	{
		// Check if attack input is performed this frame
		if (_input.AttackFlag)
		{
			Attack();
		}
	}

	private void Attack()
	{
		// Raycast from player position in the direction of the mouse
		RaycastHit2D hit = Physics2D.Raycast((Vector2)this.transform.position, _input.RelativeMousePosition.normalized);

		if (hit.collider != null)
		{

			// Try to find an EnemyController in the collider the raycast hit.
			EnemyController attackTarget = hit.collider.gameObject.GetComponent<EnemyController>();

			if (attackTarget != null)
			{
				// This is where you put your attack logic, in here you can do things like check the distance to your targetif you want a limited attack range for example.
				// For now we'll just say if you find a target, do damage to it.
				attackTarget.TakeDamage(10);
			}

		}
	}
}



// Script to control enemy characters
public class EnemyController : MonoBehavior
{
	private float _currentHealth;

	// Public method to allow other scripts to call it
	public void TakeDamage(float damage)
	{
		// In here is where the logic for the enemy's reaction to the attack
		// For example taking damge from an attack:

		_currentHealth -= damage;

		// And triggering other effects such as dying:
		if (_currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		Destroy(this.gameObject);
	}
}