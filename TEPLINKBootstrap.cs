using UnityEngine;

public class TEPLINKBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreatePrototype()
    {
        if (FindObjectOfType<TEPLINKBootstrap>() != null) return;
        var root = new GameObject("TEPLINK Prototype");
        root.AddComponent<TEPLINKBootstrap>();
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var managerObject = new GameObject("Game Manager");
        var manager = managerObject.AddComponent<TEPLINKGameManager>();

        var worldObject = new GameObject("Egyptian Neighborhood");
        worldObject.AddComponent<TEPLINKWorldBuilder>();

        var playerObject = new GameObject("Player");
        playerObject.transform.position = new Vector3(0, 1.1f, -8f);
        var controller = playerObject.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.32f;
        controller.center = new Vector3(0, 0.9f, 0);
        var player = playerObject.AddComponent<TEPLINKPlayerController>();
        player.manager = manager;

        var cameraObject = new GameObject("Player Camera");
        cameraObject.transform.SetParent(playerObject.transform);
        cameraObject.transform.localPosition = new Vector3(0, 1.52f, 0);
        cameraObject.transform.localRotation = Quaternion.identity;
        var camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 75f;
        camera.nearClipPlane = 0.05f;
        cameraObject.AddComponent<AudioListener>();
        player.playerCamera = camera;

        var lightObject = new GameObject("Sun");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(50, -25, 0);
        manager.sun = light;

        var uiObject = new GameObject("Game UI");
        var ui = uiObject.AddComponent<TEPLINKUI>();
        ui.manager = manager;
        manager.ui = ui;

        manager.player = player;
        manager.Initialize();
    }
}
