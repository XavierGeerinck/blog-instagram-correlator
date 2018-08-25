using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.IO;

namespace Step2.DataTraining.Models
{
    public class PredictedComments
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}