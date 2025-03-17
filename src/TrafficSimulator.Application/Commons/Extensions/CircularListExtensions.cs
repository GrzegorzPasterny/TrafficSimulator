using TrafficSimulator.Application.Commons.Helpers;

namespace TrafficSimulator.Application.Commons.Extensions
{
	public static class CircularListExtensions
	{
		public static bool Set<T, K>(this CircularList<T> circularList, K elementToSet, Func<T, K> selector)
		{
			if (circularList == null)
				return false; // Handle empty list case

			T firstElement = circularList.Current; // Remember start position
			IEqualityComparer<K> comparerK = EqualityComparer<K>.Default;
			IEqualityComparer<T> comparerT = EqualityComparer<T>.Default;

			do
			{
				if (comparerK.Equals(selector(circularList.Current), elementToSet))
					return true; // Stop when we find the matching element

				circularList.MoveNext();
			}
			while (!comparerT.Equals(circularList.Current, firstElement)); // Stop if we complete one full loop

			return false;
		}
	}
}
