using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BetterSmithingContinued.Settings;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("GlobalWeaponSaveData")]
	public class WeaponSaveData : SettingsSection<WeaponSaveData>
	{
		public event EventHandler WeaponsListUpdated;

		[XmlArray("SavedWeapons")]
		[XmlArrayItem("Weapon", typeof(WeaponData))]
		[SettingDefaultValue("GetDefaultSavedWeapons", true)]
		public List<WeaponData> Weapons
		{
			get
			{
				return this.m_Weapons;
			}
			set
			{
				if (this.m_Weapons != value)
				{
					this.m_Weapons = value;
					this.OnPropertyChanged("Weapons");
				}
			}
		}

		public override string FileName
		{
			get
			{
				return "GlobalWeaponSaveData";
			}
		}

		public void SaveWeapon(string weaponName, Crafting _craftingInstance)
		{
			WeaponData weaponData = WeaponData.GetWeaponData(
				weaponName,
				_craftingInstance.CurrentCraftingTemplate,
				_craftingInstance.SelectedPieces
			);
			this.Weapons.Add(weaponData);
			base.NeedsSave = true;
			this.OnWeaponsListUpdated();
		}

		public void DeleteWeapon(WeaponData _weaponToDelete, bool _pushUpdate = true)
		{
			if (_weaponToDelete != null)
			{
				this.Weapons.Remove(_weaponToDelete);
				MBObjectManager.Instance.UnregisterObject(_weaponToDelete.ItemObject);
				base.NeedsSave = true;
				if (_pushUpdate)
				{
					this.OnWeaponsListUpdated();
				}
			}
		}

		public void EditWeapon(string weaponName, Crafting _craftingInstance, WeaponData _weaponToEdit)
		{
			this.DeleteWeapon(_weaponToEdit, false);
			this.SaveWeapon(weaponName, _craftingInstance);
			base.NeedsSave = true;
		}

		protected virtual void OnWeaponsListUpdated()
		{
			EventHandler weaponsListUpdated = this.WeaponsListUpdated;
			if (weaponsListUpdated == null)
			{
				return;
			}
			weaponsListUpdated(this, EventArgs.Empty);
		}

		private List<WeaponData> GetDefaultSavedWeapons()
		{
			return new List<WeaponData>();
		}

		private List<WeaponData> m_Weapons;
	}
}
