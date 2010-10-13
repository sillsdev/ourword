#region *** JW_WindowState.cs ***
/**********************************************************************************************
 * Dll:     JWTools
 * File:    JW_WindowState.cs
 * Author:  John Wimbish
 * Created: 03 Oct 2003
 * Purpose: Saves / restores the window position and state upon app shutdown / startup.
 * Legal:   Copyright (c) 2005, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace JWTools
{
	public class JW_WindowState
	{
		// Attributes ------------------------------------------------------------------------
        public const string DefaultRegistrySubKey = "WindowState";
        public string WindowStateRegistrySubKey = DefaultRegistrySubKey;
		private Form m_form;
		#region Attr{g/s}: bool StartMaximized - if T, maximize no matter what is in the Registry
		public bool StartMaximized
		{
			get 
			{
                return JW_Registry.GetValue(WindowStateRegistrySubKey, "StartMax", true);
			}
			set 
			{
                JW_Registry.SetValue(WindowStateRegistrySubKey, "StartMax", value);
			}
		}
		#endregion
		#region Attr{g/s}: bool StateSaved - if T, we've saved the state in the Registry
		public bool StateSaved
		{
			get 
			{
                return ("State Saved" == JW_Registry.GetValue(WindowStateRegistrySubKey, "", ""));
			}
			set 
			{
                JW_Registry.SetValue(WindowStateRegistrySubKey, "", (value ? "State Saved" : ""));
			}
		}
		#endregion

		#region Attr{g}: string ScreenNumber - the number of the monitor where the form is displayed right now
		string ScreenNumber
		{
			get
			{
				var scr = Screen.FromRectangle( 	
					m_form.RectangleToScreen( m_form.DisplayRectangle ) );

                foreach (var ch in scr.DeviceName)
                {
                    if (Char.IsNumber(ch))
                        return ch.ToString();
                }

			    return "";
			}
		}
		#endregion

		// Constructors ----------------------------------------------------------------------
		#region Constructor(form, bStartMaximized)
		public JW_WindowState(Form form, bool bStartMaximized)
		{
			Debug.Assert(null != form);
			m_form = form;

			// If we don't already have a preference in the Registry, then whether or not the
			// window starts up maximized is determined by the value passed into the
			// constructor.
			if ( !StateSaved )
				StartMaximized = bStartMaximized;
		}
		#endregion

		// Save & Restore State --------------------------------------------------------------
		#region Method: SaveWindowState()
		public void SaveWindowState()
		{
			Debug.Assert(null != m_form);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "Left", m_form.Left);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "Top", m_form.Top);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "Height", m_form.Height);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "Width", m_form.Width);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "State", (int)m_form.WindowState);
            JW_Registry.SetValue(WindowStateRegistrySubKey, "Screen", ScreenNumber);
			StateSaved = true;
		}
		#endregion
		#region Method: RestoreWindowState()
		public void RestoreWindowState()
		{
			Debug.Assert(null != m_form);

			// If we've stored Maximize, then we want to start Maximized. This can either
			// be because (1) the user left it Maximized last time, or (2) there is no
			// prior value in the Registry and the default passed to the constructor
			// is for it to be maximised.
			if (StartMaximized)
			{
				m_form.WindowState = FormWindowState.Maximized;
				return;
			}

			// Retrieve the position and size values
            m_form.Left = JW_Registry.GetValue(WindowStateRegistrySubKey, "Left", m_form.Left);
            m_form.Top = JW_Registry.GetValue(WindowStateRegistrySubKey, "Top", m_form.Top);
            m_form.Height = JW_Registry.GetValue(WindowStateRegistrySubKey, "Height", m_form.Height);
            m_form.Width = JW_Registry.GetValue(WindowStateRegistrySubKey, "Width", m_form.Width);

			// Retrieve the Maximized/Minimized value.
			// - If it was Minimized, then change it to Normal (we don't want to start up
			//     with a minimized window.)
			// - If it was Maximized, then we'll leave it that way. 
            m_form.WindowState = (FormWindowState)JW_Registry.GetValue(WindowStateRegistrySubKey, 
				"State", (int)m_form.WindowState);
			if (m_form.WindowState == FormWindowState.Minimized)
				m_form.WindowState = FormWindowState.Normal;

			// Retrieve the available screen size
			var rectBound = SystemInformation.VirtualScreen;

			// Make sure the window is not too big to fit on the screen
			m_form.Width  = Math.Min(m_form.Width,  rectBound.Width);
			m_form.Height = Math.Min(m_form.Height, rectBound.Height);

			// Make sure the window is not too far left or above to be entirely seen.
			m_form.Left = Math.Max(m_form.Left, rectBound.Left);
			m_form.Top  = Math.Max(m_form.Top,  rectBound.Top);

			// Make sure the window is not too far right or below to be seen.
			var maxLeft = rectBound.Left + rectBound.Width  - m_form.Width;
			var maxTop  = rectBound.Top  + rectBound.Height - m_form.Height;
			m_form.Left = Math.Min(maxLeft, m_form.Left);
			m_form.Top  = Math.Min(maxTop,  m_form.Top);
		}
		#endregion

		#region Method: Screen GetLastScreen() - retrieve which monitor the form will load in
		public Screen GetLastScreen()
		{
		    return GetLastScreen(WindowStateRegistrySubKey);
		}
		#endregion
        #region static Method: Screen GetLastScreen() - retrieve which monitor the form will load in
        static public Screen GetLastScreen(string sWindowStateRegistrySubKey)
        {
            var sScreenName = JW_Registry.GetValue(sWindowStateRegistrySubKey, "Screen", "");

            if (sScreenName.Length > 0)
            {
                foreach (var scr in Screen.AllScreens)
                {
                    foreach (var ch in scr.DeviceName)
                    {
                        if (Char.IsNumber(ch))
                        {
                            if (ch.ToString() == sScreenName)
                                return scr;
                            break;
                        }
                    }
                }
            }

            return Screen.PrimaryScreen;
        }
        #endregion
	}

	// Testing -------------------------------------------------------------------------------
	#region OBSOLETE NUnit Testing - NEED TO REPLACE
	/***
	[TestFixture]
	public class JW_WindowStateTest : Form
	{
		// Scaffolding -----------------------------------------------------------------------
		#region Constructor - required by NUnit
		public JW_WindowStateTest()
		{
		}
		#endregion
		#region Method: void Reset() - resets to initial conditions
		private void Reset()
		{
			// Set up the registry (delete any old keys)
			JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\JW_WindowState TestApp";
			JW_Registry.DeleteSubKey(""); // Removes 'JW_WindowState TestApp' on down.
		}
		#endregion

		// Tests -----------------------------------------------------------------------------
		#region Test_PositionRestoredToSamePlace()
		[Test]
		public void Test_PositionRestoredToSamePlace()
		{
			Reset();

			// Create the form
			Form form = new Form();
			form.Show();  // This command will change the position, so do it first.
			form.Left = 10;
			form.Top  = 20;
			form.Width = 200;
			form.Height = 100;
			form.WindowState = FormWindowState.Normal;

			// Save the state
			JW_WindowState ws = new JW_WindowState(form, false);
			ws.SaveWindowState();

			// Move and resize the form
			form.Left = 110;
			form.Top  = 120;
			form.Width = 300;
			form.Height = 400;

			// Restore the state & check that everything is the same
			ws.RestoreWindowState();
			Assert.IsTrue(form.Left   == 10);
			Assert.IsTrue(form.Top    == 20);
			Assert.IsTrue(form.Width  == 200);
			Assert.IsTrue(form.Height == 100);
			Assert.IsTrue(form.WindowState == FormWindowState.Normal);
			form.Hide();

			Reset();
		}
		#endregion
		#region Test_PositionNotOffScreen()
		[Test]
		public void Test_PositionNotOffScreen()
		{
			Reset();

			// Create the form
			Form form = new Form();
			form.Show();  // This command will change the position, so do it first.
			JW_WindowState ws = new JW_WindowState(form, false);

			// Move it off screen before left,top corner
			form.Left = -10;
			form.Top  = -20;
			form.Width = 200;
			form.Height = 100;
			form.WindowState = FormWindowState.Normal;

			// Save the state
			ws.SaveWindowState();
			Assert.IsTrue(form.Left < 0);
			Assert.IsTrue(form.Top < 0);

			// Restore the state & check that left/top are visible
			ws.RestoreWindowState();
			Assert.IsTrue(form.Left   >= 0);
			Assert.IsTrue(form.Top    >= 0);
			form.Hide();

			// Move it off the screen after right, bottom corner
			Rectangle rectBound = SystemInformation.VirtualScreen;
			form.Left = rectBound.Left + rectBound.Width + 100;
			form.Top  = rectBound.Top  + rectBound.Height + 100;
			ws.SaveWindowState();
			ws.RestoreWindowState();
			Assert.IsTrue(form.Left + form.Width  <= rectBound.Left + rectBound.Width);
			Assert.IsTrue(form.Top  + form.Height <= rectBound.Top  + rectBound.Height);

			Reset();
		}
		#endregion
		#region Test_WindowNotTooBig()
		[Test]
		public void Test_WindowNotTooBig()
		{
			Reset();

			// Create the form
			Form form = new Form();
			form.Show();  // This command will change the position, so do it first.
			JW_WindowState ws = new JW_WindowState(form, false);

			// Make it too big
			Rectangle rectBound = SystemInformation.VirtualScreen;
			form.Left = rectBound.Left;
			form.Top  = rectBound.Top;
			form.Width = rectBound.Width + 100;
			form.Height = rectBound.Height + 100;
			ws.SaveWindowState();

			// Now restore it and see if it is smaller
			ws.RestoreWindowState();
			Assert.IsTrue(form.Width <= rectBound.Width);
			Assert.IsTrue(form.Height <= rectBound.Height);
			form.Hide();

			Reset();
		}
		#endregion
	}
	***/
	#endregion
}
