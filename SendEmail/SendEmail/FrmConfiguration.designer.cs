namespace SendEmail
{
    partial class FrmConfigurationServeur
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSMTP = new System.Windows.Forms.TextBox();
            this.txtExpediteur = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdEnregistrer = new System.Windows.Forms.Button();
            this.txtNomExpediteur = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboTypeCompte = new System.Windows.Forms.ComboBox();
            this.rdYahoo = new System.Windows.Forms.RadioButton();
            this.rdGmail = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serveur SMTP :";
            // 
            // txtSMTP
            // 
            this.txtSMTP.Location = new System.Drawing.Point(101, 28);
            this.txtSMTP.Name = "txtSMTP";
            this.txtSMTP.Size = new System.Drawing.Size(179, 20);
            this.txtSMTP.TabIndex = 1;
            // 
            // txtExpediteur
            // 
            this.txtExpediteur.Location = new System.Drawing.Point(101, 50);
            this.txtExpediteur.Name = "txtExpediteur";
            this.txtExpediteur.Size = new System.Drawing.Size(179, 20);
            this.txtExpediteur.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Expéditeur :";
            // 
            // cmdEnregistrer
            // 
            this.cmdEnregistrer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdEnregistrer.ForeColor = System.Drawing.Color.Green;
            this.cmdEnregistrer.Location = new System.Drawing.Point(205, 122);
            this.cmdEnregistrer.Name = "cmdEnregistrer";
            this.cmdEnregistrer.Size = new System.Drawing.Size(75, 23);
            this.cmdEnregistrer.TabIndex = 5;
            this.cmdEnregistrer.Text = "&Enregistrer";
            this.cmdEnregistrer.UseVisualStyleBackColor = true;
            this.cmdEnregistrer.Click += new System.EventHandler(this.cmdEnregistrer_Click);
            // 
            // txtNomExpediteur
            // 
            this.txtNomExpediteur.Location = new System.Drawing.Point(101, 72);
            this.txtNomExpediteur.Name = "txtNomExpediteur";
            this.txtNomExpediteur.Size = new System.Drawing.Size(179, 20);
            this.txtNomExpediteur.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Nom expéditeur :";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(100, 96);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(179, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Mot de passe :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Type de compte :";
            // 
            // cboTypeCompte
            // 
            this.cboTypeCompte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTypeCompte.FormattingEnabled = true;
            this.cboTypeCompte.Location = new System.Drawing.Point(101, 4);
            this.cboTypeCompte.Name = "cboTypeCompte";
            this.cboTypeCompte.Size = new System.Drawing.Size(179, 21);
            this.cboTypeCompte.TabIndex = 0;
            // 
            // rdYahoo
            // 
            this.rdYahoo.AutoSize = true;
            this.rdYahoo.Location = new System.Drawing.Point(67, 125);
            this.rdYahoo.Name = "rdYahoo";
            this.rdYahoo.Size = new System.Drawing.Size(56, 17);
            this.rdYahoo.TabIndex = 7;
            this.rdYahoo.TabStop = true;
            this.rdYahoo.Text = "Yahoo";
            this.rdYahoo.UseVisualStyleBackColor = true;
            this.rdYahoo.CheckedChanged += new System.EventHandler(this.rdYahoo_CheckedChanged);
            // 
            // rdGmail
            // 
            this.rdGmail.AutoSize = true;
            this.rdGmail.Location = new System.Drawing.Point(15, 125);
            this.rdGmail.Name = "rdGmail";
            this.rdGmail.Size = new System.Drawing.Size(51, 17);
            this.rdGmail.TabIndex = 6;
            this.rdGmail.TabStop = true;
            this.rdGmail.Text = "Gmail";
            this.rdGmail.UseVisualStyleBackColor = true;
            this.rdGmail.CheckedChanged += new System.EventHandler(this.rdGmail_CheckedChanged);
            // 
            // FrmConfigurationServeur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 152);
            this.Controls.Add(this.rdGmail);
            this.Controls.Add(this.rdYahoo);
            this.Controls.Add(this.cboTypeCompte);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNomExpediteur);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdEnregistrer);
            this.Controls.Add(this.txtExpediteur);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSMTP);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfigurationServeur";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration pour messages e-mail";
            this.Load += new System.EventHandler(this.FrmConfigurationParam_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSMTP;
        private System.Windows.Forms.TextBox txtExpediteur;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdEnregistrer;
        private System.Windows.Forms.TextBox txtNomExpediteur;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboTypeCompte;
        private System.Windows.Forms.RadioButton rdYahoo;
        private System.Windows.Forms.RadioButton rdGmail;
    }
}