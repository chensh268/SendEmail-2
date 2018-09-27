using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clspersonne 
{
  //***Les variables globales***
 private int   id ;
 private string   nom ;
 private string   postnom ;
 private string   prenom ;
 private string   sexe ;
 private string   etatcivil ;
 private DateTime?  datenaissance = DateTime.Today ;
 private string   telephone ;
 private string email;
 //private Byte[]   photo ;
  //***Listes***
  public List<clspersonne> listes(){
 return clsMetier.GetInstance().getAllClspersonne();
}
 public  List<clspersonne>   listes(string criteria){
 return clsMetier.GetInstance().getAllClspersonne(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClspersonne(this);
 }
 public  int  update(clspersonne varscls){
 return clsMetier.GetInstance().updateClspersonne(varscls);
 }
 public  int  delete(clspersonne varscls){
 return clsMetier.GetInstance().deleteClspersonne(varscls);
 }
  //***Le constructeur par defaut***
  public    clspersonne() 
{
}

  //***Accesseur de id***
 public  int   IdPers {

get { return id; } 
set { id = value; }
}
  //***Accesseur de nom***
 public  string   Nom {

get { return nom; }
     set { nom = value; }
}
  //***Accesseur de postnom***
 public  string   Postnom {

get { return postnom; } 
set { postnom = value; }
}
  //***Accesseur de prenom***
 public  string   Prenom {

get { return prenom; } 
set { prenom = value; }
}
  //***Accesseur de sexe***
 public  string   Sexe {

get { return sexe; } 
set { sexe = value; }
}
  //***Accesseur de etatcivil***
 public  string   Etatcivil {

get { return etatcivil; } 
set { etatcivil = value; }
}
  //***Accesseur de datenaissance***
 public  DateTime ?   Datenaissance {

get { return datenaissance; } 
set { datenaissance = value; }
}
  //***Accesseur de telephone***
 public  string   Telephone {

get { return telephone; } 
set { telephone = value; }
}
 //***Accesseur de email***
 public string Email{
 get { return email; }
 set { email = value; }
 }
  //***Accesseur de photo***
// public  Byte[]   Photo {

//get { return photo; } 
//set { photo = value; }
//}

 public string Nom_complet
 {
     get { return Nom + " " + Postnom + " " + Prenom; }
 }

 public override string ToString()
 {
     return Nom_complet;
 }

} //***fin class

} //***fin namespace
