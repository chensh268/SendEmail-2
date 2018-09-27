using System;
using System.Collections.Generic;

namespace SendEmail_LIB 
{
  public class   clsphoto 
{
  //***Les variables globales***
 private int   id ;
 private int   id_personne ;
 private Byte[]   photo ;
  //***Listes***
  public List<clsphoto> listes(){
 return clsMetier.GetInstance().getAllClsphoto();
}
 public  int  inserts(){
 return clsMetier.GetInstance().insertClsphoto(this);
 }
 public  int  update(clsphoto varscls){
 return clsMetier.GetInstance().updateClsphoto(varscls);
 }
 public  int  delete(clsphoto varscls){
 return clsMetier.GetInstance().deleteClsphoto(varscls);
 }
  //***Le constructeur par defaut***
  public    clsphoto() 
{
}

  //***Accesseur de id***
 public  int   Id {

get { return id; } 
set { id = value; }
}

 //***Accesseur de id_personne***
 public int Id_personne{     
get { return id_personne; }
set { id_personne = value; }
 }

  //***Accesseur de photo***
 public  Byte[]   Photo {

get { return photo; } 
set { photo = value; }
}
} //***fin class

} //***fin namespace
