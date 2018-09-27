using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SendEmail_LIB;

namespace SendEmail
{
    public partial class FrmConfigurationServeur : Form
    {
        public FrmConfigurationServeur()
        {
            InitializeComponent();
        }

        private void cmdEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                clsDoTraitement.GetInstance().saveParamServeur(txtSMTP.Text, txtExpediteur.Text, txtNomExpediteur.Text, txtPassword.Text,cboTypeCompte.Text);
                txtPassword.Clear();
                MessageBox.Show("Enregistrement effectué avec succès", "Enregistrement paramètre E-mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement des paramètres de base pour l'evoie des e-mails", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
        }

        private void FrmConfigurationParam_Load(object sender, EventArgs e)
        {
            //Chargement des parametres de base pour l'envoie des email
            cboTypeCompte.Items.Add("gmail");
            cboTypeCompte.Items.Add("yahoo");
            cboTypeCompte.SelectedIndex = 0;

            rdGmail.Checked = false;
            rdYahoo.Checked = false;

            try
            {
                //Charement des param de connexion
                List<string> lstValues = clsDoTraitement.GetInstance().loadParamServeur(1);
                txtSMTP.Text = lstValues[0];
                txtExpediteur.Text = lstValues[1];
                txtNomExpediteur.Text = lstValues[2];
                //txtPassword.Text = lstValues[3];
                cboTypeCompte.Text = lstValues[4];
            }
            catch (Exception) { }
        }

        private void rdGmail_CheckedChanged(object sender, EventArgs e)
        {
            if (rdGmail.Checked)
            {
                try
                {
                    //Charement des param de connexion
                    List<string> lstValues = clsDoTraitement.GetInstance().loadParamServeur(1);
                    txtSMTP.Text = lstValues[0];
                    txtExpediteur.Text = lstValues[1];
                    txtNomExpediteur.Text = lstValues[2];
                    //txtPassword.Text = lstValues[3];
                    cboTypeCompte.Text = lstValues[4];
                }
                catch (Exception) { }
            }
        }

        private void rdYahoo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdYahoo.Checked)
            {
                try
                {
                    //Charement des param de connexion
                    List<string> lstValues = clsDoTraitement.GetInstance().loadParamServeur(2);
                    txtSMTP.Text = lstValues[0];
                    txtExpediteur.Text = lstValues[1];
                    txtNomExpediteur.Text = lstValues[2];
                    //txtPassword.Text = lstValues[3];
                    cboTypeCompte.Text = lstValues[4];
                }
                catch (Exception) { }
            }
        }
    }
}