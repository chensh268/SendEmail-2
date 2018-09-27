using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clspromotion 
{
  //***Les variables globales***
 private int   id ;
 private string   designation ;
  //***Listes***
  public List<clspromotion> listes(){
 return clsMetier.GetInstance().getAllClspromotion();
}
 public  List<clspromotion>   listes(string criteria){
 return clsMetier.GetInstance().getAllClspromotion(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClspromotion(this);
 }
 public  int  update(clspromotion varscls){
 return clsMetier.GetInstance().updateClspromotion(varscls);
 }
 public  int  delete(clspromotion varscls){
 return clsMetier.GetInstance().deleteClspromotion(varscls);
 }
  //***Le constructeur par defaut***
  public    clspromotion() 
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
