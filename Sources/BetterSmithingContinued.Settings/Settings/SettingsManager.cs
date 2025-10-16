using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using HarmonyLib;

namespace BetterSmithingContinued.Settings
{
	public class SettingsManager : BetterSmithingContinued.Core.Modules.Module, ISettingsManager
	{
		private Dictionary<Type, SettingsSection> m_CompressedSettings
		{
			get
			{
				return this.m_Settings.SelectMany((KeyValuePair<string, Dictionary<Type, SettingsSection>> x) => x.Value).ToDictionary((KeyValuePair<Type, SettingsSection> x) => x.Key, (KeyValuePair<Type, SettingsSection> x) => x.Value);
			}
		}

		public event EventHandler<SettingsSection> SettingsSectionChanged;

		public void RestoreDefaults()
		{
			foreach (KeyValuePair<Type, SettingsSection> keyValuePair in this.m_Settings.SelectMany((KeyValuePair<string, Dictionary<Type, SettingsSection>> settingsFile) => settingsFile.Value))
			{
				keyValuePair.Value.RestoreDefaults();
			}
		}

		public T GetSettings<T>() where T : SettingsSection
		{
			SettingsSection settingsSection;
			if (!this.m_CompressedSettings.TryGetValue(typeof(T), out settingsSection))
			{
				settingsSection = (Activator.CreateInstance(typeof(T)) as T);
				Dictionary<Type, SettingsSection> dictionary;
				if (!this.m_Settings.TryGetValue(settingsSection.FileName, out dictionary))
				{
					dictionary = new Dictionary<Type, SettingsSection>();
					this.m_Settings.Add(settingsSection.FileName, dictionary);
				}
				dictionary.Add(typeof(T), settingsSection);
			}
			return settingsSection as T;
		}

		public void Save()
		{
			foreach (KeyValuePair<string, Dictionary<Type, SettingsSection>> keyValuePair in this.m_Settings)
			{
				if (keyValuePair.Value.Any((KeyValuePair<Type, SettingsSection> settings) => settings.Value.NeedsSave))
				{
					this.SaveFile(keyValuePair.Key);
				}
			}
		}

		private void SaveFile(string _fileName)
		{
			try
			{
				Directory.CreateDirectory(PathUtilities.GetFullBetterSmithingDocumentsFolderPath());
				XmlDocument xmlDocument = new XmlDocument();
				XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
				xmlDocument.InsertBefore(newChild, xmlDocument.DocumentElement);
				XmlElement xmlElement = xmlDocument.CreateElement("BetterSmithingContinuedConfig");
				xmlDocument.AppendChild(xmlElement);
				foreach (KeyValuePair<Type, SettingsSection> keyValuePair in this.m_Settings[_fileName])
				{
					this.SerializeObjectAndAppendAsChildOfNode<SettingsSection>(xmlElement, keyValuePair.Value);
					keyValuePair.Value.NeedsSave = false;
				}
				xmlDocument.Save(PathUtilities.GetFullBetterSmithingSettingsFilePath(_fileName));
			}
			catch (Exception ex)
			{
				FileLog.Log(ex.ToString());
				Trace.WriteLine(ex.ToString());
			}
		}

		public void LoadSettings()
		{
			foreach (string fileName in this.GetFileNames())
			{
				this.LoadFile(fileName);
			}
		}

		private IEnumerable<string> GetFileNames()
		{
			return this.m_SettingsSectionTypes.Select(delegate(Type x)
			{
				SettingsSection settingsSection = Activator.CreateInstance(x) as SettingsSection;
				if (settingsSection == null)
				{
					return null;
				}
				return settingsSection.FileName;
			});
		}

		private void LoadFile(string _fileName)
		{
			string fullBetterSmithingSettingsFilePath = PathUtilities.GetFullBetterSmithingSettingsFilePath(_fileName);
			if (!File.Exists(fullBetterSmithingSettingsFilePath))
			{
				return;
			}
			try
			{
				Dictionary<Type, SettingsSection> dictionary;
				if (!this.m_Settings.TryGetValue(_fileName, out dictionary))
				{
					dictionary = new Dictionary<Type, SettingsSection>();
					this.m_Settings.Add(_fileName, dictionary);
				}
				dictionary.Clear();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fullBetterSmithingSettingsFilePath);
				if (xmlDocument.DocumentElement != null)
				{
					foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes.OfType<XmlNode>())
					{
						Type settingsSectionTypeWithName = this.GetSettingsSectionTypeWithName(xmlNode.Name);
						if (settingsSectionTypeWithName != null)
						{
							using (XmlReader xmlReader = xmlNode.CreateNavigator().ReadSubtree())
							{
								SettingsSection settingsSection = new XmlSerializer(settingsSectionTypeWithName).Deserialize(xmlReader) as SettingsSection;
								if (settingsSection != null)
								{
									settingsSection.OnDeserialized();
								}
								dictionary.Add(settingsSectionTypeWithName, settingsSection);
								this.OnSettingsSectionChanged(settingsSection);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				FileLog.Log(ex.ToString());
				Trace.WriteLine(ex.ToString());
			}
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.Initialize();
			this.RegisterModule<ISettingsManager>("");
		}

		private void OnSettingsSectionChanged(SettingsSection _e)
		{
			EventHandler<SettingsSection> settingsSectionChanged = this.SettingsSectionChanged;
			if (settingsSectionChanged == null)
			{
				return;
			}
			settingsSectionChanged(this, _e);
		}

		private void Initialize()
		{
			this.m_Settings = new Dictionary<string, Dictionary<Type, SettingsSection>>();
			this.m_SettingsSectionTypes = (from _type in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly assembly) => assembly.GetTypes())
			where !_type.IsAbstract && !_type.IsInterface && typeof(SettingsSection).IsAssignableFrom(_type)
			select _type).ToArray<Type>();
			this.LoadSettings();
		}

		private Type GetSettingsSectionTypeWithName(string _name)
		{
			foreach (Type type in this.m_SettingsSectionTypes)
			{
				if (((XmlTypeAttribute)Attribute.GetCustomAttribute(type, typeof(XmlTypeAttribute))).TypeName == _name)
				{
					return type;
				}
			}
			throw new Exception("Settings section with name " + _name + " does not exist.");
		}

		private void SerializeObjectAndAppendAsChildOfNode<T>(XmlNode _node, T _object) where T : SettingsSection
		{
			XmlSerializer xmlSerializer = new XmlSerializer(_object.GetType());
			using (XmlWriter xmlWriter = _node.CreateNavigator().AppendChild())
			{
				xmlWriter.WriteWhitespace("");
				XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add(_node.GetNamespaceOfPrefix(_node.NamespaceURI), _node.NamespaceURI);
				xmlSerializer.Serialize(xmlWriter, _object, xmlSerializerNamespaces);
			}
		}

		private Type[] m_SettingsSectionTypes;

		private Dictionary<string, Dictionary<Type, SettingsSection>> m_Settings;
	}
}
