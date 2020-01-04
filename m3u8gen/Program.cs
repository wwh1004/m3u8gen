using System;
using System.Cli;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace m3u8gen {
	internal sealed class Settings {
		private string _directory;

		[Argument("-d", IsRequired = true)]
		public string Directory {
			get => _directory;
			set {
				if (!System.IO.Directory.Exists(value))
					throw new DirectoryNotFoundException();

				_directory = Path.GetFullPath(value);
			}
		}
	}

	internal static class Program {
		private static void Main(string[] args) {
			Settings settings;

			settings = CommandLine.Parse<Settings>(args);
			WritePlaylsit(settings.Directory, "あやぽんず", "あやぽんず＊");
			WritePlaylsit(settings.Directory, "松下", "松下");
			WritePlaylsit(settings.Directory, "HIMEHINA", "HIMEHINA");
		}

		private static void WritePlaylsit(string directory, string keyword, string playlistName) {
			string playlist;

			playlist = string.Join(Environment.NewLine, EnumerateFiles(directory, keyword).Select(ChangeLetter));
			File.WriteAllText(playlistName + ".m3u8", playlist);
		}

		private static string ChangeLetter(string s) {
			StringBuilder sb;

			sb = new StringBuilder(s);
			sb[0] = 'A';
			return sb.ToString();
		}

		private static IEnumerable<string> EnumerateFiles(string directory, string keyword) {
			foreach (string directoryPath in Directory.EnumerateDirectories(directory, keyword, SearchOption.TopDirectoryOnly))
				foreach (string filePath in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
					yield return filePath;
			foreach (string filePath in Directory.EnumerateFiles(directory, keyword, SearchOption.TopDirectoryOnly))
				yield return filePath;
		}
	}
}
