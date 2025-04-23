using SharpNeat;
using SharpNeat.Evaluation;
using System.Numerics;

namespace TrafficSimulator.AiAgentsTraining.SharpNeatDemo
{
	public class XorEvaluator<TScalar> : IPhenomeEvaluator<IBlackBox<TScalar>>
	where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
	{
		static readonly TScalar Half = TScalar.CreateChecked(0.5);
		static readonly TScalar Ten = TScalar.CreateChecked(10.0);

		public FitnessInfo Evaluate(IBlackBox<TScalar> box)
		{
			TScalar fitness = TScalar.Zero;
			bool success = true;

			TScalar output = Activate(box, TScalar.Zero, TScalar.Zero);
			success &= output < Half;
			fitness += output < Half ? TScalar.One : TScalar.Zero;

			box.Reset();

			output = Activate(box, TScalar.One, TScalar.One);
			success &= output < Half;
			fitness += output < Half ? TScalar.One : TScalar.Zero;

			box.Reset();

			output = Activate(box, TScalar.One, TScalar.Zero);
			success &= output > Half;
			fitness += output > Half ? TScalar.One : TScalar.Zero;

			box.Reset();

			output = Activate(box, TScalar.Zero, TScalar.One);
			success &= output > Half;
			fitness += output > Half ? TScalar.One : TScalar.Zero;

			// If all four responses were correct then we add 10 to the fitness.
			if (success)
				fitness += Ten;

			return new FitnessInfo(double.CreateSaturating(fitness));
		}

		private static TScalar Activate(
		IBlackBox<TScalar> box,
		TScalar in1, TScalar in2)
		{
			var inputs = box.Inputs.Span;
			var outputs = box.Outputs.Span;

			// Bias input.
			inputs[0] = TScalar.One;

			// XOR inputs.
			inputs[1] = in1;
			inputs[2] = in2;

			// Activate the black box.
			box.Activate();

			// Read output signal.
			TScalar output = outputs[0];
			Clamp(ref output);
			return output;
		}

		private static void Clamp(ref TScalar x)
		{
			if (x < TScalar.Zero) x = TScalar.Zero;
			else if (x > TScalar.One) x = TScalar.One;
		}
	}
}
