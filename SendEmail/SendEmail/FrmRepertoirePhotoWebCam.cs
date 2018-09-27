using System;
using System.Windows.Forms;
using SendEmail_LIB;

namespace SendEmail
{
    public partial class FrmConfigurationRepertoire : Form
    {
        public FrmConfigurationRepertoire()
        {
            InitializeComponent();
        }

        private void cmdEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                clsDoTraitement.GetInstance().saveParamRepertoire(txtCheminAcces.Text);
                MessageBox.Show("Enregistrement effectué avec succès", "Enregistrement chemin d'accès pour photos WebCam", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement chemin d'accès pour photo WebCam", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
        }

        private void FrmConfigurationRepertoire_Load(object sender, EventArgs e)
        {
            try
            {
                txtCheminAcces.Text = clsDoTraitement.GetInstance().loadParamRepertoire();
            }
            catch (Exception)
            {
                MessageBox.Show("Echec dce la récupération du repertoire de sauvegarde des photo capturée par WebCam", "Récupération repertoire photos WebCam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdLoadDirectory_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner un emplacement pour la sauvegarde des photos à capturer";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                dlgOpen.SupportMultiDottedExtensions = true;
                try
                {
                    txtCheminAcces.Text = dlgOpen.FileName;
                }
                catch (Exception)
                {
                    MessageBox.Show("L'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection chemin d'accès pour photos WebCam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}