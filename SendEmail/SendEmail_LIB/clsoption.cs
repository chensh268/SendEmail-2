using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsoption 
{
  //***Les variables globales***
 private int   id ;
 private string   designation ;
  //***Listes***
  public List<clsoption> listes(){
 return clsMetier.GetInstance().getAllClsoption();
}
 public  List<clsoption>   listes(string criteria){
 return clsMetier.GetInstance().getAllClsoption(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClsoption(this);
 }
 public  int  update(clsoption varscls){
 return clsMetier.GetInstance().updateClsoption(varscls);
 }
 public  int  delete(clsoption varscls){
 return clsMetier.GetInstance().deleteClsoption(varscls);
 }
  //***Le constructeur par defaut***
  public    clsoption() 
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
