using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Grid grid;
    public LayerMask groundLayer;

    public void Update()
    {
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Debug.Log($"RightClick Released");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                enemy.GetComponent<Unit>().DestroyWalkableTiles();
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3Int cell = grid.WorldToCell(hit.point);
            Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();

            if (cell != null && player != null)
            {
                Debug.Log($"Clicked cell: {cell}");
                player.OnCellClicked(cell);
            }
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3Int cell = grid.WorldToCell(hit.point);
            Debug.Log($"RightClicked cell: {cell}");
            if (cell == null) return;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (enemy.transform.position == cell)
                {
                    Debug.Log($"Found enemy at cell: {cell}");
                    enemy.GetComponent<Unit>().ShowWalkableTiles();
                    return;
                }
            }
        }
    }

    public void OnSpacebar(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();

        if (player != null)
        {
            player.EndTurn();
        }
    }
}