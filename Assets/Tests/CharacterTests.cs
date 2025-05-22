using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using StarterAssets;

public class CharacterTests : InputTestFixture
{
    // Carga el prefab del personaje desde la carpeta Resources
    GameObject character = Resources.Load<GameObject>("Character");
    Keyboard keyboard; // Referencia al teclado virtual

    // Este método se ejecuta antes de CADA prueba
    public override void Setup()
    {
        SceneManager.LoadScene("Scenes/SimpleTesting"); // Carga la escena de prueba
        base.Setup(); // Llama al Setup de la clase base (InputTestFixture)
        keyboard = InputSystem.AddDevice<Keyboard>(); // Añade un dispositivo de teclado virtual
        var mouse = InputSystem.AddDevice<Mouse>(); // Añade un dispositivo de ratón virtual
        Press(mouse.rightButton); // Simula un clic derecho para "enfocar" la entrada
        Release(mouse.rightButton);
    }

    // ---
    // Este método se ejecuta DESPUÉS de CADA prueba
    // Es crucial para limpiar y evitar interferencias entre pruebas.
    [TearDown]
    public void Teardown()
    {
        // Destruye todos los GameObjects instanciados en la escena durante la prueba.
        // Esto previene "fugas" de objetos que puedan afectar a pruebas posteriores.
        foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>())
        {
            GameObject.Destroy(gameObject);
        }
        // También puedes limpiar cualquier dispositivo de entrada añadido manualmente si es necesario,
        // aunque InputTestFixture a menudo maneja esto por sí mismo.
        InputSystem.RemoveDevice(keyboard);
    }
    // ---


    // Una prueba simple para verificar que el personaje se puede instanciar
    [Test]
    public void TestPlayerInstantiation()
    {
        // Instancia el personaje en la escena
        GameObject characterInstance = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);
        // Afirma que la instancia no es nula
        Assert.That(characterInstance, !Is.Null);
    }

    // Una prueba de modo de juego que se ejecuta a lo largo de varios frames (requiere IEnumerator y [UnityTest])
    [UnityTest]
    public IEnumerator TestPlayerMoves()
    {
        GameObject characterInstance = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);
        Vector3 initialPosition = characterInstance.transform.position;

        Press(keyboard.upArrowKey); // Simula presionar la tecla "flecha arriba"
        yield return new WaitForSeconds(1f); // Espera 1 segundo
        Release(keyboard.upArrowKey); // Simula soltar la tecla
        yield return new WaitForSeconds(1f); // Espera otro segundo para que el personaje se detenga

        // Afirma que la posición Z del personaje es mayor que su posición inicial + 1.5 unidades
        Assert.That(characterInstance.transform.position.z, Is.GreaterThan(initialPosition.z + 1.5f));
    }

    // Prueba de daño por caída (requiere el script PlayerHealth)
    // Antes de habilitar, crea el archivo PlayerHealth.cs y añádelo al prefab "Character"
    
    [UnityTest]
    public IEnumerator TestPlayerFallDamage()
    {
        // Instancia el personaje en una posición elevada para simular una caída
        GameObject characterInstance = GameObject.Instantiate(character, new Vector3(0f, 4f, 17.2f), Quaternion.identity);
        // Obtiene el componente PlayerHealth y verifica que la salud inicial sea 1 (100%)
        var characterHealth = characterInstance.GetComponent<PlayerHealth>();
        Assert.That(characterHealth.Health, Is.EqualTo(1f));

        // Simula mover el personaje para que caiga del borde
        Press(keyboard.upArrowKey);
        yield return new WaitForSeconds(0.5f); // Espera mientras cae
        Release(keyboard.upArrowKey);
        yield return new WaitForSeconds(2f); // Espera a que el personaje golpee el suelo

        // Afirma que la salud del personaje se ha reducido a 0.9 (90%)
        Assert.That(characterHealth.Health, Is.EqualTo(0.9f));
    }
    
}