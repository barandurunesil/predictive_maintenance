using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.ML.Trainers.LightGbm;
using static Microsoft.ML.DataOperationsCatalog;
using PredictiveMaintenance.DataStructures;
using System.Globalization;

namespace PredictiveMaintenance
{
    internal static class Program
    {
        //Declaring function for model paths
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        //Input Path Declaration
        private static readonly string baseDataPath = @"../../../../Data";
        private static readonly string dataRelativePath = $"{baseDataPath}/dataset.csv";
        private static readonly string dataPath = GetAbsolutePath(dataRelativePath);

        //Model Path Declaration
        private static readonly string baseModelPath = @"../../../../Data";
        private static readonly string modelRelativePath = $"{baseModelPath}/model.zip";
        private static readonly string modelPath = GetAbsolutePath(modelRelativePath);

        private static float ReadFloat(string message)
        {
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                input = input?.Replace(',', '.');

                if (float.TryParse(input,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float value))
                {
                    return value;
                }

                Console.WriteLine("    Invalid number, try again.");
            }
        }

        public static void Main(string[] args)
        {
            //Creating MLContext
            MLContext mlContext = new MLContext(seed: 1);

            IDataView dataView = mlContext.Data.LoadFromTextFile<FeedBackTrainingData>(dataPath, separatorChar: ',', hasHeader: true);

            TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            IDataView trainingData = trainTestSplit.TrainSet;
            IDataView testData = trainTestSplit.TestSet;

            //Creating Pipeline
            var pipeline =
                mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "TypeEncoded",
                    inputColumnName: @"Type",
                    outputKind: OneHotEncodingEstimator.OutputKind.Indicator)

                .Append(mlContext.Transforms.ReplaceMissingValues(new[]
                {
                    new InputOutputColumnPair(@"Air temperature", @"Air temperature"),
                    new InputOutputColumnPair(@"Process temperature", @"Process temperature"),
                    new InputOutputColumnPair(@"Rotational speed", @"Rotational speed"),
                    new InputOutputColumnPair(@"Torque", @"Torque"),
                    new InputOutputColumnPair(@"Tool wear", @"Tool wear"),
                }))

                .Append(mlContext.Transforms.Concatenate("Features", new[]
                {
                    "TypeEncoded",
                    @"Air temperature",
                    @"Process temperature",
                    @"Rotational speed",
                    @"Torque",
                    @"Tool wear"
                }))

                .Append(mlContext.BinaryClassification.Trainers.LightGbm(
                    new LightGbmBinaryTrainer.Options
                    {
                        LabelColumnName = @"Machine failure",
                        FeatureColumnName = "Features",
                        NumberOfLeaves = 64,
                        MinimumExampleCountPerLeaf = 20,
                        LearningRate = 0.05,
                        NumberOfIterations = 500
                    }));


            ITransformer trainedmodel = pipeline.Fit(trainingData);

            var predictions = trainedmodel.Transform(testData);
            var metrics = mlContext.BinaryClassification.Evaluate(
                data: predictions,
                labelColumnName: @"Machine failure");

            mlContext.Model.Save(trainedmodel, trainingData.Schema, modelPath);

            Console.WriteLine("==================================== Evaluating Model Accuracy ====================================\n");
            Console.WriteLine($"    Accuracy : {metrics.Accuracy:0.####}");
            Console.WriteLine($"    AUC      : {metrics.AreaUnderRocCurve:0.####}");
            Console.WriteLine($"    F1       : {metrics.F1Score:0.####}");
            Console.WriteLine($"    Precision: {metrics.PositivePrecision:0.####}");
            Console.WriteLine($"    Recall   : {metrics.PositiveRecall:0.####}");
            //Console.WriteLine("\n    CONFUSION MATRIX:");
            //Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());
            Console.WriteLine("\n===================================================================================================\n");

            string feedbacktype;

            while (true)
            {
                Console.Write("    Enter Type (L/M/H): ");
                feedbacktype = (Console.ReadLine() ?? "")
                    .Trim()
                    .ToUpperInvariant();

                if (feedbacktype == "L" || feedbacktype == "M" || feedbacktype == "H")
                    break;

                Console.WriteLine("    Invalid input. Please enter only L, M or H.");
            }
            float feedbackairtemp = ReadFloat("    Enter Air Temperature: ");
            float feedbackprocesstemp = ReadFloat("    Enter Process Temperature: ");
            float feedbackrpm = ReadFloat("    Enter Rotational Speed: ");
            float feedbacktorque = ReadFloat("    Enter Torque: ");
            float feedbacktoolwear = ReadFloat("    Enter Tool Wear: ");
            Console.WriteLine("\n===================================================================================================\n");

            FeedBackTrainingData feedbackinput = new FeedBackTrainingData();

            feedbackinput.Type = feedbacktype;
            feedbackinput.Air_temperature = feedbackairtemp;
            feedbackinput.Process_temperature = feedbackprocesstemp;
            feedbackinput.Rotational_speed = feedbackrpm;
            feedbackinput.Torque = feedbacktorque;
            feedbackinput.Tool_wear = feedbacktoolwear;



            var predictionEngine = mlContext.Model.CreatePredictionEngine<FeedBackTrainingData, FeedBackPrediction>(trainedmodel);
            var prediction = predictionEngine.Predict(feedbackinput);

            string resultText = prediction.PredictedFailure ? "FAILURE" : "NO FAILURE";

            Console.WriteLine($"    Prediction         : {resultText}");
            Console.WriteLine($"    Failure Probability: %{prediction.Probability * 100:0.##}");

            Console.WriteLine("\n================================== Hit any key to stop process. ===================================\n");
            Console.ReadLine();

        }

    }
}