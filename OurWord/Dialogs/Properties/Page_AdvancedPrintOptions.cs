/**********************************************************************************************
 * Project: Our Word!
 * File:    Page_AdvancedPrintOptions.cs
 * Author:  John Wimbish
 * Created: 31 May 2006
 * Purpose: Set options for printing the footer
 * Legal:   Copyright (c) 2004-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using JWTools;
using JWdb;
using OurWord.DataModel;
using OurWord.Dialogs;
#endregion

namespace OurWord
{
    public class Page_AdvancedPrintOptions : DlgPropertySheet
	{
		// Scaffolding -----------------------------------------------------------------------
		#region DIALOG CONTROLS

        private System.Windows.Forms.Label m_labelTitle;
        private PropertyGrid m_PropGrid;
		// Required designer variable.
		private System.ComponentModel.Container components = null;
		#endregion
		#region Constructor()
        public Page_AdvancedPrintOptions(DialogProperties _ParentDlg)
            : base(_ParentDlg)
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}
		#endregion
		#region Method: void Dispose(...) - Clean up any resources being used.
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_labelTitle = new System.Windows.Forms.Label();
            this.m_PropGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // m_labelTitle
            // 
            this.m_labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelTitle.Location = new System.Drawing.Point(15, 12);
            this.m_labelTitle.Name = "m_labelTitle";
            this.m_labelTitle.Size = new System.Drawing.Size(438, 23);
            this.m_labelTitle.TabIndex = 21;
            this.m_labelTitle.Text = "Advanced Printing Options";
            this.m_labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_PropGrid
            // 
            this.m_PropGrid.Location = new System.Drawing.Point(18, 38);
            this.m_PropGrid.Name = "m_PropGrid";
            this.m_PropGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.m_PropGrid.Size = new System.Drawing.Size(435, 312);
            this.m_PropGrid.TabIndex = 22;
            this.m_PropGrid.ToolbarVisible = false;
            // 
            // Page_AdvancedPrintOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.m_PropGrid);
            this.Controls.Add(this.m_labelTitle);
            this.Name = "Page_AdvancedPrintOptions";
            this.Size = new System.Drawing.Size(468, 368);
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion

        // Property Grid ---------------------------------------------------------------------
        #region PROP CONSTANTS
        const string c_sOddPages  = "propOddPages";
        const string c_sEvenPages = "propEvenPages";

        const string c_sOddLeft    = "propOddLeft";
        const string c_sOddCenter  = "propOddCenter";
        const string c_sOddRight   = "propOddRight";
        const string c_sEvenLeft   = "propEvenLeft";
        const string c_sEvenCenter = "propEvenCenter";
        const string c_sEvenRight  = "propEvenRight";

        const string c_sCopyrightNotice = "propCopyrightNotice";
        #endregion
        #region Attr{g}: PropertyBag Bag - Defines the properties to display (including localizations)
        PropertyBag Bag
        {
            get
            {
                Debug.Assert(null != m_bag);
                return m_bag;
            }
        }
        PropertyBag m_bag;
        #endregion
        #region Method: void bag_GetValue(object sender, PropertySpecEventArgs e)
        void bag_GetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sOddLeft:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.OddLeft);
                    break;
                case c_sOddCenter:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.OddMiddle);
                    break;
                case c_sOddRight:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.OddRight);
                    break;
                case c_sEvenLeft:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.EvenLeft);
                    break;
                case c_sEvenCenter:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.EvenMiddle);
                    break;
                case c_sEvenRight:
                    e.Value = (e.Property as EnumPropertySpec).GetEnumValueFor(
                        (int)G.TeamSettings.EvenRight);
                    break;
                case c_sCopyrightNotice:
                    e.Value = G.TeamSettings.CopyrightNotice;
                    break;
            }
        }
        #endregion
        #region Method: void bag_SetValue(object sender, PropertySpecEventArgs e)
        void bag_SetValue(object sender, PropertySpecEventArgs e)
        {
            switch (e.Property.ID)
            {
                case c_sOddLeft:
                    G.TeamSettings.OddLeft = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sOddCenter:
                    G.TeamSettings.OddMiddle = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sOddRight:
                    G.TeamSettings.OddRight = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sEvenLeft:
                    G.TeamSettings.EvenLeft = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sEvenCenter:
                    G.TeamSettings.EvenMiddle = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sEvenRight:
                    G.TeamSettings.EvenRight = (DTeamSettings.FooterParts)
                        (e.Property as EnumPropertySpec).GetEnumNumberFor((string)e.Value);
                    break;
                case c_sCopyrightNotice:
                    G.TeamSettings.CopyrightNotice = (string)e.Value;
                    break;
            }
        }
        #endregion
        #region Method: void SetupPropertyGrid()
        void SetupPropertyGrid()
        {
            // Create the PropertyBag for this grid
            m_bag = new PropertyBag();
            Bag.GetValue += new PropertySpecEventHandler(bag_GetValue);
            Bag.SetValue += new PropertySpecEventHandler(bag_SetValue);

            // Enumerations
            int[] vn = {
                (int)DTeamSettings.FooterParts.kBlank,
                (int)DTeamSettings.FooterParts.kCopyrightNotice,
                (int)DTeamSettings.FooterParts.kPageNumber,
                (int)DTeamSettings.FooterParts.kScriptureReference,
                (int)DTeamSettings.FooterParts.kStageAndDate,
                (int)DTeamSettings.FooterParts.kLanguageStageAndDate
            };
            string sDefaultValue = "(leave blank)";
            string[] vs = {
                sDefaultValue,
                "Copyright Notice",
                "Page Number",
                "Scripture Reference",
                "Stage and Date",
                "Language, Stage and Date"
            };

            // Odd Pages
            Bag.Properties.Add(new EnumPropertySpec(
                c_sOddLeft,
                "Left",
                c_sOddPages,
                "What do you want to print at the bottom left of odd-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));
            Bag.Properties.Add(new EnumPropertySpec(
                c_sOddCenter,
                "Center",
                c_sOddPages,
                "What do you want to print at the bottom center of odd-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));
            Bag.Properties.Add(new EnumPropertySpec(
                c_sOddRight,
                "Right",
                c_sOddPages,
                "What do you want to print at the bottom right of odd-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));

            // Even Pages
            Bag.Properties.Add(new EnumPropertySpec(
                c_sEvenLeft,
                "Left",
                c_sEvenPages,
                "What do you want to print at the bottom left of even-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));
            Bag.Properties.Add(new EnumPropertySpec(
                c_sEvenCenter,
                "Center",
                c_sEvenPages,
                "What do you want to print at the bottom center of even-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));
            Bag.Properties.Add(new EnumPropertySpec(
                c_sEvenRight,
                "Right",
                c_sEvenPages,
                "What do you want to print at the bottom right of even-numbered pages?",
                typeof(DTeamSettings.FooterParts),
                vn, vs, sDefaultValue
                ));

            // Name
            Bag.Properties.Add(new PropertySpec(
                c_sCopyrightNotice,
                "Copyright Notice",
                typeof(string),
                "",
                "Team-wide Copyright Notice (you can define a copyright for each " +
                    "individual book by using the Book Properties dialog.)",
                "",
                "",
                null));

            // Localize the bag
            LocDB.Localize(this, Bag);

            // Set the Property Grid to this PropertyBag
            m_PropGrid.SelectedObject = Bag;
        }
        #endregion

        // DlgPropertySheet overrides --------------------------------------------------------
        #region Method: override bool HarvestChanges()
        public override bool HarvestChanges()
        {
            return true;
        }
        #endregion
        #region Method: void ShowHelp()
        public override void ShowHelp()
        {
            HelpSystem.ShowTopic(HelpSystem.Topic.kPrinting);
        }
        #endregion
        #region Attr{g}: string TabText
        public override string TabText
        {
            get
            {
                return Strings.PropDlgTab_PrintOptions;
            }
        }
        #endregion

		// Command Handlers ------------------------------------------------------------------
		#region Cmd: cmdLoad
		private void cmdLoad(object sender, System.EventArgs e)
		{
            // Setup the property grid
            SetupPropertyGrid();

            // Localize
            Control[] vc = new Control[] { m_PropGrid };
            LocDB.Localize(this, vc);
		}
		#endregion
	}
}
