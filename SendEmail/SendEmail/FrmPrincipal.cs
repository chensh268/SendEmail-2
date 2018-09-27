using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DirectX.Capture;
using SendEmail_LIB;

namespace SendEmail
{
    public partial class FrmPrincipal : Form
    {
        clsConnexion connection = new clsConnexion();
        List<string> lstpieceJointe = new List<string>();
        clsetudiant etudiant = new clsetudiant();
        clspersonne personne = new clspersonne();
        clsphoto photo = new clsphoto();

        //Class et objects pour WebCam
        private Filters InputOptions = null;
        private Filter VideoInput = null;
        private Filter AudioInput = null;
        private Capture CaptureInfo = null;

        public FrmPrincipal()
        {
            InitializeComponent();
        }

        //Initialisation des tous les itemes du formulaire
        private void InitializeForm()
        {
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            cmdSupprimerDestinataire.Enabled = false;
            cmdClearDestinataires.Enabled = false;
            cmdLoadDestinataires.Enabled = false;
            txtDestinataires.Enabled = false;
            cmdEnregistrer.Enabled = false;
            cmdArreter.Enabled = false;
            
            cmdEnregistrerEtudiant.Enabled = false;
            cmdRefresh.Enabled = false;
            cmdSupprimer.Enabled = false;
            cmdModifier.Enabled = false;

            cboTypeCompte.Items.Add("gmail");
            cboTypeCompte.Items.Add("yahoo");
            cboTypeCompte.SelectedIndex = 0;

            try
            {
                //Charement des param de connexion
                this.loadParamConnexion();
            }
            catch (Exception) { }

            try
            {
                List<string> lstValues = clsDoTraitement.GetInstance().loadParamServeur(1);
                lblUserExpediteur.Text = lstValues[2];
                cboExpediteur.Text = lstValues[1];
            }
            catch (Exception) { }

            try
            {
                this.chargementWebCam();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Erreur lors du chargement des paramètres du WebCam, " + ex.Message, "Chargement paramètres WebCam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            enabledDesabledAllItem(false);
        }

        private void cmdLoadBd_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Serveur = txtServeur.Text;
                connection.User = txtUser.Text;
                connection.Pwd = txtPwd.Text;

                cboBD.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboBD.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboBD.DataSource = connection.Lstbd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chargement des base de données du serveur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdConnexion_Click(object sender, EventArgs e)
        {
            try
            {
                connecter();
                enabledDesabledAllItem(true);
                rdEtudiant.Checked = true;
                chkDestinataireMultiple.Checked = true;

                //Items pour etudiant
                cboPersonne.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cboPersonne.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboPersonne.DataSource = clsMetier.GetInstance().getAllClspersonne();
                cboInscription.DataSource = clsMetier.GetInstance().getAllClsinscription2();
                cboPersonne.SelectedIndex = 0;
                dgv.DataSource = clsMetier.GetInstance().getAllClsetudiant();
             
                cacherChamDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connexion à la base de données", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void loadParamConnexion()
        {
            //Charement des param de connexion
            List<string> lstValues = clsDoTraitement.GetInstance().loadParam(1);
            txtServeur.Text = lstValues[0];
            cboBD.Text = lstValues[1];
            txtUser.Text = lstValues[2];
        }

        public void connecter()
        {
            connection.Serveur = txtServeur.Text;
            connection.DB = cboBD.Text;
            connection.User = txtUser.Text;
            connection.Pwd = txtPwd.Text;

            clsMetier.GetInstance().Initialize(connection, 1);
            if (clsMetier.GetInstance().isConnect())
            {
                MessageBox.Show("Connexion réussie", "Connexion à la base de données", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Enregistrement des param de connexion
                clsDoTraitement.GetInstance().saveParamConnection(connection.Serveur, connection.DB, connection.User, null, 1);
            }
            else
            {
                MessageBox.Show("Echec de l'authentification de l'utilisateur", "Authentification de l'utilisateur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtUser.Clear();
                txtPwd.Clear();
                txtUser.Focus();
            }
        }

        private void enabledDesabledAllItem(bool value)
        {
            gpCatEnvoie.Enabled = value;
            pnItemsEtutiant.Enabled = value;
            pnPieceJointe.Enabled = value;
            cboTypeCompte.Enabled = value;
            //cboExpediteur.Enabled = value;
            cboCC1.Enabled = value;
            cboCC2.Enabled = value;
            //lstDestinataires.Enabled = value;
            //cmdSupprimerDestinataire.Enabled = value;
            txtMessage.Enabled = value;
            cmdEnvoyer.Enabled = value;
            cmdAnnuler.Enabled = value;
            chkDestinataireMultiple.Enabled = value;
            txtObjectMessage.Enabled = value;
            cmdParametreServeur.Enabled = value;
            cboVideoSource.Enabled = value;
            cboAudioSource.Enabled = value;
            cmdAppliquer.Enabled = value;
            picturePreview.Enabled = value;
            pbCapture.Enabled = value;
            //cmdCapturer.Enabled = value;
            //cmdEnregistrer.Enabled = value;
            cmdRepertoire.Enabled = value;
            cmdEffacer.Enabled = value;
            cboPersonne.Enabled = value;
            cboInscription.Enabled = value;
            txtMatricule.Enabled = value;
            pbPhoto.Enabled = value;
            cmdNouveau.Enabled = value;
            cmdRefresh.Enabled = value;
            
            //cmdLoadDestinataires.Enabled = value;
        }

        private void cmdConnexion_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    connecter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connexion à la base de données", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void rdAdministration_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAdministration.Checked)
            {
                pnItemsEtutiant.Enabled = false;

                //Chargement des expéditeur avec les valeurs par defauts des destinataires
                try
                {
                    //cboExpediteur.DataSource = clsMetier.GetInstance().getAllClsagent();
                    cboCC1.DataSource = clsMetier.GetInstance().getAllClsagent();
                    cboCC2.DataSource = clsMetier.GetInstance().getAllClsagent();

                    //if (cboExpediteur.Items.Count > 0) cboExpediteur.SelectedIndex = 0;
                    if (cboCC1.Items.Count > 0) cboCC1.SelectedIndex = -1;
                    if (cboCC2.Items.Count > 0) cboCC2.SelectedIndex = -1;
                }
                catch (Exception) { }
            }
        }

        private void rdEnseignant_CheckedChanged(object sender, EventArgs e)
        {
            if (rdEnseignant.Checked)
            {
                pnItemsEtutiant.Enabled = false;

                //Chargement des expéditeur avec les valeurs par defauts des destinataires
                try
                {
                    //cboExpediteur.DataSource = clsMetier.GetInstance().getAllClsenseignant();
                    cboCC1.DataSource = clsMetier.GetInstance().getAllClsenseignant();
                    cboCC2.DataSource = clsMetier.GetInstance().getAllClsenseignant();

                    //if (cboExpediteur.Items.Count > 0) cboExpediteur.SelectedIndex = 0;
                    if (cboCC1.Items.Count > 0) cboCC1.SelectedIndex = -1;
                    if (cboCC2.Items.Count > 0) cboCC2.SelectedIndex = -1;
                }
                catch (Exception) { }
            }
        }

        private void rdAutre_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAutre.Checked)
            {
                pnItemsEtutiant.Enabled = false;

                //Chargement des expéditeur avec les valeurs par defauts des destinataires
                try
                {
                    //cboExpediteur.DataSource = clsMetier.GetInstance().getAllClsexternes();
                    cboCC1.DataSource = clsMetier.GetInstance().getAllClsexternes();
                    cboCC2.DataSource = clsMetier.GetInstance().getAllClsexternes();

                    //if (cboExpediteur.Items.Count >0) cboExpediteur.SelectedIndex = 0;
                    if (cboCC1.Items.Count > 0) cboCC1.SelectedIndex = -1;
                    if (cboCC2.Items.Count > 0) cboCC2.SelectedIndex = -1;
                }
                catch (Exception) { }
            }
        }

        private void rdEtudiant_CheckedChanged(object sender, EventArgs e)
        {
            if (rdEtudiant.Checked)
            {
                pnItemsEtutiant.Enabled = true;

                //Chargement item correspondant a etudiant (Promotion, Option, Promotion et Année academique)
                try
                {
                    cboPromotion.DataSource = clsMetier.GetInstance().getAllClspromotion();
                    cboOption.DataSource = clsMetier.GetInstance().getAllClsoption();
                    cboSection.DataSource = clsMetier.GetInstance().getAllClssection();
                    cboAnneAc.DataSource = clsMetier.GetInstance().getAllClsanneeacademique();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors du chargement des Promotions, Options et Sections, " + ex.Message, "Chargement Promotions, Options et Sections", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                //Chargement des expéditeur avec les valeurs par defauts des destinataires
                try
                {
                    int id_anneeac = ((clsanneeacademique)cboAnneAc.SelectedItem).Id;
                    int id_promotion = ((clspromotion)cboPromotion.SelectedItem).Id;
                    int id_option = ((clsoption)cboOption.SelectedItem).Id;
                    int id_section = ((clssection)cboSection.SelectedItem).Id;

                    //cboExpediteur.DataSource = clsMetier.GetInstance().getAllClsetudiant(id_anneeac, id_promotion, id_option, id_section);
                    cboCC1.DataSource = clsMetier.GetInstance().getAllClsagent();
                    cboCC2.DataSource = clsMetier.GetInstance().getAllClsenseignant();

                    //if (cboExpediteur.Items.Count > 0) cboExpediteur.SelectedIndex = 0;
                    if (cboCC1.Items.Count > 0) cboCC1.SelectedIndex = -1;
                    if (cboCC2.Items.Count > 0) cboCC2.SelectedIndex = -1;
                }
                catch (Exception) { }
            }
        }

        private void chkDestinataireMultiple_CheckedChanged(object sender, EventArgs e)
        {
            if ((rdAdministration.Checked || rdEnseignant.Checked || rdAutre.Checked || rdEtudiant.Checked) && chkDestinataireMultiple.Checked)
            {
                cmdLoadDestinataires.Enabled = true;
                lstDestinataires.Enabled = true;
                cmdSupprimerDestinataire.Enabled = true;
                txtDestinataires.Enabled = false;
            }
            else
            {
                cmdLoadDestinataires.Enabled = false;
                lstDestinataires.Enabled = false;
                cmdSupprimerDestinataire.Enabled = false;
                txtDestinataires.Enabled = true;
            }
        }

        private void cmdEnvoyer_Click(object sender, EventArgs e)
        {
            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //Ajout des pièces jointes
                if (lstpieceJointe.Count > 0) lstpieceJointe.Clear();

                if (!string.IsNullOrEmpty(txtPieceJointe1.Text)) lstpieceJointe.Add(txtPieceJointe1.Text);
                if (!string.IsNullOrEmpty(txtPieceJointe2.Text)) lstpieceJointe.Add(txtPieceJointe2.Text);
                if (!string.IsNullOrEmpty(txtPieceJointe3.Text)) lstpieceJointe.Add(txtPieceJointe3.Text);

                if(chkDestinataireMultiple.Checked)
                {
                    //Envoie multiple
                    List<string> lstAllDestinataire = new List<string>();
                    for (int i = 0; i < lstDestinataires.Items.Count; i++) lstAllDestinataire.Add(((string)lstDestinataires.Items[i]));

                    int nombre_message_envoye = clsDoTraitement.GetInstance().sendEmailToMultipleDestinataire(lstAllDestinataire, cboCC1.Text, cboCC2.Text, txtObjectMessage.Text, txtMessage.Text, lstpieceJointe, cboTypeCompte.Text);
                    MessageBox.Show(nombre_message_envoye + " Message(s) envoyé(s) avec succès", "Envoie E-mail multiple", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //Envoie unique
                    clsDoTraitement.GetInstance().sendEmailToOneDestinataire(txtDestinataires.Text, cboCC1.Text, cboCC2.Text, txtObjectMessage.Text, txtMessage.Text, lstpieceJointe, cboTypeCompte.Text);
                    MessageBox.Show("Message envoyé avec succès", "Envoie E-mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                //Retour au curseur normal
                Cursor.Current = cur;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Echec de l'envoie du message, " + ex.Message, "Envoie E-mail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdAnnuler_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Envoie du message annulé", "Annulation envoie E-mail", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'annulation de l'envoie du message, " + ex.Message, "Annulation envoie E-mail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdLoadDestinataires_Click(object sender, EventArgs e)
        {
            lstDestinataires.Items.Clear();
            try
            {
                if (rdAdministration.Checked)
                {
                    //Chargement des emails des agents dans la liste des destinataires
                    foreach (clsagent obj in clsMetier.GetInstance().getAllClsagent())
                    {
                        if (!string.IsNullOrEmpty(obj.Email.ToString())) lstDestinataires.Items.Add(obj.Email);
                    }
                    if (lstDestinataires.Items.Count > 0)
                    {
                        cmdSupprimerDestinataire.Enabled = true;
                        cmdClearDestinataires.Enabled = true;
                        lblNbrDestinataires.Text = lstDestinataires.Items.Count.ToString();
                    }
                }
                else if (rdEnseignant.Checked)
                {
                    //Chargement des emails des enseignats dans la liste des destinataires
                    foreach (clsenseignant obj in clsMetier.GetInstance().getAllClsenseignant())
                    {
                        if (!string.IsNullOrEmpty(obj.Email.ToString())) lstDestinataires.Items.Add(obj.Email);
                    }
                    if (lstDestinataires.Items.Count > 0)
                    {
                        cmdSupprimerDestinataire.Enabled = true;
                        cmdClearDestinataires.Enabled = true;
                        lblNbrDestinataires.Text = lstDestinataires.Items.Count.ToString();
                    }
                }
                else if (rdEtudiant.Checked)
                {
                    //Chargement des emails des etudiants dans la liste des destinataires
                    int id_anneeac = ((clsanneeacademique)cboAnneAc.SelectedItem).Id;
                    int id_promotion = ((clspromotion)cboPromotion.SelectedItem).Id;
                    int id_option = ((clsoption)cboOption.SelectedItem).Id;
                    int id_section = ((clssection)cboSection.SelectedItem).Id;

                    foreach (clsetudiant obj in clsMetier.GetInstance().getAllClsetudiant(id_anneeac, id_promotion, id_option, id_section))
                    {
                        if (!string.IsNullOrEmpty(obj.Email.ToString())) lstDestinataires.Items.Add(obj.Email);
                    }
                    if (lstDestinataires.Items.Count > 0)
                    {
                        cmdSupprimerDestinataire.Enabled = true;
                        cmdClearDestinataires.Enabled = true;
                        lblNbrDestinataires.Text = lstDestinataires.Items.Count.ToString();
                    }
                }
                else if (rdAutre.Checked)
                {
                    //Chargement des emails des autres dans la liste des destinataires
                    foreach (clsexternes obj in clsMetier.GetInstance().getAllClsexternes())
                    {
                        if (!string.IsNullOrEmpty(obj.Email.ToString())) lstDestinataires.Items.Add(obj.Email);
                    }
                    if (lstDestinataires.Items.Count > 0)
                    {
                        cmdSupprimerDestinataire.Enabled = true;
                        cmdClearDestinataires.Enabled = true;
                        lblNbrDestinataires.Text = lstDestinataires.Items.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de chargement des destinataires, " + ex.Message, "Chargement des destinataires", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtPieceJointe_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe1.Text = dlgOpen.FileName;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cmdClearDestinataires_Click(object sender, EventArgs e)
        {
            try
            {
                lstDestinataires.Items.Clear();
            }
            catch (Exception) { MessageBox.Show("Erreur inattendue", "Effacer destinaaires", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }

        private void cmdSupprimerDestinataire_Click(object sender, EventArgs e)
        {
            if (lstDestinataires.Items.Count > 0)
            {
                try
                {
                    for (int i = 0; i < lstDestinataires.Items.Count; i++)
                    {
                        if (i == lstDestinataires.SelectedIndex)
                        {
                            lstDestinataires.Items.RemoveAt(i);
                            break;
                        }
                    }
                }
                catch (Exception) { MessageBox.Show("Erreur inattendue", "Suppression destinataire", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            }
            else MessageBox.Show("Il n'ya aucun destinataire à supprimer", "Suppression destinataire", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void cmPieceJointe1_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe1.Text = dlgOpen.FileName;
                    string extensionFichier = Path.GetExtension(dlgOpen.FileName);
                    //Vérification de l'extension du fichier pour afficher une image correspondante dans le pictureBox
                    if (extensionFichier.Equals(".doc")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Word;
                    else if (extensionFichier.Equals(".rtf")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.WordRTF;
                    else if (extensionFichier.Equals(".html")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".htm")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".mdb")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".accdb")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Access;
                    else if (extensionFichier.Equals(".xls")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".xlsx")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".pptx")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".ppsx")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".pdf")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.PdfFile;
                    else if (extensionFichier.Equals(".mp3")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.MP3File;
                    else if (extensionFichier.Equals(".mp4")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Mp4File;
                    else if (extensionFichier.Equals(".avi")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.AviFile;
                    else if (extensionFichier.Equals(".jpg")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.JPGFile;
                    else if (extensionFichier.Equals(".png")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.PNGFile;
                    else if (extensionFichier.Equals(".tif")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else if (extensionFichier.Equals(".tiff")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else pbIconeApplication1.Image = global::SendEmail.Properties.Resources.AutreFile;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void txtPieceJointe2_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe2.Text = dlgOpen.FileName;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void txtPieceJointe3_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe3.Text = dlgOpen.FileName;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cmPieceJointe2_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe2.Text = dlgOpen.FileName;
                    string extensionFichier = Path.GetExtension(dlgOpen.FileName);
                    //Vérification de l'extension du fichier pour afficher une image correspondante dans le pictureBox
                    if (extensionFichier.Equals(".doc")) pbIconeApplication1.Image = global::SendEmail.Properties.Resources.Word;
                    else if (extensionFichier.Equals(".rtf")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.WordRTF;
                    else if (extensionFichier.Equals(".html")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".htm")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".mdb")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".accdb")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Access;
                    else if (extensionFichier.Equals(".xls")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".xlsx")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".pptx")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".ppsx")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".pdf")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.PdfFile;
                    else if (extensionFichier.Equals(".mp3")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.MP3File;
                    else if (extensionFichier.Equals(".mp4")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.Mp4File;
                    else if (extensionFichier.Equals(".avi")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.AviFile;
                    else if (extensionFichier.Equals(".jpg")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.JPGFile;
                    else if (extensionFichier.Equals(".png")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.PNGFile;
                    else if (extensionFichier.Equals(".tif")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else if (extensionFichier.Equals(".tiff")) pbIconeApplication2.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else pbIconeApplication2.Image = global::SendEmail.Properties.Resources.AutreFile;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cmPieceJointe3_Click(object sender, EventArgs e)
        {
            dlgOpen.Title = "Veuillez sélectionner le fichier à joindre à votre message";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                string cheminFichier = dlgOpen.FileName;
                try
                {
                    txtPieceJointe3.Text = dlgOpen.FileName;
                    string extensionFichier = Path.GetExtension(dlgOpen.FileName);
                    //Vérification de l'extension du fichier pour afficher une image correspondante dans le pictureBox
                    if (extensionFichier.Equals(".doc")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Word;
                    else if (extensionFichier.Equals(".rtf")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.WordRTF;
                    else if (extensionFichier.Equals(".html")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".htm")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".mdb")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.WebFile;
                    else if (extensionFichier.Equals(".accdb")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Access;
                    else if (extensionFichier.Equals(".xls")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".xlsx")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Excel;
                    else if (extensionFichier.Equals(".pptx")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".ppsx")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Powerpoint;
                    else if (extensionFichier.Equals(".pdf")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.PdfFile;
                    else if (extensionFichier.Equals(".mp3")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.MP3File;
                    else if (extensionFichier.Equals(".mp4")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.Mp4File;
                    else if (extensionFichier.Equals(".avi")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.AviFile;
                    else if (extensionFichier.Equals(".jpg")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.JPGFile;
                    else if (extensionFichier.Equals(".png")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.PNGFile;
                    else if (extensionFichier.Equals(".tif")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else if (extensionFichier.Equals(".tiff")) pbIconeApplication3.Image = global::SendEmail.Properties.Resources.TiffFile;
                    else pbIconeApplication3.Image = global::SendEmail.Properties.Resources.AutreFile;
                }
                catch (Exception)
                {
                    MessageBox.Show("Le fichier à l'emplacement | " + cheminFichier + " | n'est pas valide", "Sélection pièce jointe", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cmdParametreServeur_Click(object sender, EventArgs e)
        {
            FrmConfigurationServeur fr = new FrmConfigurationServeur(); 
            fr.ShowDialog();
        }

        private void cboTypeCompte_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                List<string> lstValues = clsDoTraitement.GetInstance().loadParamServeur(cboTypeCompte.SelectedIndex + 1);
                lblUserExpediteur.Text = lstValues[2];
                cboExpediteur.Text = lstValues[1];
            }
            catch (Exception) 
            {
                MessageBox.Show("Vérifiez que les paramètres pour ce type de compte de messagerie sont déjà définis", "Paramètre du type de compte de messagerie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #region Parametrage WebCam
        private void chargementWebCam()
        {
            InputOptions = new Filters();
            cmdAppliquer.Enabled = false;
            foreach (Filter f in InputOptions.VideoInputDevices)
            {
                cboVideoSource.Items.Add(f.Name);
            }
            if (cboVideoSource.Items.Count > 0)
            {
                cmdAppliquer.Enabled = true;
                cboVideoSource.SelectedIndex = 0;
            }
            foreach (Filter f in InputOptions.AudioInputDevices)
            {
                cboAudioSource.Items.Add(f.Name);
            }
            if (cboAudioSource.Items.Count > 0) cboAudioSource.SelectedIndex = 0;
        }

        /// <summary>
        /// Configure les option d'entree
        /// </summary>
        private void Configure()
        {
            if (cboVideoSource.Items.Count < 1)
                throw new Exception();
            cboVideoSource.Enabled = false;
            cboAudioSource.Enabled = false;
            this.cmdAppliquer.Enabled = true;
            this.VideoInput = this.InputOptions.VideoInputDevices[cboVideoSource.SelectedIndex];
            if (cboAudioSource.Items.Count > 0)
                this.AudioInput = this.InputOptions.AudioInputDevices[cboAudioSource.SelectedIndex];
            this.CaptureInfo = new Capture(this.VideoInput, this.AudioInput);
            this.CaptureInfo.PreviewWindow = picturePreview;
            this.CaptureInfo.RenderPreview();
            this.CaptureInfo.FrameCaptureComplete += new Capture.FrameCapHandler(CaptureInfo_FrameCaptureComplete);
            this.cmdCapturer.Enabled = true;
            this.cmdAppliquer.Enabled = false;
        }

        /// <summary>
        /// Est effectue lorsque la capture a reussie
        /// </summary>
        /// <param name="Frame">cette photo est generee</param>
        void CaptureInfo_FrameCaptureComplete(PictureBox Frame)
        {
            pbCapture.Image = Frame.Image;
        }

        private void ErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "Test du Webcam", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        private void cmdAppliquer_Click(object sender, EventArgs e)
        {
            try
            {
                this.Configure();
                cmdArreter.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Aucun webCam trouvé !!", "Erreur de webCam", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdCapturer_Click(object sender, EventArgs e)
        {
            try
            {
                CaptureInfo.CaptureFrame();
                cmdEnregistrer.Enabled = true;
                cmdArreter.Enabled = true;
                cmdCapturer.Enabled = false;
            }
            catch (Exception ex)
            {
                this.ErrorMessage(ex);
            }
        }

        private void cmdEnregistrer_Click(object sender, EventArgs e)
        {
            try
            {
                //dlgSavePhoto.DefaultExt = ".jpg";
                //dlgSavePhoto.Title = "Enregistrement d'une photo capturée";
                //dlgSavePhoto.OverwritePrompt = true;
                //dlgSavePhoto.CheckPathExists = true;
                //dlgOpen.InitialDirectory = clsDoTraitement.GetInstance().loadParamRepertoire();
                //DialogResult dr = dlgSavePhoto.ShowDialog();

                //if (dr == DialogResult.OK)
                //{
                    string cheminAcces = clsDoTraitement.GetInstance().loadParamRepertoire() + cboPersonne.Text + "_" + cboInscription.Text + "_" + txtMatricule.Text + ".jpg";//dlgSavePhoto.FileName + ".jpg";
                    pbCapture.Image.Save(cheminAcces);
                    clsDoTraitement.GetInstance().saveParamTemporaire(cheminAcces);
                    MessageBox.Show("Enregistrement effectué avec succès", "Enregistrement photo capturée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pbPhoto.Image = (new clsDoTraitement()).getImageFromByte(clsDoTraitement.GetInstance().GetImage(cheminAcces));
                    cmdEffacer_Click(sender, e);
                //}
                //else MessageBox.Show("Enregistrement non éffectué", "Enregistrement photo capturée", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //dlgSavePhoto.Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'enregistrement du fichier !!, " + ex.Message, "Enregistrement photo capturée", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdRepertoire_Click(object sender, EventArgs e)
        {
            FrmConfigurationRepertoire fr = new FrmConfigurationRepertoire();
            fr.ShowDialog();
        }

        private void cmdArreter_Click(object sender, EventArgs e)
        {
            try
            {
                CaptureInfo.DisposeCapture();
                cmdAppliquer.Enabled = true;
                cmdCapturer.Enabled = true;
                cboVideoSource.Enabled = true;
                cboAudioSource.Enabled = true;
                cmdEnregistrer.Enabled = false;
                cmdArreter.Enabled = false;
                pbCapture.Image = null;
                picturePreview.Image = null;
            }
            catch (Exception) { }
        }

        private void cmdEffacer_Click(object sender, EventArgs e)
        {
            cmdCapturer.Enabled = true;
            pbCapture.Image = null;
            cmdEnregistrer.Enabled = false;
        }

        private void cmdNouveau_Click(object sender, EventArgs e)
        {
            cboPersonne.SelectedIndex = 0;
            cboInscription.SelectedIndex = 0;
            pbPhoto.Image = null;
            txtMatricule.Clear();
            cmdEnregistrerEtudiant.Enabled = true;
        }

        private void refresh()
        {
            try
            {
                cboPersonne.DataSource = clsMetier.GetInstance().getAllClspersonne();
                cboPersonne.SelectedIndex = 0;
                cboInscription.DataSource = clsMetier.GetInstance().getAllClsinscription();
                dgv.DataSource = clsMetier.GetInstance().getAllClsetudiant();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur lors de l'actualisation", "Actualisation affichage des étudiants", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdSupprimer_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Voulez - vous vraiment supprimer cet enregistrement ?", "Suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    etudiant = ((clsetudiant)dgv.SelectedRows[0].DataBoundItem);

                    photo = clsMetier.GetInstance().getClsphoto(etudiant.IdPers);

                    if (dgv.RowCount >0)
                    {
                        new clsetudiant().delete(etudiant);
                        photo.delete(photo);
                        personne.delete((clspersonne)etudiant);
                        MessageBox.Show("Suppression éffectuée", "Suppression", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de la suppression " + ex.Message, "Suppression", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void cacherChamDataGrid()
        {
            int i = 0;
            foreach (DataGridViewColumn dtc in dgv.Columns)
            {
                if (dtc.DataPropertyName == "IdPers") dtc.Visible = false;
                else if (dtc.DataPropertyName == "Nom_complet") dtc.Visible = false;
                else if (dtc.DataPropertyName == "IdEtudiant") dtc.Visible = false;
                else if (dtc.DataPropertyName == "Id_personne") dtc.Visible = false;
                else if (dtc.DataPropertyName == "Id_inscription") dtc.Visible = false;
                dgv.AutoResizeColumn(i);
                i++;
            }
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgv.RowCount > 0)
                {
                    etudiant = ((clsetudiant)dgv.SelectedRows[0].DataBoundItem);
                    cboPersonne.Text = clsMetier.GetInstance().getClspersonne(etudiant.Id_personne).Nom_complet;
                    cboInscription.Text = clsMetier.GetInstance().getClsinscription1(etudiant.Id_inscription).Designation_complete;
                    txtMatricule.Text = clsMetier.GetInstance().getClsetudiant(etudiant.IdEtudiant).Matricule;
                    cmdModifier.Enabled = true;
                    cmdSupprimer.Enabled = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur lors de l'affichage des information de l'étudiant", "Affichage informations de l'étudiant", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (dgv.RowCount > 0)
            {
                try
                {
                    photo = clsMetier.GetInstance().getClsphoto(etudiant.IdPers);
                    pbPhoto.Image = (new clsDoTraitement()).getImageFromByte(photo.Photo);
                }
                catch (Exception) { pbPhoto.Image = null; }
            }
        }

        private void cmdEnregistrerEtudiant_Click(object sender, EventArgs e)
        {
            cmdEnregistrerEtudiant.Enabled = false;
        }

        private void cmdModifier_Click(object sender, EventArgs e)
        {
            try
            {
                etudiant.Id_personne = ((clspersonne)cboPersonne.SelectedItem).IdPers;
                etudiant.Id_inscription = ((clsinscription)cboInscription.SelectedItem).Id;
                etudiant.Matricule = txtMatricule.Text;
                etudiant.update(etudiant);

                photo.Id_personne = etudiant.Id_personne;
                photo.Photo = clsDoTraitement.GetInstance().GetImage(clsDoTraitement.GetInstance().loadParamTemporaire());

                if (clsMetier.GetInstance().getClscountphoto(etudiant.Id_personne) > 0)
                {
                    //Modification photo
                    photo.update(photo);
                }
                else
                {
                    //Insertion photo
                    photo.inserts();
                }

                MessageBox.Show("Modification éffectuée", "Modification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de la mise à jour" + ex.Message, "Mise à jour", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
