using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.IO;
namespace SendEmail_LIB
{
    public class clsMetier
    {
        //***Les variables globales***
        private static string _ConnectionString, _host, _db, _user, _pwd;
        private static clsMetier Fact;
        private SqlConnection conn;
        public static string strChaineConnection = null;
        public static int id_du_paiement = 0;

        #region prerecquis
        public static clsMetier GetInstance()
        {
            if (Fact == null)
                Fact = new clsMetier();
            return Fact;
        }
        private object getParameter(IDbCommand cmd, string name, DbType type, int size, object value)
        {
            IDbDataParameter param = cmd.CreateParameter();
            param.Size = size;
            param.DbType = type;
            param.ParameterName = name;
            param.Value = value;
            return param;
        }
        public void Initialize(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
            conn = new SqlConnection(ConnectionString);
        }
        public void Initialize(clsConnexion con)
        {
            _host = con.Serveur;// host;
            _db = con.DB; ;
            _user = con.User;
            _pwd = con.Pwd;
            string sch = string.Format("server={0}; database={1}; user={2}; pwd={3}", _host, _db, _user, _pwd);
            conn = new SqlConnection(sch);
        }
        public void Initialize(clsConnexion con, int type)
        {
            _host = con.Serveur;// host;
            _db = con.DB; ;
            _user = con.User;
            _pwd = con.Pwd;
            string sch = string.Format("server={0}; database={1}; user={2}; pwd={3}", _host, _db, _user, _pwd);
            switch (type)
            {
                //sql server 2005
                case 1: sch = string.Format("Data Source={0};Persist Security Info=True; Initial Catalog={1};User ID={2}; Password={3}", _host, _db, _user, _pwd); break;
                //sql server 2008 Data Source=WIN7-PC\SQLEXPRESS;Initial Catalog=bihito;Persist Security Info=True;User ID=sa;Password=sa
                case 2: sch = string.Format("Data Source={0};Persist Security Info=True; Initial Catalog={1};User ID={2}; Password={3}", _host, _db, _user, _pwd); break;
                case 3: break;
            }
            conn = new SqlConnection(sch);
            strChaineConnection = sch;
        }
        public void Initialize(string host, string db, string user, string pwd)
        {
            _host = host;
            _db = db;
            _user = user;
            _pwd = pwd;
            string sch = string.Format("server={0}; database={1}; user={2}; pwd={3}", _host, _db, _user, _pwd);
            conn = new SqlConnection(sch);
        }
        public void setDB(string db)
        {
            _db = db;
        }
        public bool isConnect()
        {
            bool bl = true;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                conn.Close();
            }
            catch (Exception exc)
            {
                bl = false;
                conn.Close();
                throw new Exception(exc.Message);
            }
            return bl;
        }
        public bool isConnect(clsConnexion con)
        {
            bool bl = true;
            _host = con.Serveur;// host;
            _db = con.DB;
            _user = con.User;
            _pwd = con.Pwd;
            string sch = string.Format("server={0}; database=Master; user={1}; pwd={2}", con.Serveur, con.User, con.Pwd);
            conn = new SqlConnection(sch);
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                conn.Close();
            }
            catch (Exception exc)
            {
                sch = string.Format("server={0}; database={1};id user={2}; pwd={3}", _host, _db, _user, _pwd);
                bl = false;
                conn.Close();
                throw new Exception(exc.Message);
            }
            return bl;
        }
        public List<string> getAllDB()
        {
            List<string> lst = new List<string>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT name FROM sysdatabases where name!='master' order by name");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            lst.Add(dr["name"].ToString());
                    }
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lst;
        }

        #region OPERATION SUR LA SAUVEGARDE LOCALE DE LA BD
        /// <summary>
        /// Permet d'éffectuer une sauvegarde locale de la Base des données en passant en paramètre le chemin
        /// d'accès ou l'emplacement du fichier de sauvegarde
        /// </summary>
        /// <param name="cheminAcces">String chemin d'acces Bd</param>
        /// <param name="lecteur">string</param>
        /// <returns>string</returns>
        public string BackupLocalDataBase(string cheminAcces, string lecteur)
        {
            string requete = "";
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    if (string.IsNullOrEmpty(cheminAcces))
                    {
                        throw new Exception("Le chemin d'accès pour la sauvegarde de la base des données est invalide !!");
                    }
                    else
                    {
                        lecteur = null;
                        requete = "USE master " +
                                  "BACKUP DATABASE " + conn.Database + " " +
                                  "TO DISK = N'" + cheminAcces + "' WITH NOFORMAT," +
                                  "NOINIT,NAME = N'" + conn.Database + "_Complete_BackUpBase'";
                        cmd.CommandText = requete;
                    }
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return cheminAcces;
        }
        #endregion

        #region OPERATION SUR LA RESTAURATION DE LA BD
        /// <summary>
        /// Permet d'éffetctuer la restauration de la base des données à partir d'un fichier archive et prend respectivement
        /// comme paramètre le chemin d'accès du fichier de restauration, la lettre du lecteur de restauration ainsi que le numéro
        /// de version de PostGreSQL utilisé sur le serveur
        /// </summary>
        /// <param name="cheminAcces">string</param>
        /// <param name="lecteur">string</param>
        /// <returns>string</returns>
        public string RestoreDataBase(string cheminAcces, string lecteur)
        {
            string requete = "";
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    if (string.IsNullOrEmpty(cheminAcces))
                    {
                        throw new Exception("Le chemin d'accès pour la restauration de de la base des données est invalide !!");
                    }
                    else
                    {
                        lecteur = null;
                        requete = "USE master " +
                                  "SELECT 'kill',spid FROM sysprocesses " +
                                  "WHERE dbid=db_id('" + conn.Database + "') " +
                            //"GO " +
                                  "RESTORE DATABASE " + conn.Database + " " +
                                  "FROM DISK = N'" + cheminAcces + "'";
                        cmd.CommandText = requete;
                    }
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return cheminAcces;
        }
        #endregion

        public void closeConnexion()
        {
            try
            {
                if (conn.State.ToString().Equals("Open")) conn.Close();
            }
            catch (Exception) { }
        }

        #endregion prerecquis
        #region  CLSPERSONNE
        public clspersonne getClspersonne(object intid)
        {
            clspersonne varclspersonne = new clspersonne();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM personne WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr["id"].ToString().Trim().Equals("")) varclspersonne.IdPers = int.Parse(dr["id"].ToString());
                            varclspersonne.Nom = dr["nom"].ToString();
                            varclspersonne.Postnom = dr["postnom"].ToString();
                            varclspersonne.Prenom = dr["prenom"].ToString();
                            varclspersonne.Sexe = dr["sexe"].ToString();
                            varclspersonne.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclspersonne.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclspersonne.Telephone = dr["telephone"].ToString();
                            varclspersonne.Email = dr["email"].ToString();
                            //if (!dr["photo"].ToString().Trim().Equals("")) varclspersonne.Photo = (Byte[])dr["photo"];
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclspersonne;
        }

        public List<clspersonne> getAllClspersonne(string criteria)
        {
            List<clspersonne> lstclspersonne = new List<clspersonne>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = "SELECT *  FROM personne  WHERE 1=1";
                    sql += "  OR   nom LIKE '%" + criteria + "%'";
                    sql += "  OR   postnom LIKE '%" + criteria + "%'";
                    sql += "  OR   prenom LIKE '%" + criteria + "%'";
                    sql += "  OR   sexe LIKE '%" + criteria + "%'";
                    sql += "  OR   etatcivil LIKE '%" + criteria + "%'";
                    sql += "  OR   telephone LIKE '%" + criteria + "%'";
                    sql += "  OR   email LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clspersonne varclspersonne = null;
                        while (dr.Read())
                        {
                            varclspersonne = new clspersonne();
                            if (!dr["id"].ToString().Trim().Equals("")) varclspersonne.IdPers = int.Parse(dr["id"].ToString());
                            varclspersonne.Nom = dr["nom"].ToString();
                            varclspersonne.Postnom = dr["postnom"].ToString();
                            varclspersonne.Prenom = dr["prenom"].ToString();
                            varclspersonne.Sexe = dr["sexe"].ToString();
                            varclspersonne.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclspersonne.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclspersonne.Telephone = dr["telephone"].ToString();
                            varclspersonne.Email = dr["email"].ToString();
                            lstclspersonne.Add(varclspersonne);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclspersonne;
        }

        public List<clspersonne> getAllClspersonne()
        {
            List<clspersonne> lstclspersonne = new List<clspersonne>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM personne ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clspersonne varclspersonne = null;
                        while (dr.Read())
                        {

                            varclspersonne = new clspersonne();
                            if (!dr["id"].ToString().Trim().Equals("")) varclspersonne.IdPers = int.Parse(dr["id"].ToString());
                            varclspersonne.Nom = dr["nom"].ToString();
                            varclspersonne.Postnom = dr["postnom"].ToString();
                            varclspersonne.Prenom = dr["prenom"].ToString();
                            varclspersonne.Sexe = dr["sexe"].ToString();
                            varclspersonne.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclspersonne.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclspersonne.Telephone = dr["telephone"].ToString();
                            varclspersonne.Email = dr["email"].ToString();
                            lstclspersonne.Add(varclspersonne);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclspersonne;
        }

        public DataTable getAllClspersonne1()
        {
            DataTable lstclspersonne = new DataTable();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM personne ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        lstclspersonne.Load(dr);
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclspersonne;
        }

        public int insertClspersonne(clspersonne varclspersonne)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO personne ( nom,postnom,prenom,sexe,etatcivil,datenaissance,telephone,email ) VALUES (@nom,@postnom,@prenom,@sexe,@etatcivil,@datenaissance,@telephone,@email  )");
                    if (varclspersonne.Nom != null) cmd.Parameters.Add(getParameter(cmd, "@nom", DbType.String, 30, varclspersonne.Nom));
                    else cmd.Parameters.Add(getParameter(cmd, "@nom", DbType.String, 30, DBNull.Value));
                    if (varclspersonne.Postnom != null) cmd.Parameters.Add(getParameter(cmd, "@postnom", DbType.String, 40, varclspersonne.Postnom));
                    else cmd.Parameters.Add(getParameter(cmd, "@postnom", DbType.String, 40, DBNull.Value));
                    if (varclspersonne.Prenom != null) cmd.Parameters.Add(getParameter(cmd, "@prenom", DbType.String, 30, varclspersonne.Prenom));
                    else cmd.Parameters.Add(getParameter(cmd, "@prenom", DbType.String, 30, DBNull.Value));
                    if (varclspersonne.Sexe != null) cmd.Parameters.Add(getParameter(cmd, "@sexe", DbType.String, 1, varclspersonne.Sexe));
                    else cmd.Parameters.Add(getParameter(cmd, "@sexe", DbType.String, 1, "M"));
                    if (varclspersonne.Etatcivil != null) cmd.Parameters.Add(getParameter(cmd, "@etatcivil", DbType.String, 15, varclspersonne.Etatcivil));
                    else cmd.Parameters.Add(getParameter(cmd, "@etatcivil", DbType.String, 15, "Celibataire"));
                    if (varclspersonne.Datenaissance.HasValue) cmd.Parameters.Add(getParameter(cmd, "@datenaissance", DbType.DateTime, 8, varclspersonne.Datenaissance));
                    else cmd.Parameters.Add(getParameter(cmd, "@datenaissance", DbType.DateTime, 8, DBNull.Value));
                    if (varclspersonne.Telephone != null) cmd.Parameters.Add(getParameter(cmd, "@telephone", DbType.String, 100, varclspersonne.Telephone));
                    else cmd.Parameters.Add(getParameter(cmd, "@telephone", DbType.String, 100, DBNull.Value));
                    if (varclspersonne.Email != null) cmd.Parameters.Add(getParameter(cmd, "@email", DbType.Binary, 100, varclspersonne.Email));
                    else cmd.Parameters.Add(getParameter(cmd, "@email", DbType.String, 100, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClspersonne(clspersonne varclspersonne)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE personne  SET nom=@nom,postnom=@postnom,prenom=@prenom,sexe=@sexe,etatcivil=@etatcivil,datenaissance=@datenaissance,telephone=@telephone,email=@email  WHERE 1=1  AND id=@id ");
                    if (varclspersonne.Nom != null) cmd.Parameters.Add(getParameter(cmd, "@nom", DbType.String, 30, varclspersonne.Nom));
                    else cmd.Parameters.Add(getParameter(cmd, "@nom", DbType.String, 30, DBNull.Value));
                    if (varclspersonne.Postnom != null) cmd.Parameters.Add(getParameter(cmd, "@postnom", DbType.String, 40, varclspersonne.Postnom));
                    else cmd.Parameters.Add(getParameter(cmd, "@postnom", DbType.String, 40, DBNull.Value));
                    if (varclspersonne.Prenom != null) cmd.Parameters.Add(getParameter(cmd, "@prenom", DbType.String, 30, varclspersonne.Prenom));
                    else cmd.Parameters.Add(getParameter(cmd, "@prenom", DbType.String, 30, DBNull.Value));
                    if (varclspersonne.Sexe != null) cmd.Parameters.Add(getParameter(cmd, "@sexe", DbType.String, 1, varclspersonne.Sexe));
                    else cmd.Parameters.Add(getParameter(cmd, "@sexe", DbType.String, 1, "M"));
                    if (varclspersonne.Etatcivil != null) cmd.Parameters.Add(getParameter(cmd, "@etatcivil", DbType.String, 15, varclspersonne.Etatcivil));
                    else cmd.Parameters.Add(getParameter(cmd, "@etatcivil", DbType.String, 15, "Celibataire"));
                    if (varclspersonne.Datenaissance.HasValue) cmd.Parameters.Add(getParameter(cmd, "@datenaissance", DbType.DateTime, 8, varclspersonne.Datenaissance));
                    else cmd.Parameters.Add(getParameter(cmd, "@datenaissance", DbType.DateTime, 8, DBNull.Value));
                    if (varclspersonne.Telephone != null) cmd.Parameters.Add(getParameter(cmd, "@telephone", DbType.String, 100, varclspersonne.Telephone));
                    else cmd.Parameters.Add(getParameter(cmd, "@telephone", DbType.String, 100, DBNull.Value));
                    if (varclspersonne.Email != null) cmd.Parameters.Add(getParameter(cmd, "@email", DbType.String, 100, varclspersonne.Email));
                    else cmd.Parameters.Add(getParameter(cmd, "@email", DbType.String, 100, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclspersonne.IdPers));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClspersonne(clspersonne varclspersonne)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM personne  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclspersonne.IdPers));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSPERSONNE 
        #region  CLSAGENT
        public clsagent getClsagent(object intid)
        {
            clsagent varclsagent = new clsagent();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,agent.id AS idAg,agent.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,agent.matricule,agent.numeroinss,agent.dateangagement,agent.grade from personne
                    INNER JOIN agent ON personne.id=agent.id_personne  WHERE agent.id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //Agent
                            if (!dr["idAg"].ToString().Trim().Equals("")) varclsagent.IdAgent = int.Parse(dr["idAg"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsagent.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsagent.Matricule = dr["matricule"].ToString();
                            varclsagent.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsagent.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());
                            varclsagent.Numeroinss = dr["numeroinss"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsagent.IdPers = int.Parse(dr["id"].ToString());
                            varclsagent.Nom = dr["nom"].ToString();
                            varclsagent.Postnom = dr["postnom"].ToString();
                            varclsagent.Prenom = dr["prenom"].ToString();
                            varclsagent.Sexe = dr["sexe"].ToString();
                            varclsagent.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsagent.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsagent.Telephone = dr["telephone"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsagent;
        }

        public List<clsagent> getAllClsagent(string criteria)
        {
            List<clsagent> lstclsagent = new List<clsagent>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = @"SELECT personne.id,agent.id AS idAg,agent.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,agent.matricule,agent.numeroinss,agent.grade,agent.dateangagement from personne
                    INNER JOIN agent ON personne.id=agent.id_personne  WHERE 1=1";
                    sql += "  OR   agent.matricule LIKE '%" + criteria + "%'";
                    sql += "  OR   agent.numeroinss LIKE '%" + criteria + "%'";
                    sql += "  OR   agent.grade LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.nom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.postnom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.prenom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.sexe LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.etatcivil LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.telephone LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.email LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsagent varclsagent = null;
                        while (dr.Read())
                        {
                            varclsagent = new clsagent();
                            //Agent
                            if (!dr["idAg"].ToString().Trim().Equals("")) varclsagent.IdAgent = int.Parse(dr["idAg"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsagent.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsagent.Matricule = dr["matricule"].ToString();
                            varclsagent.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsagent.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());
                            varclsagent.Numeroinss = dr["numeroinss"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsagent.IdPers = int.Parse(dr["id"].ToString());
                            varclsagent.Nom = dr["nom"].ToString();
                            varclsagent.Postnom = dr["postnom"].ToString();
                            varclsagent.Prenom = dr["prenom"].ToString();
                            varclsagent.Sexe = dr["sexe"].ToString();
                            varclsagent.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsagent.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsagent.Telephone = dr["telephone"].ToString();
                            varclsagent.Email = dr["email"].ToString();
                            //if (!dr["photo"].ToString().Trim().Equals("")) varclsagent.Photo = (Byte[])dr["photo"];
                            lstclsagent.Add(varclsagent);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsagent;
        }

        public List<clsagent> getAllClsagent()
        {
            List<clsagent> lstclsagent = new List<clsagent>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,agent.id AS idAg,agent.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,agent.matricule,agent.grade,agent.numeroinss,agent.dateangagement from personne
                    INNER JOIN agent ON personne.id=agent.id_personne");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsagent varclsagent = null;
                        while (dr.Read())
                        {
                            varclsagent = new clsagent();

                            //Agent
                            if (!dr["idAg"].ToString().Trim().Equals("")) varclsagent.IdAgent = int.Parse(dr["idAg"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsagent.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsagent.Matricule = dr["matricule"].ToString();
                            varclsagent.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsagent.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());
                            varclsagent.Numeroinss = dr["numeroinss"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsagent.IdPers = int.Parse(dr["id"].ToString());
                            varclsagent.Nom = dr["nom"].ToString();
                            varclsagent.Postnom = dr["postnom"].ToString();
                            varclsagent.Prenom = dr["prenom"].ToString();
                            varclsagent.Sexe = dr["sexe"].ToString();
                            varclsagent.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsagent.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsagent.Telephone = dr["telephone"].ToString();
                            varclsagent.Email = dr["email"].ToString();
                            lstclsagent.Add(varclsagent);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsagent;
        }

        public int insertClsagent(clsagent varclsagent)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    DateTime? dateeng = DateTime.Today;
                    if (!varclsagent.Dateangagement.HasValue) dateeng = DateTime.Today;
                    else dateeng = varclsagent.Dateangagement;

                    cmd.CommandText = string.Format("INSERT INTO agent ( id_personne,matricule,grade,dateangagement,numeroinss ) VALUES (@id_personne,@matricule,@grade,@dateangagement,@numeroinss  )");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsagent.Id_personne));
                    if (varclsagent.Matricule != null) cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, varclsagent.Matricule));
                    else cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, DBNull.Value));
                    if (varclsagent.Grade != null) cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, varclsagent.Grade));
                    else cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, DBNull.Value));
                    if (varclsagent.Dateangagement.HasValue) cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    else cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    if (varclsagent.Numeroinss != null) cmd.Parameters.Add(getParameter(cmd, "@numeroinss", DbType.String, 20, varclsagent.Numeroinss));
                    else cmd.Parameters.Add(getParameter(cmd, "@numeroinss", DbType.String, 20, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsagent(clsagent varclsagent)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    DateTime? dateeng = DateTime.Today;
                    if (!varclsagent.Dateangagement.HasValue) dateeng = DateTime.Today;
                    else dateeng = varclsagent.Dateangagement;

                    cmd.CommandText = string.Format("UPDATE agent  SET id_personne=@id_personne,matricule=@matricule,grade=@grade,dateangagement=@dateangagement,numeroinss=@numeroinss  WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsagent.Id_personne));
                    if (varclsagent.Matricule != null) cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, varclsagent.Matricule));
                    else cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, DBNull.Value));
                    if (varclsagent.Grade != null) cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, varclsagent.Grade));
                    else cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, DBNull.Value));
                    if (varclsagent.Dateangagement.HasValue) cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    else cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    if (varclsagent.Numeroinss != null) cmd.Parameters.Add(getParameter(cmd, "@numeroinss", DbType.String, 20, varclsagent.Numeroinss));
                    else cmd.Parameters.Add(getParameter(cmd, "@numeroinss", DbType.String, 20, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsagent.IdAgent));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsagent(clsagent varclsagent)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM agent  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsagent.IdAgent));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSAGENT
        #region  CLSANNEEACADEMIQUE
        public clsanneeacademique getClsanneeacademique(object intid)
        {
            clsanneeacademique varclsanneeacademique = new clsanneeacademique();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM anneeacademique WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            if (!dr["id"].ToString().Trim().Equals("")) varclsanneeacademique.Id = int.Parse(dr["id"].ToString());
                            varclsanneeacademique.Designation = dr["designation"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsanneeacademique;
        }

        public List<clsanneeacademique> getAllClsanneeacademique(string criteria)
        {
            List<clsanneeacademique> lstclsanneeacademique = new List<clsanneeacademique>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = "SELECT *  FROM anneeacademique  WHERE 1=1";
                    sql += "  OR   designation LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clsanneeacademique varclsanneeacademique = null;
                        while (dr.Read())
                        {

                            varclsanneeacademique = new clsanneeacademique();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsanneeacademique.Id = int.Parse(dr["id"].ToString());
                            varclsanneeacademique.Designation = dr["designation"].ToString();
                            lstclsanneeacademique.Add(varclsanneeacademique);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsanneeacademique;
        }

        public List<clsanneeacademique> getAllClsanneeacademique()
        {
            List<clsanneeacademique> lstclsanneeacademique = new List<clsanneeacademique>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM anneeacademique ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clsanneeacademique varclsanneeacademique = null;
                        while (dr.Read())
                        {

                            varclsanneeacademique = new clsanneeacademique();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsanneeacademique.Id = int.Parse(dr["id"].ToString());
                            varclsanneeacademique.Designation = dr["designation"].ToString();
                            lstclsanneeacademique.Add(varclsanneeacademique);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsanneeacademique;
        }

        public int insertClsanneeacademique(clsanneeacademique varclsanneeacademique)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO anneeacademique ( designation ) VALUES (@designation  )");
                    if (varclsanneeacademique.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclsanneeacademique.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsanneeacademique(clsanneeacademique varclsanneeacademique)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE anneeacademique  SET designation=@designation  WHERE 1=1  AND id=@id ");
                    if (varclsanneeacademique.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclsanneeacademique.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsanneeacademique.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsanneeacademique(clsanneeacademique varclsanneeacademique)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM anneeacademique  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsanneeacademique.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSANNEEACADEMIQUE
        #region  CLSPROMOTION
        public clspromotion getClspromotion(object intid)
        {
            clspromotion varclspromotion = new clspromotion();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM promotion WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr["id"].ToString().Trim().Equals("")) varclspromotion.Id = int.Parse(dr["id"].ToString());
                            varclspromotion.Designation = dr["designation"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclspromotion;
        }

        public List<clspromotion> getAllClspromotion(string criteria)
        {
            List<clspromotion> lstclspromotion = new List<clspromotion>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = "SELECT *  FROM promotion  WHERE 1=1";
                    sql += "  OR   designation LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clspromotion varclspromotion = null;
                        while (dr.Read())
                        {

                            varclspromotion = new clspromotion();
                            if (!dr["id"].ToString().Trim().Equals("")) varclspromotion.Id = int.Parse(dr["id"].ToString());
                            varclspromotion.Designation = dr["designation"].ToString();
                            lstclspromotion.Add(varclspromotion);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclspromotion;
        }

        public List<clspromotion> getAllClspromotion()
        {
            List<clspromotion> lstclspromotion = new List<clspromotion>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM promotion ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clspromotion varclspromotion = null;
                        while (dr.Read())
                        {

                            varclspromotion = new clspromotion();
                            if (!dr["id"].ToString().Trim().Equals("")) varclspromotion.Id = int.Parse(dr["id"].ToString());
                            varclspromotion.Designation = dr["designation"].ToString();
                            lstclspromotion.Add(varclspromotion);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclspromotion;
        }

        public int insertClspromotion(clspromotion varclspromotion)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO promotion ( designation ) VALUES (@designation  )");
                    if (varclspromotion.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclspromotion.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClspromotion(clspromotion varclspromotion)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE promotion  SET designation=@designation  WHERE 1=1  AND id=@id ");
                    if (varclspromotion.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclspromotion.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclspromotion.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClspromotion(clspromotion varclspromotion)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM promotion  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclspromotion.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSPROMOTION
        #region  CLSOPTION
        public clsoption getClsoption(object intid)
        {
            clsoption varclsoption = new clsoption();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM optio WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            if (!dr["id"].ToString().Trim().Equals("")) varclsoption.Id = int.Parse(dr["id"].ToString());
                            varclsoption.Designation = dr["designation"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsoption;
        }

        public List<clsoption> getAllClsoption(string criteria)
        {
            List<clsoption> lstclsoption = new List<clsoption>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = "SELECT *  FROM optio  WHERE 1=1";
                    sql += "  OR   designation LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clsoption varclsoption = null;
                        while (dr.Read())
                        {

                            varclsoption = new clsoption();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsoption.Id = int.Parse(dr["id"].ToString());
                            varclsoption.Designation = dr["designation"].ToString();
                            lstclsoption.Add(varclsoption);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsoption;
        }

        public List<clsoption> getAllClsoption()
        {
            List<clsoption> lstclsoption = new List<clsoption>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM optio ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clsoption varclsoption = null;
                        while (dr.Read())
                        {

                            varclsoption = new clsoption();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsoption.Id = int.Parse(dr["id"].ToString());
                            varclsoption.Designation = dr["designation"].ToString();
                            lstclsoption.Add(varclsoption);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsoption;
        }

        public int insertClsoption(clsoption varclsoption)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO optio ( designation ) VALUES (@designation  )");
                    if (varclsoption.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclsoption.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsoption(clsoption varclsoption)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE optio  SET designation=@designation  WHERE 1=1  AND id=@id ");
                    if (varclsoption.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclsoption.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsoption.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsoption(clsoption varclsoption)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM optio  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsoption.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSOPTION
        #region  CLSSECTON
        public clssection getClssection(object intid)
        {
            clssection varclssection = new clssection();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM section WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            if (!dr["id"].ToString().Trim().Equals("")) varclssection.Id = int.Parse(dr["id"].ToString());
                            varclssection.Designation = dr["designation"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclssection;
        }

        public List<clssection> getAllClssection(string criteria)
        {
            List<clssection> lstclssection = new List<clssection>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = "SELECT *  FROM section  WHERE 1=1";
                    sql += "  OR   designation LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clssection varclssection = null;
                        while (dr.Read())
                        {

                            varclssection = new clssection();
                            if (!dr["id"].ToString().Trim().Equals("")) varclssection.Id = int.Parse(dr["id"].ToString());
                            varclssection.Designation = dr["designation"].ToString();
                            lstclssection.Add(varclssection);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclssection;
        }

        public List<clssection> getAllClssection()
        {
            List<clssection> lstclssection = new List<clssection>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM section ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clssection varclssection = null;
                        while (dr.Read())
                        {

                            varclssection = new clssection();
                            if (!dr["id"].ToString().Trim().Equals("")) varclssection.Id = int.Parse(dr["id"].ToString());
                            varclssection.Designation = dr["designation"].ToString();
                            lstclssection.Add(varclssection);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclssection;
        }

        public int insertClssection(clssection varclssection)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO section ( designation ) VALUES (@designation  )");
                    if (varclssection.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclssection.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClssection(clssection varclssection)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE section  SET designation=@designation  WHERE 1=1  AND id=@id ");
                    if (varclssection.Designation != null) cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, varclssection.Designation));
                    else cmd.Parameters.Add(getParameter(cmd, "@designation", DbType.String, 30, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclssection.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClssection(clssection varclssection)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM section  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclssection.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSSECTON
        #region  CLSENSEIGNANT
        public clsenseignant getClsenseignant(object intid)
        {
            clsenseignant varclsenseignant = new clsenseignant();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,enseignant.id AS idEns,enseignant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,enseignant.dateangagement,enseignant.grade from personne
                    INNER JOIN enseignant ON personne.id=enseignant.id_personne  WHERE enseignant.id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //Enseignant
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsenseignant.IdEnseignant = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsenseignant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsenseignant.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsenseignant.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsenseignant.IdPers = int.Parse(dr["id"].ToString());
                            varclsenseignant.Nom = dr["nom"].ToString();
                            varclsenseignant.Postnom = dr["postnom"].ToString();
                            varclsenseignant.Prenom = dr["prenom"].ToString();
                            varclsenseignant.Sexe = dr["sexe"].ToString();
                            varclsenseignant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsenseignant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsenseignant.Telephone = dr["telephone"].ToString();
                            varclsenseignant.Email = dr["email"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsenseignant;
        }

        public List<clsenseignant> getAllClsenseignant(string criteria)
        {
            List<clsenseignant> lstclsenseignant = new List<clsenseignant>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = @"SELECT personne.id,enseignant.id AS idEns,enseignant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,enseignant.grade,enseignant.dateangagement from personne
                    INNER JOIN enseignant ON personne.id=enseignant.id_personne  WHERE 1=1";
                    sql += "  OR   enseignant.grade LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.nom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.postnom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.prenom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.sexe LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.etatcivil LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.telephone LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.email LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsenseignant varclsenseignant = null;
                        while (dr.Read())
                        {
                            varclsenseignant = new clsenseignant();
                            //Enseignant
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsenseignant.IdEnseignant = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsenseignant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsenseignant.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsenseignant.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsenseignant.IdPers = int.Parse(dr["id"].ToString());
                            varclsenseignant.Nom = dr["nom"].ToString();
                            varclsenseignant.Postnom = dr["postnom"].ToString();
                            varclsenseignant.Prenom = dr["prenom"].ToString();
                            varclsenseignant.Sexe = dr["sexe"].ToString();
                            varclsenseignant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsenseignant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsenseignant.Telephone = dr["telephone"].ToString();
                            varclsenseignant.Email = dr["email"].ToString();
                            //if (!dr["photo"].ToString().Trim().Equals("")) varclsenseignant.Photo = (Byte[])dr["photo"];
                            lstclsenseignant.Add(varclsenseignant);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsenseignant;
        }

        public List<clsenseignant> getAllClsenseignant()
        {
            List<clsenseignant> lstclsenseignant = new List<clsenseignant>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,enseignant.id AS idEns,enseignant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,enseignant.grade,enseignant.dateangagement from personne
                    INNER JOIN enseignant ON personne.id=enseignant.id_personne");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsenseignant varclsenseignant = null;
                        while (dr.Read())
                        {
                            varclsenseignant = new clsenseignant();

                            //Enseignant
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsenseignant.IdEnseignant = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsenseignant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsenseignant.Grade = dr["grade"].ToString();
                            if (!dr["dateangagement"].ToString().Trim().Equals("")) varclsenseignant.Dateangagement = DateTime.Parse(dr["dateangagement"].ToString());

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsenseignant.IdPers = int.Parse(dr["id"].ToString());
                            varclsenseignant.Nom = dr["nom"].ToString();
                            varclsenseignant.Postnom = dr["postnom"].ToString();
                            varclsenseignant.Prenom = dr["prenom"].ToString();
                            varclsenseignant.Sexe = dr["sexe"].ToString();
                            varclsenseignant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsenseignant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsenseignant.Telephone = dr["telephone"].ToString();
                            varclsenseignant.Email = dr["email"].ToString();
                            lstclsenseignant.Add(varclsenseignant);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsenseignant;
        }

        public int insertClsenseignant(clsenseignant varclsenseignant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    DateTime? dateeng = DateTime.Today;
                    if (!varclsenseignant.Dateangagement.HasValue) dateeng = DateTime.Today;
                    else dateeng = varclsenseignant.Dateangagement;

                    cmd.CommandText = string.Format("INSERT INTO enseignant ( id_personne,grade,dateangagement ) VALUES (@id_personne,@grade,@dateangagement  )");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsenseignant.Id_personne));
                    if (varclsenseignant.Grade != null) cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, varclsenseignant.Grade));
                    else cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, DBNull.Value));
                    if (varclsenseignant.Dateangagement.HasValue) cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    else cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsenseignant(clsenseignant varclsenseignant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    DateTime? dateeng = DateTime.Today;
                    if (!varclsenseignant.Dateangagement.HasValue) dateeng = DateTime.Today;
                    else dateeng = varclsenseignant.Dateangagement;

                    cmd.CommandText = string.Format("UPDATE enseignant  SET id_personne=@id_personne,grade=@grade,dateangagement=@dateangagement  WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsenseignant.Id_personne));
                    if (varclsenseignant.Grade != null) cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, varclsenseignant.Grade));
                    else cmd.Parameters.Add(getParameter(cmd, "@grade", DbType.String, 30, DBNull.Value));
                    if (varclsenseignant.Dateangagement.HasValue) cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    else cmd.Parameters.Add(getParameter(cmd, "@dateangagement", DbType.DateTime, 8, dateeng));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsenseignant.IdEnseignant));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsenseignant(clsenseignant varclsenseignant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM enseignant  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsenseignant.IdEnseignant));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSENSEIGNANT
        #region  CLSINSCRIPTION
        public clsinscription getClsinscription(object intid)
        {
            clsinscription varclsinscription = new clsinscription();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT * FROM inscription WHERE id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr["id"].ToString().Trim().Equals("")) varclsinscription.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_anneeacademique"].ToString().Trim().Equals("")) varclsinscription.Id_anneeacademique = int.Parse(dr["id_anneeacademique"].ToString());
                            if (!dr["id_promotion"].ToString().Trim().Equals("")) varclsinscription.Id_promotion = int.Parse(dr["id_promotion"].ToString());
                            if (!dr["id_option"].ToString().Trim().Equals("")) varclsinscription.Id_option = int.Parse(dr["id_option"].ToString());
                            if (!dr["id_section"].ToString().Trim().Equals("")) varclsinscription.Id_section = int.Parse(dr["id_section"].ToString());
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsinscription;
        }

        public clsinscription getClsinscription1(object intid)
        {
            clsinscription varclsinscription = new clsinscription();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT inscription.*,promotion.designation AS promotion,optio.designation AS optio,section.designation AS section,anneeacademique.designation AS anneeacademique FROM inscription
                    INNER JOIN promotion ON promotion.id=inscription.id_promotion
                    INNER JOIN optio ON optio.id=inscription.id_option
                    INNER JOIN section ON section.id= inscription.id_section
                    INNER JOIN anneeacademique ON anneeacademique.id=inscription.id_anneeacademique 
                    WHERE inscription.id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr["id"].ToString().Trim().Equals("")) varclsinscription.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_anneeacademique"].ToString().Trim().Equals("")) varclsinscription.Id_anneeacademique = int.Parse(dr["id_anneeacademique"].ToString());
                            if (!dr["id_promotion"].ToString().Trim().Equals("")) varclsinscription.Id_promotion = int.Parse(dr["id_promotion"].ToString());
                            if (!dr["id_option"].ToString().Trim().Equals("")) varclsinscription.Id_option = int.Parse(dr["id_option"].ToString());
                            if (!dr["id_section"].ToString().Trim().Equals("")) varclsinscription.Id_section = int.Parse(dr["id_section"].ToString());
                            varclsinscription.Designation_complete = dr["promotion"].ToString() + dr["optio"].ToString() + dr["section"].ToString() + "_" + dr["anneeacademique"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsinscription;
        }

        public List<clsinscription> getAllClsinscription(string criteria)
        {
            List<clsinscription> lstclsinscription = new List<clsinscription>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = @"SELECT * FROM inscription WHERE 1=1";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsinscription varclsinscription = null;
                        while (dr.Read())
                        {
                            varclsinscription = new clsinscription();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsinscription.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_anneeacademique"].ToString().Trim().Equals("")) varclsinscription.Id_anneeacademique = int.Parse(dr["id_anneeacademique"].ToString());
                            if (!dr["id_promotion"].ToString().Trim().Equals("")) varclsinscription.Id_promotion = int.Parse(dr["id_promotion"].ToString());
                            if (!dr["id_option"].ToString().Trim().Equals("")) varclsinscription.Id_option = int.Parse(dr["id_option"].ToString());
                            if (!dr["id_section"].ToString().Trim().Equals("")) varclsinscription.Id_section = int.Parse(dr["id_section"].ToString());
                            lstclsinscription.Add(varclsinscription);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsinscription;
        }

        public List<clsinscription> getAllClsinscription2()
        {
            List<clsinscription> lstclsinscription = new List<clsinscription>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT inscription.*,promotion.designation AS promotion,optio.designation AS optio,section.designation AS section,anneeacademique.designation AS anneeacademique FROM inscription
                    INNER JOIN promotion ON promotion.id=inscription.id_promotion
                    INNER JOIN optio ON optio.id=inscription.id_option
                    INNER JOIN section ON section.id= inscription.id_section
                    INNER JOIN anneeacademique ON anneeacademique.id=inscription.id_anneeacademique");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsinscription varclsinscription = null;
                        while (dr.Read())
                        {
                            varclsinscription = new clsinscription();

                            if (!dr["id"].ToString().Trim().Equals("")) varclsinscription.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_anneeacademique"].ToString().Trim().Equals("")) varclsinscription.Id_anneeacademique = int.Parse(dr["id_anneeacademique"].ToString());
                            if (!dr["id_promotion"].ToString().Trim().Equals("")) varclsinscription.Id_promotion = int.Parse(dr["id_promotion"].ToString());
                            if (!dr["id_option"].ToString().Trim().Equals("")) varclsinscription.Id_option = int.Parse(dr["id_option"].ToString());
                            if (!dr["id_section"].ToString().Trim().Equals("")) varclsinscription.Id_section = int.Parse(dr["id_section"].ToString());
                            varclsinscription.Designation_complete = dr["promotion"].ToString() + dr["optio"].ToString() + dr["section"].ToString() + "_" + dr["anneeacademique"].ToString();
                            lstclsinscription.Add(varclsinscription);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsinscription;
        }

        public List<clsinscription> getAllClsinscription()
        {
            List<clsinscription> lstclsinscription = new List<clsinscription>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT * FROM inscription");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsinscription varclsinscription = null;
                        while (dr.Read())
                        {
                            varclsinscription = new clsinscription();

                            if (!dr["id"].ToString().Trim().Equals("")) varclsinscription.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_anneeacademique"].ToString().Trim().Equals("")) varclsinscription.Id_anneeacademique = int.Parse(dr["id_anneeacademique"].ToString());
                            if (!dr["id_promotion"].ToString().Trim().Equals("")) varclsinscription.Id_promotion = int.Parse(dr["id_promotion"].ToString());
                            if (!dr["id_option"].ToString().Trim().Equals("")) varclsinscription.Id_option = int.Parse(dr["id_option"].ToString());
                            if (!dr["id_section"].ToString().Trim().Equals("")) varclsinscription.Id_section = int.Parse(dr["id_section"].ToString());
                            lstclsinscription.Add(varclsinscription);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsinscription;
        }

        public DataTable getAllClsinscription1()
        {
            DataTable lstclsinscription = new DataTable("tbl");
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT * FROM inscription ORDER BY id ASC");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        lstclsinscription.Load(dr);
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsinscription;
        }

        public int insertClsinscription(clsinscription varclsinscription)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO inscription ( id_anneeacademique,id_promotion,id_option,id_section ) VALUES (@id_anneeacademique,@id_promotion,@id_option,@id_section)");
                    cmd.Parameters.Add(getParameter(cmd, "@id_anneeacademique", DbType.Int32, 4, varclsinscription.Id_anneeacademique));
                    cmd.Parameters.Add(getParameter(cmd, "@id_promotion", DbType.Int32, 4, varclsinscription.Id_promotion));
                    cmd.Parameters.Add(getParameter(cmd, "@id_option", DbType.Int32, 4, varclsinscription.Id_option));
                    cmd.Parameters.Add(getParameter(cmd, "@id_section", DbType.Int32, 4, varclsinscription.Id_section));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsinscription(clsinscription varclsinscription)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE etudiant  SET id_anneeacademique=@id_anneeacademique,id_promotion=@id_promotion,id_option=@id_option,id_section=@id_section WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_anneeacademique", DbType.Int32, 4, varclsinscription.Id_anneeacademique));
                    cmd.Parameters.Add(getParameter(cmd, "@id_promotion", DbType.Int32, 4, varclsinscription.Id_promotion));
                    cmd.Parameters.Add(getParameter(cmd, "@id_option", DbType.Int32, 4, varclsinscription.Id_option));
                    cmd.Parameters.Add(getParameter(cmd, "@id_section", DbType.Int32, 4, varclsinscription.Id_section));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsinscription.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsinscription(clsinscription varclsinscription)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM etudiant  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsinscription.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSINSCRIPTION
        #region  CLSETUDIANT
        public clsetudiant getClsetudiant(object intid)
        {
            clsetudiant varclsetudiant = new clsetudiant();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,etudiant.id AS idEtud,etudiant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,etudiant.matricule,etudiant.id_inscription from personne
                    INNER JOIN etudiant ON personne.id=etudiant.id_personne  WHERE etudiant.id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //Etudiant
                            if (!dr["idEtud"].ToString().Trim().Equals("")) varclsetudiant.IdEtudiant = int.Parse(dr["idEtud"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsetudiant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            //if (!dr["id_inscription"].ToString().Trim().Equals("")) varclsetudiant.Id_inscription = int.Parse(dr["id_inscription"].ToString());
                            varclsetudiant.Matricule = dr["matricule"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsetudiant.IdPers = int.Parse(dr["id"].ToString());
                            varclsetudiant.Nom = dr["nom"].ToString();
                            varclsetudiant.Postnom = dr["postnom"].ToString();
                            varclsetudiant.Prenom = dr["prenom"].ToString();
                            varclsetudiant.Sexe = dr["sexe"].ToString();
                            varclsetudiant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsetudiant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsetudiant.Telephone = dr["telephone"].ToString();
                            varclsetudiant.Email = dr["email"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsetudiant;
        }

        public List<clsetudiant> getAllClsetudiant(string criteria)
        {
            List<clsetudiant> lstclsetudiant = new List<clsetudiant>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = @"SELECT personne.id,etudiant.id AS idEtud,etudiant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,etudiant.matricule,etudiant.id_inscription from personne
                    INNER JOIN etudiant ON personne.id=etudiant.id_personne  WHERE 1=1";
                    sql += "  OR   etudiant.matricule LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.nom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.postnom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.prenom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.sexe LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.etatcivil LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.telephone LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.email LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsetudiant varclsetudiant = null;
                        while (dr.Read())
                        {
                            varclsetudiant = new clsetudiant();
                            //Etudiant
                            if (!dr["idEtud"].ToString().Trim().Equals("")) varclsetudiant.IdEtudiant = int.Parse(dr["idEtud"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsetudiant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            if (!dr["id_inscription"].ToString().Trim().Equals("")) varclsetudiant.Id_inscription = int.Parse(dr["id_inscription"].ToString());
                            varclsetudiant.Matricule = dr["matricule"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsetudiant.IdPers = int.Parse(dr["id"].ToString());
                            varclsetudiant.Nom = dr["nom"].ToString();
                            varclsetudiant.Postnom = dr["postnom"].ToString();
                            varclsetudiant.Prenom = dr["prenom"].ToString();
                            varclsetudiant.Sexe = dr["sexe"].ToString();
                            varclsetudiant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsetudiant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsetudiant.Telephone = dr["telephone"].ToString();
                            varclsetudiant.Email = dr["email"].ToString();
                            lstclsetudiant.Add(varclsetudiant);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsetudiant;
        }

        public List<clsetudiant> getAllClsetudiant()
        {
            List<clsetudiant> lstclsetudiant = new List<clsetudiant>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,etudiant.id AS idEtud,etudiant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,etudiant.matricule,etudiant.id_inscription from personne
                    INNER JOIN etudiant ON personne.id=etudiant.id_personne");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsetudiant varclsetudiant = null;
                        while (dr.Read())
                        {
                            varclsetudiant = new clsetudiant();

                            //Etudiant
                            if (!dr["idEtud"].ToString().Trim().Equals("")) varclsetudiant.IdEtudiant = int.Parse(dr["idEtud"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsetudiant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            if (!dr["id_inscription"].ToString().Trim().Equals("")) varclsetudiant.Id_inscription = int.Parse(dr["id_inscription"].ToString());
                            varclsetudiant.Matricule = dr["matricule"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsetudiant.IdPers = int.Parse(dr["id"].ToString());
                            varclsetudiant.Nom = dr["nom"].ToString();
                            varclsetudiant.Postnom = dr["postnom"].ToString();
                            varclsetudiant.Prenom = dr["prenom"].ToString();
                            varclsetudiant.Sexe = dr["sexe"].ToString();
                            varclsetudiant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsetudiant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsetudiant.Telephone = dr["telephone"].ToString();
                            varclsetudiant.Email = dr["email"].ToString();
                            lstclsetudiant.Add(varclsetudiant);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsetudiant;
        }

        public List<clsetudiant> getAllClsetudiant(int id_anneeacademique,int id_promotion,int id_option,int id_section)
        {
            List<clsetudiant> lstclsetudiant = new List<clsetudiant>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,etudiant.id AS idEtud,etudiant.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,etudiant.matricule,etudiant.id_inscription from personne
                    INNER JOIN etudiant ON personne.id=etudiant.id_personne
                    INNER JOIN inscription ON inscription.id=etudiant.id_inscription
                    INNER JOIN anneeacademique ON anneeacademique.id=inscription.id_anneeacademique
                    INNER JOIN promotion ON promotion.id=inscription.id_promotion
                    INNER JOIN optio ON optio.id=inscription.id_option
                    INNER JOIN section ON section.id=inscription.id_section
                    WHERE anneeacademique.id={0} AND promotion.id={1} AND optio.id={2} AND section.id={3} ",id_anneeacademique,id_promotion,id_option,id_section);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsetudiant varclsetudiant = null;
                        while (dr.Read())
                        {
                            varclsetudiant = new clsetudiant();

                            //Etudiant
                            if (!dr["idEtud"].ToString().Trim().Equals("")) varclsetudiant.IdEtudiant = int.Parse(dr["idEtud"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsetudiant.Id_personne = int.Parse(dr["id_personne"].ToString());
                            if (!dr["id_inscription"].ToString().Trim().Equals("")) varclsetudiant.Id_inscription = int.Parse(dr["id_inscription"].ToString());
                            varclsetudiant.Matricule = dr["matricule"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsetudiant.IdPers = int.Parse(dr["id"].ToString());
                            varclsetudiant.Nom = dr["nom"].ToString();
                            varclsetudiant.Postnom = dr["postnom"].ToString();
                            varclsetudiant.Prenom = dr["prenom"].ToString();
                            varclsetudiant.Sexe = dr["sexe"].ToString();
                            varclsetudiant.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsetudiant.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsetudiant.Telephone = dr["telephone"].ToString();
                            varclsetudiant.Email = dr["email"].ToString();
                            lstclsetudiant.Add(varclsetudiant);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsetudiant;
        }

        public DataTable getAllClsetudiant1()
        {
            DataTable lstclsetudiant = new DataTable("tbl");
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,etudiant.id AS idEtud,etudiant.id_personne,isnull(personne.nom,'') + ' ' + isnull(personne.postnom,'') + ' ' + isnull(personne.prenom,'') AS nom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,etudiant.matricule,etudiant.id_inscription from personne
                    INNER JOIN etudiant ON personne.id=etudiant.id_personne ORDER BY personne.nom ASC");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        lstclsetudiant.Load(dr);
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsetudiant;
        }

        public int insertClsetudiant(clsetudiant varclsetudiant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO etudiant ( id_personne,id_inscription,matricule ) VALUES (@id_personne,@id_inscription,@matricule)");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsetudiant.Id_personne));
                    cmd.Parameters.Add(getParameter(cmd, "@id_inscription", DbType.Int32, 4, varclsetudiant.Id_inscription));
                    if (varclsetudiant.Matricule != null) cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, varclsetudiant.Matricule));
                    else cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsetudiant(clsetudiant varclsetudiant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE etudiant  SET id_personne=@id_personne,id_inscription=@id_inscription,matricule=@matricule WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsetudiant.Id_personne));
                    cmd.Parameters.Add(getParameter(cmd, "@id_inscription", DbType.Int32, 4, varclsetudiant.Id_inscription));
                    if (varclsetudiant.Matricule != null) cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, varclsetudiant.Matricule));
                    else cmd.Parameters.Add(getParameter(cmd, "@matricule", DbType.String, 20, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsetudiant.IdEtudiant));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsetudiant(clsetudiant varclsetudiant)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM etudiant  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsetudiant.IdEtudiant));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }
        #endregion CLSETUDIANT
        #region  CLSEXTERNES
        public clsexternes getClsexternes(object intid)
        {
            clsexternes varclsexternes = new clsexternes();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,externes.id AS idEns,externes.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,externes.observation from personne
                    INNER JOIN externes ON personne.id=externes.id_personne  WHERE externes.id={0}", intid);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            //Externes
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsexternes.IdExternes = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsexternes.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsexternes.Observation = dr["observation"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsexternes.IdPers = int.Parse(dr["id"].ToString());
                            varclsexternes.Nom = dr["nom"].ToString();
                            varclsexternes.Postnom = dr["postnom"].ToString();
                            varclsexternes.Prenom = dr["prenom"].ToString();
                            varclsexternes.Sexe = dr["sexe"].ToString();
                            varclsexternes.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsexternes.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsexternes.Telephone = dr["telephone"].ToString();
                            varclsexternes.Email = dr["email"].ToString();
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsexternes;
        }

        public List<clsexternes> getAllClsexternes(string criteria)
        {
            List<clsexternes> lstclsexternes = new List<clsexternes>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    string sql = @"SELECT personne.id,externes.id AS idEns,externes.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,externes.observation from personne
                    INNER JOIN externes ON personne.id=externes.id_personne  WHERE 1=1";
                    sql += "  OR   externes.observation LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.nom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.postnom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.prenom LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.sexe LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.etatcivil LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.telephone LIKE '%" + criteria + "%'";
                    sql += "  OR   personne.email LIKE '%" + criteria + "%'";
                    cmd.CommandText = string.Format(sql);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsexternes varclsexternes = null;
                        while (dr.Read())
                        {
                            varclsexternes = new clsexternes();
                            //externes
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsexternes.IdExternes = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsexternes.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsexternes.Observation = dr["observation"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsexternes.IdPers = int.Parse(dr["id"].ToString());
                            varclsexternes.Nom = dr["nom"].ToString();
                            varclsexternes.Postnom = dr["postnom"].ToString();
                            varclsexternes.Prenom = dr["prenom"].ToString();
                            varclsexternes.Sexe = dr["sexe"].ToString();
                            varclsexternes.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsexternes.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsexternes.Telephone = dr["telephone"].ToString();
                            varclsexternes.Email = dr["email"].ToString();
                            //if (!dr["photo"].ToString().Trim().Equals("")) varclsexternes.Photo = (Byte[])dr["photo"];
                            lstclsexternes.Add(varclsexternes);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsexternes;
        }

        public List<clsexternes> getAllClsexternes()
        {
            List<clsexternes> lstclsexternes = new List<clsexternes>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT personne.id,externes.id AS idEns,externes.id_personne,personne.nom,personne.postnom,personne.prenom,personne.sexe,personne.etatcivil,personne.datenaissance,personne.telephone,personne.email,externes.observation from personne
                    INNER JOIN externes ON personne.id=externes.id_personne");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        clsexternes varclsexternes = null;
                        while (dr.Read())
                        {
                            varclsexternes = new clsexternes();

                            //externes
                            if (!dr["idEns"].ToString().Trim().Equals("")) varclsexternes.IdExternes = int.Parse(dr["idEns"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsexternes.Id_personne = int.Parse(dr["id_personne"].ToString());
                            varclsexternes.Observation = dr["observation"].ToString();

                            //Personne
                            if (!dr["id"].ToString().Trim().Equals("")) varclsexternes.IdPers = int.Parse(dr["id"].ToString());
                            varclsexternes.Nom = dr["nom"].ToString();
                            varclsexternes.Postnom = dr["postnom"].ToString();
                            varclsexternes.Prenom = dr["prenom"].ToString();
                            varclsexternes.Sexe = dr["sexe"].ToString();
                            varclsexternes.Etatcivil = dr["etatcivil"].ToString();
                            if (!dr["datenaissance"].ToString().Trim().Equals("")) varclsexternes.Datenaissance = DateTime.Parse(dr["datenaissance"].ToString());
                            varclsexternes.Telephone = dr["telephone"].ToString();
                            varclsexternes.Email = dr["email"].ToString();
                            lstclsexternes.Add(varclsexternes);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsexternes;
        }

        public int insertClsexternes(clsexternes varclsexternes)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO externes ( id_personne,observation ) VALUES (@id_personne,@observation )");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsexternes.Id_personne));
                    if (varclsexternes.Observation != null) cmd.Parameters.Add(getParameter(cmd, "@observation", DbType.String, 30, varclsexternes.Observation));
                    else cmd.Parameters.Add(getParameter(cmd, "@observation", DbType.String, 100, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsexternes(clsexternes varclsexternes)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE externes  SET id_personne=@id_personne,observation=@observation  WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsexternes.Id_personne));
                    if (varclsexternes.Observation != null) cmd.Parameters.Add(getParameter(cmd, "@observation", DbType.String, 30, varclsexternes.Observation));
                    else cmd.Parameters.Add(getParameter(cmd, "@observation", DbType.String, 100, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsexternes.IdExternes));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsexternes(clsexternes varclsexternes)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM externes  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsexternes.IdExternes));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSEXTERNES
        #region  CLSPHOTO
        public clsphoto getClsphoto(object intid_personne)
        {
            clsphoto varclsphoto = new clsphoto();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT photo.* FROM photo
                    INNER JOIN personne ON personne.id=photo.id_personne
                    WHERE personne.id={0}", intid_personne);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (!dr["id"].ToString().Trim().Equals("")) varclsphoto.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsphoto.Id_personne = int.Parse(dr["id_personne"].ToString());
                            if (!dr["photo"].ToString().Trim().Equals("")) varclsphoto.Photo = (Byte[])dr["photo"];
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsphoto;
        }

        public int getClscountphoto(object intid_personne)
        {
            int varclsphoto = -1;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(@"SELECT count(photo.id) AS nbr FROM photo
                    INNER JOIN personne ON personne.id=photo.id_personne
                    WHERE personne.id={0}", intid_personne);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) varclsphoto = int.Parse(dr["nbr"].ToString());
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return varclsphoto;
        }

        public List<clsphoto> getAllClsphoto()
        {
            List<clsphoto> lstclsphoto = new List<clsphoto>();
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("SELECT *  FROM photo ");
                    using (IDataReader dr = cmd.ExecuteReader())
                    {

                        clsphoto varclsphoto = null;
                        while (dr.Read())
                        {
                            varclsphoto = new clsphoto();
                            if (!dr["id"].ToString().Trim().Equals("")) varclsphoto.Id = int.Parse(dr["id"].ToString());
                            if (!dr["id_personne"].ToString().Trim().Equals("")) varclsphoto.Id_personne = int.Parse(dr["id_personne"].ToString());
                            if (!dr["photo"].ToString().Trim().Equals("")) varclsphoto.Photo = (Byte[])dr["photo"];
                            lstclsphoto.Add(varclsphoto);
                        }
                    }
                }
                conn.Close();
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return lstclsphoto;
        }

        public int insertClsphoto(clsphoto varclsphoto)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("INSERT INTO photo (id_personne,photo ) VALUES (@id_personne,@photo  )");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.String, 30, varclsphoto.Id_personne));
                    if (varclsphoto.Photo != null) cmd.Parameters.Add(getParameter(cmd, "@photo", DbType.Binary, Int32.MaxValue, varclsphoto.Photo));
                    else cmd.Parameters.Add(getParameter(cmd, "@photo", DbType.Binary, Int32.MaxValue, DBNull.Value));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int updateClsphoto(clsphoto varclsphoto)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE photo  SET id_personne=@id_personne,photo=@photo  WHERE 1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id_personne", DbType.Int32, 4, varclsphoto.Id_personne));
                    if (varclsphoto.Photo != null) cmd.Parameters.Add(getParameter(cmd, "@photo", DbType.Binary, Int32.MaxValue, varclsphoto.Photo));
                    else cmd.Parameters.Add(getParameter(cmd, "@photo", DbType.Binary, Int32.MaxValue, DBNull.Value));
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 4, varclsphoto.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        public int deleteClsphoto(clsphoto varclsphoto)
        {
            int i = 0;
            try
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format("DELETE FROM photo  WHERE  1=1  AND id=@id ");
                    cmd.Parameters.Add(getParameter(cmd, "@id", DbType.Int32, 100, varclsphoto.Id));
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception exc)
            {
                conn.Close();
                throw new Exception(exc.Message);
            }
            return i;
        }

        #endregion CLSPHOTO 

    } //***fin class 
} //***fin namespace 
