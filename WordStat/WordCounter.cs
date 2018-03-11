using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WordStat
{
	/// <summary>
	/// Счетчик слов в файле
	/// </summary>
	public class WordCounter
	{
		public static Dictionary<string, int> GetWordLengthStatistic(
			string filePath,
			int minimalLength)
		{
			var wordStatistics = new Dictionary<string, int>(1024);
			var wordPatternString = String.Format(
				@"(?<word>\w{{{0},}})", minimalLength);
			var wordPattern = new Regex(wordPatternString, RegexOptions.IgnoreCase);
			
			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			using (var file = new StreamReader(stream))
			{
				var currentString = file.ReadLine();
				while (currentString != null)
				{
					// ищем уникальные слова 
					var matches = wordPattern.Matches(currentString);
					// добавляем в статистику
					foreach (Match match in matches)
					{
						// Добавляем количество встреченных слов
						if (wordStatistics.ContainsKey(match.Value))
						{
							wordStatistics[match.Value] += 1;
						}
						else
						{
							wordStatistics.Add(match.Value, 1);
						}
					}
					currentString = file.ReadLine();
				}
			}
			return wordStatistics;
		}
	}
}
