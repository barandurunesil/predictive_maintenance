using Microsoft.ML.Data;

namespace PredictiveMaintenance.DataStructures
{
    public class FeedBackTrainingData
    {
        [LoadColumn(1), ColumnName(@"Product ID")]
        public string Product_ID { get; set; }

        [LoadColumn(2), ColumnName(@"Type")]
        public string Type { get; set; }

        [LoadColumn(3), ColumnName(@"Air temperature")]
        public float Air_temperature { get; set; }

        [LoadColumn(4), ColumnName(@"Process temperature")]
        public float Process_temperature { get; set; }

        [LoadColumn(5), ColumnName(@"Rotational speed")]
        public float Rotational_speed { get; set; }

        [LoadColumn(6), ColumnName(@"Torque")]
        public float Torque { get; set; }

        [LoadColumn(7), ColumnName(@"Tool wear")]
        public float Tool_wear { get; set; }

        // ✅ Binary label
        [LoadColumn(8), ColumnName(@"Machine failure")]
        public bool Machine_failure { get; set; }

        // (Şimdilik kullanmıyoruz, ama load kalsın)
        [LoadColumn(9), ColumnName(@"TWF")]
        public float TWF { get; set; }

        [LoadColumn(10), ColumnName(@"HDF")]
        public float HDF { get; set; }

        [LoadColumn(11), ColumnName(@"PWF")]
        public float PWF { get; set; }

        [LoadColumn(12), ColumnName(@"OSF")]
        public float OSF { get; set; }

        [LoadColumn(13), ColumnName(@"RNF")]
        public float RNF { get; set; }
    }
}