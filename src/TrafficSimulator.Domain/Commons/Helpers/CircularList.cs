namespace TrafficSimulator.Application.Commons.Helpers
{
	public class CircularList<T>
	{
		private readonly List<T> _items;
		private int _currentIndex;

		public CircularList(IEnumerable<T> items)
		{
			_items = items.ToList();

			if (_items.Count is 0)
			{
				throw new ArgumentException($"Empty list has been loaded into {nameof(CircularList<T>)}");
			}

			_currentIndex = 0;
		}

		public T Current => _items[_currentIndex];

		public void MoveNext()
		{
			_currentIndex = (_currentIndex + 1) % _items.Count;
		}

		public void MovePrevious()
		{
			_currentIndex = (_currentIndex - 1 + _items.Count) % _items.Count;
		}

		public void Reset()
		{
			_currentIndex = 0;
		}
	}
}
