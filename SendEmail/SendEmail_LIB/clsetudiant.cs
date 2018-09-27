using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsetudiant : clspersonne
{
  //***Les variables globales***
 private int   id ;
 private int   id_inscription ;
 private int   id_personne ;
 private string   matricule ;

  //***Listes***
  public new List<clsetudiant> listes(){
 return clsMetier.GetInstance().getAllClsetudiant();
}
 public  new List<clsetudiant>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsetudiant(criteria);
 }
 public  new int  inserts(){
 return clsMetier.GetInstance().insertClsetudiant(this);
 }
 public  int  update(clsetudiant varscls){
 return clsMetier.GetInstance().updateClsetudiant(varscls);
 }
 public  int  delete(clsetudiant varscls){
 return clsMetier.GetInstance().deleteClsetudiant(varscls);
 }
  //***Le constructeur par defaut***
  public    clsetudiant() 
{
}

  //***Accesseur de id***
 public  int   IdEtudiant {

get { return id; } 
set { id = value; }
}
 //***Accesseur de id_personne***
 public int Id_personne{
 get { return id_personne; }
 set { id_personne = value; }
 }
  //***Accesseur de id_inscription***
 public  int   Id_inscription {

get { return id_inscription; } 
set { id_inscription = value; }
}
  //***Accesseur de matricule***
 public  string   Matricule {

get { return matricule; } 
set { matricule = value; }
}
 public override string ToString()
 {
     return this.Email;
 }
 }
} //***fin namespace
