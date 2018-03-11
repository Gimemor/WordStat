using System;
using System.Collections;
using System.Collections.Generic;

namespace WordStat
{
	/// <summary>
	/// Класс представляет коллекцию путей до файлов
	/// </summary>
	public class FileGroup : IEnumerable<string>
	{
		public List<string> FileNames { get; private set; }
		public FileGroup(List<string> group)
		{
			FileNames = new List<string>(group);
		}

		public void Add(List<string> group)
		{
			FileNames.AddRange(group);
		}
		public IEnumerator<string> GetEnumerator()
		{
			return new FileEnumerator(FileNames);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
	/// <summary>
	/// Перечислитель для коллекции
	/// </summary>
	public class FileEnumerator : IEnumerator<string>
	{
		private List<string>_fileNames;
		private int _currentIndex = -1;

		public FileEnumerator(List<string> fileNames)
		{
			_fileNames = fileNames;
		}


		object IEnumerator.Current => Current;
		public string Current
		{
			get
			{
				try
				{
					return _fileNames[_currentIndex];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException("Недопустимый индекс");
				}
			}
		}



		public void Dispose()
		{
			
		}

		public bool MoveNext()
		{
			_currentIndex++;
			return _currentIndex < _fileNames.Count;
		}

		public void Reset()
		{
			_currentIndex = -1;
		}
	}
}