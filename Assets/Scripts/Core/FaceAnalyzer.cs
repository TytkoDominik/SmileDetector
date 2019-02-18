using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using DependencyInjection;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Core
{
    public class FaceAnalyzer
    {
        const string SubscriptionKey = "9a21246717644dc49fc3c046e5b6ca30";
        const string UriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

        [Inject]
        private SignalBus _signalBus;
        
        [Inject]
        private void Initialize()
        {
            _signalBus.Subscribe<PhotoTakenSignal>(s => MakeAnalysisRequest(s.PhotoData, s.PhotoName));
        }

        public async void MakeAnalysisRequest(byte[] photoData, string photoName)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            string requestParameters = "returnFaceAttributes=smile";
            string uri = UriBase + "?" + requestParameters;
            HttpResponseMessage response;

            using (ByteArrayContent content = new ByteArrayContent(photoData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();

                dynamic faces = JsonConvert.DeserializeObject<dynamic>(contentString);
                double smile = 0d;
                
                foreach (dynamic face in faces)
                {
                    smile = Mathf.Max((float)face["faceAttributes"]["smile"].Value, (float)smile);
                }
                
                bool isAnybodySmiling = smile > 0.3f;
                
                _signalBus.Fire(new PhotoAnalyzedSignal(isAnybodySmiling, photoName));
            }
        }
    }
}
