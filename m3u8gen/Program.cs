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
			WritePlaylsit(settings.Directory, "あやぽんず＊.local", false, s => s.Contains("あやぽんず"));
			WritePlaylsit(settings.Directory, "あやぽんず＊", true, s => s.Contains("あやぽんず"));
			WritePlaylsit(settings.Directory, "松下.local", false, s => s.Contains("松下"));
			WritePlaylsit(settings.Directory, "松下", true, s => s.Contains("松下"));
			WritePlaylsit(settings.Directory, "HIMEHINA.local", false, s => s.Contains("HIMEHINA"));
			WritePlaylsit(settings.Directory, "HIMEHINA", true, s => s.Contains("HIMEHINA"));
			WritePlaylsit(settings.Directory, "Mixed1.local", false, s => s.Contains("あやぽんず") || s.Contains("松下") || s.Contains("HIMEHINA"));
			WritePlaylsit(settings.Directory, "Mixed1", true, s => s.Contains("あやぽんず") || s.Contains("松下") || s.Contains("HIMEHINA"));
			WritePlaylsit(settings.Directory, "中文.local", false, IsChineseTrack);
			WritePlaylsit(settings.Directory, "中文", true, IsChineseTrack);
			WritePlaylsit(settings.Directory, "English.local", false, IsEnglishTrack);
			WritePlaylsit(settings.Directory, "English", true, IsEnglishTrack);

			bool IsChineseTrack(string _filePath) {
				string _part;
				bool _hasChineseChar;

				_part = _filePath.Substring(settings.Directory.Length + 1);
				if (_part.IndexOf('\\') != -1)
					return false;
				_hasChineseChar = false;
				foreach (char c in _part) {
					if (IsLatinChar(c))
						continue;
					if (IsChineseChar(c)) {
						_hasChineseChar = true;
						continue;
					}
					return false;
				}
				return _hasChineseChar;
			}

			bool IsEnglishTrack(string _filePath) {
				string _part;

				_part = _filePath.Substring(settings.Directory.Length + 1);
				if (_part.IndexOf('\\') != -1)
					return false;
				foreach (char c in _part)
					if (!IsLatinChar(c))
						return false;
				return true;
			}

			bool IsLatinChar(char c) {
				return c >= 0x0000 && c <= 0x00FF;
			}

			bool IsChineseChar(char c) {
				// https://www.qqxiuzi.cn/zh/hanzi-unicode-bianma.php
				if (c >= 0x4E00 && c <= 0x9FA5)
					return true;
				if (c >= 0x9FA6 && c <= 0x9FEF)
					return true;
				return false;
			}
		}

		private static void WritePlaylsit(string directory, string playlistName, bool changeLetter, Func<string, bool> selector) {
			string playlist;

			playlist = string.Join(Environment.NewLine, EnumerateAudioFiles(directory, selector).Select(s => changeLetter ? ChangeLetter(s) : s));
			File.WriteAllText(playlistName + ".m3u8", playlist);
		}

		private static string ChangeLetter(string s) {
			StringBuilder sb;

			sb = new StringBuilder(s);
			sb[0] = 'A';
			return sb.ToString();
		}

		private static IEnumerable<string> EnumerateAudioFiles(string directory, Func<string, bool> selector) {
			foreach (string filePath in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
				if (IsAudioFile(filePath) && selector(filePath))
					yield return filePath;
		}

		private static bool IsAudioFile(string filePath) {
			string extension;

			extension = Path.GetExtension(filePath);
			foreach (string audioExtension in _audioExtensions)
				if (string.Equals(extension, audioExtension, StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}
	}
}
