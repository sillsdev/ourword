#region *** DlgTscReport.cs ***
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;
using TRIS.FormFill.Lib;
#endregion

// This is totally based on the existence of the code here, which permits merging 
// into a Word document.
// http://www.codeproject.com/KB/office/Fill_Mergefields.aspx?fid=1544578&fr=1&df=90&mpp=25&noise=3&sort=Position&view=Quick#xx0xx

namespace OurWord.Dialogs.TscReport
{
    public partial class DlgTscReport : Form
    {
        readonly TscGoals m_Goals = new TscGoals();

        #region Constructor()
        public DlgTscReport()
        {
            InitializeComponent();

            m_vQuarters = new List<Quarter> {
                Quarter.CreateFromDate(DateTime.Now),
                Quarter.CreateFromDate(DateTime.Now.AddMonths(3)),
                Quarter.CreateFromDate(DateTime.Now.AddMonths(-3))
           };
        }
        #endregion

        // Merge into Word Document ----------------------------------------------------------
        #region method: void FillInProjectInformation(Dictionary values)
        void FillInProjectInformation(Dictionary<string, string> values)
        {
            values[@"OW_PROJECT_NAME"] = DB.Project.DisplayName;
            values[@"OW_PROJECT_LOCATION"] = DB.Project.TeamSettings.DisplayName;
            values[@"OW_DATE_OF_REPORT"] = DateTime.Today.ToLongDateString();
            values[@"OW_LANGUAGE_NAME"] = DB.Project.DisplayName;

            values["OW_QTR_MONTHS"] = Current.ToString();

            // Form Closing will save these into the DB; but for now, read them 
            // from the dialog controls
            values[@"OW_COMPLETION"] = m_cScheduledCompletion.Text;
            values[@"OW_REPORT_WRITER"] = m_tYourName.Text;
        }
        #endregion
        #region smethod: DataTable FillInBookChart()
        DataTable FillInBookChart()
        {
            var table = new DataTable("C");
            table.Columns.Add("BOOK");
            table.Columns.Add("VERSES");
            table.Columns.Add("DRAFTED");
            table.Columns.Add("TEAM");
            table.Columns.Add("COMM");
            table.Columns.Add("BT");
            table.Columns.Add("CONS");
            table.Columns.Add("PUBLISH");

            foreach(var book in DB.TargetTranslation.BookList)
            {
                var sEnglishName = BookNames.English[book.BookIndex];

                var info = G.BookGroups.FindBook(book.BookAbbrev);
                var sVerses = info.VerseCount.ToString();

                // Default to blanks
                var sFirstDraft = " ";
                var sTeamCheck = " ";
                var sCommunityCheck = " ";
                var sBackTranslation = " ";
                var sConsultantCheck = " ";
                var sPublish = " ";

                // Add checkmarks according to the OurWord understanding of what's been done
                const string sDone = "√";
                foreach(var stage in DB.TeamSettings.Stages)
                {
                    if (stage.ID == Stage.c_idDraft)
                        sFirstDraft = sDone;
                    else if (stage.ID == Stage.c_idTeamCheck)
                        sTeamCheck = sDone;
                    else if (stage.ID == Stage.c_idCommunityCheck)
                        sCommunityCheck = sDone;
                    else if (stage.ID == Stage.c_idBackTranslation)
                        sBackTranslation = sDone;
                    else if (stage.ID == Stage.c_idConsultantCheck)
                        sConsultantCheck = sDone;
                    else if (stage.ID == Stage.c_idFinalForPrinting)
                        sPublish = sDone;

                    if (stage == book.Stage)
                        break;
                }

                // Account for the current set of goals
                var vRelevantGoals = m_Goals.GetGoalsForBook(book.BookAbbrev);
                foreach (var goal in vRelevantGoals)
                {
                    var stage = DB.TeamSettings.Stages.Find(StageList.FindBy.EnglishName, 
                        goal.EnglishStage);

                    var sGoal = "";
                    if (goal.DoneThisQuarter == TscGoal.Activity.Completed)
                        sGoal = "Done This Qtr";
                    else if (goal.DoneThisQuarter == TscGoal.Activity.PartyDone)
                    {
                        sGoal = "Begun This Qtr";
                        if (goal.PlannedNextQuarter)
                            sGoal += ", Continues Next Qtr";
                    }
                    else if (goal.PlannedNextQuarter)
                        sGoal = "Planned Next Qtr";

                    if (stage.ID == Stage.c_idDraft)
                        sFirstDraft = sGoal;
                    else if (stage.ID == Stage.c_idTeamCheck)
                        sTeamCheck = sGoal;
                    else if (stage.ID == Stage.c_idCommunityCheck)
                        sCommunityCheck = sGoal;
                    else if (stage.ID == Stage.c_idBackTranslation)
                        sBackTranslation = sGoal;
                    else if (stage.ID == Stage.c_idConsultantCheck)
                        sConsultantCheck = sGoal;
                    else if (stage.ID == Stage.c_idFinalForPrinting)
                        sPublish = sGoal;
                }

                table.Rows.Add(new[] { sEnglishName, sVerses, sFirstDraft, sTeamCheck, 
                    sCommunityCheck, sBackTranslation, sConsultantCheck, sPublish });
            }


            return table;
        }
        #endregion
        #region Method: DataTable FillInOutcomes()
        DataTable FillInOutcomes()
        {
            var table = new DataTable("OUTCOMES");
            table.Columns.Add("GOAL");
            table.Columns.Add("EFFECT");

            foreach (var goal in m_Goals)
            {
                if (goal.DoneThisQuarter == TscGoal.Activity.Completed ||
                    goal.DoneThisQuarter == TscGoal.Activity.PartyDone)
                {
                    var sGoal = string.Format("{0}: {1}", goal.EnglishBookName, goal.EnglishStage);
                    table.Rows.Add(new[] { sGoal, "" });
                }
            }

            return table;
        }
        #endregion
        #region Method: DataTable FillInProblems()
        DataTable FillInProblems()
        {
            var table = new DataTable("PROBLEMS");
            table.Columns.Add("GOALS");
            table.Columns.Add("REASONS");

            foreach (var goal in m_Goals)
            {
                if (goal.DoneThisQuarter == TscGoal.Activity.NotStarted ||
                    goal.DoneThisQuarter == TscGoal.Activity.PartyDone)
                {
                    var sGoal = string.Format("{0}: {1}", goal.EnglishBookName, goal.EnglishStage);
                    table.Rows.Add(new[] {sGoal, ""});
                }
            }

            return table;
        }
        #endregion
        #region method: DataTable FillInGoalsUpcoming()
        DataTable FillInGoalsUpcoming()
        {
            var table = new DataTable("UPCOMING");
            table.Columns.Add("GOALS");
            table.Columns.Add("DESIRES");

            foreach (var goal in m_Goals)
            {
                if (!goal.PlannedNextQuarter) 
                    continue;
                var sGoal = string.Format("{0}: {1}", goal.EnglishBookName, goal.EnglishStage);
                table.Rows.Add(new[] {sGoal, ""});
            }

            return table;
        }
        #endregion
        #region cmd: cmdGenerate
        private void cmdGenerate(object sender, EventArgs e)
        {
            m_Goals.HarvestGrid(m_grid);

            // Compute WordTemplate path
            const string csWordTemplateFileName = "TSCqr091118.docx";
            var sOurWordFolder = JWU.GetLocalApplicationDataFolder("OurWord");
            var sWordTemplatePath = Path.Combine(sOurWordFolder, csWordTemplateFileName);
            if (!File.Exists(sWordTemplatePath))
            {
                LocDB.Message("kTscReportNoWordTemplateFile",
                    "The TSC Report was not generated due to reason:\n" +
                    "The file {0} could not be found in the OurWord installation folder.",
                    new[] { csWordTemplateFileName }, LocDB.MessageTypes.Error);
                return;
            }

            // Compute destination file name
            var sFileName = string.Format("TSC Report {0} {1}.docx", Current.Year, Current.StartMonth);
            var sFolder = JWU.GetSpecialFolder(Environment.SpecialFolder.Desktop, null);
            var sDestinationPath = Path.Combine(sFolder, sFileName);
            if (File.Exists(sDestinationPath))
            {
                var bRemove = LocDB.Message("kTscReportOverwriteFile",
                    "The file {0} already exists on your desktop. Is it OK to overwrite it?",
                    new[] {sFileName},
                    LocDB.MessageTypes.WarningYN);
                if (!bRemove)
                    return;
                File.Delete(sDestinationPath);
            }

            // Non-table items
            var values = new Dictionary<string, string>();
            FillInProjectInformation(values);

            // Tables
            var tablesDataSet = new DataSet();
            tablesDataSet.Tables.Add(FillInOutcomes());
            tablesDataSet.Tables.Add(FillInProblems());
            tablesDataSet.Tables.Add(FillInGoalsUpcoming());
            tablesDataSet.Tables.Add(FillInBookChart());

            // Attempt to do the merge and write the file
            try
            {
                var data = FormFiller.GetWordReport(sWordTemplatePath, tablesDataSet, values);

                File.WriteAllBytes(sDestinationPath, data);

                LocDB.Message("kTscReportSuceeded",
                    "The TSC Report was generated and saved to your desktop as:\n" +
                    "    {0}.\n\n" +
                    "You will still need to open it in Word and add the information " +
                    "required in the report that OurWord does not store.",
                    new[] { sFileName }, LocDB.MessageTypes.Info);

                // No need to keep the form up; the report has been generated.
                Close();
            } 
            catch (Exception err)
            {
                LocDB.Message("kTscReportFailed", 
                    "The TSC Report was not generated due to reason:\n{0}",
                    new[] {err.Message}, LocDB.MessageTypes.Error);
            }
        }
        #endregion

