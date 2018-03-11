using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WordStat
{
	/// <summary>
	/// Класс служит для управления потоками вычисления
	/// Является синглтоном
	/// </summary>
	class TaskController : IDisposable
	{
		// Очередь заявок
		private TaskQueue _taskQueue;
		// Список исполнителей
		private List<TaskProcessor> _taskList;
		// Источник токена отмены
		private CancellationTokenSource _cancellationTokenSource;

		private TaskController()
		{
			_taskQueue = TaskQueue.GetInstance();
			_taskList = new List<TaskProcessor>(32);
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private static TaskController _instance;
		public static TaskController GetInstance()
		{
			if (_instance == null)
			{
				_instance = new TaskController();
			}
			return _instance;
		}

		/// <summary>
		/// Запуск потоков обработки задачи
		/// </summary>
		public void Start(int threadCount)
		{
			// Запускаем n потоков на исполнение
			for (int i = 0; i < threadCount; i++)
			{
				var currentTask = new TaskProcessor(_cancellationTokenSource.Token);
				_taskList.Add(currentTask);
				currentTask.Start();
			}
		}
		/// <summary>
		/// Завершение работы потоков обработки задачи
		/// </summary>
		public void Stop()
		{
			_cancellationTokenSource.Cancel();
			foreach (var task in _taskList)
			{
				task.Stop();
			}
		}

		public void Dispose()
		{
			_cancellationTokenSource.Dispose();
		}
	}
}
