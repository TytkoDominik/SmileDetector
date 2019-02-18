using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DependencyInjection;
using Newtonsoft.Json;
using Zenject;
using Application = UnityEngine.Application;

namespace Core
{
    public class DataSerializer
    {
        [Inject] private SignalBus _signalBus;
        private List<PhotoData> _photoData = new List<PhotoData>();
        private int _counter;

        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<PhotoTakenSignal>(s => SerializePhotoFromByteArray(s.PhotoData, s.PhotoName));
            _signalBus.Subscribe<PhotoAnalyzedSignal>(s => AppendAnalyticsData(s.PhotoName, s.IsAnybodySmiling));
            _signalBus.Subscribe<MiniaturesPreparedSignal>(() =>
            {
                foreach (PhotoData data in _photoData)
                {
                    _signalBus.Fire(new UpdateMiniatureSignal(data.Smile, data.Name));
                }
            });
            _signalBus.Subscribe<ClearAllDataSignal>(ClearAllData);
            
            if (!PlayerPrefs.HasKey("PhotoCounter"))
            {
                PlayerPrefs.SetInt("PhotoCounter", 0);
                PlayerPrefs.Save();
            }

            _counter = PlayerPrefs.GetInt("PhotoCounter");

            _photoData = GetSerializedPhotoData();
        }

        private void ClearAllData()
        {
            string path = GetPhotoDataPath();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            _photoData.Clear();
            
            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
            
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.FullName.EndsWith(".png"))
                {
                    file.Delete();
                }
            }
        }

        private void SerializePhotoData()
        {
            string path = GetPhotoDataPath();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(JsonConvert.SerializeObject(_photoData));
            }
        }

        private List<PhotoData> GetSerializedPhotoData()
        {
            string path = GetPhotoDataPath();

            if (!File.Exists(path))
            {
                SerializePhotoData();
            }

            List<PhotoData> photoData;
            
            using (StreamReader reader = new StreamReader(path))
            {
                string unparsedData = reader.ReadToEnd();
                photoData = JsonConvert.DeserializeObject<List<PhotoData>>(unparsedData);
            }

            return photoData;
        }

        private void AppendAnalyticsData(string photoName, bool isAnybodySmiling)
        {
            _photoData.Add(new PhotoData(photoName, isAnybodySmiling));
            SerializePhotoData();
        }

        private async void SerializePhotoFromByteArray(byte[] photoData, string photoName)
        {
            string imageFilePath = GetPhotoFilePath(photoName);
            
            _counter++;
            PlayerPrefs.SetInt("PhotoCounter", _counter);
            PlayerPrefs.Save();
            
            using (FileStream fileStream = File.Open(imageFilePath, FileMode.OpenOrCreate))
            {
                await fileStream.WriteAsync(photoData, 0, photoData.Length);
            }

            _signalBus.Fire<PhotoSerializedSignal>();      
        }

        public List<Photo> GetNotYetLoadedMiniatures(List<string> alreadyLoadedMiniatures)
        {
            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath);
            List<Photo> miniatures = new List<Photo>();
            
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.FullName.EndsWith(".png") && !alreadyLoadedMiniatures.Contains(file.Name))
                {
                    miniatures.Add(new Photo(file.Name, GetTexture2DFromFile(file.FullName)));
                }
            }

            return miniatures;
        }

        private Texture2D GetTexture2DFromFile(string path)
        {
            if (File.Exists(path))
            {
                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.LoadImage(File.ReadAllBytes(path));
                texture.Apply();
                
                return texture;
            }
            
            return null;
        }

        public string GetNextPhotoName()
        {
            return String.Format("Image{0}.png", _counter);
        }

        private string GetPhotoFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        private string GetPhotoDataPath()
        {
            return Path.Combine(Application.persistentDataPath, "photoData");
        }
    }
    
    public class Photo
    {
        public Texture2D Texture;
        public string Name;

        public Photo(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;
        }
    }

    public class PhotoData
    {
        public string Name;
        public bool Smile;

        public PhotoData(string name, bool smile)
        {
            Name = name;
            Smile = smile;
        }
    }
}