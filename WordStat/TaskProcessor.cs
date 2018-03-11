using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WordStat
{
	/// <summary>
	/// Класс для управления обработчиком задачи подсчета в группе файлов
	/// </summary>
	class TaskProcessor
	{
		private Thread _thread;
		private TaskQueue _taskQueue;
		private CancellationToken _cancellationToken;

		public TaskProcessor(CancellationToken cts)
		{
			_taskQueue = TaskQueue.GetInstance();
			_thread = new Thread(Execute);
			_cancellationToken = cts;
		}
		/// <summary>
		/// Запуск потока
		/// </summary>
		public void Start()
		{
			_thread.IsBackground = true;
			_thread.Start();
		}

		/// <summary>
		/// Выполнение задачи
		/// </summary>
		public void Execute()
		{
			while (_cancellationToken.IsCancellationRequested == false)
			{
				// Взять группу файлов из очереди
				var task = _taskQueue.GetTask();
				if (task == null)
				{
					continue;
				}

				var group = task.Item1;
				// Обработать группу файлов
				foreach (var path in group)
				{
					var currentStatistics = WordCounter.GetWordLengthStatistic(path, task.Item2);
					// Положить результат в выходной буфер
					_taskQueue.AddResult(currentStatistics);
				}
			}
		}

		/// <summary>
		/// Остановка задачи
		/// </summary>
		public void Stop()
		{
			_thread.Join();
		}

	}
}
