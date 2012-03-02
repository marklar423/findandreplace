﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindAndReplace
{
	public class ReplacerEventArgs : EventArgs
	{
		public Replacer.ReplaceResultItem ResultItem { get; set; }
		public int TotalFilesCount { get; set; }
		public Statistic Stats { get; set; }

		public ReplacerEventArgs(Replacer.ReplaceResultItem resultItem, int fileCount, Statistic statistic)
		{
			ResultItem = resultItem;
			TotalFilesCount = fileCount;
			Stats = statistic;
		}
	}

	public delegate void ReplaceFileProcessedEventHandler(object sender, ReplacerEventArgs e);
	
	public class Replacer
	{
		public string Dir { get; set; }
		public string FileMask { get; set; }
		public bool IncludeSubDirectories { get; set; }
		public string FindText { get; set; }
		public string ReplaceText { get; set; }
		public bool IsCaseSensitive { get; set; }
		
		public class ReplaceResultItem 
		{
			public string FileName { get; set; }
			public string FilePath { get; set; }
			public string FileRelativePath { get; set; }
			public int NumMatches { get; set; }
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
			public MatchCollection Matches { get; set; }
			public bool IsSuccessOpen { get; set; }
			public bool IsSuccessWrite { get; set; }
		}

		public class ReplaceResult
		{
			public List<ReplaceResultItem> ResultItems { get; set; }

			public Statistic Stats { get; set; }
		}

		public ReplaceResult Replace()
		{
			Verify.Argument.IsNotEmpty(Dir, "Dir");
			Verify.Argument.IsNotEmpty(FileMask, "FileMask");
			Verify.Argument.IsNotEmpty(FindText, "FindText");
			Verify.Argument.IsNotEmpty(FindText, "ReplaceText");

			string[] filesInDirectory = Utils.GetFilesInDirectory(Dir, FileMask, IncludeSubDirectories);

			var resultItems = new List<ReplaceResultItem>();
			var stats = new Statistic();

			foreach (string filePath in filesInDirectory)
			{
				stats.TotalFilesCount++;
				
				var resultItem = ReplaceTextInFile(filePath);
				if (!resultItem.IsSuccessOpen) stats.FailedToOpen++;
					else stats.TotalMathes += resultItem.NumMatches;
				
				if (!resultItem.IsSuccessWrite) stats.FailedToWrite++;
				else stats.TotalReplaces += resultItem.NumMatches;

				if (resultItem.NumMatches>0) stats.FilesWithMathesCount++;

				//Skip files that don't have matches
				if (resultItem.NumMatches > 0)
					resultItems.Add(resultItem);

				OnFileProcessed(new ReplacerEventArgs(resultItem, filesInDirectory.Length, stats));
			}

			if (filesInDirectory.Length == 0) OnFileProcessed(new ReplacerEventArgs(new ReplaceResultItem(), filesInDirectory.Length, stats));

			return new ReplaceResult(){ResultItems = resultItems, Stats = stats};
		}
		
		private ReplaceResultItem ReplaceTextInFile(string filePath)
		{
			string content = string.Empty;

			var resultItem = new ReplaceResultItem() {IsSuccessOpen = true, IsSuccessWrite = true};
			
			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					content = sr.ReadToEnd();
				}
			}
			catch(Exception exception)
			{
				resultItem.IsSuccessOpen = false;
				resultItem.ErrorMessage = exception.Message;
			}


			if (resultItem.IsSuccessOpen)
			{
				RegexOptions regexOptions = Utils.GetRegExOptions(IsCaseSensitive);

				var matches = Regex.Matches(content, Regex.Escape(FindText), regexOptions);

				resultItem.FileName = Path.GetFileName(filePath);
				resultItem.FilePath = filePath;
				resultItem.FileRelativePath = "." + filePath.Substring(Dir.Length);

				resultItem.NumMatches = matches.Count;
				resultItem.Matches = matches;
				resultItem.IsSuccess = matches.Count > 0;

				if (matches.Count > 0)
				{
					try
					{
						string newContent = Regex.Replace(content, Regex.Escape(FindText), ReplaceText, regexOptions);

						using (var sw = new StreamWriter(filePath))
						{
							sw.Write(newContent);
						}
					}
					catch (Exception ex)
					{
						resultItem.IsSuccess = false;
						resultItem.IsSuccessWrite = false;
						resultItem.ErrorMessage = ex.Message;
					}
				}
			}

			return resultItem;
		}

		public event ReplaceFileProcessedEventHandler FileProcessed;

		protected virtual void OnFileProcessed(ReplacerEventArgs e)
		{
			if (FileProcessed != null)
				FileProcessed(this, e);
		}
	}
}
