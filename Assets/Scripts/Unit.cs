using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private const float TURN_SPEED = 540f;
    [SerializeField]
    private string unitName;
    [SerializeField]
    private int movementPoints;
    private int _remainingMovementPoints;
    public bool isAlive = true;
    public bool blockRotation = false;
    [SerializeField]
    private bool isPlayerControlled;

    [SerializeField] private LayerMask obstacleLayer;

    private TurnManager turnManager;
    private bool _currentlyUnitTurn = false;
    [SerializeField] private GameObject _walkableTilePrefab;
    private List<Vector3> _walkableTiles = new();
    private Dictionary<Vector3, GameObject> _walkableTileObjects = new();

    public Grid grid;
    private const float gridSize = 1.0f;
    private const float SPEED = 5f;
    private const float ROTATE_SPEED = 180f;
    private Vector3Int? _isInteractingWithCell;

    private Animator _animator;
    [SerializeField] private Material _litMaterial;
    [SerializeField] private Material _attackableMaterial;

    private Quaternion _forwardRotation;
    void Awake()
    {
        GameObject tmObj = GameObject.FindGameObjectWithTag("Manager");
        turnManager = tmObj.GetComponent<TurnManager>();
        _animator = GetComponentInChildren<Animator>();
        _forwardRotation = transform.rotation;
    }

    void Start()
    {
        StartCoroutine(RotateToPlayer());
    }

    public bool GetIsPlayerControlled()
    {
        return isPlayerControlled;
    }

    public void BeginTurn()
    {
        _currentlyUnitTurn = true;

        if (isPlayerControlled)
        {
            _remainingMovementPoints = movementPoints;
            UpdateRemainingMovement();
            return;
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
            if (!enemy.GetComponent<Unit>().isAlive) continue;
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

    public bool IsCellWalkable(Vector3 targetPosition)
    {
        if (isPlayerControlled)
        {
            int minX = -10;
            int maxX = 9;
            int minZ = -10;
            int maxZ = 9;
            Vector3Int cell = grid.WorldToCell(targetPosition);

            if (!(cell.x >= minX && cell.x <= maxX
                && cell.z >= minZ && cell.z <= maxZ)) return false;
        }

        return !Physics.CheckSphere(targetPosition, 0.4f, obstacleLayer);
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

                    if (IsCellWalkable(vector))
                    {
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
        List<Vector3> fullPath = FindPath(transform.position, target);

        if (fullPath.Count == 1)
        {
            yield return StartCoroutine(AttackUnit(GameObject.FindGameObjectWithTag("Player")));
            yield break;
        }

        if (fullPath.Count > 0)
        {
            int stepsToTake = Mathf.Min(movementPoints, fullPath.Count - 1);

            if (stepsToTake > 0)
            {
                Vector3Int destinationCell = Vector3Int.FloorToInt(fullPath[stepsToTake - 1]);
                yield return StartCoroutine(MoveToCell(destinationCell));
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && DistanceToUnitPosition(player.transform.position) <= 1)
            {
                yield return StartCoroutine(AttackUnit(player));
            }
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

        var entity = unit.GetComponent<Unit>();
        if (entity.GetIsPlayerControlled())
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
            else
            {
                entity.isAlive = false;
                UpdateRemainingMovement();
            }
        }
    }

    private List<Vector3> FindPath(Vector3 start, Vector3 target)
    {
        Queue<Vector3> queue = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        while (queue.Count > 0)
        {
            Vector3 current = queue.Dequeue();

            if (Vector3.Distance(current, target) < 0.1f) break;

            foreach (Vector3 dir in directions)
            {
                Vector3 next = current + dir;

                if (!cameFrom.ContainsKey(next) && (IsCellWalkable(next) || Vector3.Distance(next, target) < 0.1f))
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        Vector3 currNode = target;

        while (currNode != start && cameFrom.ContainsKey(currNode))
        {
            path.Add(currNode);
            currNode = cameFrom[currNode];
        }

        path.Reverse();
        return path;
    }

    public IEnumerator MoveToCell(Vector3Int cell)
    {
        if (!_currentlyUnitTurn || (isPlayerControlled && !_walkableTiles.Contains(cell))) yield break;

        _isInteractingWithCell = cell;

        // Trazar ruta libre de obstáculos mediante Pathfinding
        List<Vector3> path = FindPath(transform.position, cell);
        if (_animator != null && isPlayerControlled) _animator.SetBool(IsMovingHash, true);

        foreach (Vector3 targetNode in path)
        {
            Vector3 dir = targetNode - transform.position;
            dir.y = 0f;
            Quaternion targetRotation = dir.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(dir, Vector3.up)
                : transform.rotation;

            while (Vector3.Distance(transform.position, targetNode) > 0.001f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    TURN_SPEED * Time.deltaTime
                );

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetNode,
                    SPEED * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetNode;
        }
        if (_animator != null && isPlayerControlled) _animator.SetBool(IsMovingHash, false);
        if (isPlayerControlled)
        {
            while (Quaternion.Angle(transform.rotation, _forwardRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    _forwardRotation,
                    TURN_SPEED * Time.deltaTime
                );
                yield return null;
            }
            transform.rotation = _forwardRotation;
        }
        _isInteractingWithCell = null;
        _remainingMovementPoints -= path.Count;

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
        if (!_currentlyUnitTurn || _isInteractingWithCell != null) return;

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