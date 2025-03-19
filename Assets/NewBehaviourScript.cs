using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI; // Para los botones UI

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets; // Arreglo de los 5 marcadores
    public float speed = 1.0f; // Velocidad de movimiento
    private bool isMoving = false; // Indica si el modelo está en movimiento

    public void MoveToTarget(int targetIndex)
    {
        if (!isMoving && targetIndex >= 0 && targetIndex < ImageTargets.Length)
        {
            StartCoroutine(MoveModel(targetIndex)); // Inicia la corrutina de movimiento
        }
    }

    private IEnumerator MoveModel(int targetIndex)
    {
        isMoving = true;

        ObserverBehaviour target = ImageTargets[targetIndex]; // Obtiene el marcador seleccionado

        if (target == null || target.TargetStatus.Status == Status.NO_POSE)
        {
            isMoving = false;
            yield break; // Sale si el marcador no está detectado
        }

        Vector3 startPosition = model.transform.position; // Posición inicial
        Vector3 endPosition = target.transform.position; // Posición final

        // Calcular la dirección hacia el nuevo objetivo
        Vector3 direction = (endPosition - startPosition).normalized;

        // Rotar instantáneamente el modelo hacia la dirección del movimiento
        if (direction != Vector3.zero)
        {
            model.transform.rotation = Quaternion.LookRotation(direction);
        }

        float journey = 0;
        while (journey <= 1.0f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey); // Movimiento suave
            yield return null;
        }

        isMoving = false;
    }

    void Update()
    {
        // No es necesario modificar Update()
    }
}
