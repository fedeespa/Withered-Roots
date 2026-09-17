
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class TurnManager : MonoBehaviour
{
    private List<GameObject> allUnits = new();

    private Queue<Unit> turnQueue = new();

    private Unit activeUnit;

    void Start()
    {

        DetermineTurnOrder();
        StartNextTurn();
    }

    void DetermineTurnOrder()
    {
        allUnits.Clear();
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        allUnits.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        turnQueue.Clear();
        foreach (GameObject unit in allUnits)
        {
            if (unit.GetComponent<Unit>().GetIsAlive())
            {
                turnQueue.Enqueue(unit.GetComponent<Unit>());
            }
        }
    }

    public void StartNextTurn()
    {

        if (turnQueue.Count == 0)
        {
            DetermineTurnOrder();
        }

        // Sacamos a la siguiente unidad de la cola
        activeUnit = turnQueue.Dequeue();

        if (activeUnit == null)
        {
            StartNextTurn();
            return;
        }

        // Le avisamos a esa unidad específica que ahora es su turno
        activeUnit.BeginTurn();
    }

    // Este método lo llamas desde el script de la unidad cuando termine de atacar o mover
    public void EndCurrentTurn()
    {
        StartNextTurn();
    }
}