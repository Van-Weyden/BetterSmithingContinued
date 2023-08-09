using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Annotations;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BetterSmithingContinued.Utilities
{
	public static class ItemRosterExtensions
	{
		public static void CopyAllElementsTo(this ItemRoster _origin, ItemRoster _destination, bool _deepCopy = true)
		{
			foreach (ItemRosterElement itemRosterElement in _origin)
			{
				if (_deepCopy)
				{
					_destination.Add(new ItemRosterElement(itemRosterElement.EquipmentElement, itemRosterElement.Amount));
				}
				else
				{
					_destination.SafeAddToCounts(itemRosterElement.EquipmentElement, itemRosterElement.Amount);
				}
			}
		}

		public static ItemRosterElement[] GetData(this ItemRoster _itemRoster)
		{
			FieldInfo value = ItemRosterExtensions.m_LazyDataFieldInfo.Value;
			return (ItemRosterElement[])((value != null) ? value.GetValue(_itemRoster) : null);
		}

		public static void SetCount(this ItemRoster _itemRoster, int _newCount)
		{
			ItemRosterExtensions.m_LazyDataFieldInfo.Value.SetValue(_itemRoster, _newCount);
		}

		public static int GetCount(this ItemRoster _itemRoster)
		{
			return (int)ItemRosterExtensions.m_LazyDataFieldInfo.Value.GetValue(_itemRoster);
		}

		public static int AddNewElement(this ItemRoster _itemRoster, ItemRosterElement rosterElement, bool insertAtFront = false)
		{
			return (int)ItemRosterExtensions.m_LazyAddNewElementMethodInfo.Value.Invoke(_itemRoster, new object[]
			{
				rosterElement,
				insertAtFront
			});
		}

		public static void OnRosterUpdated(this ItemRoster _itemRoster, ref ItemRosterElement _itemRosterElement, int count)
		{
			object[] array = new object[]
			{
				_itemRosterElement,
				count
			};
			MethodInfo value = ItemRosterExtensions.m_LazyOnRosterUpdatedMethodInfo.Value;
			if (value != null)
			{
				value.Invoke(_itemRoster, array);
			}
			_itemRosterElement = (ItemRosterElement)array[0];
		}

		public static void CompressIdenticalCraftedWeapons(this ItemRoster _itemRoster)
		{
			if (_itemRoster.Count <= 0)
			{
				return;
			}
			try
			{
				List<ItemRosterElement> list = _itemRoster.ToArray<ItemRosterElement>().Where(delegate(ItemRosterElement item)
				{
					ItemRosterElement itemRosterElement3 = item;
					if (itemRosterElement3.EquipmentElement.Item != null)
					{
						itemRosterElement3 = item;
						return itemRosterElement3.EquipmentElement.Item.IsCraftedByPlayer;
					}
					return false;
				}).ToList<ItemRosterElement>();
				for (int i = 0; i < list.Count; i++)
				{
					ItemRosterElement itemRosterElement = list[i];
					for (int j = i + 1; j < list.Count; j++)
					{
						ItemRosterElement itemRosterElement2 = list[j];
						if (itemRosterElement.EquipmentElement.Item.CompareTo(itemRosterElement2.EquipmentElement.Item))
						{
							_itemRoster.SafeAddToCounts(itemRosterElement2.EquipmentElement.Item, -itemRosterElement2.Amount);
							_itemRoster.SafeAddToCounts(itemRosterElement.EquipmentElement.Item, itemRosterElement2.Amount);
							list.RemoveAt(j);
							j--;
						}
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_07}Issue occurred while grouping items. Exception: ", null).ToString() + ex.ToString()));
			}
		}

		public static ItemObject CompressIdenticalCraftedWeapons(this ItemRoster _itemRoster, ItemObject _weapon)
		{
			if (_itemRoster.Count <= 0)
			{
				return _weapon;
			}
			try
			{
				foreach (ItemRosterElement itemRosterElement in _itemRoster)
				{
					ItemObject item = itemRosterElement.EquipmentElement.Item;
					if (item.IsCraftedByPlayer && !item.Equals(_weapon) && item.CompareTo(_weapon))
					{
						PartyBase.MainParty.ItemRoster.SafeAddToCounts(_weapon, -1);
						MBObjectManager.Instance.UnregisterObject(_weapon);
						PartyBase.MainParty.ItemRoster.SafeAddToCounts(item, 1);
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_07}Issue occurred while grouping items. Exception: ", null).ToString() + ex.ToString()));
			}
			return _weapon;
		}

		public static int SafeAddToCounts(this ItemRoster _roster, ItemObject _itemObject, int _amount)
		{
			object obj = ItemRosterExtensions.m_LazyAddToCountsItemObjectMethodInfo.Value(_roster, _itemObject, _amount);
			if (obj == null)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_08}Could not find ItemRoster.AddToCounts method. Item grouping will not function.", null).ToString()));
				return -1;
			}
			return (int)obj;
		}

		public static int SafeAddToCounts(this ItemRoster _roster, EquipmentElement _rosterElement, int _amount)
		{
			object obj = ItemRosterExtensions.m_LazyAddToCountsEquipmentElementMethodInfo.Value(_roster, _rosterElement, _amount);
			if (obj == null)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_08}Could not find ItemRoster.AddToCounts method. Item grouping will not function.", null).ToString()));
				return -1;
			}
			return (int)obj;
		}

		[CanBeNull]
		private static MethodInfo GetAddToCountsMethodWithParams(Type[] _params)
		{
			return typeof(ItemRoster).GetMethod("AddToCounts", MemberExtractor.PublicMemberFlags, null, CallingConventions.Any, _params, new ParameterModifier[0]);
		}

		private static Lazy<MethodInfo> m_LazyOnRosterUpdatedMethodInfo = new Lazy<MethodInfo>(() => MemberExtractor.GetPrivateMethodInfo<ItemRoster>("OnRosterUpdated"));
		private static Lazy<MethodInfo> m_LazyAddNewElementMethodInfo = new Lazy<MethodInfo>(() => MemberExtractor.GetPrivateMethodInfo<ItemRoster>("AddNewElement"));
		private static Lazy<FieldInfo> m_LazyCountFieldInfo = new Lazy<FieldInfo>(() => MemberExtractor.GetPrivateFieldInfo<ItemRoster>("_count"));
		private static Lazy<FieldInfo> m_LazyDataFieldInfo = new Lazy<FieldInfo>(() => MemberExtractor.GetPrivateFieldInfo<ItemRoster>("_data"));

		private static readonly Lazy<Func<ItemRoster, ItemObject, int, object>> m_LazyAddToCountsItemObjectMethodInfo = new Lazy<Func<ItemRoster, ItemObject, int, object>>(delegate()
		{
			MethodInfo addToCountsMethodWithParams = ItemRosterExtensions.GetAddToCountsMethodWithParams(new Type[]
			{
				typeof(ItemObject),
				typeof(int)
			});
			if (addToCountsMethodWithParams == null)
			{
				return (ItemRoster instance, ItemObject itemObject, int amount) => null;
			}
			return (ItemRoster instance, ItemObject itemObject, int amount) => (int)addToCountsMethodWithParams.Invoke(instance, new object[]
			{
				itemObject,
				amount
			});
		});

		private static readonly Lazy<Func<ItemRoster, EquipmentElement, int, object>> m_LazyAddToCountsEquipmentElementMethodInfo = new Lazy<Func<ItemRoster, EquipmentElement, int, object>>(delegate()
		{
			MethodInfo addToCountsMethodWithParams = ItemRosterExtensions.GetAddToCountsMethodWithParams(new Type[]
			{
				typeof(EquipmentElement),
				typeof(int)
			});
			if (addToCountsMethodWithParams == null)
			{
				return (ItemRoster instance, EquipmentElement itemObject, int amount) => null;
			}
			return (ItemRoster instance, EquipmentElement itemObject, int amount) => (int)addToCountsMethodWithParams.Invoke(instance, new object[]
			{
				itemObject,
				amount
			});
		});
	}
}
