using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsanneeacademique 
{
  //***Les variables globales***
 private int   id ;
 private string   designation ;
  //***Listes***
  public List<clsanneeacademique> listes(){
 return clsMetier.GetInstance().getAllClsanneeacademique();
}
 public  List<clsanneeacademique>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsanneeacademique(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClsanneeacademique(this);
 }
 public  int  update(clsanneeacademique varscls){
 return clsMetier.GetInstance().updateClsanneeacademique(varscls);
 }
 public  int  delete(clsanneeacademique varscls){
 return clsMetier.GetInstance().deleteClsanneeacademique(varscls);
 }
  //***Le constructeur par defaut***
  public    clsanneeacademique() 
{
}

  //***Accesseur de id***
 public  int   Id {

get { return id; } 
set { id = value; }
}
  //***Accesseur de designation***
 public  string   Designation {

get { return designation; } 
set { designation = value; }
}
 //***Accesseur de designation***
 public override string ToString()
 {
     return this.Designation;
 }
} //***fin class

} //***fin namespace
