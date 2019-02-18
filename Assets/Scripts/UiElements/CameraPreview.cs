using System.Collections;
using System.Linq;
using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
        public class CameraPreview : MonoBehaviour
        {
                [Inject(Id = "MainCamera")] private Camera _mainCamera;
                [Inject(Id = "SnapshotCamera")] private Camera _snapshotCamera;
                [Inject] private SignalBus _signalBus;
                private bool _isCameraFacingPlayer = true;
                private bool _isSwitching = false;
                private WebCamDevice[] _webCamDevices;
                private WebCamTexture _webCamTexture;
                private RawImage _cameraPreview;
        
                private void Start()
                {
                        _cameraPreview = GetComponent<RawImage>();
                        _webCamDevices = WebCamTexture.devices;
                        
                        SwitchCamera();
                        
                        _signalBus.Subscribe<SwitchCameraSignal>(SwitchCamera);
                }

                private void SwitchCamera()
                {
                        if (_isSwitching)
                        {
                                return;
                        }

                        string cameraName =
                                _webCamDevices.FirstOrDefault(d => d.isFrontFacing == _isCameraFacingPlayer).name;
                        
                        StartCoroutine(SetupWebCamTexture(cameraName));

                }

                private IEnumerator SetupWebCamTexture(string deviceName)
                {
                        _isSwitching = true;
                        _webCamTexture = new WebCamTexture(deviceName);
                        _cameraPreview.texture = _webCamTexture;
                        _webCamTexture.requestedFPS = 30f;
                        _webCamTexture.Play();
                        
#if UNITY_ANDROID && !UNITY_EDITOR
                        additionalRotationAngle = 180f;
#else
                        float additionalRotationAngle = 0f;
#endif                        
                        
                        _cameraPreview.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _webCamTexture.videoRotationAngle + additionalRotationAngle));
                        
                        yield return new WaitUntil(() => _webCamTexture.width > 16);
                
                        _cameraPreview.rectTransform.sizeDelta = new Vector2(_webCamTexture.width, _webCamTexture.height);
                        _mainCamera.orthographicSize = _webCamTexture.height / 2f;
                        _snapshotCamera.orthographicSize = _webCamTexture.height / 2f;

                        Vector2 renderTextureSize = GetRenderTextureSize();
                        
                        _snapshotCamera.targetTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 0);
                        
                        _isCameraFacingPlayer = !_isCameraFacingPlayer;
                        _isSwitching = false;
                }

                private Vector2 GetRenderTextureSize()
                {
                        if (_mainCamera.pixelWidth * 4 == _mainCamera.pixelHeight * 3)
                        {
                                return new Vector2(_mainCamera.pixelWidth, _mainCamera.pixelHeight);
                        }

                        if (_mainCamera.pixelWidth * 4 > _mainCamera.pixelHeight * 3)
                        {
                                return new Vector2(_mainCamera.pixelHeight * 3f/4f, _mainCamera.pixelHeight);   
                        }

                        return new Vector2(_mainCamera.pixelWidth, _mainCamera.pixelWidth * 4f/3f); 
                }
        }
}