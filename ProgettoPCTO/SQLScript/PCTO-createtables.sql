-- SCRIPT PER CREAZIONE DATABASE E TABELLE

CREATE DATABASE Videogame;
GO

USE Videogame
GO

CREATE TABLE Account(
	Username CHAR(15) PRIMARY KEY NOT NULL,
	Email CHAR(340),
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
	Name CHAR(20),
	Description VARCHAR(100),
	ImageURL VARCHAR(50),
	UnlockingItem CHAR(15),
	IDForward INT REFERENCES Situation(IDSituation),
	IDRight INT REFERENCES Situation(IDSituation),
	IDBackward INT REFERENCES Situation(IDSituation),
	IDLeft INT REFERENCES Situation(IDSituation)
	
);

CREATE TABLE SituationVariable(
	IDInstance INT PRIMARY KEY NOT NULL REFERENCES Situation(IDSituation),
	Unlocked BIT DEFAULT 0,
	IDGameplay INT REFERENCES Gameplay(IDGameplay)
);

CREATE TABLE Action(
	IDSituation INT REFERENCES Situation(IDSituation) ON DELETE CASCADE,
	Dialogue VARCHAR(100),
	IDGameplay INT REFERENCES Gameplay(IDGameplay) ON DELETE CASCADE
);

CREATE TABLE Image(
	IDImage INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Name CHAR(20),
	Description VARCHAR(100),
	X REAL,
	Y REAL,
	ImageURL VARCHAR(50),
	Width INT,
	Height INT,
	Dialogue VARCHAR(1000),
	IsCharacter BIT DEFAULT 0,
	IsItem BIT DEFAULT 0,
	IDSituation INT REFERENCES Situation(IDSituation)
);

CREATE TABLE Item(
	IDItem INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	IsCollectable BIT NOT NULL,
	IsVisible BIT NOT NULL,
	Effectiveness INT CHECK(Effectiveness >= 0),
	IDPlayer INT DEFAULT NULL,
	IDImage INT REFERENCES Image(IDImage) ON DELETE CASCADE ON UPDATE CASCADE,
	IDGameplay INT REFERENCES Gameplay(IDGameplay) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Character(
	IDCharacter INT PRIMARY KEY NOT NULL IDENTITY(1,1),
	Strength INT CHECK(Strength >= 0),
	IsVisible BIT NOT NULL,
	EffectiveWeapon CHAR(15),
	IDImage INT REFERENCES Image(IDImage) ON DELETE CASCADE ON UPDATE CASCADE,
	IDGameplay INT REFERENCES Gameplay(IDGameplay) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Player(
	IDCharacter INT PRIMARY KEY NOT NULL REFERENCES Character(IDCharacter),
	Health INT CHECK(Health >= 0),
	Armor INT CHECK(Armor >= 0),
	Experience INT CHECK(Experience >= 0),
	IDGameplay INT REFERENCES Gameplay(IDGameplay) ON DELETE CASCADE ON UPDATE CASCADE
);

ALTER TABLE Item
ADD CONSTRAINT FKPlayer
FOREIGN KEY (IDPlayer) REFERENCES Player(IDCharacter);

