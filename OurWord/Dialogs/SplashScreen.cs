/**********************************************************************************************
 * App:     Josiah
 * File:    SplashScreen.cs
 * Author:  John Wimbish
 * Created: 24 July 2004
 * Purpose: Display a splash screen on startup so the user is patient during what can be
 *          a lengthy load process.
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Header: Using, etc.
using System;
using System.Collections;
using System.Globalization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;

using JWTools;
#endregion

namespace OurWord.Dialogs
{
	public class SplashScreen : System.Windows.Forms.Form
	{
		// Threading -------------------------------------------------------------------------
        static public bool IsEnabled = true;   // Set to false if needed for debugging
        static Thread s_Thread = null;
        #region SAttr{g}: SplashScreen Wnd
        static public SplashScreen Wnd
        {
            get
            {
                return s_wndSplash;
            }
        }
        static SplashScreen s_wndSplash = null;
        #endregion

        // Delegates for cross-thread calls -------------------------------------------------
        public delegate void SetStatus_Callback(string sWhatIsCurrentlyLoading, int cProgressIndicatorCount);
        public delegate void IncrementProgress_Callback();
        public delegate void ResetProgress_Callback();
        public delegate void StopSplashScreen_Callback(Form f);

        // Controls --------------------------------------------------------------------------
		private Label m_lblProgramName;
		private Label m_lblVersion;
		private Label m_lblAdditional;
		private Label m_lblStatus;

        // Pre-localized data to go into the controls. We set these prior to starting the
        // thread, so as to avoid the cross-thread issues with LocDB.
        #region Attr{g/s}: string ProgramName
        public static string ProgramName
        {
            get
            {
                return s_sProgramName;
            }
            set
            {
                s_sProgramName = value;
            }
        }
        static string s_sProgramName;
        #endregion
        #region Attr{g/s}: string Version
        public static string Version
        {
            get
            {
                return s_sVersion;
            }
            set
            {
                s_sVersion = value;
            }
        }
        static string s_sVersion;
        #endregion
        #region Attr{g/s}: string Additional
        public static string Additional
        {
            get
            {
                return s_sAdditional;
            }
            set
            {
                s_sAdditional = value;
            }
        }
        static string s_sAdditional;
        #endregion
        #region Attr{g/s}: string StatusMessage
        public static string StatusMessage
        {
            get
            {
                return s_sStatusMessage;
            }
            set
            {
                s_sStatusMessage = value;
            }
        }
        static string s_sStatusMessage = "";
        #endregion
        #region Attr{g/s}: string StatusBase
        public static string StatusBase
        {
            get
            {
                return s_sStatusBase;
            }
            set
            {
                s_sStatusBase = value;
            }
        }
        static string s_sStatusBase = "";
        #endregion

        static public Form ParentWnd;

        // Fade in and out -------------------------------------------------------------------
		private double m_dblOpacityIncrement = .05;
        private double m_dblOpacityDecrement = .08;
        private System.Timers.Timer m_Timer;
		private const int TIMER_INTERVAL = 50;
		#region Method: void OnTimerTick(object sender, System.EventArgs e)

        delegate void OnTimerTick_Callback(object sender, System.EventArgs e);

		private void OnTimerTick(object sender, System.EventArgs e)
		{
            if (s_wndSplash.InvokeRequired)
            {
                OnTimerTick_Callback cb = new OnTimerTick_Callback(OnTimerTick);
                s_wndSplash.Invoke(cb, new object[] { sender, e });
            }
            else
            {
                // Deal with fading in / fading out
                if (m_dblOpacityIncrement > 0)
                {
                    if (this.Opacity < 1)
                        this.Opacity += m_dblOpacityIncrement;
                }
                else
                {
                    if (this.Opacity > 0)
                        this.Opacity += m_dblOpacityIncrement;
                    else
                    {
                        m_Timer.Dispose();
                        this.Close();
                        this.Dispose();
                        s_wndSplash = null;
                        s_Thread = null;
                        return;
                    }
                }

            // Make sure the Status label is displaying the most up-to-date message
            m_lblStatus.Text = s_sStatusMessage;

            // Update the progress bar
            UpdateProgressBarDisplay();
            }
		}
		#endregion

		// Status Message --------------------------------------------------------------------
        #region Method: void SetStatus(string sWhatIsCurrentlyLoading, cProgressIndicatorCount)
        public void SetStatus(string sWhatIsCurrentlyLoading, int cProgressIndicatorCount)
		{
			if( s_wndSplash == null )
				return;

            SetProgressBound(cProgressIndicatorCount);

            // Insert the object we're loading
            int iPos = StatusBase.IndexOf("{0}");
            if (iPos >= 0)
            {
                string sFirst = StatusBase.Substring(0, iPos);
                string sLast = StatusBase.Substring(iPos + 3);
                s_sStatusMessage = sFirst + sWhatIsCurrentlyLoading + sLast;
            }
		}

		#endregion

		// Progress Indicator ----------------------------------------------------------------
		private Panel m_panelStatus;                   // Control which serves as progress bar
		private double m_dblCompletionFraction = 0;    // Between 0.0 and 1.0
		private double m_dblTickAmount = 0.1;          // Amount to increment
		private Rectangle m_rProgress;                 // onPaint uses to know what to draw
		#region Method: private void onPaintProgress(...)
		private void onPaintProgress(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if( e.ClipRectangle.Width > 0 && m_dblCompletionFraction > 0 )
			{
				LinearGradientBrush brBackground = 
					new LinearGradientBrush(m_rProgress, 
						Color.FromArgb(255, 255, 125),   // 50, 50, 200
						Color.FromArgb(150,  75, 160), // 150, 150, 255
						LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(brBackground, m_rProgress);
			}		
		}
		#endregion
		#region Method: private void UpdateProgressBarDisplay() - Called by timer to update bar
		private void UpdateProgressBarDisplay()
		{
			int width = (int)Math.Floor(m_panelStatus.ClientRectangle.Width 
				* m_dblCompletionFraction);
			int height = m_panelStatus.ClientRectangle.Height;
			int x = m_panelStatus.ClientRectangle.X;
			int y = m_panelStatus.ClientRectangle.Y;
			if( width > 0 && height > 0 )
			{
				m_rProgress = new Rectangle( x, y, width, height);
				m_panelStatus.Invalidate(m_rProgress);
			}		
		}
		#endregion

		#region Method: void IncrementProgress()
		public void IncrementProgress()
		{
			if (null == s_wndSplash || !IsEnabled)
				return;
			s_wndSplash.m_dblCompletionFraction += s_wndSplash.m_dblTickAmount;
			s_wndSplash.m_dblCompletionFraction = 
				Math.Min(s_wndSplash.m_dblCompletionFraction, 1.0);
		}
		#endregion
		#region Method: void SetProgressBound(int cAmount)
		static private void SetProgressBound(int cAmount)
		{
			if (null == s_wndSplash || !IsEnabled)
				return;
			s_wndSplash.m_dblTickAmount = 1.0 / (double)cAmount;
		}
		#endregion
		#region Method: void ResetProgress()
		public void ResetProgress()
		{
            if (null == s_wndSplash || !IsEnabled)
                return;

            if (null == s_wndSplash)
                return;

            s_wndSplash.m_dblCompletionFraction = 0;
            s_wndSplash.UpdateProgressBarDisplay();
            s_wndSplash.Invalidate(s_wndSplash.m_panelStatus.ClientRectangle);
            s_wndSplash.Refresh();
		}
		#endregion

		// Open and Close --------------------------------------------------------------------
		#region Method: void Start()
		static public void Start()
		{
			// Make sure it is only launched once.
			if( s_wndSplash != null || !IsEnabled)
				return;

            s_Thread = new Thread(new ThreadStart(SplashScreen.ShowForm));
            s_Thread.IsBackground = true;
            s_Thread.Name = "SplashScreen";
            s_Thread.SetApartmentState(ApartmentState.STA);
            s_Thread.Start();
		}
		#endregion
		#region Attr{g}: bool IsShowing
		static public bool IsShowing
		{
			get
			{
				return ( null != s_wndSplash );
			}
		}
		#endregion

        #region Method: void Stop(Form wndParent)
        public void Stop(Form wndParent)
        {
            if (!IsEnabled)
                return;

            if (null != s_wndSplash)
            {
                // The timer code will start the fade-out; when the fade is done,
                // the window will be closed.
                s_wndSplash.m_dblOpacityIncrement = -s_wndSplash.m_dblOpacityDecrement;

                // By setting a parent, we avoid the problem of the main form no longer
                // being on top. (It otherwise has a tendency to dissappear in the Z order.)
                // Update the owner a thread safe fashion!
// TODO: Must do this in a thread-safe manner.
// Doing the CheckForIllegalCrossThreadCalls thing in the constructor for now, if Mono will let me get away with it
               if (null != wndParent)
                   s_wndSplash.Owner = wndParent;

            }
        }
        #endregion

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public SplashScreen()
		{
			// Required for Windows Form Designer support
			InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

			// By having the client resize to the background image, we can use
			// different images without having to programmatically worry about
			// the window size.
			ClientSize = this.BackgroundImage.Size;

			// Position it on the proper monitor. This will be the primary monitor
			// unless we find in the registry that it was previously on another monitor
			// (and of course, if that monitor currently exists.)
			Screen scr = JW_WindowState.GetLastScreen( JW_WindowState.DefaultRegistrySubKey);
			Rectangle ScreenBounds = scr.Bounds;
			int xCenter = ScreenBounds.Left + ScreenBounds.Width / 2;
			int yCenter = ScreenBounds.Top + ScreenBounds.Height / 2;
			Location = new Point( 
				xCenter - ClientSize.Width / 2 ,
				yCenter - ClientSize.Height / 2 );

			// Initialize the timer and fadeout
            m_Timer = new System.Timers.Timer();
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
			Opacity = .0;
			m_Timer.Interval = TIMER_INTERVAL;
			m_Timer.Start();
		}
		#endregion
		#region Method: override void Dispose( bool disposing 
		// Clean up any resources being used.
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
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.m_lblStatus = new System.Windows.Forms.Label();
            this.m_panelStatus = new System.Windows.Forms.Panel();
            this.m_lblProgramName = new System.Windows.Forms.Label();
            this.m_lblVersion = new System.Windows.Forms.Label();
            this.m_lblAdditional = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_lblStatus
            // 
            this.m_lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.m_lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblStatus.ForeColor = System.Drawing.Color.Navy;
            this.m_lblStatus.Location = new System.Drawing.Point(8, 352);
            this.m_lblStatus.Name = "m_lblStatus";
            this.m_lblStatus.Size = new System.Drawing.Size(376, 23);
            this.m_lblStatus.TabIndex = 0;
            this.m_lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_panelStatus
            // 
            this.m_panelStatus.BackColor = System.Drawing.Color.Transparent;
            this.m_panelStatus.Location = new System.Drawing.Point(8, 376);
            this.m_panelStatus.Name = "m_panelStatus";
            this.m_panelStatus.Size = new System.Drawing.Size(376, 16);
            this.m_panelStatus.TabIndex = 1;
            this.m_panelStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaintProgress);
            // 
            // m_lblProgramName
            // 
            this.m_lblProgramName.BackColor = System.Drawing.Color.Transparent;
            this.m_lblProgramName.Font = new System.Drawing.Font("Forte", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblProgramName.ForeColor = System.Drawing.Color.Navy;
            this.m_lblProgramName.Location = new System.Drawing.Point(8, 16);
            this.m_lblProgramName.Name = "m_lblProgramName";
            this.m_lblProgramName.Size = new System.Drawing.Size(416, 64);
            this.m_lblProgramName.TabIndex = 2;
            this.m_lblProgramName.Text = "Our Word!";
            this.m_lblProgramName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblVersion
            // 
            this.m_lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.m_lblVersion.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblVersion.ForeColor = System.Drawing.Color.Navy;
            this.m_lblVersion.Location = new System.Drawing.Point(56, 80);
            this.m_lblVersion.Name = "m_lblVersion";
            this.m_lblVersion.Size = new System.Drawing.Size(368, 23);
            this.m_lblVersion.TabIndex = 3;
            this.m_lblVersion.Text = "Version {0}";
            this.m_lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_lblAdditional
            // 
            this.m_lblAdditional.BackColor = System.Drawing.Color.Transparent;
            this.m_lblAdditional.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAdditional.ForeColor = System.Drawing.Color.Navy;
            this.m_lblAdditional.Location = new System.Drawing.Point(56, 120);
            this.m_lblAdditional.Name = "m_lblAdditional";
            this.m_lblAdditional.Size = new System.Drawing.Size(376, 176);
            this.m_lblAdditional.TabIndex = 4;
            // 
            // SplashScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(448, 400);
            this.Controls.Add(this.m_lblAdditional);
            this.Controls.Add(this.m_lblVersion);
            this.Controls.Add(this.m_lblProgramName);
            this.Controls.Add(this.m_lblStatus);
            this.Controls.Add(this.m_panelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.cmdLoad);
            this.ResumeLayout(false);

		}
		#endregion
        #region Method: void ShowForm() - Starts the Splash Screen as an Application
        static private void ShowForm()
        {
            s_wndSplash = new SplashScreen();
            Application.Run(s_wndSplash);
        }
        #endregion
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
            // Handle Localization
        {
            // Place already-localized values into their labels
            m_lblVersion.Text = Version;

            if ("-" != Additional)
                m_lblAdditional.Text = Additional;

            m_lblProgramName.Text = ProgramName;
        }
        #endregion
    }
}
