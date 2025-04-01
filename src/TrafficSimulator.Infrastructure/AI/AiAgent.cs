using Microsoft.ML;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.AI;

namespace TrafficSimulator.Infrastructure.AI
{
	public class AiAgent : IAiAgent
	{
		private readonly MLContext _mlContext;
		private readonly ITransformer _model;
		private readonly PredictionEngine<TrafficState, TrafficState> _predictionEngine;

		public AiAgent(string modelPath)
		{
			_mlContext = new MLContext();
			var dataView = _mlContext.Data.LoadFromEnumerable(new List<TrafficState>());

			var pipeline = _mlContext.Transforms.ApplyOnnxModel(
				modelFile: modelPath,
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
