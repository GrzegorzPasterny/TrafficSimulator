using Microsoft.ML;
using TrafficSimulator.Application.Commons.Interfaces;
using TrafficSimulator.Domain.AI;

namespace TrafficSimulator.Infrastructure.AI
{
	/// <summary>
	/// AI agent capable of using reinforcement learning to train to steer traffic lights
	/// </summary>
	public class AiRlAgent : IAiLearningAgent
	{
		private readonly MLContext _mlContext;
		private ITransformer _model;
		private readonly List<TrafficState> _trainingData = new();
		private readonly string _modelPath;
		private PredictionEngine<TrafficState, TrafficState> _predictionEngine;

		public AiRlAgent(string modelPath)
		{
			_modelPath = modelPath;
			_mlContext = new MLContext();
			LoadOrCreateModel();
		}

		public IReadOnlyList<float> Predict(IEnumerable<float> input)
		{
			var prediction = _predictionEngine.Predict(new TrafficState { Inputs = input.ToArray() });
			return prediction.QValues;
		}

		public void CollectTrainingData(TrafficState state)
		{
			_trainingData.Add(state);
		}

		public void TrainModel()
		{
			if (_trainingData.Count < 100) return;

			var dataView = _mlContext.Data.LoadFromEnumerable(_trainingData);

			var pipeline = _mlContext.Transforms
				.Concatenate("Features", nameof(TrafficState.Inputs))
				.Append(_mlContext.Regression.Trainers.Sdca());

			_model = pipeline.Fit(dataView);
			_predictionEngine = _mlContext.Model.CreatePredictionEngine<TrafficState, TrafficState>(_model);

			Directory.CreateDirectory(Path.GetDirectoryName(_modelPath)!);

			using var fs = new FileStream(_modelPath, FileMode.Create, FileAccess.Write);
			_mlContext.Model.Save(_model, dataView.Schema, fs);

			_trainingData.Clear();
		}

		private void LoadOrCreateModel()
		{
			if (File.Exists(_modelPath))
			{
				using var fs = new FileStream(_modelPath, FileMode.Open, FileAccess.Read);
				_model = _mlContext.Model.Load(fs, out _);
			}
			else
			{
				var dataView = _mlContext.Data.LoadFromEnumerable(
					new[]
					{
						new TrafficState
						{
							Inputs = new float[8],
							Reward = 0f
						}
					});

				var pipeline = _mlContext.Transforms
					.Concatenate("Features", nameof(TrafficState.Inputs))
					.Append(_mlContext.Transforms.CopyColumns("Score", "Label"))
					;

				_model = pipeline.Fit(dataView);
			}

			_predictionEngine = _mlContext.Model.CreatePredictionEngine<TrafficState, TrafficState>(_model);
		}
	}
}
