using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Runtime.Api;
using Step2.DataTraining.Models;

namespace Step2.DataTraining
{
    public class Program
    {
        static string dataPath = "../Step1.DataPrep/output/result.csv";
        static void Main(string[] args)
        {
            // Features: is our input (tags is our input -> based on what do we predict?) 
            // Labels: is our output (comments is our output -> what do we want to predict?)
            Console.WriteLine("=================== Training Model Likes ===================");
            CreateModelLikes().Wait();
            Console.WriteLine("=================== Training Model Comments ===================");
            CreateModelComments().Wait();
        }

        public static async Task CreateModelLikes()
        {
            // 1. Load in our data which has the header on the top and we use a csv so ';'
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(dataPath).CreateFrom<InstagramData>(useHeader: true, separator: ';'));

            // 2. Prepare our Label (what do we want to predict?)
            //      ColumnCopier: Duplicates columns from the dataset 
            //          -> we need to do this since Label is the column used and ours is called LabelLikes
            pipeline.Add(new ColumnCopier(("LabelLikes", "Label")));

            // 3. Prepare our features (based on what do we predict?)
            // Process Tags in a OneHotVector
            //      WordTokenizer: splits the text into words using the separator characters
            //      CategoricalOneHotVectorizer: We create a table of the different tags and set a 1 where the tag appears in our dataset
            pipeline.Add(new WordTokenizer("Tags") { TermSeparators = "," });
            pipeline.Add(new CategoricalOneHotVectorizer("Tags"){ OutputKind = CategoricalTransformOutputKind.Bag } );

            // Prepare Features to train with, in this case with the "Tags" of our instagram pictures
            //      ColumnConcatenator: Concatenates one or more columns of the same item type
            var features = new string[] { "Tags" };
            pipeline.Add(new ColumnConcatenator("Features", "Tags"));

            // 4. Train our model
            pipeline.Add(new StochasticDualCoordinateAscentRegressor());
            PredictionModel<InstagramData, PredictedLikes> model = pipeline.Train<InstagramData, PredictedLikes>();

            // 5. Save the trained model
            await model.WriteAsync("model_likes.zip");
        }

        public static async Task CreateModelComments()
        {

            // 1. Load in our data which has the header on the top and we use a csv so ';'
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(dataPath).CreateFrom<InstagramData>(useHeader: true, separator: ';'));

            // 2. Prepare our Label (what do we want to predict?)
            //      ColumnCopier: Duplicates columns from the dataset 
            //          -> we need to do this since Label is the column used and ours is called LabelComments
            pipeline.Add(new ColumnCopier(("LabelComments", "Label")));

            // 3. Prepare our features (based on what do we predict?)
            // Process Tags in a OneHotVector
            //      WordTokenizer: splits the text into words using the separator characters
            //      CategoricalOneHotVectorizer: We create a table of the different tags and set a 1 where the tag appears in our dataset
            pipeline.Add(new WordTokenizer("Tags") { TermSeparators = "," });
            pipeline.Add(new CategoricalOneHotVectorizer("Tags"){ OutputKind = CategoricalTransformOutputKind.Bag } );

            // Prepare Features to train with, in this case with the "Tags" of our instagram pictures
            //      ColumnConcatenator: Concatenates one or more columns of the same item type
            var features = new string[] { "Tags" };
            pipeline.Add(new ColumnConcatenator("Features", "Tags"));

            // 4. Train our model
            pipeline.Add(new StochasticDualCoordinateAscentRegressor());
            var model = pipeline.Train<InstagramData, PredictedComments>();

            // 5. Save the trained model
            await model.WriteAsync("model_comments.zip");
        }
    }
}