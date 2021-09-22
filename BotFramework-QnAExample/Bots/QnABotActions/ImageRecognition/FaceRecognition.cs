using BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition;
using BotFramework_QnAExample.Util.Image;
using BotFramework_QnAExample.Utils.ImageAPI.API;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotFramework_QnAExample.Bots.QnABotActions
{
    public class FaceRecognition : IImgRecognition
    {
        string _SUBSCRIPTION_KEY;
        string _ENDPOINT;
        IFaceClient _client;
        IList<DetectedFace> _detectedFaces;

        public FaceRecognition(string subscriptionKey, string endpoint)
        {
            _SUBSCRIPTION_KEY = subscriptionKey;
            _ENDPOINT = endpoint;
            _client = Authenticate(_ENDPOINT, _SUBSCRIPTION_KEY);
        }

        public IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public async Task AnalyzeImgAttachment(Attachment attachment)
        {
            //saving the image sent by the user as bytes
            var imageBytes = ImageUtils.StreamImageFromUrl(attachment.ContentUrl, ""); //This image url doesn't contain content type in the url

            //Uploading the sent image and getting the link to access it
            IImageAPI imgurApi = new ImgurAPI();
            var imgLink = await imgurApi.UploadImageFromStream(imageBytes, attachment.ContentType);

            var attributes = new List<FaceAttributeType>
            {
                FaceAttributeType.Accessories,
                FaceAttributeType.Age,
                FaceAttributeType.Blur,
                FaceAttributeType.Emotion,
                FaceAttributeType.Exposure,
                FaceAttributeType.FacialHair,
                FaceAttributeType.Gender,
                FaceAttributeType.Glasses,
                FaceAttributeType.Hair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup,
                FaceAttributeType.Noise,
                FaceAttributeType.Occlusion,
                FaceAttributeType.Smile
            };

            // Detect faces with all attributes from image url.
            _detectedFaces = await _client.Face.DetectWithUrlAsync($"{imgLink}",
                    returnFaceAttributes: attributes,
                    // We specify detection model 1 because we are retrieving attributes.
                    detectionModel: DetectionModel.Detection01,
                    recognitionModel: RecognitionModel.Recognition04);
        }

        public string GetImgTagNames()
        {
            var details = new List<string>();
            var faceIndex = 0;
            foreach (var face in _detectedFaces)
            {
                faceIndex += 1;
                details.Add($"{faceIndex}. face");

                // Get bounding box of the faces
                details.Add($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");

                // Get accessories of the faces
                List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                int count = face.FaceAttributes.Accessories.Count;
                string accessory; string[] accessoryArray = new string[count];
                if (count == 0) { accessory = "NoAccessories"; }
                else
                {
                    for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                    accessory = string.Join(",", accessoryArray);
                }
                details.Add($"Accessories : {accessory}");

                // Get face other attributes
                details.Add($"Age : {face.FaceAttributes.Age}");
                details.Add($"Blur : {face.FaceAttributes.Blur.BlurLevel}");

                // Get emotion on the face
                string emotionType = string.Empty;
                double emotionValue = 0.0;
                Emotion emotion = face.FaceAttributes.Emotion;
                if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                details.Add($"Emotion : {emotionType}");

                // Get more face attributes
                details.Add($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
                details.Add($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                details.Add($"Gender : {face.FaceAttributes.Gender}");
                details.Add($"Glasses : {face.FaceAttributes.Glasses}");

                // Get hair color
                Hair hair = face.FaceAttributes.Hair;
                string color = null;
                if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                HairColorType returnColor = HairColorType.Unknown;
                double maxConfidence = 0.0f;
                foreach (HairColor hairColor in hair.HairColor)
                {
                    if (hairColor.Confidence <= maxConfidence) { continue; }
                    maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                }
                details.Add($"Hair : {color}");

                // Get more attributes
                details.Add($"HeadPose : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}");
                details.Add($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                details.Add($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                details.Add($"Occlusion : {string.Format("EyeOccluded = {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
                    $" {string.Format("ForeheadOccluded = {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded = {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
                details.Add($"Smile : {face.FaceAttributes.Smile}");
            }

            var joinedTagsString = String.Join("\n\n", details);
            var response = String.IsNullOrEmpty(joinedTagsString) ? "" : $"This image contains the following information: {joinedTagsString}";

            return response;
        }
    }
}
