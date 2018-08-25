using System;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Step3.DataPrediction.Models;

namespace Step3.DataPrediction
{
    class Program
    {
        static readonly string modelPathComments = "../Step2.DataTraining/model_comments.zip";
        static readonly string modelPathLikes = "../Step2.DataTraining/model_likes.zip";

        static void Main(string[] args)
        {
            var tags = args[0];
            PredictLikes(tags).Wait();
            PredictComments(tags).Wait();
        }

        public async static Task PredictLikes(string tags)
        {
            var model = await PredictionModel.ReadAsync<InstagramData, PredictedLikes>(modelPathLikes);

            // Predict
            var prediction = model.Predict(new InstagramData
            {
                Tags = tags,
            });

            Console.WriteLine($"Predicted likes: {prediction.Score}");
        }

        public async static Task PredictComments(string tags)
        {
            var model = await PredictionModel.ReadAsync<InstagramData, PredictedComments>(modelPathComments);

            // Predict
            var prediction = model.Predict(new InstagramData
            {
                Tags = tags,
            });

            Console.WriteLine($"Predicted comments: {prediction.Score}");
        }
    }
}
