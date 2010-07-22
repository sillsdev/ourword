using System;
using System.Windows.Forms;
using JWTools;
using OurWord.Ctrls.Commands;
using OurWordData.DataModel;
using OurWordData.DataModel.Membership;

namespace OurWord.Dialogs.Properties
{
    public partial class Ctrl_UserIsNotProjectMember : UserControl
    {
        public SimpleHandler OnGrantMembership;

        public Ctrl_UserIsNotProjectMember()
        {
            InitializeComponent();
        }

        private void cmdGrantMembership(object sender, EventArgs e)
        {
            if (null != OnGrantMembership)
                OnGrantMembership();
        }

        private void cmdLoad(object sender, EventArgs e)
        {
            const string sBaseEnglish = "{0} is not a member of the {1} project. Click on the button " +
                "below to grant membership, so that you can then set editing permissions.";
            var sBase = G.GetLoc_String("UserIsNotMemberOfProject", sBaseEnglish);
            m_labelInfo.Text = string.Format(sBase, Users.Current.UserName, DB.TargetTranslation.DisplayName);

            m_bGrantMembership.Text = G.GetLoc_String("GrantMembership", "Grant Membership");
        }
    }
}
