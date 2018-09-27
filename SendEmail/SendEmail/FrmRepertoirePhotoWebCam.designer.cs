namespace SendEmail
{
    partial class FrmConfigurationRepertoire
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
            this.txtCheminAcces = new System.Windows.Forms.TextBox();
            this.cmdEnregistrer = new System.Windows.Forms.Button();
            this.cmdLoadDirectory = new System.Windows.Forms.Button();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // txtCheminAcces
            // 
            this.txtCheminAcces.BackColor = System.Drawing.Color.LavenderBlush;
            this.txtCheminAcces.Location = new System.Drawing.Point(206, 12);
            this.txtCheminAcces.Name = "txtCheminAcces";
            this.txtCheminAcces.Size = new System.Drawing.Size(534, 20);
            this.txtCheminAcces.TabIndex = 1;
            // 
            // cmdEnregistrer
            // 
            this.cmdEnregistrer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdEnregistrer.ForeColor = System.Drawing.Color.Green;
            this.cmdEnregistrer.Location = new System.Drawing.Point(665, 38);
            this.cmdEnregistrer.Name = "cmdEnregistrer";
            this.cmdEnregistrer.Size = new System.Drawing.Size(75, 23);
            this.cmdEnregistrer.TabIndex = 2;
            this.cmdEnregistrer.Text = "&Enregistrer";
            this.cmdEnregistrer.UseVisualStyleBackColor = true;
            this.cmdEnregistrer.Click += new System.EventHandler(this.cmdEnregistrer_Click);
            // 
            // cmdLoadDirectory
            // 
            this.cmdLoadDirectory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdLoadDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdLoadDirectory.Location = new System.Drawing.Point(9, 10);
            this.cmdLoadDirectory.Name = "cmdLoadDirectory";
            this.cmdLoadDirectory.Size = new System.Drawing.Size(190, 23);
            this.cmdLoadDirectory.TabIndex = 0;
            this.cmdLoadDirectory.Text = "Spécifier un emplacement .....";
            this.cmdLoadDirectory.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cmdLoadDirectory.UseVisualStyleBackColor = true;
            this.cmdLoadDirectory.Click += new System.EventHandler(this.cmdLoadDirectory_Click);
            // 
            // FrmConfigurationRepertoire
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 72);
            this.Controls.Add(this.cmdLoadDirectory);
            this.Controls.Add(this.cmdEnregistrer);
            this.Controls.Add(this.txtCheminAcces);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfigurationRepertoire";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration chemin d\'acces pour photos du WebCam";
            this.Load += new System.EventHandler(this.FrmConfigurationRepertoire_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCheminAcces;
        private System.Windows.Forms.Button cmdEnregistrer;
        private System.Windows.Forms.Button cmdLoadDirectory;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
    }
}