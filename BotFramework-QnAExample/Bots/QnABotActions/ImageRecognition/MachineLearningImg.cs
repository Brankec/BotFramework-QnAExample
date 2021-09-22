using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using System;
using System.IO;
using BotFramework_QnAExample.Util.Image;
using BotFramework_QnAExample.Utils;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition
{
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }
    
    public class ImagePrediction : ImageData
    {
        public float[] Score;

        public string PredictedLabelValue;
    }
    
    public class MachineLearningImg : IImgRecognition
    {
        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets/ML");
        static readonly string _imgsFolder = Path.Combine(_assetsPath, "images");
        static readonly string _trainTagsTsv = Path.Combine(_imgsFolder, "tags.tsv");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");
        public static ImagePrediction _imgPrediction = new ImagePrediction();

        public MachineLearningImg()
        {
        }
        
        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
        
        public static ITransformer GenerateModel(MLContext mlContext)
        {
            //Generating a model for image prediction
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input",
                    imageFolder: _imgsFolder, inputColumnName: nameof(ImageData.ImagePath))
                // The image transforms transform the images into the model's expected format.
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input",
                    imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight,
                    inputColumnName: "input"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input",
                    interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean)).Append(
                    mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).ScoreTensorFlowModel(
                        outputColumnNames: new[] {"softmax2_pre_activation"}, inputColumnNames: new[] {"input"},
                        addBatchDimensionInput: true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey",
                    inputColumnName: "Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey",
                    featureColumnName: "softmax2_pre_activation")).Append(
                    mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);

            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path:  _trainTagsTsv, hasHeader: false);
            ITransformer model = pipeline.Fit(trainingData);
            
            return model;
        }
        
        public static void ClassifySingleImage(MLContext mlContext, ITransformer model, string imagePath)
        {
            var imageData = new ImageData()
            {
                ImagePath = imagePath
            };
            
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            _imgPrediction = predictor.Predict(imageData);
        }
        
        public async Task AnalyzeImgAttachment(Attachment attachment)
        {
            var filePath = "";
            var guid = Guid.NewGuid();
            MLContext mlContext = new MLContext();
            
            ITransformer model = GenerateModel(mlContext);
            
            var imgType = ImageUtils.GetImageTypeFromContentType(attachment.ContentType);
            var imageName = guid + imgType;
            ImageUtils.SaveImageFromUrl(attachment.ContentUrl, _imgsFolder+"/"+imageName);
            
            //Saving the image with a GUID name so that we can save it and feed the machine learning with more images for more accurate classification
            ClassifySingleImage(mlContext, model, filePath);
            ResourceFiles.Add2ColumnLineToTextFile(_trainTagsTsv, imageName, _imgPrediction.PredictedLabelValue, "\t");
        }

        public string GetImgTagNames()
        {
            var joinedTagsString = String.Join(", ", new[] { _imgPrediction.PredictedLabelValue });
            var response = String.IsNullOrEmpty(joinedTagsString) ? "" : $"This image contains the following tag(s): {joinedTagsString}";

            return response;
        }
    }
}