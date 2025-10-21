using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core;
using TaleWorlds.Library;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;

namespace BetterSmithingContinued.MainFrame.UI
{
	[ViewModelMixin("RefreshSkills")]
	public class CraftingAvailableHeroItemVMMixin : BaseViewModelMixin<CraftingAvailableHeroItemVM>
	{
		public CraftingAvailableHeroItemVMMixin(CraftingAvailableHeroItemVM vm) : base(vm)
		{
			this.HeroSmithSkillColor = NoCapReachedColor;
		}

		[DataSourceProperty]
		public string HeroSmithSkillColor
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
				CraftingAvailableHeroItemVM viewModel = base.ViewModel;
				if (viewModel == null)
				{
					return;
				}
				viewModel.OnPropertyChangedWithValue(value, "HeroSmithSkillColor");
			}
		}

		public override void OnRefresh()
		{
			CraftingAvailableHeroItemVM viewModel = base.ViewModel;
			if (viewModel != null)
			{
				this.UpdateHeroSmithSkillColor(viewModel.Hero);
			}
		}

		private void UpdateHeroSmithSkillColor(Hero hero)
		{
			if (this.IsHeroReachedHardCap(hero))
			{
				this.HeroSmithSkillColor = HardCapReachedColor;
			}
			else if (hero.GetSkillValue(DefaultSkills.Crafting) >= this.HeroSmithSkillSoftCap(hero))
			{
				this.HeroSmithSkillColor = SoftCapReachedColor;
			}
			else
			{
				this.HeroSmithSkillColor = NoCapReachedColor;
			}
		}

		private bool IsHeroReachedHardCap(Hero hero)
		{
            return hero.HeroDeveloper.GetFocusFactor(DefaultSkills.Crafting) < 0.001f;
		}

		private float HeroSmithSkillSoftCap(Hero hero)
		{
			return Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(
                hero.CharacterAttributes,
				hero.HeroDeveloper.GetFocus(DefaultSkills.Crafting),
                DefaultSkills.Crafting,
				false
			).ResultNumber;
		}

		private string GetColorType()
		{
			if (this.m_color == NoCapReachedColor)
			{
				return "none";
			}
			if (this.m_color == SoftCapReachedColor)
			{
				return "soft";
			}
			if (this.m_color == HardCapReachedColor)
			{
				return "hard";
			}
			return "null";
		}

		private const string NoCapReachedColor = "#80AB22FF";
		private const string SoftCapReachedColor = "#EFAB6BFF";
		private const string HardCapReachedColor = "#C75808FF";

        private string m_color;
    }
}
