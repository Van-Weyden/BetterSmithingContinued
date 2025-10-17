using System;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Core
{
	public static class PathUtilities
	{
		public static string GetFullFilePath(PlatformFilePath filePath)
		{
			return Common.PlatformFileHelper.GetFileFullPath(filePath);
		}

		public static string GetFullDirectoryPath(PlatformDirectoryPath directoryPath)
		{
			return Common.PlatformFileHelper.GetFileFullPath(new PlatformFilePath(directoryPath, ""));
		}

		public static PlatformFilePath GetFilePath(PlatformDirectoryPath fileDirectoryPath, string fileName)
		{
			return new PlatformFilePath(fileDirectoryPath, fileName);
		}

		public static string GetFullFilePath(PlatformDirectoryPath fileDirectoryPath, string fileName)
		{
			return PathUtilities.GetFullFilePath(PathUtilities.GetFilePath(fileDirectoryPath, fileName));
		}

		public static PlatformDirectoryPath GetDocumentsFolderPath()
		{
			return new PlatformDirectoryPath(PlatformFileType.User, "");
		}

		public static PlatformDirectoryPath GetDocumentsConfigsFolderPath()
		{
			return EngineFilePaths.ConfigsPath;
		}

		public static string GetFullDocumentsFolderPath()
		{
			return PathUtilities.GetFullDirectoryPath(PathUtilities.GetDocumentsFolderPath());
		}

		public static string GetFullDocumentsConfigsFolderPath()
		{
			return PathUtilities.GetFullDirectoryPath(PathUtilities.GetDocumentsConfigsFolderPath());
		}

		public static string GetFullModulesPath()
		{
			return System.IO.Path.GetFullPath(System.IO.Path.Combine(Utilities.GetBasePath(), "Modules"));
		}

		public static string GetFullBetterSmithingModulePath()
		{
			return System.IO.Path.Combine(PathUtilities.GetFullModulesPath(), "BetterSmithingContinued");
		}

		public static string GetFullBetterSmithingGUIPath()
		{
			return System.IO.Path.Combine(PathUtilities.GetFullBetterSmithingModulePath(), "GUI");
		}

		public static string GetBetterSmithingGUIFilePath(string _fileName)
		{
			string[] array = Directory.GetFiles(PathUtilities.GetFullBetterSmithingGUIPath(), "*.xml", SearchOption.AllDirectories);
			array = (from x in array
			where string.Equals(System.IO.Path.GetFileNameWithoutExtension(x), _fileName, StringComparison.InvariantCultureIgnoreCase)
			select x).ToArray<string>();
			if (array.Length <= 1 && array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public static string GetBetterSmithingGUIFileContent(string _fileName)
		{
			string betterSmithingGUIFilePath = PathUtilities.GetBetterSmithingGUIFilePath(_fileName);
			XmlDocument xmlDocument = new XmlDocument();
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = true
			};
			XmlReader reader = XmlReader.Create(betterSmithingGUIFilePath, settings);
			xmlDocument.Load(reader);
			return xmlDocument.InnerXml;
		}

		public static PlatformDirectoryPath GetBetterSmithingDocumentsFolderPath()
		{
			return new PlatformDirectoryPath(PlatformFileType.User, "Configs\\ModSettings\\BetterSmithingContinued");
		}

		public static string GetFullBetterSmithingDocumentsFolderPath()
		{
			return PathUtilities.GetFullDirectoryPath(PathUtilities.GetBetterSmithingDocumentsFolderPath());
		}

		public static PlatformFilePath GetBetterSmithingSettingsFilePath(string fileNameWithoutExtension)
		{
			return PathUtilities.GetFilePath(PathUtilities.GetBetterSmithingDocumentsFolderPath(), fileNameWithoutExtension + ".xml");
		}

		public static string GetFullBetterSmithingSettingsFilePath(string fileNameWithoutExtension)
		{
			return PathUtilities.GetFullFilePath(PathUtilities.GetBetterSmithingSettingsFilePath(fileNameWithoutExtension));
		}
	}
}
