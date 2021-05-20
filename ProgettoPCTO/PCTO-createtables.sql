-- SCRIPT PER CREAZIONE DATABASE E TABELLE

CREATE DATABASE Videogame;
GO

USE Videogame

CREATE TABLE Account(
	Username CHAR(15) PRIMARY KEY NOT NULL,
	Password CHAR(256)
);

-- CREAZIONE TABELLE
CREATE TABLE Gameplay(
	IDGameplay INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	--CurrentAreaID INT REFERENCES Situation(IDSituation) 
	CurrentAreaID CHAR(7), -- Da controllare meglio
	Username CHAR(15) REFERENCES Account(Username) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Situation(
	IDSituation INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Title VARCHAR(7),
	Name CHAR(7),
	Description VARCHAR(100),
	ImageURL VARCHAR(50),
	UnlockingItem CHAR(15),
	IDGameplay INT REFERENCES Gameplay(IDGameplay),
	IDForward INT REFERENCES Situation(IDSituation),
	IDRight INT REFERENCES Situation(IDSituation),
	IDBackward INT REFERENCES Situation(IDSituation),
	IDLeft INT REFERENCES Situation(IDSituation)
	
);

CREATE TABLE Action(
	IDSituation INT REFERENCES Situation(IDSituation) ON DELETE CASCADE,
	Dialogue VARCHAR(100)
);

CREATE TABLE Image(
	IDImage INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Name CHAR(20),
	Description VARCHAR(100),
	X REAL,
	Y REAL,
	ImageURL VARCHAR(50),
	Width INT,
	Heigth INT,
	Dialogue VARCHAR(100),
	IsCharacter BIT,
	IsItem BIT,
	IDSituation INT REFERENCES Situation(IDSituation)
);

CREATE TABLE Item(
	IDItem INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	IsCollectable BIT NOT NULL,
	IsVisible BIT NOT NULL,
	Effectiveness INT CHECK(Effectiveness >= 0),
	IDPlayer INT,
	IDImage INT REFERENCES Image(IDImage) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Character(
	IDCharacter INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Strength INT CHECK(Strength >= 0),
	Health INT CHECK(Health >= 0),
	IsVisible BIT NOT NULL,
	EffectiveWeapon CHAR(15),
	IDImage INT REFERENCES Image(IDImage) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Player(
	IDCharacter INT PRIMARY KEY NOT NULL REFERENCES Character(IDCharacter),
	Armor INT CHECK(Armor >= 0),
	Experience INT CHECK(Experience >= 0),
	IDGameplay INT REFERENCES Gameplay(IDGameplay) ON DELETE CASCADE ON UPDATE CASCADE
);

ALTER TABLE Situation
ADD CONSTRAINT FKItem
FOREIGN KEY (IDUnlockingItem) REFERENCES Image(IDImage);

ALTER TABLE Item
ADD CONSTRAINT FKPlayer
FOREIGN KEY (IDPlayer) REFERENCES Player(IDCharacter) ON DELETE SET NULL ON UPDATE CASCADE;

