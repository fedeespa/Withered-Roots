using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Grid grid;
    public LayerMask groundLayer;

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