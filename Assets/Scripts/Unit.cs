using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private int movementPoints; 
    private bool isAlive = true;
    [SerializeField]
    private bool isPlayerControlled; // Para saber si habilitar clics o usar IA

    private TurnManager turnManager;

    void Awake()
    {
        GameObject tmObj = GameObject.FindGameObjectWithTag("Manager");
        turnManager = tmObj.GetComponent<TurnManager>();
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
        Debug.Log("Es el turno de: " + unitName);

        if (isPlayerControlled)
        {
            turnManager.EndCurrentTurn();
            // 1. Mostrar casillas a las que se puede mover (Grilla)
            // 2. Habilitar la selección de objetivos para atacar
        }
        else
        {
            // Ejecutar Inteligencia Artificial del enemigo
            StartCoroutine(ExecuteEnemyAI());
        }
    }

    System.Collections.IEnumerator ExecuteEnemyAI()
    {
        yield return new WaitForSeconds(1.5f); // Pausa simulando pensamiento

        // Lógica de IA: Buscar objetivo más cercano, moverse y atacar

        // Al finalizar sus acciones, le avisa al gestor para pasar al siguiente
        turnManager.EndCurrentTurn();
    }
}