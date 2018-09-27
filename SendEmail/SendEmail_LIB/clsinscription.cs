using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsinscription
{
  //***Les variables globales***
 private int   id ;
 private int   id_promotion ;
 private int   id_option ;
 private int   id_section ;
 private int   id_anneeacademique ;
 private string designation_complete;

  //***Listes***
  public List<clsinscription> listes(){
 return clsMetier.GetInstance().getAllClsinscription();
}
 public  List<clsinscription>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsinscription(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClsinscription(this);
 }
 public  int  update(clsinscription varscls){
 return clsMetier.GetInstance().updateClsinscription(varscls);
 }
 public  int  delete(clsinscription varscls){
 return clsMetier.GetInstance().deleteClsinscription(varscls);
 }
  //***Le constructeur par defaut***
  public    clsinscription() 
{
}

  //***Accesseur de id***
 public  int   Id {

get { return id; } 
set { id = value; }
}
  //***Accesseur de id_anneeacademique***
 public  int   Id_anneeacademique {

get { return id_anneeacademique; } 
set { id_anneeacademique = value; }
}
  //***Accesseur de id_promotion***
 public  int   Id_promotion {

get { return id_promotion; } 
set { id_promotion = value; }
}
  //***Accesseur de id_option***
 public  int   Id_option {

get { return id_option; } 
set { id_option = value; }
}
  //***Accesseur de id_section***
 public  int   Id_section {

get { return id_section; } 
set { id_section = value; }
}
 //***Accesseur de designation_complete***
 public string Designation_complete
 {
     get { return designation_complete; }
     set { designation_complete = value; }
 }

 public override string ToString()
 {
     return Designation_complete;
 }
 }
} //***fin namespace