        // Form handlers ---------------------------------------------------------------------
        #region Cmd: cmdLoad
        private void cmdLoad(object sender, EventArgs e)
        {
            m_Goals.Read();

            // Report Author
            m_tYourName.Text = string.IsNullOrEmpty(m_Goals.ReportAuthor)
                ? Users.Current.UserName
                : m_Goals.ReportAuthor;

            // Init the Year combo box
            for (var nYear = DateTime.Today.Year - 1; nYear < DateTime.Today.Year + 20; nYear++)
                m_cScheduledCompletion.Items.Add(nYear.ToString());
            m_cScheduledCompletion.Text = m_Goals.ScheduledCompletionYear.ToString();

            // Quarter combo: Current and Previous
            m_cPeriod.Items.Clear();
            foreach (var quarter in m_vQuarters)
                m_cPeriod.Items.Add(quarter.ToString());
            m_cPeriod.Text = Current.ToString();

            // Grid rows
            foreach(var goal in m_Goals)
            {
                var row = new DataGridViewRow();
                row.CreateCells(m_grid);
                TscGoal.PopulateRowCombos(row, goal);
                m_grid.Rows.Add(row);
                SetRowStyles(row);
            }
        }
        #endregion
        #region Cmd: cmdFormClosing
        private void cmdFormClosing(object sender, FormClosingEventArgs e)
        {
            // Author
            m_Goals.ReportAuthor = m_tYourName.Text;

            // Completion Date
            try
            {
                var sYear = m_cScheduledCompletion.Text;
                var nYear = Convert.ToInt16(sYear);
                m_Goals.ScheduledCompletionYear = nYear;
            }
            catch (Exception) {}

            // Goals
            m_Goals.HarvestGrid(m_grid);

            m_Goals.Save();
        }
        #endregion

