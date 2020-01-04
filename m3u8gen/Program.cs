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
		private static readonly string[] _audioExtensions = new[] { ".aac", ".ape", ".flac", ".m4a", ".mp3", ".ogg", ".wav", ".wma" };

		private static void Main(string[] args) {
			Settings settings;

			settings = CommandLine.Parse<Settings>(args);
			WritePlaylsit(settings.Directory, "*あやぽんず*", "あやぽんず＊.local", false);
			WritePlaylsit(settings.Directory, "*あやぽんず*", "あやぽんず＊", true);
			WritePlaylsit(settings.Directory, "*松下*", "松下.local", false);
			WritePlaylsit(settings.Directory, "*松下*", "松下", true);
			WritePlaylsit(settings.Directory, "*HIMEHINA*", "HIMEHINA.local", false);
			WritePlaylsit(settings.Directory, "*HIMEHINA*", "HIMEHINA", true);
			WritePlaylsit(settings.Directory, new string[] { "*あやぽんず*", "*松下*", "*HIMEHINA*" }, "Mixed1.local", false);
			WritePlaylsit(settings.Directory, new string[] { "*あやぽんず*", "*松下*", "*HIMEHINA*" }, "Mixed1", true);
		}

		private static void WritePlaylsit(string directory, string keyword, string playlistName, bool changeLetter) {
			string playlist;

			playlist = string.Join(Environment.NewLine, EnumerateAudioFiles(directory, keyword).Select(s => changeLetter ? ChangeLetter(s) : s));
			File.WriteAllText(playlistName + ".m3u8", playlist);
		}

		private static void WritePlaylsit(string directory, string[] keywords, string playlistName, bool changeLetter) {
			string playlist;

			playlist = string.Join(Environment.NewLine, keywords.SelectMany(s => EnumerateAudioFiles(directory, s)).Select(s => changeLetter ? ChangeLetter(s) : s));
			File.WriteAllText(playlistName + ".m3u8", playlist);
		}

		private static string ChangeLetter(string s) {
			StringBuilder sb;

			sb = new StringBuilder(s);
			sb[0] = 'A';
			return sb.ToString();
		}

		private static IEnumerable<string> EnumerateAudioFiles(string directory, string keyword) {
			foreach (string filePath in Directory.EnumerateFiles(directory, keyword, SearchOption.TopDirectoryOnly).Where(IsAudioFile))
				yield return filePath;
			foreach (string directoryPath in Directory.EnumerateDirectories(directory, keyword, SearchOption.TopDirectoryOnly))
				foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories).Where(IsAudioFile))
					yield return filePath;
		}

		private static bool IsAudioFile(string filePath) {
			string extension;

			extension = Path.GetExtension(filePath);
			return _audioExtensions.Any(s => extension.Equals(s, StringComparison.OrdinalIgnoreCase));
		}
	}
}
