using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsexternes : clspersonne
{
  //***Les variables globales***
 private int   id ;
 private int   id_personne ;
 private string observation;
  //***Listes***
  public new List<clsexternes> listes(){
 return clsMetier.GetInstance().getAllClsexternes();
}
 public  new List<clsexternes>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsexternes(criteria);
 }
 public  new int  inserts(){
 return clsMetier.GetInstance().insertClsexternes(this);
 }
 public  int  update(clsexternes varscls){
 return clsMetier.GetInstance().updateClsexternes(varscls);
 }
 public  int  delete(clsexternes varscls){
 return clsMetier.GetInstance().deleteClsexternes(varscls);
 }
  //***Le constructeur par defaut***
  public    clsexternes() 
{
}

  //***Accesseur de id***
 public  int   IdExternes {

get { return id; } 
set { id = value; }
}
  //***Accesseur de id_personne***
 public  int   Id_personne {

get { return id_personne; } 
set { id_personne = value; }
}
 //***Accesseur de observation***
 public string Observation{
 get { return observation; }
 set { observation = value; }
 }

 public override string ToString()
 {
     return this.Email;
 }
} //***fin class

} //***fin namespace
