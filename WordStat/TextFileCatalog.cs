using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WordStat
{
	/// <summary>
	/// Класс предоставляет функционал перечисления, подсчета и группировки файлов
	/// в директории
	/// 1) Подсчитать файлы в директории
	/// 2) Разбить файлы на n групп
	/// 3) Перечислить группы
	/// </summary>
	public class TextFileCatalog : IEnumerable<FileGroup> 
	{
		private List<string> _fileList;
		private List<FileGroup> _fileGroup;

		public string DirectoryPath { get; private set; }
		public int FileCount { get; private set; }


		public TextFileCatalog(string path)
		{
			DirectoryPath = path;
			FileCount = FileList.Count;
			_fileGroup = new List<FileGroup>(16);
		}

		public List<string> FileList
		{
			get
			{
				if (_fileList == null)
				{
					_fileList = Directory.EnumerateFiles(DirectoryPath)
									.Where(t => Regex.IsMatch(t, ".*[.]txt")).ToList();
				}
				return _fileList;
			}
		}

		public void DivideIntoGroups(int groupCount)
		{
			int groupSize = FileCount / groupCount;
			int remainingFiles = FileCount % groupCount;

			_fileGroup.Clear();
			for (int i = 0; i < groupCount; i++)
			{
				_fileGroup.Add(
					new FileGroup(
						_fileList.Skip(groupSize * i).Take(groupSize).ToList()));
			}
			if (remainingFiles > 0)
			{
				_fileGroup.Last().Add(_fileList.Skip(groupSize * groupCount)
				                 .Take(remainingFiles).ToList());
			}
		}


		public IEnumerator<FileGroup> GetEnumerator()
		{
			return new FileGroupEnumerator(_fileGroup);
		}

		IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)GetEnumerator();
		 
	}

	public class FileGroupEnumerator : IEnumerator<FileGroup>
	{
		private List<FileGroup> _fileGroup;
		private int _currentGroupIndex = -1;

		object IEnumerator.Current => (object)Current;
		public FileGroup Current
		{
			get
			{
				try
				{
					return _fileGroup[_currentGroupIndex];
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException("Недопустимый индекс");
				}

			}
		}

		public FileGroupEnumerator(List<FileGroup> group)
		{
			_fileGroup = group;
		}

		public bool MoveNext()
		{
			_currentGroupIndex++;
			return _currentGroupIndex < _fileGroup.Count;
		}

		public void Reset()
		{
			_currentGroupIndex = 0;
		}

		public void Dispose()
		{
 		}
	}
}
