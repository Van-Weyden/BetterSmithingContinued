using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Utilities;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame
{
	public class SmeltingItemRosterWrapper : IDisposable
	{
		public SmeltingItemVM CurrentlySelectedItem
		{
			get
			{
				return this.m_CurrentlySelectedItem;
			}
			set
			{
				if (this.m_CurrentlySelectedItem != value)
				{
					if (this.m_CurrentlySelectedItem != null)
					{
						this.m_CurrentlySelectedItem.IsSelected = false;
						this.m_CurrentlySelectedItem.RefreshValues();
					}
					this.m_CurrentlySelectedItem = value;
					if (this.m_CurrentlySelectedItem != null)
					{
						this.m_SmeltingVM.SmartSelectItem(this.m_CurrentlySelectedItem);
						return;
					}
					this.m_SmeltingVM.CurrentSelectedItem = null;
				}
			}
		}

		public MBBindingList<SmeltingItemVM> DisplayedSmeltableItemList { get; }

		public SmeltingItemRosterWrapper(SmeltingVM _smeltingVM)
		{
			this.m_PartyItemRoster = MobileParty.MainParty.ItemRoster;
			this.m_SmeltingItemVMList = new Dictionary<string, SmeltingItemRosterWrapper.ViewableSmeltingItem>();
			this.DisplayedSmeltableItemList = new MBBindingList<SmeltingItemVM>();
			this.m_StoredCurrentItemIndex = -1;
			this.m_SmeltingVM = _smeltingVM;
			if (this.m_SmeltingVM != null)
			{
				this.m_ProcessLockItemMethodInfo = MemberExtractor.GetPrivateMethodInfo<SmeltingVM>("ProcessLockItem");
				this.m_IsItemLockedMethodInfo = MemberExtractor.GetPrivateMethodInfo<SmeltingVM>("IsItemLocked");
			}
			this.m_CurrentFilter = ((EquipmentElement _element) => true);
			this.m_SmeltingVM.SmeltableItemList = this.DisplayedSmeltableItemList;
			this.PerformFullRefresh(null);
		}

		public void Dispose()
		{
			this.m_SmeltingItemVMList.Clear();
			this.DisplayedSmeltableItemList.Clear();
		}

		public void StoreCurrentSelectedItemIndex()
		{
			if (this.CurrentlySelectedItem != null)
			{
				this.m_StoredCurrentItemIndex = this.DisplayedSmeltableItemList.IndexOf(this.CurrentlySelectedItem);
				this.CurrentlySelectedItem = null;
			}
		}

		public void RestoreCurrentSelectedItemIndex()
		{
			if (this.m_StoredCurrentItemIndex >= 0)
			{
				this.SetNextSelectedItem(this.m_StoredCurrentItemIndex);
				this.m_StoredCurrentItemIndex = -1;
			}
		}

		public EquipmentElement GetItemAtIndex(int _index, out int _itemQuantity)
		{
			if (_index < 0 || _index >= this.DisplayedSmeltableItemList.Count)
			{
				_itemQuantity = -1;
				return EquipmentElement.Invalid;
			}
			SmeltingItemVM smeltingItemVM = this.DisplayedSmeltableItemList[_index];
			if (smeltingItemVM == null)
			{
				Messaging.DisplayMessage("SmeltingItemVM at given index is null.");
			}
			else if (smeltingItemVM.EquipmentElement.Item == null)
			{
				Messaging.DisplayMessage("EquipmentElement at given index is Invalid.");
			}
			_itemQuantity = ((smeltingItemVM != null) ? smeltingItemVM.NumOfItems : 0);
			if (smeltingItemVM == null)
			{
				return EquipmentElement.Invalid;
			}
			return smeltingItemVM.EquipmentElement;
		}

		public int GetItemIndex(EquipmentElement equipmentElement, out int _itemQuantity)
		{
			if (equipmentElement.Item == null)
			{
				Messaging.DisplayMessage("Received a null EquipmentElement in GetItemIndex function.");
			}
			int itemQuantity = 0;
			int result = this.DisplayedSmeltableItemList.FindIndex(delegate(SmeltingItemVM x)
			{
				if (this.GetStringID(x.EquipmentElement) == this.GetStringID(equipmentElement))
				{
					itemQuantity = x.NumOfItems;
					return true;
				}
				return false;
			});
			_itemQuantity = itemQuantity;
			return result;
		}

		public bool IsItemLocked(EquipmentElement _equipmentElement)
		{
			MethodInfo isItemLockedMethodInfo = this.m_IsItemLockedMethodInfo;
			return (bool) isItemLockedMethodInfo?.Invoke(this.m_SmeltingVM, new object[] {
				_equipmentElement
			});
		}

		public void ModifyItem(EquipmentElement _equipmentElement, int _count)
		{
			if (_count == 0)
			{
				return;
			}
			string stringID = this.GetStringID(_equipmentElement);
			SmeltingItemRosterWrapper.ViewableSmeltingItem viewableSmeltingItem;
			if (this.m_SmeltingItemVMList.TryGetValue(stringID, out viewableSmeltingItem))
			{
				if (_count < 0 && Math.Abs(_count) >= viewableSmeltingItem.SmeltingItemVM.NumOfItems)
				{
					int nextSelectedItem = this.DisplayedSmeltableItemList.IndexOf(viewableSmeltingItem.SmeltingItemVM);
					this.m_SmeltingItemVMList.Remove(stringID);
					if (viewableSmeltingItem.IsVisible)
					{
						this.DisplayedSmeltableItemList.Remove(viewableSmeltingItem.SmeltingItemVM);
						this.m_SmeltingVM.RefreshValues();
					}
					if (viewableSmeltingItem.SmeltingItemVM.Equals(this.CurrentlySelectedItem) && this.m_StoredCurrentItemIndex < 0)
					{
						this.SetNextSelectedItem(nextSelectedItem);
						return;
					}
				}
				else
				{
					viewableSmeltingItem.SmeltingItemVM.NumOfItems += _count;
					if (viewableSmeltingItem.IsVisible)
					{
						viewableSmeltingItem.SmeltingItemVM.RefreshValues();
					}
				}
				return;
			}
			if (_count < 0)
			{
				this.PerformFullRefresh(new TextObject("{=BSC_Msg_INF}Could not find item {ITEM_NAME} in smelting list. Performing full refresh.", null).SetTextVariable("ITEM_NAME", _equipmentElement.Item.Name).ToString());
				this.CurrentlySelectedItem = null;
				return;
			}
			Func<EquipmentElement, bool> currentFilter = this.m_CurrentFilter;
			bool flag = currentFilter == null || currentFilter(_equipmentElement);
			SmeltingItemVM smeltingItemVM = SmeltingItemVMUtilities.CreateSmeltingItemVM(_equipmentElement, new Action<SmeltingItemVM>(this.SetCurrentItem), _count, new Action<SmeltingItemVM, bool>(this.ProcessLockItem), this.IsItemLocked(_equipmentElement));
			this.m_SmeltingItemVMList.Add(stringID, new SmeltingItemRosterWrapper.ViewableSmeltingItem(smeltingItemVM, flag));
			if (flag)
			{
				this.DisplayedSmeltableItemList.Add(smeltingItemVM);
				this.m_SmeltingVM.RefreshList();
			}
		}

		public void UpdateFilter(Func<EquipmentElement, bool> _itemIsVisible)
		{
			this.m_CurrentFilter = _itemIsVisible;
			foreach (KeyValuePair<string, SmeltingItemRosterWrapper.ViewableSmeltingItem> keyValuePair in this.m_SmeltingItemVMList)
			{
				bool flag = this.m_CurrentFilter(keyValuePair.Value.SmeltingItemVM.EquipmentElement);
				if (flag != keyValuePair.Value.IsVisible)
				{
					if (flag)
					{
						this.DisplayedSmeltableItemList.Add(keyValuePair.Value.SmeltingItemVM);
					}
					else
					{
						this.DisplayedSmeltableItemList.Remove(keyValuePair.Value.SmeltingItemVM);
					}
					keyValuePair.Value.IsVisible = flag;
				}
			}
			this.m_SmeltingVM.RefreshValues();
		}

		private void SetNextSelectedItem(int _index)
		{
			int count = this.DisplayedSmeltableItemList.Count;
			if (count <= 0)
			{
				this.SetCurrentItem(null);
				return;
			}
			if (count <= _index)
			{
				_index = count - 1;
			}
			SmeltingItemVM currentItem = this.DisplayedSmeltableItemList[_index];
			this.SetCurrentItem(currentItem);
		}

		private void SetCurrentItem(SmeltingItemVM _newItem)
		{
			this.CurrentlySelectedItem = _newItem;
		}

		private void ProcessLockItem(SmeltingItemVM _item, bool _isLocked)
		{
			MethodInfo processLockItemMethodInfo = this.m_ProcessLockItemMethodInfo;
			if (processLockItemMethodInfo != null)
			{
				processLockItemMethodInfo.Invoke(this.m_SmeltingVM, new object[]
				{
					_item,
					_isLocked
				});
			}
			bool flag = this.m_CurrentFilter(_item.EquipmentElement);
			if (!flag)
			{
				int num = this.DisplayedSmeltableItemList.IndexOf(_item);
				if (num >= 0)
				{
					SmeltingItemRosterWrapper.ViewableSmeltingItem itemWrapper = this.GetItemWrapper(_item);
					this.DisplayedSmeltableItemList.RemoveAt(num);
					if (_item.Equals(this.CurrentlySelectedItem))
					{
						this.SetNextSelectedItem(num);
					}
					if (itemWrapper != null)
					{
						itemWrapper.IsVisible = flag;
					}
					this.m_SmeltingVM.RefreshValues();
				}
			}
		}

		private SmeltingItemRosterWrapper.ViewableSmeltingItem GetItemWrapper(SmeltingItemVM item)
		{
			foreach (SmeltingItemRosterWrapper.ViewableSmeltingItem viewableSmeltingItem in this.m_SmeltingItemVMList.Values)
			{
				if (viewableSmeltingItem.SmeltingItemVM == item)
				{
					return viewableSmeltingItem;
				}
			}
			return null;
		}

		private void PerformFullRefresh(string _message = null)
		{
			this.m_SmeltingItemVMList.Clear();
			this.DisplayedSmeltableItemList.Clear();

			IEnumerable<ItemRosterElement> craftedItemList = this.GetCraftedItemList();
			foreach (ItemRosterElement itemRosterElement in craftedItemList)
			{
				SetItemDisplay(itemRosterElement);
			}

			if (!string.IsNullOrEmpty(_message))
			{
				Messaging.DisplayMessage(_message);
			}
		}

		private IEnumerable<ItemRosterElement> GetCraftedItemList()
		{
			return this.m_PartyItemRoster.Where(delegate (ItemRosterElement x) {
				ItemRosterElement itemRosterElement2 = x;
				return itemRosterElement2.EquipmentElement.Item.IsCraftedWeapon;
			});
		}

		private void SetItemDisplay(ItemRosterElement itemRosterElement)
		{
			SmeltingItemVM smeltingItemVM = SmeltingItemVMUtilities.CreateSmeltingItemVM(itemRosterElement.EquipmentElement, new Action<SmeltingItemVM>(this.SetCurrentItem), itemRosterElement.Amount, new Action<SmeltingItemVM, bool>(this.ProcessLockItem), this.IsItemLocked(itemRosterElement.EquipmentElement));
			bool isNeedDisplayItem = this.IsNeedDisplayItem(itemRosterElement.EquipmentElement);
			this.m_SmeltingItemVMList.Add(this.GetStringID(itemRosterElement.EquipmentElement), new SmeltingItemRosterWrapper.ViewableSmeltingItem(smeltingItemVM, isNeedDisplayItem));
			if (isNeedDisplayItem)
			{
				this.DisplayedSmeltableItemList.Add(smeltingItemVM);
			}
		}

		private bool IsNeedDisplayItem(EquipmentElement _equipmentElement)
		{
			return this.m_CurrentFilter(_equipmentElement);
		}

		private string GetStringID(EquipmentElement _equipmentElement)
		{
			string text = _equipmentElement.Item.StringId;
			ItemModifier itemModifier = _equipmentElement.ItemModifier;
			if (((itemModifier != null) ? itemModifier.StringId : null) != null)
			{
				text += _equipmentElement.ItemModifier.StringId;
			}
			return text;
		}

		private ItemRoster m_PartyItemRoster;

		private Dictionary<string, SmeltingItemRosterWrapper.ViewableSmeltingItem> m_SmeltingItemVMList;

		private SmeltingVM m_SmeltingVM;

		private MethodInfo m_ProcessLockItemMethodInfo;

		private MethodInfo m_IsItemLockedMethodInfo;

		private Func<EquipmentElement, bool> m_CurrentFilter;

		private SmeltingItemVM m_CurrentlySelectedItem;

		private int m_StoredCurrentItemIndex;

		private class ViewableSmeltingItem
		{
			public SmeltingItemVM SmeltingItemVM { get; }

			public bool IsVisible { get; set; }

			public ViewableSmeltingItem(SmeltingItemVM _smeltingItemVm, bool _isVisible)
			{
				this.SmeltingItemVM = _smeltingItemVm;
				this.IsVisible = _isVisible;
			}
		}
	}
}
