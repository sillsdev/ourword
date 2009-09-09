#region ***** Layout.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    Layouts\Layout.cs
 * Author:  John Wimbish
 * Created: 10 Aug 2009
 * Purpose: Common functionality for the various views
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;

using OurWord.Edit;
using OurWord.Layouts;
using JWdb;
using JWdb.DataModel;
using JWTools;
#endregion
#endregion

namespace OurWord.Layouts
{
    public class Layout : OWWindow
    {
        #region Constructor(sName, cColumnCout)
        protected Layout(string sName, int cColumnCount)
            : base(sName, cColumnCount)
        {
        }
        #endregion

        #region Method: OWPara CreateFootnotePara(...)
        protected OWPara CreateFootnotePara(DFootnote footnote, 
            JWritingSystem ws,
            Color backColor, 
            OWPara.Flags flags)
        {
            var owp = new OWPara(
                ws,
                footnote.Style,
                footnote,
                backColor,
                flags);

            if (!string.IsNullOrEmpty(footnote.VerseReference))
            {
                var f = footnote.Style.CharacterStyle.FindOrAddFontForWritingSystem(ws);
                var label = new DLabel(footnote.VerseReference + ": ");
                owp.InsertAt(0, new OWPara.ELabel(f, label));
            }

            return owp;
        }
        #endregion

        #region VMethod: void SetupInsertNoteDropdown(btnInsertNote)
        public virtual void SetupInsertNoteDropdown(ToolStripDropDownButton btnInsertNote)
            // Default is that we just insert a General Note, thus no dropdown items
        {
            foreach (ToolStripItem item in btnInsertNote.DropDownItems)
                item.Visible = false;
            btnInsertNote.ShowDropDownArrow = false;
        }
        #endregion
    }
}
