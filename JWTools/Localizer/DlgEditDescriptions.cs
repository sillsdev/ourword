/**********************************************************************************************
 * Dll:     JWTools
 * File:    DlgEditDescriptions.cs
 * Author:  John Wimbish
 * Created: 22 Aug 2007
 * Purpose: Allows me to edit the English descriptions for the Localization tool.
 * Legal:   Copyright (c) 2005-08, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
#endregion

namespace JWTools
{
    public partial class DlgEditDescriptions : Form
    {
        #region Attr{g}: string GroupDescription
        public string GroupDescription
        {
            get
            {
                return m_textGroupDescription.Text;
            }
            set
            {
                m_textGroupDescription.Text = value;
            }
        }
        #endregion
        #region Attr{g}: string ItemDescription
        public string ItemDescription
        {
            get
            {
                return m_textItemDescription.Text;
            }
            set
            {
                m_textItemDescription.Text = value;
            }
        }
        #endregion

        #region Constructor()
        public DlgEditDescriptions()
        {
            InitializeComponent();
        }
        #endregion
    }
}