/**********************************************************************************************
 * Project: Our Word!
 * File:    OurWordMain.cs
 * Author:  John Wimbish
 * Created: 2 Dec 2003
 * Purpose: Main window and app for the application.
 * Legal:   Copyright (c) 2004-10, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
#region Using
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

using JWTools;
using OurWord.Ctrls.Navigation;
using OurWord.Dialogs.Export;
using OurWord.Dialogs.Membership;
using OurWord.Edit.Blocks;
using OurWord.Printing;
using OurWordData;
using OurWordData.DataModel;
using OurWordData.DataModel.Annotations;
using OurWordData.DataModel.Membership;
using OurWordData.Styles;

using OurWord.Edit;
using OurWord.Layouts;
using OurWord.Dialogs;
using OurWord.Dialogs.History;
using OurWordData.Synchronize;
using Shortcut=OurWord.Utilities.Shortcut;

#endregion

namespace OurWord
{
	public class OurWordMain : Form
		// Main application window and top-level message routing
	{
		// Static Objects & Globals ----------------------------------------------------------
		#region Attr{g}: bool TargetIsLocked
		static public bool TargetIsLocked
		{
			get
			{
				Debug.Assert(null != DB.Project);

				var section = DB.Project.STarget;

				var book = (null == section) ? null : section.Book;
				if (null == book)
					return false;

                return (Users.Current.GetEditability(DB.TargetBook) != User.TranslationSettings.Editability.Full);
			}
		}
		#endregion
        #region Attr{g}: UndoRedoStack URStack
        public UndoRedoStack URStack
        {
            get
            {
                Debug.Assert(null != m_URStack);
                return m_URStack;
            }
        }
	    readonly UndoRedoStack m_URStack;
        #endregion

        // Client Windows --------------------------------------------------------------------
        #region CLIENT WINDOWS
        #region Method: void SetCurrentLayout(sLayoutName)
        public void SetCurrentLayout(string sLayoutName)
        {
            if (WLayout.SetCurrentLayout(sLayoutName))
                ResetWindowContents();
        }
        #endregion

        private CtrlNavigation m_Navigation;
        private Ctrls.Commands.CtrlCommands m_Commands;
        #region VAttr{g}: Layout CurrentLayout
        public WLayout CurrentLayout
        {
            get
            {
                return WLayout.CurrentLayout;
            }
        }
        #endregion

        #region VAttr{g}: Control FocusedWindow - the window with current Focus, or null
        OWWindow FocusedWindow
        {
            get
            {
                if (null == CurrentLayout)
                    return null;
                if (CurrentLayout.Focused)
                    return CurrentLayout;
                return null;
            }
        }
        #endregion

		#region Method: void SetTitleBarText()
		private void SetTitleBarText()
			// Titlebar - sets title bar text to "OurWord - ProjName"
		{
            // First, Display "Our Word"
			Text = LanguageResources.AppTitle;

            // If a project exists, then display its name
			if (null != DB.Project)
				Text += (@" - " + DB.Project.DisplayName);

            // If a book is loaded, then display its name
            if (null != DB.Project && null != DB.TargetBook)
            {
                Text += (@" - " + DB.TargetBook.DisplayName);
                return;
            }
		}
		#endregion
        #region Method: void ResetWindowContents() - called whenever content changes (OnEnterSection)
        public void ResetWindowContents()
        {
            // Set the TitleBar text
            SetTitleBarText();

            // Toolbar button enabling
            SetupMenusAndToolbarsVisibility();

            // Do we have a valid project? Can't do much if not.
            if (null == DB.Project || null == DB.FrontSection || null == DB.TargetSection)
            {
                CurrentLayout.Clear();
                CurrentLayout.Invalidate();
                return;
            }

            // Loading the main window should also load the data in the side windows, as this
            // is generally built as the main window is loaded (or as items there are selected)
            CurrentLayout.LoadData();

            // Place focus in the main window, so that it is ready for editing
            CurrentLayout.Focus();
        }
        #endregion
        #region Method: void Dim()
        void Dim()
        {
            if (ScreenDraw.Dim)
                return;

            ScreenDraw.Dim = true;
            CurrentLayout.Invalidate();
        }
        #endregion
        #region Method: void UnDim()
        void UnDim()
        {
            if (!ScreenDraw.Dim)
                return;

            ScreenDraw.Dim = false;
            CurrentLayout.Invalidate();
        }
        #endregion
        #endregion

        // Autosave --------------------------------------------------------------------------
		private System.Windows.Forms.Timer m_AutoSaveTimer;
		#region Method: void InitializeAutoSave()
		private void InitializeAutoSave()
		{
			// We'll have an autosave every 10 minutes
			const int c_AutoSaveMinutes = 10;

			m_AutoSaveTimer = new System.Windows.Forms.Timer(this.components);
			m_AutoSaveTimer.Tick += new System.EventHandler(OnTimerAutoSave);
			m_AutoSaveTimer.Interval = c_AutoSaveMinutes * 60 * 1000;

			m_AutoSaveTimer.Start();
		}
		#endregion
		#region Handler: void OnTimerAutoSave(object sender, System.EventArgs e)
		private void OnTimerAutoSave(object sender, System.EventArgs e)
		{
			//Console.WriteLine("--AutoSave Timer Tick--");

            if (null == DB.Project)
                return;

            // Do the save
            DB.Project.Save(G.CreateProgressIndicator());
		}
		#endregion

        // Toolbar, MenuBar, Taskbar & StatusBar ---------------------------------------------
        #region Toolbar, Taskbar & StatusBar
        #region Menu/Toolbar attributes


        // Taskbar
        #endregion
        #region Method: void EnableItalicsButton() - unique function for faster speed
        public void EnableItalicsButton()
        {
            var wnd = FocusedWindow;
            var bCanItalic = (null != wnd && wnd.canItalic);

            m_Commands.EnableItalics(bCanItalic);
        }
        #endregion
 
        #region Method: void SetupMenusAndToolbarsVisibility()
        void SetupMenusAndToolbarsVisibility()
            // Turn features on/off according to settings and environment
            //
            // Notes:
            //  - The Tools menu has GotoNext and GotoPrevious items. These are defined as
            //      NotVisible always. They exist only to provide the ShortcutKey methods
            //      of quickly going to Next/Previous sections.
        {
            // Localization
            LocDB.DB.SetPrimary(Users.Current.PrimaryUiLanguage);
            LocDB.DB.SetSecondary(Users.Current.SecondaryUiLanguage);

            m_Commands.Setup();

            EnableItalicsButton();
        }
        #endregion
        #endregion

        // Private attributes ----------------------------------------------------------------
		private readonly JW_WindowState m_WindowState;   // Save/restore state on close/launch of app

		// Scaffolding -----------------------------------------------------------------------
		#region Constructor()
		public OurWordMain()
			// Constructor, initializes the application
		{
			// Required for Windows Form Designer support
            components = new System.ComponentModel.Container();
			InitializeComponent();

            // Register Commands
		    m_Commands.OnExit += cmdExit;
		    m_Commands.OnSaveProject += cmdSaveProject;
		    m_Commands.OnPrintBook += cmdPrintBook;
		    m_Commands.OnDownloadRepositoryFromInternet += cmdDownloadRepository;
            m_Commands.OnCreateProject += cmdCreateProject;
            m_Commands.OnOpenProject += cmdOpenProject;
		    m_Commands.OnExportTranslation += cmdExportTranslation;
		    m_Commands.OnHistory += cmdHistory;
		    m_Commands.OnSwitchLayout += cmdSwitchLayout;
		    m_Commands.OnChangeZoomPercent += cmdChangeZoomPercent;
		    m_Commands.OnInsertNote += cmdInsertNote;
            // Editig commands
		    m_Commands.OnUndo += cmdUndo;
            m_Commands.OnRedo += cmdRedo;
		    m_Commands.OnCut += cmdCut;
		    m_Commands.OnCopy += cmdCopy;
		    m_Commands.OnPaste += cmdPaste;
		    m_Commands.OnItalic += cmdItalic;
		    m_Commands.OnChangeParagraphStyle += cmdChangeParagraphStyle;
		    m_Commands.OnInsertFootnote += cmdInsertFootnote;
		    m_Commands.OnDeleteFootnote += cmdDeleteFootnote;
		    m_Commands.OnCopyBtFromFront += cmdCopyBTFromFront;
            // Tools commands
		    m_Commands.OnSendReceive += cmdSendReceive;
		    m_Commands.OnCheckForUpdates += cmdCheckForUpdates;
		    m_Commands.OnConfigure += cmdConfigure;
		    m_Commands.OnIncrementBookStatus += cmdIncrementBookStatus;
		    m_Commands.OnRestoreFromBackup += cmdRestoreBackup;
		    m_Commands.OnLocalizerTool += cmdLocalizer;
		    m_Commands.OnHelpTopics += cmdHelpTopics;
		    m_Commands.OnAbout += cmdAbout;
            // User commands
		    m_Commands.OnAddNewUser += cmdAddNewUser;
		    m_Commands.OnChangeUser += cmdChangeUser;

            // Register Navigation
		    m_Navigation.OnGoToChapter += cmdGoToChapter;
            m_Navigation.OnGoToBook += cmdGoToBook;
		    m_Navigation.OnGoToSection += cmdGoToSection;


            // Suspend the layout while we create the windows. We create them here in order
            // to get the propert z-order
            // TO DO: Still necessary? Can we move this to OnLoad?
            SuspendLayout();

            // Create the "job" windows
            WLayout.RegisterLayout(this, new WndDrafting());
            WLayout.RegisterLayout(this, new WndNaturalness());
            WLayout.RegisterLayout(this, new WndBackTranslation());
            WLayout.RegisterLayout(this, new WndConsultantPreparation());

            // Let the window go ahead and proceed with the layout
            ResumeLayout();

			// Initialize the window state mechanism. We'll default to a full screen
			// the first time we are launched.
			m_WindowState = new JW_WindowState(this, true);

            // Create the Undo/Redo Stack
            m_URStack = m_Commands.SetupUndoRedoStack();

			// Initialize to a blank project (if there is a recent project
			// in the MRU, this will get overridden.)
			DB.Project = new DProject();
		}
		#endregion
		#region Method: void Dispose(...) - cleans up any resources being used
		protected override void Dispose( bool disposing )
			// Clean up any resources being used.
			// disposing - true to release both managed and unmanaged resources; false to
			//    release only unmanaged resources.
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion
		#region Windows Form Designer generated code

		private System.ComponentModel.IContainer components;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OurWordMain));
            this.m_Commands = new OurWord.Ctrls.Commands.CtrlCommands();
            this.m_Navigation = new OurWord.Ctrls.Navigation.CtrlNavigation();
            this.SuspendLayout();
            // 
            // m_Commands
            // 
            this.m_Commands.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Commands.BackColor = System.Drawing.Color.DarkGray;
            this.m_Commands.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_Commands.Location = new System.Drawing.Point(0, 0);
            this.m_Commands.Name = "m_Commands";
            this.m_Commands.Size = new System.Drawing.Size(413, 78);
            this.m_Commands.TabIndex = 4;
            // 
            // m_Navigation
            // 
            this.m_Navigation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Navigation.BackColor = System.Drawing.Color.DarkGray;
            this.m_Navigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_Navigation.Location = new System.Drawing.Point(409, 0);
            this.m_Navigation.Name = "m_Navigation";
            this.m_Navigation.Size = new System.Drawing.Size(333, 78);
            this.m_Navigation.TabIndex = 3;
            // 
            // OurWordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(742, 526);
            this.Controls.Add(this.m_Commands);
            this.Controls.Add(this.m_Navigation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OurWordMain";
            this.Text = "Our Word!";
            this.Load += new System.EventHandler(this.cmdLoad);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.cmdClosing);
            this.Resize += new System.EventHandler(this.cmdResize);
            this.ResumeLayout(false);

		}
		#endregion
		#region Attr{g}: static OurWordMain App
		public static OurWordMain App
		{
			get
			{
				return s_App;
			}
            set  // Used by NUnit tests; otherwise, stay away!
            {
                s_App = value;
            }
		}
		private static OurWordMain s_App = null;
		#endregion
        #region SMethod: bool _GrabTokenForThisInstance() - only run OurWord one-at-a-time
        private static Mutex s_EnsureOneInstanceOnlyMutex;

        static private bool _GrabTokenForThisInstance()
        {
            bool bMutexCreated;

            string sMutexID = "OneInstanceOnlyAllowedForOurWord";

            s_EnsureOneInstanceOnlyMutex = new Mutex(true, sMutexID, out bMutexCreated);

            if (!bMutexCreated)
            {
                return false;
            }

            return true;
        }
        #endregion
        #region Method: static void Main() - main entry point for the application
        [STAThread]
		static void Main(string[] vArgs)
		{
            // Initialize the registry (Needed by, e.g., Splash Screen and the
            // single-instance error message.)
            JW_Registry.RootKey = "SOFTWARE\\The Seed Company\\Our Word!";

            // We only want a single instance of OurWord to be running at a time, 
			// Because of the confusion that could reign if we had multiple 
            // instances modifying the Translation and Project settings file, 
            // we elect instead to just have a single instance.
            if (!_GrabTokenForThisInstance())
                return;

            // Initialize the Localizations Database. We need this prior to the
            // splash screen being activated.
            LocDB.Initialize(Loc.FolderOfLocFiles);

            // Set the resource location (so the splash picture will be visible)
            JWU.ResourceLocation = "OurWord.Res.";

            // Display a splash screen while we're loading. We want to retrieve everything
            // needed from the localization database, so that the SplashScreen, which runs
            // on its own thread, isn't having to go cross-thread to get them.
			foreach (var s in vArgs)
			{
				if (s == "-nosplash")
					SplashScreen.IsEnabled = false;
			}

            SplashScreen.Additional = G.GetLoc_Splash("sOptionalAdditional", "-");
            SplashScreen.StatusMessage = G.GetLoc_Splash("sLoadingOurWord", "Loading Our Word...");
            SplashScreen.Version = G.GetLoc_DialogCommon("m_lblVersion", "Version {0}", 
                new string[] { G.Version });
            SplashScreen.ProgramName = G.GetLoc_DialogCommon("m_lblProgramName", "Our Word!", null);
            SplashScreen.StatusBase = G.GetLoc_Splash("sLoading", "Loading {0}...");
            SplashScreen.Start();

            // First time we run, we create a shortcut. Since the first time is a result of being
            // called from the Setup program, the user should have a shortcut immediately after
            // installing.
            const string sShortcutCreated = "ShortcutCreated";
            if (JW_Registry.GetValue(sShortcutCreated, false) == false)
                (new Shortcut("OurWord")).CreateIfDoesntExist();
            JW_Registry.SetValue(sShortcutCreated, true);

            // Now start loading & run the program
            OurWordMain.s_App = new OurWordMain();
            Application.EnableVisualStyles();
            Application.Run(s_App);

            // All done, release the Mutex
            if (s_EnsureOneInstanceOnlyMutex != null)
               s_EnsureOneInstanceOnlyMutex.ReleaseMutex();
        }
		#endregion

		// Misc Methods ---------------------------------------------------------------------
		#region Method: void OnLeaveSection()
		private void OnLeaveSection()
		{
            // Update the filter (if a filter is active) in case this section no
            // longer matches it.
            UpdateFilterForCurrentSection();
		}
		#endregion
		#region Method: void OnEnterSection()
		private void OnEnterSection()
		{
		    m_Navigation.Setup(DB.TargetSection);

            // Load the data into the individual windows on the screen
            ResetWindowContents();

			// Set up the UI (menu, toolbars) to reflect the current section and
			// status (e.g., the Navigation toolbar buttons dropdown contents depends
			// upon which section is currently being displayed.)
            SetupMenusAndToolbarsVisibility();

            // Clear the Undo stack since we now have new data we're working with
            G.URStack.Clear();
        }
		#endregion
        #region Method: void OnLeaveProject()
        private void OnLeaveProject()
		{
			// Do everything we would normally do on leaving a section (e.g., getting
			// the data from the cache.)
			OnLeaveSection();

			// Prompt so that we save to disk
            DB.Project.Save(G.CreateProgressIndicator());

            // Dispose of the Dictionary object if necessary
            DB.Project.Dispose();
		}
		#endregion
		#region method: void OnEnterProject()
	    private void OnEnterProject()
		{
            // If we aren't already navigated to a book, then attempt to find one.
			DB.Project.Nav.GoToReasonableBook(G.CreateProgressIndicator());

            // Window contents, etc.
			OnEnterSection();
        }
		#endregion

        // Event Handlers --------------------------------------------------------------------
        #region Event: cmdLoad
        private void cmdLoad(object sender, System.EventArgs e)
		{
            // Init the Help system
            HelpSystem.Initialize(this, "OurWordHelp.chm");

            // Retrieve the most recent project (and go to the book-section), 
            // or load the Sample Project if the Most Recent is not available
            if (DProject.LoadMostRecentProject(G.CreateProgressIndicator()))
            {
                SetCurrentLayout(WLayout.GetLayoutFromRegistry(WndDrafting.c_sName));
            }
            else
            {
                DProject.LoadSampleProject();
                SetCurrentLayout(WndDrafting.c_sName);
            }
            m_Commands.SetLayout(WLayout.CurrentLayout.Name);

            // Set up the views, make the initial selection, etc.
            OnEnterProject();

			// Remember the previous placement of the window on the screen (Do this late
			// in the load sequence so that we avoid a multiple screen redraw.) 
            m_WindowState.StartMaximized = Users.Current.MaximizeWindowOnStartup;
			m_WindowState.RestoreWindowState();

            // Initial controls
            m_Commands.Width = m_Navigation.Left;
            cmdResize(null, null);

			// Initialize AutoSave Timer
			InitializeAutoSave();

            // Loading all done: Close the splash screen
			if (SplashScreen.IsEnabled)
			{
				if (SplashScreen.Wnd.InvokeRequired)
				{
					var d = new SplashScreen.StopSplashScreen_Callback(SplashScreen.Wnd.Stop);
					this.Invoke(d, new object[] { this });
				}
				else
				{
					SplashScreen.Wnd.Stop(this);
				}
			}

            // Leave everything in a state where the main window has focus, so that the
            // Text Selection will be appropriately flashing its readiness
            CurrentLayout.Focus();
        }
		#endregion

        #region Event: cmdClosing - save window state, data, etc.
        private void cmdClosing(object sender, FormClosingEventArgs e)
        {
			// Save data
			OnLeaveProject();

			// Save the current project's registry info so we can return to the same
            // place the next time OW is launched.
            DB.Project.SaveMostRecentRegistryInfo();
		
			// Save the window position
			m_WindowState.SaveWindowState();

			DB.TeamSettings.WriteToFile(G.CreateProgressIndicator());
        }
        #endregion
        #region Event: OnResize - prevent the size from becomming too small
        protected override void OnResize(EventArgs e)
            // The form is being resized, but internals are not yet set. So
            // we can't set our own internal sizes here; rather than must
            // be done in the cmdResize method, which is called after the
            // sizing is finished.
        {
            // We want to preserve a minimum size. We'll set this based on the OLPC spec,
            // which as of 6 Apr 07 at http://wiki.laptop.org/go/Hardware_specification 
            // is 800 x 600 in its smallest (color) mode. We will permit a size just 
            // smaller than this OLPC minimum.
            if (WindowState != FormWindowState.Minimized)
            {
                Width = Math.Max(Width, 770);
                Height = Math.Max(Height, 570);
            }

            base.OnResize(e);
        }
        #endregion
        #region Event: cmdResize - Adjust our controls to the window's new size
        private void cmdResize(object sender, EventArgs e)
            // The form has been resized and we now need to adjust our internal windows
            // to match the correct sizes
        {
            // Tests to see if we can proceed with layout and sizing
            if (null != G.App && G.App.WindowState == FormWindowState.Minimized)
                return;
            if (null == CurrentLayout)
                return;

            // the MainWindow needs to have a size for its double buffering
            CurrentLayout.Location = new Point(0, m_Commands.Height);
            CurrentLayout.SetSize(ClientRectangle.Width, ClientRectangle.Height - m_Commands.Height);

            // Re-do the layout and redraw
            CurrentLayout.DoLayout();
            CurrentLayout.Invalidate();
        }
        #endregion

        // Commands --------------------------------------------------------------------------
        #region cmd: cmdExit
        static void cmdExit()
        {
            Application.Exit();
        }
        #endregion
        #region cmd: cmdPrintBook
        private void cmdPrintBook()
		{
            Dim();;
			OnLeaveSection();

            var p = new Printer(DB.TargetSection);
			p.Do();

            UnDim();
        }
		#endregion
        #region cmd: cmdDownloadRepository

	    private string m_sClusterName;
	    private string m_sUserName;
	    private string m_sPassword;

        private void cmdDownloadRepository()
        {
            Dim();

            // Make sure the current project is saved and up-to-date, before we create
            // the new one. 
            OnLeaveProject();

            // To make life easier in case things fail, we'll remember what the user
            // entered in case he has to try again.
            var wiz = new WizInitializeFromRepository() 
            {
                InitialUserName = m_sUserName,
                InitialPassword = m_sPassword,
                InitialClusterName = m_sClusterName
            };

            // Get the user's settings for the to-be-downloaded cluster
            var wizResult = wiz.ShowDialog(this);
            m_sClusterName = wiz.ClusterName;
            m_sUserName = wiz.UserName;
            m_sPassword = wiz.Password;
            if (DialogResult.OK != wizResult)
                goto done;

            // The ClusterInfo tells us where the new cluster will be stored
            var sParentFolder = (wiz.IsInMyDocuments) ?
                JWU.GetMyDocumentsFolder(null) :
                JWU.GetLocalApplicationDataFolder(ClusterInfo.c_sLanguageDataFolder);
            var ci = new ClusterInfo(wiz.ClusterName, sParentFolder);

            // Attempt the clone
            var internetRepository = new InternetRepository(m_sClusterName,
                m_sUserName, m_sPassword)
            {
                Server = wiz.Url,
            };
            var localRepository = new LocalRepository(ci.ClusterFolder);
            var method = new Synchronize(localRepository, internetRepository, 
                Users.Current.UserName);
            var bSuccessful = method.CloneFromOther();

            // If an error, then abort
            if (!bSuccessful)
            {
                JWU.SafeFolderDelete(ci.ClusterFolder);
                goto done;
            }

            // Open the first project we find
            var vsProjects = ci.GetClusterLanguageList(true);
            if (null != vsProjects && vsProjects.Count > 0)
            {
                var sPath = ci.GetProjectPath(vsProjects[0]);
                DB.Project = new DProject();
                DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
                DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
                Users.Current = Users.Observer;
                OnEnterProject();
            }

            // Display a dialog declaring Success
            LocDB.Message("msgRepoCreated",
                "The Cluster has been successfully downloaded to your computer.",
                null, LocDB.MessageTypes.Info);

            done:
                UnDim();
        }
        #endregion
        #region cmd: cmdHistory
        private void cmdHistory()
        {
            Dim();
            var history = new DlgHistory(DB.TargetSection);
            history.ShowDialog(this);
            UnDim();
        }
        #endregion
        #region cmd: cmdSwitchLayout
        void cmdSwitchLayout(string sNewLayoutName)
        {
            SetCurrentLayout(sNewLayoutName);
            G.URStack.Clear();
            cmdResize(null,null);
        }
        #endregion
        #region cmd: cmdChangeZoomPercent
        void cmdChangeZoomPercent(int nNewZoomPercent)
        {
            if (Users.Current.ZoomPercent == nNewZoomPercent) 
                return;

            Users.Current.ZoomPercent = nNewZoomPercent;
            OnEnterSection();
        }
        #endregion
        #region cmd: cmdInsertNote
        private void cmdInsertNote()
        {
            // If the Main Window is not focused, we don't have a context for inserting.
            if (null == CurrentLayout || !CurrentLayout.Focused)
                return;

            // If we don't have a selection, we inform the user; as we want our notes to be
            // about something (and it gives us an initial title for our annotation.)
            var selection = CurrentLayout.Selection;
            if (null == selection || !selection.IsContentSelection)
            {
                LocDB.Message(
                    "msgNeedTextSelectionForInsertNote",
                    "Please select the text for which your note will be about.",
                    null,
                    LocDB.MessageTypes.Warning);
                return;
            }

            // Perform the undoable action
            var action = new InsertNoteAction(CurrentLayout, TranslatorNote.General);
            action.Do();

            // Launch the tooltip window for the new annotation
            var vParas = CurrentLayout.Contents.AllParagraphs;
            foreach (var owp in vParas)
            {
                foreach (var item in owp.SubItems)
                {
                    var en = item as ENote;
                    if (null != en && en.Note == action.Note)
                    {
                        en.LaunchToolTip();
                    }
                }
            }
        }
        #endregion

        // Project
        #region cmd: cmdCreateProject
        private void cmdCreateProject()
        {
            Dim();

            // Walk through the wizard; we do nothing unless the User makes it through
            // (as signaled by DialogResult.OK).
            var wiz = new Dialogs.WizNewProject.WizNewProject();
            if (DialogResult.OK != wiz.ShowDialog())
                goto done;

            // Make sure the current project is saved and up-to-date, before we create
            // the new one. 
            OnLeaveProject();

            // Create and initialize the new project according to the settings
            var project = new DProject(wiz.ProjectName)
            {
                TeamSettings = new DTeamSettings(wiz.ChosenCluster.Name)
            };

            // Team Settings: start with the factory default; load over it if a file already exists,
            // otherwise create the new cluster
            DTeamSettings.EnsureInitialized();
            project.TeamSettings.InitialCreation(G.CreateProgressIndicator());
            StyleSheet.Initialize(null);

            // Create the front translation. If the settings file exists, load it; otherwise
            // create its folder, settings file, etc.
            project.FrontTranslation = new DTranslation(wiz.FrontName);
            project.FrontTranslation.Initialize();

            // Target Translation
            project.TargetTranslation = new DTranslation(wiz.ProjectName);
            project.TargetTranslation.Initialize();

            // Set OW to this project
            DB.Project = project;

            // Save everything
            DB.Project.WriteToFile(G.CreateProgressIndicator());
            DB.TargetTranslation.WriteToFile(G.CreateProgressIndicator());

            // Update the UI, views, etc
            DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
            OnEnterProject();

            // Edit properties?
            if (wiz.LaunchPropertiesDialogWhenDone)
                cmdConfigure();

        done:
            UnDim();
        }
		#endregion
        #region cmd: cmdOpenProject
        private void cmdOpenProject(string sPathToProjectFile)
        {
            var sPath = sPathToProjectFile;

            OnLeaveProject();
            DB.Project = new DProject();
            DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
            DB.Project.Nav.GoToFirstAvailableBook(G.CreateProgressIndicator());
            OnEnterProject();
        }
        #endregion
        #region cmd: cmdSaveProject
        void cmdSaveProject()
        {
            OnLeaveSection();
            DB.Project.Save(G.CreateProgressIndicator());
        }
        #endregion
        #region cmd: cmdExportTranslation
        private void cmdExportTranslation()
        {
            var method = new ExportTranslation(DB.TargetTranslation);
            if (!method.CanExportTranslation)
                return;

            Dim();

            method.Do(this);

            UnDim();
        }
        #endregion

        // Editing
        #region Cmd: cmdUndo
        private void cmdUndo()
        {
            if (Users.Current.CanUndoRedo)
                URStack.Undo();
        }
        #endregion
        #region Cmd: cmdRedo
        private void cmdRedo()
        {
            if (Users.Current.CanUndoRedo)
                URStack.Redo();
        }
        #endregion
        #region Cmd: cmdCut
        private void cmdCut()
		{
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdCut();
		}
		#endregion
		#region Cmd: cmdCopy
        private void cmdCopy()
		{
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdCopy();
		}
		#endregion
		#region Cmd: cmdPaste
        private void cmdPaste()
		{
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdPaste();
		}
		#endregion
		#region Cmd: cmdItalic
        private void cmdItalic()
		{
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdToggleItalics();
		}
		#endregion
        #region Cmd: cmdChangeParagraphStyle
        private void cmdChangeParagraphStyle(ParagraphStyle newStyle)
        {
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdChangeParagraphTo(newStyle);
        }
        #endregion
        #region Cmd: cmdInsertFootnote
        private void cmdInsertFootnote()
        {
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdInsertFootnote();
        }
        #endregion
        #region Cmd: cmdDeleteFootnote
        private void cmdDeleteFootnote()
        {
            var wnd = FocusedWindow;
            if (null != wnd)
                wnd.cmdDeleteFootnote();
        }
        #endregion
        #region Cmd: cmdCopyBTFromFront
        private void cmdCopyBTFromFront()
        {
            // We only do this in select views
            if (!WLayout.CurrentLayoutIs(new[] {
                WndBackTranslation.c_sName, 
                WndConsultantPreparation.c_sName }))
            {
                return;
            }

            OnLeaveSection();

            (new CopyBtFromFrontMethod()).Run();

            OnEnterSection();
        }
        #endregion

        // Tools
        #region Cmd: cmdSendReceive
        private void cmdSendReceive()
        {
            Dim();

            // Save everything, but don't commit, cause Synchronize will do a commit
            DB.Project.Nav.SavePositionToRegistry();
            OnLeaveProject();

            // Do the Synchronize
            var local = DB.TeamSettings.GetLocalRepository();
            var remote = DB.TeamSettings.GetInternetRepository();
            var synch = new Synchronize(local, remote, Users.Current.UserName);
            using (new Chorus.Utilities.ShortTermEnvironmentalVariable("OurWordExeVersion", G.Version))
                synch.SynchLocalToOther();

            // We have to unload, then reload everything
            var sPath = DB.Project.StoragePath;
            DB.Project = new DProject();
            DB.Project.LoadFromFile(ref sPath, G.CreateProgressIndicator());
            DB.Project.Nav.RetrievePositionFromRegistry(G.CreateProgressIndicator());
            OnEnterProject();

            UnDim();
        }
        #endregion
        #region Cmd: cmdCheckForUpdates
        private void cmdCheckForUpdates()
        {
            DB.Project.Nav.SavePositionToRegistry();
            OnLeaveProject();

            // Invoke
            var checkForUpdateMethod = new InvokeCheckForUpdates
            {
                QuietMode = false
            };
            
            checkForUpdateMethod.Do(this);
        }
        #endregion
		#region Cmd: cmdConfigure
        private void cmdConfigure()
		{
            if (!Users.HasAdministrator)
            {
                Messages.NeedAdministrator();
                return;
            }

            Dim();

            // An administrator login is required to access the Configuration
            if (!Users.Current.IsAdministrator)
            {
                var dlgAuthenticate = new DlgAdministratorLogin();
                var bAuthenticated = (DialogResult.OK == dlgAuthenticate.ShowDialog(this));
                if (!bAuthenticated)
                {
                    UnDim();
                    return;
                }
            }


            // Retrieve data and save the project to disk. We don't know if the
			// user might remove the book from the project, so we need to make sure
			// it was saved just in case.
			OnLeaveProject();

            // Let the user change the properties
            var dlg = new DialogProperties();
            dlg.ShowDialog(this);
            UnDim();

            // Re-initialize everything
			OnEnterProject();
		}
		#endregion
		#region Can: canIncrementBookStatus
        private static bool canIncrementBookStatus
		{
			get
			{
				if (null == DB.Project)
					return false;
				if (null == DB.Project.FrontTranslation)
					return false;
				if (null == DB.Project.TargetTranslation)
					return false;
				if (null == DB.Project.Nav.STarget)
					return false;
				return true;
			}
		}
		#endregion
        #region Cmd: cmdIncrementBookStatus
        private void cmdIncrementBookStatus()
		{
			// Check that we can increment (we need to have a valid project)
			if (!canIncrementBookStatus)
				return;

			// Perform the dialog to get the increment desired
			var book = DB.Project.STarget.Book;
			var dlg = new IncrementBookStatus(book);
            Dim();
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                UnDim(); 
                return; 
            }

			// If we incremented, then we need to save the file, so that it will be
			// stored on the disk under the new filename (and thus synchronized with
			// what is in the Project. (See Bug0084)
            book.DeclareDirty();
			cmdSaveProject();
            UnDim();

            // Make sure the UI updates to show the correct file name
            ResetWindowContents();

            G.URStack.Clear();
		}
		#endregion
		#region Cmd: cmdRestoreBackup
        private void cmdRestoreBackup()
		{
			if (!canIncrementBookStatus)
				return;

			var BTarget = DB.Project.STarget.Book;

            Dim();
			var dlg = new DialogRestoreFromBackup(BTarget);
			if (DialogResult.OK == dlg.ShowDialog())
			{
				OnLeaveProject();

				DBook.RestoreFromBackup(BTarget, dlg.BackupPathName, G.CreateProgressIndicator());

				DB.Project.Nav.GoToFirstSection();
				OnEnterSection();
			}
            UnDim();
		}
		#endregion
        #region Cmd: cmdLocalizer
        private void cmdLocalizer()
        {
            Dim();

            OnLeaveProject();

            var dlg = new Localizer(LocDB.DB);
            if (DialogResult.OK == dlg.ShowDialog())
                OnEnterProject();

            UnDim();
        }
        #endregion
        #region Cmd: cmdHelpTopics
        private void cmdHelpTopics()
		{
			HelpSystem.ShowDefaultTopic();
		}
		#endregion
		#region Cmd: cmdAbout
        private void cmdAbout()
		{
            Dim();
			var dlg = new DialogHelpAbout();
			dlg.ShowDialog(this);
            UnDim();
		}
		#endregion

        // Users
        #region cmd: cmdAddNewUser
        void cmdAddNewUser()
        {
            var dlg = new DlgAddNewUser();
            if (DialogResult.OK != dlg.ShowDialog(G.App))
                return;

            var newUser = dlg.InitializeAs.Clone();
            newUser.UserName = dlg.FullName;
            newUser.Password = dlg.Password;
            newUser.NoteAuthorsName = dlg.FullName;

            Users.Add(newUser);

            Users.Current = newUser;

            OnEnterProject();
        }
        #endregion
        #region cmd: cmdChangeUser
        private static void cmdChangeUser(User newUser)
        {
            // If the new user is the same as the current, then there's nothing to do
            if (newUser.UserName == Users.Current.UserName)
                return;

            // Determine if we need to ask for the password. We don't if
            // the current user is an administrator; or if we are changing
            // to the Observer user.
            var bNeedPasswordCheck = true;
            if (newUser == Users.Observer)
                bNeedPasswordCheck = false;
            if (Users.Current.IsAdministrator)
                bNeedPasswordCheck = false;

            // Request password
            if (bNeedPasswordCheck)
            {
                var dlgPassword = new DlgPassword { UserName = newUser.UserName };
                if (DialogResult.OK != dlgPassword.ShowDialog(G.App))
                    return;
                if (!newUser.VerifyPassword(dlgPassword.Password))
                    return;
            }

            Users.Current = newUser;
            G.App.OnEnterProject();
        }
        #endregion

        // Navigation
        #region cmd: cmdGoToChapter(nChapterNumber)
        void cmdGoToChapter(int nChapterNumber)
        {
            var section = DB.TargetBook.GetSectionContainingChapter(nChapterNumber);
            if (null == section)
                return;

            var iSection = DB.TargetBook.Sections.FindObj(section);
            Debug.Assert(-1 != iSection);
                
            // Navigate to that section
            OnLeaveSection();
            DB.Project.Nav.GoToSection(iSection);
            OnEnterSection();
        }
        #endregion
        #region cmd: cmdGoToBook(sBookAbbrev)
        void cmdGoToBook(string sBookAbbrev)
        {
            Debug.Assert(!string.IsNullOrEmpty(sBookAbbrev));

            OnLeaveSection();
            DB.Project.Save(G.CreateProgressIndicator());
            DB.Project.Nav.GoToBook(sBookAbbrev, G.CreateProgressIndicator());
            OnEnterSection();
        }
        #endregion
        #region cmd: cmdGoToSection(section)
        void cmdGoToSection(DSection section)
        {
            if (section == DB.TargetSection)
                return;

            var iSection = DB.TargetBook.Sections.FindObj(section);
            Debug.Assert(-1 != iSection);

            OnLeaveSection();
            DB.Project.Nav.GoToSection(iSection);
            OnEnterSection();
        }
        #endregion



        // FILTERS need to rework --------------

		// We make the dialog static, so that its settings persist throughout the OW
		// session. (We don't want to save them beyond, thus we don't write them
		// to the registry or anywhere.)
		static DialogFilter m_dlgFilter = null;

		#region Method: bool FilterTest(DSection section) - perform the test on a given section
		public bool FilterTest(DialogFilter dlg, DSection section)
		{
			if (null == m_dlgFilter)
				return false;

			return section.FilterTest( 
				m_dlgFilter.OneOnly, 
				m_dlgFilter.Filter_VernacularText, m_dlgFilter.Filter_VernacularSearchString,
				m_dlgFilter.Filter_FrontText,      m_dlgFilter.Filter_FrontSearchString,
				m_dlgFilter.Filter_VernacularBT,   m_dlgFilter.Filter_VernacularBTSearchString,
				m_dlgFilter.Filter_UntranslatedText,
				m_dlgFilter.Filter_MismatchedQuotes,
				m_dlgFilter.Filter_PictureWithCaption, 
				m_dlgFilter.Filter_ParagraphHasQuote,
				m_dlgFilter.Filter_PunctuationProblem,
				m_dlgFilter.Filter_PictureCannotBeLocatedOnDisk);
		}
		#endregion
		#region Method: void UpdateFilterForCurrentSection()
		void UpdateFilterForCurrentSection()
		{
			// Don't bother if there is no target section, no filter, etc.
			if (null == DB.TargetSection)
				return;
			if (!DSection.FilterIsActive)
				return;
			if (!DB.TargetSection.MatchesFilter)
				return;

			// Run the test on the current section to update it.
			DB.Project.STarget.MatchesFilter = FilterTest(m_dlgFilter, DB.Project.STarget);

			// If the filter no longer matches this section...
			if (!DB.Project.STarget.MatchesFilter)
			{
				// Are there any other sections that still match the filter?
				bool bNoMatches = true;
				foreach(DSection section in DB.TargetBook.Sections)
				{
					if (section.MatchesFilter)
						bNoMatches = false;
				}

				// If not, then we must turn off filters
				if (bNoMatches)
				{
					DB.TargetBook.RemoveFilter();
				}
			}

			// Update the navigation bar
            SetupMenusAndToolbarsVisibility();
		}
		#endregion


    }

	#region CLASS G - Globals for convenient access
	public static class G
	{
        // Stuff still in OurWordMain
        #region Attr{g}: OurWordMain App
        static public OurWordMain App
        {
            get
            {
                return OurWordMain.App;
            }
        }
        #endregion
        #region SAttr{g}:  UndoRedoStack URStack
        static public UndoRedoStack URStack
        {
            get
            {
                return App.URStack;
            }
        }
        #endregion

        // Status Bar ------------------------------------------------------------------------
        #region SMethod: IProgressIndicator CreateProgressIndicator()
        static public IProgressIndicator CreateProgressIndicator()
        {
            if (SplashScreen.IsShowing)
                return new SplashProgress();

            return new NullProgress();
        }
        #endregion

		// Browse Directory ------------------------------------------------------------------
		#region Attr{g/s}: string BrowseDirectory - last navigated-to directory (runtime only)
		public static string BrowseDirectory
		{
			get
			{
				if (s_sBrowseDirectory.Length == 0)
					s_sBrowseDirectory = DB.TeamSettings.ClusterFolder;
				return s_sBrowseDirectory;
			}
			set
			{
				s_sBrowseDirectory = value;
			}
		}
		static string s_sBrowseDirectory = "";
		#endregion
        #region SAttr{g}: string FolderOfExe
        static public string FolderOfExe
        {
            get
            {
                string sPathOfExe;
                var bUnitTesting = Assembly.GetEntryAssembly() == null;
                if (bUnitTesting)
                {
                    sPathOfExe = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
                    sPathOfExe = Uri.UnescapeDataString(sPathOfExe);
                }
                else
                {
                    sPathOfExe = Assembly.GetExecutingAssembly().Location;
                }
                Debug.Assert(!string.IsNullOrEmpty(sPathOfExe));

                var sFolderOfExe = Path.GetDirectoryName(sPathOfExe);
                return sFolderOfExe;
            }
        }
        #endregion

        // Misc ------------------------------------------------------------------------------
        #region SAttr{g}: bool IsLinux
        static public bool IsLinux
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    return true;
                return false;
            }
        }
        #endregion
        #region SAttr{g}: string Version
        static public string Version
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return JWU.BuildFriendlyVersion(v);
            }
        }
        #endregion
        #region SAttr{g}: BookGroups BookGroups
        static public readonly BookGroups BookGroups = new BookGroups();
        #endregion

        // LocDB strings - access to various Groups in the LocDB -----------------------------
        #region SMethod: string GetLoc_String(sItemID, sEnglish) -        "Strings" 
        static public string GetLoc_String(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_GeneralUI(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_GeneralUI(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "GeneralUI" },
                sItemId, 
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_Files(sItemID, sEnglish) -         "Strings\Files"
        static public string GetLoc_Files(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "Files" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_UndoRedo(sItemID, sEnglish) -      "Strings/UndoRedo"
        static public string GetLoc_UndoRedo(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "Strings", "UndoRedo" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
        #region SMethod: string GetLoc_DialogCommon(sItemID, sEnglish, vsIns) -  "Strings\DialogCommon"
        static public string GetLoc_DialogCommon(string sItemId, string sEnglish, string[] vsInsert)
        {
            return LocDB.GetValue(
                new[] { "Strings", "DialogCommon" },
                sItemId,
                sEnglish,
                null,
                vsInsert);
        }
        #endregion
        #region SMethod: string GetLoc_Splash(sItemID, sEnglish) -     "Strings\GeneralUI"
        static public string GetLoc_Splash(string sItemId, string sEnglishDefault)
        {
            return LocDB.GetValue(
                new[] { "SplashScreen" },
                sItemId,
                sEnglishDefault,
                null,
                null);
        }
        #endregion
    }
	#endregion

    public class SplashProgress : IProgressIndicator
    {
        #region Method: void Start(string sMessage, int nCount)
        public void Start(string sMessage, int nCount)
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.SetStatus_Callback(SplashScreen.Wnd.SetStatus);
                    G.App.Invoke(d, new object[] { sMessage, nCount });
                }
                else
                {
                    SplashScreen.Wnd.SetStatus(sMessage, nCount);
                }
            }
        }
        #endregion
        #region Method: void Step()
        public void Step()
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.IncrementProgress_Callback(SplashScreen.Wnd.IncrementProgress);
                    G.App.Invoke(d, new object[] { });
                }
                else
                {
                    SplashScreen.Wnd.IncrementProgress();
                }
            }
        }
        #endregion
        #region Method: void End()
        public void End()
        {
            if (SplashScreen.IsShowing)
            {
                if (SplashScreen.Wnd.InvokeRequired)
                {
                    var d = new SplashScreen.ResetProgress_Callback(SplashScreen.Wnd.ResetProgress);
                    G.App.Invoke(d, new object[] { });
                }
                else
                {
                    SplashScreen.Wnd.ResetProgress();
                }
            }
        }
        #endregion
    }


}






