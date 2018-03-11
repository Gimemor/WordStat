using System;
using System.Collections.Generic;
using System.Text;

namespace WordStat
{
	/// <summary>
	/// Класс приложения асинхронного счетчика слов
	/// </summary>
	class Application : IDisposable
	{

		private TaskController _taskController;
		private TaskQueue      _taskQueue;
		public Application()
		{
			_taskController = TaskController.GetInstance();
			_taskQueue = TaskQueue.GetInstance();
		}

		static int GetOptimalThreadCount(int fileCount)
		{
			// Распредилить нагрузку равномерно по потокам, либо max = количество процессоров
			int cpuCount = Environment.ProcessorCount;
			return fileCount <= cpuCount ? fileCount : cpuCount;
		}

		/// <summary>
		/// Выполнить подсчет
		/// </summary>
		public void Execute(string path, int wordLength = 8, int resultSetSize = 10)
		{
			// Создаем катлог файлов в указаном каталоге
			TextFileCatalog fileCatalog = null;

			if (wordLength <= 0 || resultSetSize <= 0)
			{
				Console.WriteLine(
					"Длина слов и размер множества должны быть положительным числом");
				return;
			}

			try
			{
				fileCatalog = new TextFileCatalog(path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					String.Format("Не удалось описать каталог. Проверьте путь:\n{0}",
					ex.Message));
				return;
			}

			// Считаем оптимальное количество обработчиков
			int threadCount = GetOptimalThreadCount(fileCatalog.FileCount);
			// Запускаем потоки на исполнение
			_taskController.Start(threadCount);
			// Делим файлы на группы
			fileCatalog.DivideIntoGroups(threadCount);
			// Передаем группы в очередь заявок
			foreach (var group in fileCatalog)
			{
				_taskQueue.AddTask(group, wordLength);
			}
			// Теперь очередь заполнена.
			// Ожидаем пока уйдут все заявки
			_taskQueue.WaitForQueueComplete();
			// Завершаем обработку заявок и отключаем исполнителя
			_taskController.Stop();
			// Получаем результаты
			PrintResult(resultSetSize);
		}

		private void PrintResult(int resultSetSize)
		{
			var statistic = _taskQueue.GetAnswer(resultSetSize);
			foreach (var entry in statistic)
			{
				Console.WriteLine("{0}\t{1}", entry.Item2, entry.Item1);
			}
		}

		public void Dispose()
		{
			_taskController.Dispose();
		}
	}
}
