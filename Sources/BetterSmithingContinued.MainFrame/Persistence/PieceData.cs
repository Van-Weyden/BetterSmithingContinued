using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using BetterSmithingContinued.Annotations;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("Piece")]
	public class PieceData
	{
		public event PropertyChangedEventHandler PropertyChanged;

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

		[XmlAttribute("Type")]
		public CraftingPiece.PieceTypes PieceType
		{
			get
			{
				return this.m_PieceType;
			}
			set
			{
				if (this.m_PieceType != value)
				{
					this.m_PieceType = value;
					this.OnPropertyChanged("PieceType");
				}
			}
		}

		[XmlAttribute("scale_factor")]
		public int ScaleFactor
		{
			get
			{
				return this.m_ScaleFactor;
			}
			set
			{
				if (this.m_ScaleFactor != value)
				{
					this.m_ScaleFactor = value;
					this.OnPropertyChanged("ScaleFactor");
				}
			}
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

		private string m_Id;

		private CraftingPiece.PieceTypes m_PieceType;

		private int m_ScaleFactor;
	}
}
