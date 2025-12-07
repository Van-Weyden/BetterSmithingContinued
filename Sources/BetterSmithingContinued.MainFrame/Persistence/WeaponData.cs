using BetterSmithingContinued.Annotations;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("CraftedItem")]
	public class WeaponData
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public static WeaponData GetWeaponData(string _weaponName, CraftingTemplate _craftingTemplate, WeaponDesignElement[] _selectedPieces)
		{
			WeaponData weaponData = new WeaponData();
			weaponData.m_Id = _craftingTemplate.StringId;
			weaponData.m_Name = _weaponName;
			weaponData.m_CraftingTemplate = _craftingTemplate.WeaponDescriptions.Last().WeaponClass;
			weaponData.m_PieceData = (from x in _selectedPieces where x.IsValid select new PieceData {
				Id = x.CraftingPiece.StringId,
				ScaleFactor = x.ScalePercentage,
				PieceType = x.CraftingPiece.PieceType
			}).ToArray();
			return weaponData;
		}

		[XmlAttribute("id")]
		public string Id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				if (this.m_Id != value)
				{
					this.m_Id = value;
					this.OnPropertyChanged("Id");
				}
			}
		}

		[XmlAttribute("crafting_template")]
		public WeaponClass CraftingTemplate
		{
			get
			{
				return this.m_CraftingTemplate;
			}
			set
			{
				if (this.m_CraftingTemplate != value)
				{
					this.m_CraftingTemplate = value;
					this.OnPropertyChanged("CraftingTemplate");
				}
			}
		}

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (this.m_Name != value)
				{
					this.m_Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		[XmlElement("Pieces")]
		public PieceData[] PieceData
		{
			get
			{
				return this.m_PieceData;
			}
			set
			{
				if (this.m_PieceData != value)
				{
					this.m_PieceData = value;
					this.OnPropertyChanged("PieceData");
				}
			}
		}

		[XmlIgnore]
		public ItemObject ItemObject
		{
			get
			{
				if (this.m_ItemObject == null)
				{
					this.RegisterWeaponData();
				}
				return this.m_ItemObject;
			}
		}

		public bool ApplyWeaponData(WeaponDesignVM _weaponDesignVMInstance)
		{
			try
			{
				Crafting craftingComponent = _weaponDesignVMInstance.GetCraftingComponent();
				CraftingTemplate craftingTemplate = CraftingTemplateUtilities.GetAll().FirstOrDefault((CraftingTemplate x) => x.StringId == this.Id);
				_weaponDesignVMInstance.SelectPrimaryWeaponClass(craftingTemplate);
				_weaponDesignVMInstance.RefreshValues();
				foreach (PieceData pieceData in this.PieceData)
				{
					if (pieceData.PieceType != CraftingPiece.PieceTypes.Invalid && !string.IsNullOrEmpty(pieceData.Id) && craftingTemplate.IsPieceTypeUsable(pieceData.PieceType))
					{
						CraftingPiece craftingPiece = CraftingPiece.All.FirstOrDefault((CraftingPiece p) => p.StringId == pieceData.Id);
						MBBindingList<CraftingPieceVM> pieces = this.m_LazyPieceLists.Value(_weaponDesignVMInstance)[craftingPiece.PieceType].Pieces;
						CraftingPieceVM craftingPieceVM = pieces.FirstOrDefault((CraftingPieceVM piece) => piece.CraftingPiece.CraftingPiece == craftingPiece)
														?? pieces.FirstOrDefault();
						if (craftingPieceVM != null)
						{
							this.m_LazyOnSetItemPart.Value(_weaponDesignVMInstance, craftingPieceVM, pieceData.ScaleFactor, true, false);
							craftingComponent.ScaleThePiece(craftingPiece.PieceType, pieceData.ScaleFactor);
						}
					}
				}
				craftingComponent.SetCraftedWeaponName(new TextObject("{=!}" + this.Name, null));

                _weaponDesignVMInstance.RefreshValues();
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(_propertyName));
		}

		private void RegisterWeaponData()
		{
			try
			{
				CraftingTemplate template = CraftingTemplateUtilities.GetAll().FirstOrDefault(x => x.StringId == this.Id);
                WeaponDesignElement[] array = new WeaponDesignElement[4];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes)i);
                }
				foreach (PieceData pieceData in this.PieceData)
				{
					WeaponDesignElement weaponDesignElement = WeaponDesignElement.CreateUsablePiece(
						CraftingPiece.All.FirstOrDefault(p => p.StringId == pieceData.Id),
						pieceData.ScaleFactor
					);
					array[(int)pieceData.PieceType] = weaponDesignElement;
                }


                TextObject name = new TextObject("{=!}" + this.Name, null);
                WeaponDesign weaponDesign = new WeaponDesign(template, name, array);
				string id = weaponDesign.HashedCode;		
                this.m_ItemObject = MBObjectManager.Instance.GetObject<ItemObject>(id);
				if (this.m_ItemObject == null)
				{
                    ItemObject itemObject = new ItemObject();
                    Crafting crafting = Instances.SmithingManager.WeaponDesignVM.GetCraftingComponent();
                    CraftingUtils.SmartGenerateItem(
                        weaponDesign,
                        name,
                        Hero.MainHero.Culture,
                        weaponDesign.Template.ItemModifierGroup,
                        ref itemObject,
                        weaponDesign.HashedCode
                    );
                    this.m_ItemObject = MBObjectManager.Instance.RegisterObject(itemObject);
                }
			}
			catch (Exception value)
            {
                Console.WriteLine(value);
				throw new Exception("RegisterWeaponData: can't register weapon with name '" + this.Name + "'");
			}
		}

		private readonly Lazy<Func<WeaponDesignVM, Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM>>> m_LazyPieceLists = new Lazy<Func<WeaponDesignVM, Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM>>>(delegate()
		{
			FieldInfo fieldInfo = MemberExtractor.GetPrivateFieldInfo<WeaponDesignVM>("_pieceListsDictionary");
			return delegate(WeaponDesignVM _vm)
			{
				return (fieldInfo?.GetValue(_vm)) as Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM>;
			};
		});

		private readonly Lazy<Action<WeaponDesignVM, CraftingPieceVM, int, bool, bool>> m_LazyOnSetItemPart = new Lazy<Action<WeaponDesignVM, CraftingPieceVM, int, bool, bool>>(delegate()
		{
			MethodInfo methodInfo = MemberExtractor.GetPrivateMethodInfo<WeaponDesignVM>("OnSetItemPiece");
			if (methodInfo == null)
			{
				return delegate(WeaponDesignVM instance, CraftingPieceVM part, int scalePercentage, bool shouldUpdateWholeWeapon, bool forceUpdatePiece)
				{
				};
			}
			return delegate(WeaponDesignVM instance, CraftingPieceVM part, int scalePercentage, bool shouldUpdateWholeWeapon, bool forceUpdatePiece)
			{
				methodInfo.Invoke(instance, new object[]
				{
					part,
					scalePercentage,
					shouldUpdateWholeWeapon,
					forceUpdatePiece
				});
			};
		});

		private string m_Id;
		private WeaponClass m_CraftingTemplate;
		private string m_Name;
		private PieceData[] m_PieceData;
		private ItemObject m_ItemObject;
	}
}
