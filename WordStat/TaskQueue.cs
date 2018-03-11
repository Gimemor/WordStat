using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WordStat
{
	/// <summary>
	/// Очередь задач
	/// </summary>
	public class TaskQueue
	{
		private static TaskQueue _instance;
		private ConcurrentQueue<Tuple<FileGroup, int>> _taskList;
		private ConcurrentDictionary<string, int> _answerList;
		private ManualResetEvent _queueIsEmpty;
		private TaskQueue()
		{
			_taskList = new ConcurrentQueue<Tuple<FileGroup, int>>();
			_answerList = new ConcurrentDictionary<string, int>();
			_queueIsEmpty = new ManualResetEvent(false);
		}

		public static TaskQueue GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TaskQueue();
			}
			return _instance;
		}


		/// <summary>
		/// Добавить задание в очередь
		/// Задание состоит из пары - группа файлов, минимальная длина
		/// </summary>
		public void AddTask(FileGroup files, int wordLength)
		{
			_taskList.Enqueue(new Tuple<FileGroup, int>(files, wordLength));
			_queueIsEmpty.Reset();
		}
		/// <summary>
		/// Получить задание из очереди
		/// </summary>
		/// <returns>Кортеж с группой файлов и минмальной длины</returns>
		public Tuple<FileGroup, int> GetTask()
		{
			Tuple<FileGroup, int> task = null;
			var result = _taskList.TryDequeue(out task);
			if (_taskList.Count == 0)
			{
				_queueIsEmpty.Set();
			}
			return result ? task : null;
		}
		
		/// <summary>
		/// Ожидаем, пока все задачи не будут разобраны
		/// </summary>
		public void WaitForQueueComplete()
		{
			_queueIsEmpty.WaitOne();
		}
		/// <summary>
		/// Добавить результат в общую статистику
		/// </summary>
		public void AddResult(Dictionary<string, int> result)
		{
			int i = 0;
			while(i < result.Keys.Count)
			{
				var key = result.Keys.ElementAt(i);
				var value = result[key];
				if (_answerList.ContainsKey(key))
				{
					_answerList[key] += value;
				}
				else if(!_answerList.TryAdd(key, value))
				{
					continue;
				}
				i++;
			}

		}
		/// <summary>
		/// Получаем ответ на задание
		/// </summary>
		internal List<Tuple<int, string>> GetAnswer(int resultSetSize = 10)
		{
			return _answerList.Select(t => new Tuple<int, string>(t.Value, t.Key))
			           .OrderByDescending(t => t.Item1).Take(resultSetSize).ToList();
		}
	}
}
