using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private int movementPoints;
    private int _remainingMovementPoints;
    private bool isAlive = true;
    public bool blockRotation = false;
    [SerializeField]
    private bool isPlayerControlled; // Para saber si habilitar clicks o usar IA

    private TurnManager turnManager;
    private bool _currentlyUnitTurn = false;
    [SerializeField] private GameObject _walkableTilePrefab;
    private List<Vector3> _walkableTiles = new();
    private Dictionary<Vector3, GameObject> _walkableTileObjects = new();

    private const float gridSize = 1.0f;
    private const float SPEED = 5f;
    private const float ROTATE_SPEED = 180f;
    private Vector3Int? _isInteractingWithCell;

    [SerializeField] private Material _litMaterial;
    [SerializeField] private Material _attackableMaterial;

    void Awake()
    {
        GameObject tmObj = GameObject.FindGameObjectWithTag("Manager");
        turnManager = tmObj.GetComponent<TurnManager>();
    }

    void Start()
    {
        RotateToPlayer();
    }

    public bool GetIsPlayerControlled()
    {
        return isPlayerControlled;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void BeginTurn()
    {
        _currentlyUnitTurn = true;
        Debug.Log("Es el turno de: " + unitName);

        if (isPlayerControlled)
        {
            _remainingMovementPoints = movementPoints;
            UpdateRemainingMovement();
            return;
            // turnManager.EndCurrentTurn();
            // 1. Mostrar casillas a las que se puede mover (Grilla)
            // 2. Habilitar la selección de objetivos para atacar
        }
        else
        {
            // Ejecutar Inteligencia Artificial del enemigo
            StartCoroutine(ExecuteEnemyAI());
        }
    }

    public void DestroyWalkableTiles()
    {
        foreach (var tile in _walkableTiles)
        {
            Destroy(_walkableTileObjects[tile]);
        }
        _walkableTiles.Clear();
        _walkableTileObjects.Clear();
    }

    private void UpdateRemainingMovement()
    {
        ShowWalkableTiles(_remainingMovementPoints);

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var enemyPosition = Vector3Int.FloorToInt(enemy.transform.position);
            if (DistanceToUnitPosition(enemyPosition) <= _remainingMovementPoints + 1)
            {
                if (_walkableTiles.Contains(enemyPosition))
                {
                    Destroy(_walkableTileObjects[enemyPosition]);
                    _walkableTiles.Remove(enemyPosition);
                }
                enemy.GetComponent<Unit>().BecomeAttackable();
            }
            else
            {
                enemy.GetComponent<Unit>().BecomeLit();
            }

        }
    }

    public void ShowWalkableTiles(int? totalMovementPoints = null)
    {
        var remainingMovementPoints = totalMovementPoints ?? movementPoints;
        DestroyWalkableTiles();
        var currentPosition = gameObject.transform.position;
        for (var i = -remainingMovementPoints; i <= remainingMovementPoints; ++i)
        {
            for (var j = -remainingMovementPoints; j <= remainingMovementPoints; ++j)
            {
                if (i == 0 && j == 0) continue;

                if (Math.Abs(i) + Math.Abs(j) <= remainingMovementPoints)
                {
                    var vector = currentPosition + new Vector3Int(i, 0, j);
                    _walkableTiles.Add(vector);
                    _walkableTileObjects[vector] = Instantiate(_walkableTilePrefab, vector, Quaternion.identity);
                    if (!isPlayerControlled)
                    {
                        var renderer = _walkableTileObjects[vector].GetComponentInChildren<Renderer>();
                        renderer.transform.position += new Vector3(0, 0.01f, 0);
                        renderer.material = _attackableMaterial;
                    }
                }
            }
        }
    }

    private IEnumerator ExecuteEnemyAI()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            yield return StartCoroutine(GetClosestReachable(player.transform.position));
        }
        Debug.Log("Ending Turn");
        EndTurn();
    }

    public IEnumerator GetClosestReachable(Vector3 target)
    {
        var start = transform.position;
        int dx = (int)(target.x - start.x);
        int dz = (int)(target.z - start.z);
        int distance = Mathf.Abs(dx) + Mathf.Abs(dz);

        if (distance == 1)
        {
            yield return StartCoroutine(AttackUnit(GameObject.FindGameObjectWithTag("Player")));
            yield break;
        }

        if (distance > movementPoints)
        {
            int stepX = Mathf.Clamp(dx, -movementPoints, movementPoints);
            int remaining = movementPoints - Mathf.Abs(stepX);
            int stepZ = Mathf.Clamp(dz, -remaining, remaining);

            var targetLocation = new Vector3Int((int)start.x + stepX, 0, (int)start.z + stepZ);

            yield return StartCoroutine(MoveToCell(targetLocation));

            if (distance - movementPoints == 1)
            {
                yield return StartCoroutine(AttackUnit(GameObject.FindGameObjectWithTag("Player")));
            }

            yield break;
        }

        if (distance > 0)
        {
            int stepsToTake = distance - 1;

            int stepX = Mathf.Clamp(dx, -stepsToTake, stepsToTake);
            int remaining = stepsToTake - Mathf.Abs(stepX);
            int stepZ = Mathf.Clamp(dz, -remaining, remaining);

            var targetLocation = new Vector3Int((int)start.x + stepX, 0, (int)start.z + stepZ);

            yield return StartCoroutine(MoveToCell(targetLocation));
            yield return StartCoroutine(AttackUnit(GameObject.FindGameObjectWithTag("Player")));
        }
    }

    private float DistanceToUnitPosition(Vector3 unitPosition)
    {
        var currentPosition = transform.position;
        var unitDistanceX = Math.Abs(currentPosition.x - unitPosition.x);
        var unitDistanceZ = Math.Abs(currentPosition.z - unitPosition.z);

        return unitDistanceX + unitDistanceZ;
    }

    public void OnCellClicked(Vector3Int cell)
    {
        if (_isInteractingWithCell != null) return;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var enemyPosition = Vector3Int.FloorToInt(enemy.transform.position);
            Debug.Log($"Enemy Position {enemyPosition} {cell}");
            if (enemyPosition == cell && DistanceToUnitPosition(enemyPosition) <= 1)
            {
                Debug.Log($"Attacking Enemy");
                _isInteractingWithCell = cell;
                StartCoroutine(AttackUnit(enemy));
                return;
            }
        }

        StartCoroutine(MoveToCell(cell));
    }

    public IEnumerator AttackUnit(GameObject unit)
    {
        unit.GetComponent<Unit>().blockRotation = true;
        var target = Quaternion.LookRotation(-Vector3.down, -unit.transform.forward);
        Debug.Log($"Rotating to {target}");
        while (Quaternion.Angle(unit.transform.rotation, target) > 0.01f)
        {
            unit.transform.rotation = Quaternion.RotateTowards(
                unit.transform.rotation,
                target,
                ROTATE_SPEED * Time.deltaTime
            );
            yield return null;
        }

        unit.transform.rotation = target;
        _isInteractingWithCell = null;

        if (unit.GetComponent<Unit>().GetIsPlayerControlled())
        {
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            var remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length - 1;
            Destroy(unit);

            if (remainingEnemies == 0)
            {
                SceneManager.LoadScene("GameWonScene");
            }
        }
    }

    public IEnumerator MoveToCell(Vector3Int cell)
    {
        if (!_currentlyUnitTurn || (isPlayerControlled && !_walkableTiles.Contains(cell))) yield break;

        _isInteractingWithCell = cell;
        Vector3 start = transform.position;

        // Smoothly move over the specified duration
        while (Vector3.Distance(transform.position, cell) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                cell,
                SPEED * Time.deltaTime
            );
            yield return null;
        }

        // Snap to exact position to avoid floating point errors
        transform.position = cell;
        _isInteractingWithCell = null;
        _remainingMovementPoints = _remainingMovementPoints - (int)Math.Abs(start.x - cell.x) - (int)Math.Abs(start.z - cell.z);

        if (isPlayerControlled)
        {
            UpdateRemainingMovement();
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            StartCoroutine(enemy.GetComponent<Unit>().RotateToPlayer());
        }
    }

    public void EndTurn()
    {
        if (!_currentlyUnitTurn) return;

        if (isPlayerControlled)
        {
            DestroyWalkableTiles();

            foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                enemy.GetComponent<Unit>().BecomeLit();
            }
        }

        _currentlyUnitTurn = false;
        turnManager.EndCurrentTurn();
    }

    public void BecomeAttackable()
    {
        SetMaterial(_attackableMaterial);
    }

    public void BecomeLit()
    {
        SetMaterial(_litMaterial);
    }

    private void SetMaterial(Material material)
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        Material[] mats = new Material[rend.materials.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = material;
        }
        rend.materials = mats;
    }

    public IEnumerator RotateToPlayer()
    {
        if (isPlayerControlled || blockRotation) yield break;

        Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        direction.y = 0f;

        var target = Quaternion.LookRotation(direction, Vector3.up);
        while (Quaternion.Angle(transform.rotation, target) > 0.01f)
        {
            if (blockRotation) yield break;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target,
                ROTATE_SPEED * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = target;
    }
}