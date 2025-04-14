using Microsoft.ML;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.AI;

namespace TrafficSimulator.Infrastructure.AI
{
	/// <summary>
	/// Uses external onnx file to load the trained AI agent
	/// </summary>
	public class AiOnnxAgent : IAiAgent
	{
		private readonly MLContext _mlContext;
		private ITransformer _model;
		private readonly string _modelPath;
		private PredictionEngine<TrafficState, TrafficState> _predictionEngine;

		public AiOnnxAgent(string modelPath)
		{
			if (!File.Exists(modelPath))
			{
				throw new Exception($"Onnx model file does not exist [FileName = {modelPath}]");
			}

			_modelPath = modelPath;
			_mlContext = new MLContext();
			var dataView = _mlContext.Data.LoadFromEnumerable(new List<TrafficState>());

			var pipeline = _mlContext.Transforms.ApplyOnnxModel(
				modelFile: _modelPath,
				inputColumnNames: new[] { "Inputs" },
				outputColumnNames: new[] { "QValues" });

			_model = pipeline.Fit(dataView);
			_predictionEngine = _mlContext.Model.CreatePredictionEngine<TrafficState, TrafficState>(_model);
		}

		public IReadOnlyList<float> Predict(IEnumerable<float> input)
		{
			var prediction = _predictionEngine.Predict(new TrafficState { Inputs = input.ToArray() });
			return prediction.QValues;
		}
	}
}
