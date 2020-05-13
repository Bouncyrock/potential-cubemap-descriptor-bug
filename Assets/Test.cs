using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Camera _captureCamera;

    // These two are only using SerializeField so we can inspect the results easily in the editor
    [SerializeField] RenderTexture _renderTextureFromDescriptor;
    [SerializeField] RenderTexture _renderTexture;

    // Make a stretched sphere of a given color
    static GameObject MakeTestObj(Transform parent, Vector3 pos, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.SetParent(parent, worldPositionStays: true);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1, 8, 1);
        var renderer = go.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", color);
        return go;
    }

    // create a angled ring of test objects (stretched spheres in this case)
    static void SetupThingsToRender(Transform parent)
    {
        var axis = new Vector3(1, 1, 0).normalized;
        var colors = new Color[] {
            Color.cyan,
            Color.clear,
            Color.grey,
            Color.gray,
            Color.magenta,
            Color.red,
            Color.yellow,
            Color.black,
            Color.white,
            Color.green,
            Color.blue
        };
        var len = colors.Length;
        var angStep = 360f / len;
        for (var i = 0; i < len; i++)
        {
            var rot = Quaternion.AngleAxis(angStep * i, axis);
            var pos = rot * new Vector3(0, 0, 5);
            MakeTestObj(parent, pos, colors[i]);
        }
    }

    void SetupCubemapCam()
    {
        _captureCamera.gameObject.SetActive(false);
        _captureCamera.depthTextureMode = DepthTextureMode.Depth;
        _captureCamera.clearFlags = CameraClearFlags.SolidColor;
        _captureCamera.backgroundColor = Color.black;
        _captureCamera.allowHDR = false;
    }

    void CreateRenderTexture()
    {
        // Approach1
        var descriptor = new RenderTextureDescriptor();
        descriptor.dimension = UnityEngine.Rendering.TextureDimension.Cube;
        descriptor.width = 512;
        descriptor.height = 512;
        descriptor.volumeDepth = 1;
        descriptor.depthBufferBits = 16; // set so it matches Approach2
        descriptor.stencilFormat = GraphicsFormat.None; // set so it matches Approach2
        descriptor.bindMS = false;
        descriptor.msaaSamples = 1; // set as required
        descriptor.graphicsFormat = GraphicsFormat.R32G32_SFloat;
        descriptor.useMipMap = false;
        descriptor.autoGenerateMips = false;
        _renderTextureFromDescriptor = new RenderTexture(descriptor);
        _renderTextureFromDescriptor.filterMode = FilterMode.Point;

        // Approach2
        _renderTexture = new RenderTexture(512, 512, 1,  GraphicsFormat.R32G32_SFloat);
        _renderTexture.useMipMap = false;
        _renderTexture.autoGenerateMips = false;
        _renderTexture.useDynamicScale = false;
        _renderTexture.dimension = TextureDimension.Cube;
        _renderTexture.filterMode = FilterMode.Point;
    }

    void Awake()
    {
        SetupThingsToRender(transform);
        SetupCubemapCam();
        CreateRenderTexture();
    }

    void Start()
    {
        _captureCamera.RenderToCubemap(_renderTextureFromDescriptor);
        _captureCamera.RenderToCubemap(_renderTexture);
    }
}
