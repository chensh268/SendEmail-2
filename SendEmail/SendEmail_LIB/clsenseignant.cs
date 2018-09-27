using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsenseignant : clspersonne
{
  //***Les variables globales***
 private int   id ;
 private int   id_personne ;
 private string   grade ;
 private DateTime?  dateangagement ;
  //***Listes***
  public new List<clsenseignant> listes(){
 return clsMetier.GetInstance().getAllClsenseignant();
}
 public  new List<clsenseignant>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsenseignant(criteria);
 }
 public  new int  inserts(){
 return clsMetier.GetInstance().insertClsenseignant(this);
 }
 public  int  update(clsenseignant varscls){
 return clsMetier.GetInstance().updateClsenseignant(varscls);
 }
 public  int  delete(clsenseignant varscls){
 return clsMetier.GetInstance().deleteClsenseignant(varscls);
 }
  //***Le constructeur par defaut***
  public    clsenseignant() 
{
}

  //***Accesseur de id***
 public  int   IdEnseignant {

get { return id; } 
set { id = value; }
}
  //***Accesseur de id_personne***
 public  int   Id_personne {

get { return id_personne; } 
set { id_personne = value; }
}
  //***Accesseur de grade***
 public  string   Grade {

get { return grade; } 
set { grade = value; }
}
  //***Accesseur de dateangagement***
 public  DateTime ?   Dateangagement {

get { return dateangagement; } 
set { dateangagement = value; }
}
 public override string ToString()
 {
     return this.Email;
 }
} //***fin class

} //***fin namespace
