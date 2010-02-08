#region ***** EnumeratedStepsProgressDlg.cs *****
/**********************************************************************************************
 * Project: Our Word!
 * File:    EnumeratedStepsProgressDlg.cs
 * Author:  John Wimbish
 * Created: 10 Nov 2009
 * Purpose: Show progress to the user during a process which is of indeterminate duration
 * Legal:   Copyright (c) 2005-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace JWTools
{
    public partial class EnumeratedStepsProgressDlg : Form
    {
        // Steps -----------------------------------------------------------------------------
        #region Embedded Class Step
        class Step
        {
            public const int RowHeight = 22;
            readonly string ID;
            readonly string LocalizedName;
            public readonly Label LabelControl;
            public readonly PictureBox PictureControl;

            #region Constructor(sEnglishName, yTop, nStepNumber)
            public Step(string sEnglishName, int yTop, int nStepNumber)
            {
                ID = GetIdFor(sEnglishName);
                LocalizedName = GetLocalizationFor(ID, sEnglishName);

                var y = yTop + nStepNumber * RowHeight;

                PictureControl = new PictureBox
                {
                    Name = "m_picture" + ID,
                    Location = new Point(10, y),
                    Size = new Size(16, 16),
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    TabStop = false,
                    TabIndex = nStepNumber * 2 + 101,
                    Tag = ID
                };

                LabelControl = new Label
                {
                    Name = "m_label" + ID,
                    Text = LocalizedName, 
                    Location = new Point(30, y),
                    Size = new Size(315, RowHeight),
                    TextAlign = ContentAlignment.TopLeft,
                    TabStop = false,
                    TabIndex = nStepNumber * 2 + 100,
                    Tag = ID
                };
            }
            #endregion
        }
        #endregion
        private List<Step> m_Steps;
        static private string[] s_StepStrings;
        private int m_CurrentStep;
        #region Attr{g}: bool IsValidStep
        bool IsValidStep
        {
            get
            {
                return (m_CurrentStep >= 0 && m_CurrentStep < m_Steps.Count);
            }
        }
        #endregion
        #region Attr{g}: PictureBox CurrentPicture
        PictureBox CurrentPicture
        {
            get
            {
                return m_Steps[m_CurrentStep].PictureControl;
            }
        }
        #endregion

        // Progress --------------------------------------------------------------------------
        #region SMethod: void IncrementStep()
        static public void IncrementStep()
        {
            if (DisableForTesting)
                return;

            // Set the old step to Completed
            if (s_Dialog.IsValidStep)
                s_Dialog.SetPicture(s_Dialog.CurrentPicture, PictureState.Complete);

            s_Dialog.m_CurrentStep++;

            // Set the new step to InProcess
            if (s_Dialog.IsValidStep)
                s_Dialog.SetPicture(s_Dialog.CurrentPicture, PictureState.InProcess);
        }
        #endregion
        #region SMethod: void Fail(sId, sMessage)
        static public void Fail(string sId, string sMessage)
        {
            if (DisableForTesting)
                return;

            var sLocalizedMessage = LocDB.GetValue(
                    new[] { "Strings", "ProgressMessages" },
                    sId,
                    sMessage,
                    null,
                    null);
            Fail(sLocalizedMessage);
        }
        #endregion
        #region SMethod: void Fail(sLocalizedMessage)
        static public void Fail(string sLocalizedMessage)
        {
            if (DisableForTesting)
                return;

            if (!s_Dialog.IsValidStep)
                return;

            s_Dialog.SetPicture(s_Dialog.CurrentPicture, PictureState.Failure);

            s_Dialog.ShowFailure(sLocalizedMessage);         
        }
        #endregion
        #region Method: void ShowFailure(string sMessage)
        delegate void ShowFailureDelegate(string sMessage);
        void ShowFailure(string sMessage)
        {
            if (InvokeRequired)
            {
                var d = new ShowFailureDelegate(ShowFailure);
                Invoke(d, new object[] { sMessage });
            }
            else
            {
                // Make the error stuff visible
                m_ErrorPanel.Visible = true;
                m_labelError.Text = sMessage;
                m_labelError.Visible = true;
                m_bOK.Visible = true;
                m_ErrorIcon.Visible = true;

                // Make the progress stuff hidden
                m_progressBar.Visible = false;

                // Highlight the step we failed on
                m_Steps[m_CurrentStep].LabelControl.ForeColor = Color.Navy;
                m_Steps[m_CurrentStep].LabelControl.Font =
                    new Font(m_Steps[m_CurrentStep].LabelControl.Font, FontStyle.Bold);

                // Resize and relocate
                m_ErrorPanel.Top = m_progressBar.Top;
                m_bOK.Top = m_ErrorPanel.Bottom + 10;
                var nDifference = m_progressBar.Bottom - m_bOK.Bottom;
                Height -= nDifference;
                TopMost = true;
            }
        }
        #endregion

        // Pictures --------------------------------------------------------------------------
        enum PictureState { Pending, InProcess, Complete, Failure };
        #region Method: void SetPicture(PictureBox, newPictureState)
        delegate void SetPictureDelegate(PictureBox pict, PictureState newPictureState);
        void SetPicture(PictureBox pict, PictureState newPictureState)
        {
            if (InvokeRequired)
            {
                var d = new SetPictureDelegate(SetPicture);
                Invoke(d, new object[] { pict, newPictureState });
            }
            else
            {
                switch (newPictureState)
                {
                    case PictureState.Pending:
                        pict.Image = JWU.GetBitmap("BlueDot.ico");
                        break;
                    case PictureState.InProcess:
                        pict.Image = JWU.GetBitmap("Project.ico");
                        break;
                    case PictureState.Complete:
                        pict.Image = JWU.GetBitmap("Check.ico");
                        break;
                    case PictureState.Failure:
                        pict.Image = JWU.GetBitmap("Failure.ico");
                        break;
                }
            }
        }
        #endregion

        // Localization ----------------------------------------------------------------------
        #region Method: string GetLocalizationFor(sEnglish)
        static string GetLocalizationFor(string sId, string sEnglish)
        {
            return LocDB.GetValue(
                    new[] { "Strings", "ProgressSteps" },
                    sId,
                    sEnglish,
                    null,
                    null);
        }
        #endregion
        #region Method: string GetIdFor(sEnglish)
        static string GetIdFor(string sEnglish)
        {
            return sEnglish.Replace(" ", "");
        }
        #endregion

        // Construction ----------------------------------------------------------------------
        #region Method: void LoadSteps()
        void LoadSteps()
        {
            m_Steps = new List<Step>();

            var yTop = m_ProcessName.Bottom + 10;

            SuspendLayout();
            var nStepNumber = 0;
            foreach (var sEnglishStepName in s_StepStrings)
            {
                var step = new Step(sEnglishStepName, yTop, nStepNumber++);
                m_Steps.Add(step);

                SetPicture(step.PictureControl, PictureState.Pending);

                Controls.Add(step.PictureControl);
                Controls.Add(step.LabelControl);

                step.LabelControl.Size = new Size(m_progressBar.Right, step.LabelControl.Height);
            }
            ResumeLayout(); 
           
            m_CurrentStep = -1;
        }
        #endregion
        #region Method: void SetDialogHeight()
        void SetDialogHeight()
        {
            if (m_Steps.Count == 0)
                return;

            var yLastStepBottom = m_Steps[m_Steps.Count - 1].LabelControl.Bottom;

            var nBottomMargin = Bottom - m_progressBar.Bottom;
            const int nMarginBeforeProgressBar = (int)(Step.RowHeight * 1.2);

            var nDesiredClientHeight = yLastStepBottom + nMarginBeforeProgressBar +
                m_progressBar.Height + nBottomMargin;
            var nCurrentClientHeight = ClientRectangle.Height;
            var nDifference = nDesiredClientHeight - nCurrentClientHeight;

            Height += nDifference;
        }
        #endregion
        #region Constructor()
        public EnumeratedStepsProgressDlg()
        {
            InitializeComponent();

            // Hidden controls
            m_ErrorPanel.Visible = false;
            m_labelError.Visible = false;
            m_bOK.Visible = false;
            m_ErrorIcon.Visible = false;
            m_ErrorIcon.Image = SystemIcons.Warning.ToBitmap();

            m_ProcessName.Text = GetLocalizationFor(GetIdFor(s_ProcessTitle), s_ProcessTitle);

            LoadSteps();

            SetDialogHeight();
        }
        #endregion

        // Startup / Shutdown ----------------------------------------------------------------
        static public bool DisableForTesting { private get; set; }
        static private EnumeratedStepsProgressDlg s_Dialog;
        private static string s_ProcessTitle;
        #region SMethod: public void Start(string[] steps)
        static public void Start(string sProcessTitle, string[] stepStrings)
        {
            if (DisableForTesting)
                return;

            s_ProcessTitle = sProcessTitle;
            s_StepStrings = stepStrings;
            s_Dialog = null;

            var t = new Thread(StartDialog)
            {
                IsBackground = true,
                Name = "Synchronize Progress"
            };
            //t.SetApartmentState(ApartmentState.STA);
            t.Start();

            while (null == s_Dialog)
                Thread.Sleep(500);
            Thread.Sleep(2000);
        }
        static private void StartDialog()
        {
            s_Dialog = new EnumeratedStepsProgressDlg();
            Application.Run(s_Dialog);
        }
        #endregion
        #region SMethod: void Stop()
        delegate void StopDelegate();
        static public void Stop()
        {
            if (DisableForTesting)
                return;

            if (s_Dialog.InvokeRequired)
            {
                var d = new StopDelegate(Stop);
                s_Dialog.Invoke(d);
            }
            else
            {
                s_Dialog.Close();
                DisableForTesting = false;
            }
        }
        #endregion
        #region Cmd: OnDone
        private void OnDone(object sender, System.EventArgs e)
        {
            Stop();
        }
        #endregion
    }
}