        // Reporting Period ------------------------------------------------------------------
        #region CLASS: Quarter
        public class Quarter
        {
            public readonly string StartMonth;
            private readonly string EndMonth;
            public readonly int Year;

            #region OMethod: string ToString()
            public override string ToString()
            {
                return string.Format("{0} to {1} {2}", StartMonth, EndMonth, Year);
            }
            #endregion

            #region constructor(sStartMonth, sEndMonth, nYear)
            private Quarter(string sStartMonth, string sEndMonth, int nYear)
            {
                StartMonth = sStartMonth;
                EndMonth = sEndMonth;
                Year = nYear;
            }
            #endregion
            #region SMethod: Quarter CreateFromDate(DateTime date)
            public static Quarter CreateFromDate(DateTime date)
            {
                if (date.Month < 3)
                    return new Quarter("Jan", "Mar", date.Year);
                if (date.Month < 6)
                    return new Quarter("Apr", "Jun", date.Year);
                if (date.Month < 9)
                    return new Quarter("Jul", "Sep", date.Year);
                return new Quarter("Nov", "Dec", date.Year);
            }
            #endregion
        }
        #endregion
        private readonly List<Quarter> m_vQuarters;
        private static Quarter Current = Quarter.CreateFromDate(DateTime.Now);
        #region Cmd: cmdPeriodChanged
        private void cmdPeriodChanged(object sender, EventArgs e)
        {
            foreach(var quarter in m_vQuarters)
            {
                if (quarter.ToString() != m_cPeriod.Text) 
                    continue;
                Current = quarter;
                break;
            }
        }
        #endregion

        // Grid ------------------------------------------------------------------------------
        #region cmd: cmdCellValueChanged
        private void cmdCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > m_grid.Rows.Count)
                return;

            var row = m_grid.Rows[e.RowIndex];
            SetRowStyles(row);
        }
        #endregion
        #region smethod: void SetRowStyles(DataGridViewRow row)
        static void SetRowStyles(DataGridViewRow row)
        {
            var goal = TscGoal.CreateFromGridRow(row);
            if (null == goal)
                return;

            // Whether or not Next Quarter is enabled depends on the value in
            // the current quarter
            var bIsCompleted = (goal.DoneThisQuarter == TscGoal.Activity.Completed);
            var cellNextQuarter = row.Cells[3];
            cellNextQuarter.ReadOnly = bIsCompleted;
            if (bIsCompleted)
                cellNextQuarter.Value = " ";
        }
        #endregion
        #region cmd: cmdDefaultValuesNeeded
        private void cmdDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
           TscGoal.PopulateRowCombos(e.Row, null);
        }
        #endregion

    }


}
