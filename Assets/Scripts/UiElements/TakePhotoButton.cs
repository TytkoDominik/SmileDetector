using System;
using System.IO;
using Core;
using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UiElements
{
    public class TakePhotoButton : MonoBehaviour
    {
        [Inject(Id = "SnapshotCamera")] private Camera _snapshotCamera;
        [Inject] private SignalBus _signalBus;
        [Inject] private DataSerializer _serializer;
        private bool _shouldTakePhoto;
    
        [Inject]
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                _shouldTakePhoto = true;
            });
        }

        private void Update()
        {
            if (!_shouldTakePhoto)
            {
                return;
            }

            Texture2D snap = GetTexture2DFromRenderTexture(_snapshotCamera.targetTexture);
            _signalBus.Fire(new PhotoTakenSignal(snap.EncodeToPNG(), _serializer.GetNextPhotoName()));
            _shouldTakePhoto = false;
        }
        
        private Texture2D GetTexture2DFromRenderTexture(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
        
    }
}