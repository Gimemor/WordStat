using System;
using System.Diagnostics;

namespace WordStat
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			// Разбираем аргументы командной строки, либо используем аргументы по-умолчанию
			string path = @"..\WordCorpus\text_splitted_corpus";
			int wordLength = 8;
			if (args.Length > 0)
			{
				path = args[0];
			}
			if (args.Length > 1 && !Int32.TryParse(args[1], out wordLength))
			{
				Console.WriteLine("Невозможно разобрать аргумент 'Длина слова'");
				return;
			}
			// Создаем объект приложения и выполняем вычисления
			using (var app = new Application())
			{
				app.Execute(path, wordLength);
			}

			stopWatch.Stop();
			var time = stopWatch.Elapsed;
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				time.Hours, time.Minutes, time.Seconds,
				time.Milliseconds / 10);
			Console.WriteLine("Вермя выполнения " + elapsedTime);
		}
	}
}