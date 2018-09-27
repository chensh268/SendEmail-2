--drop database sendemail
create database sendemail
go
use sendemail
go


create table personne
(
	id int identity,
	nom varchar(30) not null,
	postnom varchar(40),
	prenom varchar(30),
	sexe varchar(1) default 'M',
	etatcivil varchar(15) default 'Celibataire',
	datenaissance smalldatetime,
	telephone varchar(100),
	email varchar(100),
	constraint pk_personne primary key(id)
)
go

create table photo
(
	id int identity,
	id_personne int not null,
	photo image,
	constraint pk_photo primary key(id),
	constraint fk_photo_id_personne foreign key(id_personne) references personne(id)
)
go
 
create table agent
(
	id int identity,
	id_personne int not null,
	matricule varchar(20),
	grade varchar(30),
	dateangagement smalldatetime default GETDATE(),
	numeroinss varchar(20)
	constraint pk_agent primary key(id),
	constraint fk_agent_id_personne foreign key(id_personne) references personne(id)
)
go

create table enseignant
(
	id int identity,
	id_personne int not null,
	grade varchar(30),
	dateangagement smalldatetime default GETDATE(),
	constraint pk_enseignant primary key(id),
	constraint fk_enseignant_id_personne foreign key(id_personne) references personne(id)
)
go

create table externes
(
	id int identity,
	id_personne int not null,
	observation varchar(100),
	constraint pk_externes primary key(id),
	constraint fk_externes_id_personne foreign key(id_personne) references personne(id)
)
go

create table promotion
(
	id int identity,
	designation varchar(50) not null,
	constraint pk_promotion primary key(id),
	constraint uk_promotion_designation unique(designation)
)
go

create table optio
(
	id int identity,
	designation varchar(50) not null,
	constraint pk_option primary key(id),
	constraint uk_option_designation unique(designation)
)
go

create table section
(
	id int identity,
	designation varchar(50) not null,
	constraint pk_section primary key(id),
	constraint uk_section_designation unique(designation)
)
go

create table anneeacademique
(
	id int identity,
	designation varchar(50) not null,
	constraint pk_anneeacademique primary key(id),
	constraint uk_anneeacademique_designation unique(designation)
)
go

create table inscription
(
	id int identity,
	id_anneeacademique int not null,
	id_promotion int not null,
	id_option int not null,
	id_section int not null,
	constraint pk_inscription primary key(id),
	constraint fk_inscription_id_promotion foreign key(id_promotion) references promotion(id),
	constraint fk_inscription_id_option foreign key(id_option) references optio(id),
	constraint fk_inscription_id_section foreign key(id_section) references section(id),
	constraint fk_inscription_id_anneeacademique foreign key(id_anneeacademique) references anneeacademique(id)
)
go

create table etudiant
(
	id int identity,
	id_personne int not null,
	id_inscription int not null,
	matricule varchar(20) not null,
	constraint pk_etudiant primary key(id),
	constraint fk_etudiant_id_inscription foreign key(id_inscription) references inscription(id),
	constraint fk_etudiant_id_personne foreign key(id_personne) references personne(id),
	constraint uk_etudiant_matricule unique(matricule)
)
go

