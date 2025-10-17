using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class SmeltingItemVMUtilities
	{
		public static SmeltingItemVM CreateSmeltingItemVM(EquipmentElement _equipmentElement, Action<SmeltingItemVM> _onSelection, int _numOfItems, Action<SmeltingItemVM, bool> _onItemLockedStateChanged = null, bool _isLocked = false)
		{
			if (_onItemLockedStateChanged == null)
			{
				_onItemLockedStateChanged = delegate(SmeltingItemVM item, bool state)
				{
				};
			}
			return SmeltingItemVMUtilities.m_SmeltingItemVMActivator.Value(_equipmentElement, _onSelection, _onItemLockedStateChanged, _isLocked, _numOfItems);
		}

		private static Lazy<Func<EquipmentElement, Action<SmeltingItemVM>, Action<SmeltingItemVM, bool>, bool, int, SmeltingItemVM>> m_SmeltingItemVMActivator = new Lazy<Func<EquipmentElement, Action<SmeltingItemVM>, Action<SmeltingItemVM, bool>, bool, int, SmeltingItemVM>>(delegate()
		{
			Activator<SmeltingItemVM> activator = typeof(SmeltingItemVM).GetActivator<SmeltingItemVM>(new Type[]
			{
				typeof(EquipmentElement),
				typeof(Action<SmeltingItemVM>),
				typeof(Action<SmeltingItemVM, bool>),
				typeof(bool),
				typeof(int)
			});
			return (EquipmentElement _equipmentElement, Action<SmeltingItemVM> _onSelection, Action<SmeltingItemVM, bool> _onItemLockedStateChanged, bool _isLocked, int _numOfItems) => activator(new object[]
			{
				_equipmentElement,
				_onSelection,
				_onItemLockedStateChanged,
				_isLocked,
				_numOfItems
			});
		});
	}
}
