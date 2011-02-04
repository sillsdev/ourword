using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using JWTools;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.TscReport
{

    public class TscGoal
    {
        // Attrs -----------------------------------------------------------------------------
        public string BookAbbrev;
        public string EnglishStage;

        public enum Activity { Completed, NotStarted, PartyDone, NotApplicable };
        public Activity DoneThisQuarter = Activity.NotApplicable;

        public bool PlannedNextQuarter;

        #region VAttr{g}: string EnglishBookName
        public string EnglishBookName
        {
            get
            {
                var book = DB.TargetTranslation.FindBook(BookAbbrev);
                return BookNames.English[book.BookIndex];
            }
        }
        #endregion

        // Public Grid Actions ---------------------------------------------------------------
        #region SMethod:void PopulateRowCombos(DataGridViewRow, TscGoal)
        public static void PopulateRowCombos(DataGridViewRow row, TscGoal goal)
        {
            // Books
            var cellBooks = row.Cells[0] as DataGridViewComboBoxCell;
            if (null == cellBooks)
                throw new Exception("Bad cellBooks");
            foreach (var book in DB.TargetTranslation.BookList)
                cellBooks.Items.Add(BookToString(book.BookAbbrev));
            if (null != goal && !string.IsNullOrEmpty(goal.BookAbbrev))
                cellBooks.Value = BookToString(goal.BookAbbrev);

            // Stages
            var cellStages = row.Cells[1] as DataGridViewComboBoxCell;
            if (null == cellStages)
                throw new Exception("Bad cellStages");
            foreach (var stage in DB.TeamSettings.Stages)
                cellStages.Items.Add(StageToString(stage.EnglishName));
            if (null != goal && !string.IsNullOrEmpty(goal.EnglishStage))
                cellStages.Value = StageToString(goal.EnglishStage);

            // ThisQuarter
            var cellThisQuarter = row.Cells[2] as DataGridViewComboBoxCell;
            if (null == cellThisQuarter)
                throw new Exception("Bad cellThisQuarter");
            foreach (Activity activity in Enum.GetValues(typeof(Activity)))
                cellThisQuarter.Items.Add(ActivityToString(activity));
            if (null != goal)
                cellThisQuarter.Value = ActivityToString(goal.DoneThisQuarter);

            // NextQuarter
            var cellNextQuarter = row.Cells[3] as DataGridViewComboBoxCell;
            if (null == cellNextQuarter)
                throw new Exception("Bad cellNextQuarter");
            cellNextQuarter.Items.Add(PlanToString(true));
            cellNextQuarter.Items.Add(PlanToString(false));
            if (null != goal)
                cellNextQuarter.Value = PlanToString(goal.PlannedNextQuarter);
        }
        #endregion
        #region SMethod: TscGoal CreateFromGridRow(DataGridViewRow row)
        static public TscGoal CreateFromGridRow(DataGridViewRow row)
        {
            var sBookAbbrev = (string)row.Cells[0].Value;
            var sStage = (string)row.Cells[1].Value;
            var sDoneThisQuarter = (string)row.Cells[2].Value;
            var sPlannedNextQuarter = (string)row.Cells[3].Value;

            if (string.IsNullOrEmpty(sBookAbbrev) ||
                string.IsNullOrEmpty(sStage))
            {
                return null;
            }

            return new TscGoal {
                BookAbbrev = BookAbbrevFromString(sBookAbbrev),
                EnglishStage = StageFromString(sStage),
                DoneThisQuarter = ActivityFromString(sDoneThisQuarter),
                PlannedNextQuarter = PlanFromString(sPlannedNextQuarter),
            };
        }
        #endregion

        // Private Grid helper methods -------------------------------------------------------
        // Grid - Books
        #region smethod: string BookToString(sBookAbbrev)
        private static string BookToString(string sBookAbbrev)
        {
            var book = DB.TargetTranslation.FindBook(sBookAbbrev);
            if (null == book)
                throw new Exception("Unknown book: " + sBookAbbrev);

            return string.Format("{0} - {1}", sBookAbbrev, book.DisplayName);
        }
        #endregion
        #region smethod: string BookAbbrevFromString(sComboText)
        private static string BookAbbrevFromString(string sComboText)
        {
            foreach (var book in DB.TargetTranslation.BookList)
            {
                if (BookToString(book.BookAbbrev) == sComboText)
                    return book.BookAbbrev;
            }

            throw new Exception("Unknown book: " + sComboText);
        }
        #endregion
        // Grid - Stages
        #region smethod: string StageToString(sEnglishName)
        private static string StageToString(string sEnglishName)
        {
            foreach (var stage in DB.TeamSettings.Stages)
            {
                if (stage.EnglishName == sEnglishName)
                    return stage.LocalizedAbbrev;
            }
            throw new Exception("Unknown stage: " + sEnglishName);
        }
        #endregion
        #region smethod: string StageFromString(sComboText)
        private static string StageFromString(string sComboText)
        {
            foreach (var stage in DB.TeamSettings.Stages)
            {
                if (StageToString(stage.EnglishName) == sComboText)
                    return stage.EnglishName;
            }
            throw new Exception("Unknown stage: " + sComboText);
        }
        #endregion
        // Grid - DoneThisQuarter: Activities
        #region smethod: string ActivityToString(Activity)
        private static string ActivityToString(Activity activity)
        {
            switch (activity)
            {
                case Activity.NotStarted:
                    return "Failed to Start";
                case Activity.PartyDone:
                    return "Partly Done";
                case Activity.Completed:
                    return "Completed";
                case Activity.NotApplicable:
                    return " ";
            }
            throw new Exception("Unknown activity: " + activity);
        }
        #endregion
        #region smethod: Activity ActivityFromString(sActivity)
        private static Activity ActivityFromString(string sActivity)
        {
            if (string.IsNullOrEmpty(sActivity))
                return Activity.NotApplicable;

            foreach (Activity activity in Enum.GetValues(typeof(Activity)))
            {
                if (sActivity == ActivityToString(activity))
                    return activity;
            }
            throw new Exception("Unknown activity: " + sActivity);
        }
        #endregion
        // Grid - PlannedNextQuarter
        #region smethod: string PlanToString(bIsPlanned)
        private static string PlanToString(bool bIsPlanned)
        {
            return (bIsPlanned) ? "Planned" : " ";
        }
        #endregion
        #region smethod: bool PlanFromString(sPlan)
        private static bool PlanFromString(string sPlan)
        {
            return (sPlan == PlanToString(true));
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sTag = "Goal";
        private const string c_sAttrBookAbbrev = "book";
        private const string c_sAttrEnglishStage = "stage";
        private const string c_sAttrDone = "done";
        private const string c_sAttrPlanned = "planned";
        #endregion
        #region Method: void Save(XmlDoc, nodeParent)
        public void Save(XmlDoc doc, XmlNode nodeParent)
        {
            var node = doc.AddNode(nodeParent, c_sTag);
            doc.AddAttr(node, c_sAttrBookAbbrev, BookAbbrev);
            doc.AddAttr(node, c_sAttrEnglishStage, EnglishStage);
            doc.AddAttr(node, c_sAttrDone, DoneThisQuarter.ToString());
            doc.AddAttr(node, c_sAttrPlanned, PlannedNextQuarter);
        }
        #endregion
        #region SMethod: TscGoal Create(nodeGoal)
        static public TscGoal Create(XmlNode node)
        {
            if (node.Name != c_sTag)
                return null;
           
            var goal = new TscGoal {
                BookAbbrev = XmlDoc.GetAttrValue(node, c_sAttrBookAbbrev, ""),
                EnglishStage = XmlDoc.GetAttrValue(node, c_sAttrEnglishStage, ""),
                DoneThisQuarter = (Activity)Enum.Parse(typeof(Activity),
                    XmlDoc.GetAttrValue(node, c_sAttrDone, Activity.NotApplicable.ToString()),
                    true),
                PlannedNextQuarter = XmlDoc.GetAttrValue(node, c_sAttrPlanned, false)
            };

            if (string.IsNullOrEmpty(goal.BookAbbrev) ||
                string.IsNullOrEmpty(goal.EnglishStage))
            {
                return null;
            }

            return goal;
        }
        #endregion
    }

    public class TscGoals : List<TscGoal>
    {
        public int ScheduledCompletionYear;
        public string ReportAuthor = "(your name)";

        // Scaffolding -----------------------------------------------------------------------
        #region Constructor()
        public TscGoals()
        {
            ReportAuthor = Users.Current.UserName;
            ScheduledCompletionYear = DateTime.Today.Year;
        }
        #endregion

        // Report Building -------------------------------------------------------------------
        #region Method: IEnumerable<TscGoal> GetGoalsForBook(sBookAbbrev)
        public IEnumerable<TscGoal> GetGoalsForBook(string sBookAbbrev)
        {
            var v = new List<TscGoal>();
            foreach(var goal in this)
            {
                if (goal.BookAbbrev == sBookAbbrev)
                    v.Add(goal);
            }

            return v;
        }
        #endregion

        // DataViewGrid ----------------------------------------------------------------------
        #region Method: void HarvestGrid(DataGridView grid)
        public void HarvestGrid(DataGridView grid)
        {
            Clear();
            foreach (DataGridViewRow row in grid.Rows)
            {
                var goal = TscGoal.CreateFromGridRow(row);
                if (null != goal)
                    Add(goal);
            }
        }
        #endregion

        // I/O -------------------------------------------------------------------------------
        #region Constants
        private const string c_sTag = "goals";
        private const string c_sattrAuthor = "author";
        private const string c_sattrCompletionYear = "year";
        #endregion
        #region sattr{g}: string GoalsPath
        static string GoalsPath
        {
            get
            {
                var sFilename = string.Format("ReportInfo {0}.tsc", DB.Project.DisplayName);
                return Path.Combine(DB.TeamSettings.SettingsFolder, sFilename);
            }
        }
        #endregion
        #region Method: void Save()
        public void Save()
        {
            var doc = new XmlDoc();
            doc.AddXmlDeclaration();

            var nodeGoals = doc.AddNode(doc, c_sTag);

            doc.AddAttr(nodeGoals, c_sattrAuthor, ReportAuthor);
            doc.AddAttr(nodeGoals, c_sattrCompletionYear, ScheduledCompletionYear);

            foreach(var goal in this)
                goal.Save(doc, nodeGoals);

            doc.Write(GoalsPath);
        }
        #endregion
        #region Method: void Read()
        public void Read()
        {
            if (!File.Exists(GoalsPath))
                return;

            var doc = new XmlDoc();
            doc.Load(GoalsPath);
            var nodeGoals = XmlDoc.FindNode(doc, c_sTag);

            ReportAuthor = XmlDoc.GetAttrValue(nodeGoals, c_sattrAuthor, "");
            ScheduledCompletionYear = XmlDoc.GetAttrValue(nodeGoals, c_sattrCompletionYear, DateTime.Today.Year);

            Clear();
            foreach(XmlNode child in nodeGoals.ChildNodes)
            {
                var goal = TscGoal.Create(child);
                if (null != goal)
                    Add(goal);
            }
        }
        #endregion
    }
}
