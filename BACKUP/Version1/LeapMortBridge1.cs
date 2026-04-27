using UnityEngine;
using UnityEngine.UI;

public class LeapMortBridge1 : MonoBehaviour
{
    public ComputeShader bypassShader;
    public RawImage displayOutput;
    public RenderTexture cameraInput; // DRAG YOUR CAMERA RENDER TEXTURE HERE
    public int resolution = 2048;
    public int photonsPerFrame = 500000;

    private RenderTexture _renderTexture;

    void Start()
    {
        _renderTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();
        displayOutput.texture = _renderTexture;
    }

    void Update()
    {
        // Clear the LEAP-MORT buffer
        Graphics.Blit(Texture2D.blackTexture, _renderTexture);

        if (cameraInput == null) return;

        int kernel = bypassShader.FindKernel("CSMain");

        // Connect the textures to the GPU
        bypassShader.SetTexture(kernel, "Result", _renderTexture);
        bypassShader.SetTexture(kernel, "CameraSource", cameraInput);

        // Pass variables
        bypassShader.SetFloat("Time", Time.time);
        bypassShader.SetInt("Resolution", resolution);
        bypassShader.SetInt("PhotonCountPerFrame", photonsPerFrame);

        // Run the bypass math
        int threadGroups = Mathf.CeilToInt(photonsPerFrame / 64f);
        bypassShader.Dispatch(kernel, threadGroups, 1, 1);
    }

}