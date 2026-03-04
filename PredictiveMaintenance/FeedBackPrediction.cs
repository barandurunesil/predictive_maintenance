using Microsoft.ML.Data;

namespace PredictiveMaintenance.DataStructures
{
    public class FeedBackPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedFailure { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }
}