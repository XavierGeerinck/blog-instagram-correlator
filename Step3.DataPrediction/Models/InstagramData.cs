using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.IO;

namespace Step3.DataPrediction.Models
{
    // input for prediction operations
    // - First 4 properties are inputs/features used to predict the label
    // - Label is what you are predicting, and is only set when training
    public class InstagramData
    {
        [Column(ordinal: "0")]
        public float Shortcode { get; set; }

        [Column(ordinal: "1")]
        public float URL { get; set; }

        // We set name: "Label" since we will predict this
        [Column(ordinal: "2", name: "LabelLikes")]
        public float LabelLikes { get; set; }

        [Column(ordinal: "3", name: "LabelComments")]
        public float LabelComments { get; set; }

        [Column(ordinal: "4", name: "Tags")]
        public string Tags { get; set; }
    }
}