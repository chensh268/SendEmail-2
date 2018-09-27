using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clssection 
{
  //***Les variables globales***
 private int   id ;
 private string   designation ;
  //***Listes***
  public List<clssection> listes(){
 return clsMetier.GetInstance().getAllClssection();
}
 public  List<clssection>   listes(string criteria){
 return clsMetier.GetInstance().getAllClssection(criteria);
 }
 public  int  inserts(){
 return clsMetier.GetInstance().insertClssection(this);
 }
 public  int  update(clssection varscls){
 return clsMetier.GetInstance().updateClssection(varscls);
 }
 public  int  delete(clssection varscls){
 return clsMetier.GetInstance().deleteClssection(varscls);
 }
  //***Le constructeur par defaut***
  public    clssection() 
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
