using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SendEmail_LIB
{
    public class clsDoTraitement
    {
        private static clsDoTraitement Fact;
        private static string fileNamePostGres = "parametresPostGres.par";
        private static string fileNameSQLServer = "parametresSQLServer.par";
        private static string fileParamServeurGmail = "parametresServeur.par";
        private static string fileParamServeurYahoo = "parametresServeur1.par";
        private static string fileParamRepertoire = "parametreRepertoire.par";
        private static string fileParamTempData = "parametreTemp.par";

        public clsDoTraitement()
        {
        }
        public static clsDoTraitement GetInstance()
        {
            if (Fact == null)
                Fact = new clsDoTraitement();
            return Fact;
        }

        #region ACTION SUR CHEMIN ACCES CONNECTION, PARAMETRE SERVEUR ET REPERTOIRE PHOTO WEBCAM
        public static string updateCreateDirectory(string nomRepertoire)
        {//ParametersProgramms
            string cheminAccesRepertoire = "";
            //Recuperation du nom du lecteur dans lequel le projet se trouve
            string lecteur = Environment.CurrentDirectory.ToString().Substring(0, 2);
            DirectoryInfo directory = new DirectoryInfo(lecteur + @"\" + nomRepertoire);
            if (!directory.Exists)
            {
                //Creation du repertoire
                directory.Create();
                directory.Attributes = FileAttributes.Hidden;
                cheminAccesRepertoire = directory.FullName;
                //Console.WriteLine("directory.FullName = " + directory.FullName);
            }
            else
            {
                //Dossier existant
                cheminAccesRepertoire = directory.FullName;
                //throw new Exception("Echec de la céeation du répertoire");
            }
            return cheminAccesRepertoire;
        }
        /// <summary>
        ///Permet d'enregistrer la chaîne de connexion pour une base des donnée PostGresSQL dans un fichier texte 
        /// </summary>
        public void saveParamConnection(string serveur, string bd, string userName, string port, int valueBD)
        {
            if (valueBD == 0)
            {
                //PostGresSQL
                using (StreamWriter srw = new StreamWriter(updateCreateDirectory("SendEmail") + @"\" + fileNamePostGres, false))
                {
                    string chaine = string.Format("{0};{1};{2};{3}", serveur, bd, userName, port);
                    //On crypte la chaine a sauvegarde
                    srw.WriteLine("{0}", CryptageJosam_LIB.clsMetier.GetInstance().doCrypte(chaine));
                    srw.Close();
                }
            }
            else if (valueBD == 1)
            {
                //Sql serveur
                using (StreamWriter srw = new StreamWriter(updateCreateDirectory("SendEmail") + @"\" + fileNameSQLServer, false))
                {
                    string chaine = string.Format("{0};{1};{2}", serveur, bd, userName);
                    //On crypte la chaine a sauvegarde
                    srw.WriteLine("{0}", CryptageJosam_LIB.clsMetier.GetInstance().doCrypte(chaine));
                    srw.Close();
                }
            }
        }

        /// <summary>
        ///Permet d'enregistrer les paramètres de base de l'utilisateur d'envoie des e-mail : Expéditeur principal
        /// </summary>
        public void saveParamServeur(string serveurSMTP, string email, string nomComplet, string motPass,string typeCompte)
        {
            string fileName = "";
            if (typeCompte.Equals("gmail")) fileName = fileParamServeurGmail;
            else if (typeCompte.Equals("yahoo")) fileName = fileParamServeurYahoo;
            using (StreamWriter srw = new StreamWriter(updateCreateDirectory("SendEmail") + @"\" + fileName, false))
            {
                if (!string.IsNullOrEmpty(serveurSMTP))
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        if (!string.IsNullOrEmpty(nomComplet))
                        {
                            if (!string.IsNullOrEmpty(motPass))
                            {
                                string chaine = string.Format("{0};{1};{2};{3};{4}", serveurSMTP, email, nomComplet, motPass, typeCompte);
                                //On crypte la chaine a sauvegarde
                                srw.WriteLine("{0}", CryptageJosam_LIB.clsMetier.GetInstance().doCrypte(chaine));
                                srw.Close();
                            }
                            else throw new Exception("Veuillez saisir un mot de passe valide");
                        }
                        else throw new Exception("Veuillez saisir un nom valide");
                    }
                    else throw new Exception("Veuillez saisir une adresse e-mail valide");
                }
                else throw new Exception("Veuillez saisir un serveur SMTP valide");    
            }
        }

        /// <summary>
        /// Permet de charger la chaîne de connection à partir d'un fichier texte pour une Base PostGresSql et retourne un tableau
        /// contenant ces différentes valeurs (Serveur, Base de données, Nom d'utilisateur et numero de port)
        /// </summary>
        /// <returns>retourne un tableau</returns>
        public List<string> loadParam(int valueDB)
        {
            List<string> listValues = new List<string>();
            if (valueDB == 0)
            {
                //PostGresSQL
                if (File.Exists(updateCreateDirectory("SendEmail") + @"\" + fileNamePostGres))
                {
                    using (StreamReader sr = new StreamReader(updateCreateDirectory("SendEmail") + @"\" + fileNamePostGres))
                    {
                        if (!sr.EndOfStream)
                        {
                            string str = CryptageJosam_LIB.clsMetier.GetInstance().doDeCrypte(sr.ReadLine());
                            string[] value = str.Split(new char[] { ';' });
                            foreach (string str1 in value) listValues.Add(str1);
                            sr.Close();
                        }
                    }
                }
            }
            else if (valueDB == 1)
            {
                //SQLServer
                if (File.Exists(updateCreateDirectory("SendEmail") + @"\" + fileNameSQLServer))
                {
                    using (StreamReader sr = new StreamReader(updateCreateDirectory("SendEmail") + @"\" + fileNameSQLServer))
                    {
                        if (!sr.EndOfStream)
                        {
                            string str = CryptageJosam_LIB.clsMetier.GetInstance().doDeCrypte(sr.ReadLine());
                            string[] value = str.Split(new char[] { ';' });
                            foreach (string str1 in value) listValues.Add(str1);
                            sr.Close();
                        }
                    }
                }
            }
            return listValues;
        }

        /// <summary>
        /// Permet de charger les paramètres de base pour l'nvoie des email à partir d'un fichier texte et retourne un tableau
        /// respectivement du serveur smtp, de l'adresse e-mail de l'utilisateur, de son nom et de son mot de passe.
        /// Parametre entier 1 pour gmail et 2 pour yahoo
        /// </summary>
        /// <returns>retourne un tableau</returns>
        public List<string> loadParamServeur(int typeCompte)
        {
            List<string> listValues = new List<string>();
            string fileName = "";
            if (typeCompte == 1) fileName = fileParamServeurGmail;
            else if (typeCompte == 2) fileName = fileParamServeurYahoo;

            if (File.Exists(updateCreateDirectory("SendEmail") + @"\" + fileName))
            {
                using (StreamReader sr = new StreamReader(updateCreateDirectory("SendEmail") + @"\" + fileName))
                {
                    if (!sr.EndOfStream)
                    {
                        string str = CryptageJosam_LIB.clsMetier.GetInstance().doDeCrypte(sr.ReadLine());
                        string[] value = str.Split(new char[] { ';' });
                        foreach (string str1 in value) listValues.Add(str1);
                        sr.Close();
                    }
                }
            }
            return listValues;
        }

        /// <summary>
        ///Permet d'enregistrer le chemin d'acces complet pour la sauvegarde des photos prises par le WebCam
        /// </summary>
        public void saveParamRepertoire(string cheminAcces)
        {
            if (!string.IsNullOrEmpty(cheminAcces))
            {
                using (StreamWriter srw = new StreamWriter(updateCreateDirectory("SendEmail") + @"\" + fileParamRepertoire, false))
                {
                    string chaine = string.Format("{0}", cheminAcces);
                    //On crypte la chaine a sauvegarde
                    srw.WriteLine("{0}", CryptageJosam_LIB.clsMetier.GetInstance().doCrypte(chaine));
                    srw.Close();
                }
            }
            else throw new Exception("Le chemin d'accès spécifié est invalide");
        }

        /// <summary>
        /// Permet de charger lemin d'acces complet pour la sauvegarde des photos prise par le WebCam
        /// </summary>
        /// <returns>retourne un string</returns>
        public string loadParamRepertoire()
        {
            string str = "";
            if (File.Exists(updateCreateDirectory("SendEmail") + @"\" + fileParamRepertoire))
            {
                using (StreamReader sr = new StreamReader(updateCreateDirectory("SendEmail") + @"\" + fileParamRepertoire))
                {
                    if (!sr.EndOfStream)
                    {
                        str = CryptageJosam_LIB.clsMetier.GetInstance().doDeCrypte(sr.ReadLine());
                        sr.Close();
                    }
                }
            }
            return str;
        }

        /// <summary>
        ///Permet d'enregistrer le chemin d'acces complet pour la sauvegarde des photos prises par le WebCam
        /// </summary>
        public void saveParamTemporaire(string cheminAcces)
        {
            if (!string.IsNullOrEmpty(cheminAcces))
            {
                using (StreamWriter srw = new StreamWriter(updateCreateDirectory("SendEmail") + @"\" + fileParamTempData, false))
                {
                    string chaine = string.Format("{0}", cheminAcces);
                    //On crypte la chaine a sauvegarde
                    srw.WriteLine("{0}", CryptageJosam_LIB.clsMetier.GetInstance().doCrypte(chaine));
                    srw.Close();
                }
            }
            else throw new Exception("Le chemin d'accès spécifié est invalide");
        }

        /// <summary>
        /// Permet de charger lemin d'acces complet pour la sauvegarde des photos prise par le WebCam
        /// </summary>
        /// <returns>retourne un string</returns>
        public string loadParamTemporaire()
        {
            string str = "";
            if (File.Exists(updateCreateDirectory("SendEmail") + @"\" + fileParamTempData))
            {
                using (StreamReader sr = new StreamReader(updateCreateDirectory("SendEmail") + @"\" + fileParamTempData))
                {
                    if (!sr.EndOfStream)
                    {
                        str = CryptageJosam_LIB.clsMetier.GetInstance().doDeCrypte(sr.ReadLine());
                        sr.Close();
                    }
                }
            }
            return str;
        }
        #endregion

        #region Manipulation de la photo
        public Bitmap getImageFromByte(byte[] byteArray)
        {
            Bitmap image;
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                image = new Bitmap(stream);
            } return image;
        }

        /// <summary>
        /// Permet de convertire l'image en données binaires
        /// </summary>
        /// <param name="filePath">Chemin de l'image</param>
        /// <returns>L'image sous forme de byte[]</returns>
        public byte[] GetImage(string cheminFichier)
        {
            FileStream fs = new FileStream(cheminFichier, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] img = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            return img;
        }
        #endregion

        #region Actions sur envoi d'un e-mail
        public void sendEmailToOneDestinataire(string destinataire, string cc1, string cc2, string objectMsg, string messageContent, List<string> pieceJointe, string typeEmail)
        {
            bool okDestinataire = false, OkObjectMsg = false;
            int portMsg = 0, typeMessagerie = 0;
            if (typeEmail.Equals("yahoo"))
            {
                //portMsg = 465;
                portMsg = 587;
                typeMessagerie = 2;
            }
            else if (typeEmail.Equals("gmail"))
            {
                portMsg = 587;
                typeMessagerie = 1;
            }

            List<string> lstValues = loadParamServeur(typeMessagerie);

            if (string.IsNullOrEmpty(destinataire)) throw new Exception("Veuillez saisir un destinataire valide");
            else okDestinataire = true;
            if (okDestinataire)
            {
                if (string.IsNullOrEmpty(objectMsg)) throw new Exception("Veuillez saisir un object de message valide");
                else OkObjectMsg = true;
            }
            if (OkObjectMsg)
            {
                MailMessage mail = new MailMessage(lstValues[1],destinataire,objectMsg, messageContent);//Email Expediteur,Email Destinataire,Object message, Contenu message
                SmtpClient smtp = new SmtpClient(lstValues[0], portMsg);//Serveur smtp, numero de port
                smtp.Credentials = new NetworkCredential(lstValues[1], lstValues[3]);//Email Expediteur,Mot de passe de l'expediteur
                smtp.EnableSsl = true;

                //smtp.Timeout = 10000;
                if (!string.IsNullOrEmpty(cc1)) mail.CC.Add(cc1);
                if (!string.IsNullOrEmpty(cc2)) mail.Bcc.Add(cc2);

                //////Ajout des pieces jointes
                Attachment att = null;
                for (int i = 0; i < pieceJointe.Count; i++)
                {
                    if (!string.IsNullOrEmpty(pieceJointe[i].ToString()))
                    {
                        att = new Attachment(pieceJointe[i]);
                        mail.Attachments.Add(att);
                    }
                }
                smtp.Send(mail);
            }
        }

        public int sendEmailToMultipleDestinataire(List<string> destinataire, string cc1, string cc2, string objectMsg, string messageContent, List<string> pieceJointe, string typeEmail)
        {
            int portMsg = 0, typeMessagerie = 0, nbrMessage = 0;
            bool okDestinataire = false,OkObjectMsg = false;
            if (typeEmail.Equals("yahoo"))
            {
                //portMsg = 465;
                portMsg = 587;
                typeMessagerie = 2;
            }
            else if (typeEmail.Equals("gmail"))
            {
                portMsg = 587;
                typeMessagerie = 1;
            }

            List<string> lstValues = loadParamServeur(typeMessagerie);

            if (destinataire.Count == 0) throw new Exception("Veuillez sélectionner des destinataire valides");
            else
            {
                //foreach (string t in destinataire)
                //    mail.To.Add(t);
                okDestinataire = true;
            }
            if (okDestinataire)
            {
                if (string.IsNullOrEmpty(objectMsg)) throw new Exception("Veuillez saisir un object de message valide");
                else OkObjectMsg = true;
            }
            if (OkObjectMsg)
            {
                MailMessage mail = null;
                int t = 0;
                for (int i = 0; i < destinataire.Count; i++)
                {
                    t++;
                    mail = new MailMessage(lstValues[1], destinataire[i], objectMsg, messageContent);//Email Expediteur,Email Destinataire,Object message, Contenu message

                    SmtpClient smtp = new SmtpClient(lstValues[0], portMsg);//Serveur smtp, numero de port
                    smtp.Credentials = new NetworkCredential(lstValues[1], lstValues[3]);//Email Expediteur,Mot de passe de l'expediteur
                    smtp.EnableSsl = true;

                    if (t == 1)
                    {
                        if (!string.IsNullOrEmpty(cc1)) mail.CC.Add(cc1);
                        if (!string.IsNullOrEmpty(cc2)) mail.Bcc.Add(cc2);
                    }

                    //Ajout des pieces jointes
                    Attachment att = null;
                    for (int j = 0; j < pieceJointe.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(pieceJointe[j].ToString()))
                        {
                            att = new Attachment(pieceJointe[j]);
                            mail.Attachments.Add(att);
                        }
                    }
                    smtp.Send(mail);
                    nbrMessage++;
                }                
            }
            return nbrMessage;
        }
        #endregion

        #region creation des parametres
        public void createParamString(string ParamName, string value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamInt(string ParamName, int value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamDate(string ParamName, DateTime value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamDate(string ParamName, DateTime? value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamDouble(string ParamName, Double value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamBool(string ParamName, bool value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamByteTable(string ParamName, byte[] value, IDbCommand cmd)
        {
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamInt(string ParamName, int value, SqlCommand cmd)
        {
            SqlParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamDate(string ParamName, DateTime value, SqlCommand cmd)
        {
            SqlParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        public void createParamDouble(string ParamName, Double value, SqlCommand cmd)
        {
            SqlParameter param = cmd.CreateParameter();
            param.ParameterName = ParamName;
            param.Value = value;
            cmd.Parameters.Add(param);
        }

        #endregion
    }
}
