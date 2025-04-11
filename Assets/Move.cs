using System.Collections;
using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    public GameObject model; // Oso
    public ObserverBehaviour[] ImageTargets; // Marcadores
    public GameObject[] modelsOnTargets; // Modelos en cada marcador
    public GameObject[] Botones;
    public Animator animController; // Animator
    public float speed = 1.0f;
    public float rotationSpeed = 5.0f;
    private bool isMoving = false; // Evita que el oso inicie otro movimiento mientras ya está caminando.

    public GameObject finalImage; // Imagen que aparece cuando todos los modelos están activos

    private void Start()
    {
        // Oculta todos los modelos excepto el del primer marcador
        for (int i = 1; i < modelsOnTargets.Length; i++)
        {
            modelsOnTargets[i].SetActive(false);
        }

        for (int i = 0; i < 5; i++)
        {
            Botones[i].SetActive(false);
        }

        // Oculta la imagen final al inicio
        if (finalImage != null)
        {
            finalImage.SetActive(false);
        }

        // Establecer la animación inicial (idle) para el modelo
        animController.Play("Breathing Idle");

        // Suscribirse a los eventos de Vuforia solo para los ImageTargets
        for (int i = 0; i < ImageTargets.Length; i++)
        {
            ImageTargets[i].OnTargetStatusChanged += OnMarkerStatusChanged;
        }
    }

    private void OnDestroy()
    {
        // Desuscribirse de los eventos cuando el objeto se destruya
        for (int i = 0; i < ImageTargets.Length; i++)
        {
            ImageTargets[i].OnTargetStatusChanged -= OnMarkerStatusChanged;
        }
    }

    private void OnMarkerStatusChanged(ObserverBehaviour target, TargetStatus status)
    {
        int targetIndex = System.Array.IndexOf(ImageTargets, target);

        if (status.Status == Status.NO_POSE) // Cuando el marcador desaparece
        {
            Botones[targetIndex].SetActive(false); // Oculta el botón correspondiente
        }
        else if (IsTargetTracked(target))
        {
            Botones[targetIndex].SetActive(true); // Muestra el botón correspondiente
        }
    }

    // Método para revelar un modelo en el marcador
    public void RevealModel(int targetIndex)
    {
        if (targetIndex > 0 && targetIndex < modelsOnTargets.Length) // Evita afectar el primer marcador
        {
            if (IsTargetTracked(ImageTargets[targetIndex])) // Solo activa si el marcador está visible
            {
                moveToNextMarker(targetIndex); // Mueve el oso
            }
        }
    }

    public void moveToNextMarker(int targetIndex) // Movimiento del modelo al seleccionar un marcador
    {
        if (!isMoving && targetIndex >= 0 && targetIndex < ImageTargets.Length)
        {
            StartCoroutine(MoveModel(targetIndex));
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

        // Cambiar la animación a "Walk" mientras se mueve entre los marcadores
        animController.Play("Strut Walking");

        float journey = 0;
        while (journey <= 1.0f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey); // Movimiento suave
            yield return null;
        }

        model.transform.SetParent(target.transform); // Cambiar el modelo de jerarquía al ImageTarget

        modelsOnTargets[targetIndex].SetActive(true); // Activa el modelo en el ImageTarget correspondiente

        // Cambiar la animación según el marcador alcanzado
        switch (targetIndex)
        {
            case 0:
                animController.Play("Breathing Idle"); // Idle cuando está en el primer marcador
                break;
            case 1:
                animController.Play("Fighting Idle"); // Fight en el ImageTarget 1

                // Verificar si todos los modelos están activos
                bool allModelsActive = true;
                for (int i = 0; i < modelsOnTargets.Length; i++)
                {
                    if (!modelsOnTargets[i].activeSelf)
                    {
                        allModelsActive = false;
                        break;
                    }
                }

                if (allModelsActive)
                {
                    Debug.Log("Hola");
                    if (finalImage != null)
                    {
                        finalImage.SetActive(true); // Muestra la imagen final
                    }
                }
                else
                {
                    RestartApp(); // Reinicia si no están todos activos
                }
                break;
            case 2:
                animController.Play("Laying Hand Gesture"); // Sleep en el ImageTarget 2
                break;
            case 3:
                animController.Play("Drinking"); // Drink en el ImageTarget 3
                break;
            case 4:
                animController.Play("Warming Up"); // Train en el ImageTarget 4
                break;
        }

        isMoving = false;
    }

    private bool IsTargetTracked(ObserverBehaviour target) // Verifica si el marcador sigue rastreado antes de mover al oso.
    {
        return target != null &&
               (target.TargetStatus.Status == Status.TRACKED || target.TargetStatus.Status == Status.EXTENDED_TRACKED);
    }

    public void RestartApp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}