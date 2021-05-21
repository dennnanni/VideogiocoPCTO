-- SCRIPT PER INSERT DEI DATI DI BASE
USE Videogame
GO

INSERT INTO Account VALUES ('default', 'default');

INSERT INTO Gameplay VALUES ('area0', 'default');

-- Insertion of some situations
-- 1
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area1', 'ingresso', 'Benvenuto, da qui inizia la tua avventura',
		'~\Img\Areas\area1.png');
-- 2
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area2', 'interno', '',
		'~\Img\Areas\area2.png');
-- 3
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area2a', 'vicolo cieco', 'L''unica soluzione è tornare indietro...',
		'~\Img\Areas\area2a.png');
-- 4
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area3', 'corridoio', '',
		'~\Img\Areas\area3.png');
-- 5		
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area4', 'bivio', 'Dove vuoi andare ora?',
		'~\Img\Areas\area4.png');
-- 6
INSERT INTO Situation(Title, Name, Description, ImageURL) 
VALUES ('area5', 'corridoio', 'Prosegui dritto',
		'~\Img\Areas\area5.png');

UPDATE Situation SET IDForward = 2,IDRight = NULL, IDBackward = NULL, IDLeft = NULL WHERE IDSituation = 1;
UPDATE Situation SET IDForward = 4, IDRight = NULL, IDBackward = 1, IDLeft = 3 WHERE IDSituation = 2;
UPDATE Situation SET IDForward = NULL, IDRight = NULL, IDBackward = 2, IDLeft = NULL WHERE IDSituation = 3;
UPDATE Situation SET IDForward = 5, IDRight = NULL, IDBackward = 2, IDLeft = NULL WHERE IDSituation = 4;
UPDATE Situation SET IDForward = NULL, IDRight = NULL, IDBackward = 4, IDLeft = 6 WHERE IDSituation = 5;

-- Insertion of player
INSERT INTO Character VALUES (50, 0, NULL, NULL);
INSERT INTO Player VALUES (1, 100, 0, 0, 1);

-- Insertion of Images
-- Steve 1
INSERT INTO Image VALUES ('Steve', 'Parla con lui per scoprire cosa vuole dirti.', 320, 160, '~\Img\Characters\steve.png',
110, 230, '"Io sono Steve e sarò la tua guida per le prime parti di questa avventura. Adesso ti spiegherò come utilizzare l''interfaccia:
- Puoi muoverti nel gioco con i pulsanti in basso a sinistra in cui sono indicate le direzioni in cui puoi proseguire. Alcuni passaggi saranno chiusi e ti serviranno degli oggetti per poterli aprire.
- Puoi trovare le azioni che puoi svolgere nell''ambiente qui sotto, premi "Agisci" per svolgere l''azione selezionata.
- Potrai trovare oggetti nella tua strada, alcuni ti permetteranno di aprire passaggi e altri ti proteggeranno. Un oggetto può essere messo nell''inventario attraverso l''azione, se questo non è già pieno; se dovesse esserlo, potrai scegliere un oggetto da lasciare. Gli oggetti scompaiono se lasciati, quindi fai attenzione!
- Incontrerai delle entità nel tuo cammino, alcune amiche, altre nemiche, scoprirai cosa vogliono da te."
Per iniziare, spostati all''interno del labirinto.', 1, 0, 1);
-- Sword 2
INSERT INTO Image VALUES ('Spada', 'Usala per uccidere i tuoi nemici.', 440, 360, '~\Img\Items\sword.png',
100, 90, 'Hai aggiunto Spada al tuo inventario!', 0, 1, 2);
--	Creeper 3
INSERT INTO Image VALUES ('Creeper', 'Uccidilo con la spada.', 250, 160, '~\Img\Characters\creeper.png',
220, 230, 'Hai ucciso il creeper!', 1, 0, 2);
-- Health potion 4
INSERT INTO Image VALUES ('Pozione della salute', 'Restituisce 30 punti salute!', 240, 360, '~\Img\Items\healthpotion.png',
60, 60, 'Hai aggiunto Pozione della salute al tuo inventario.', 0, 1, 3);

-- Insertion of characters
INSERT INTO Character VALUES (0, 1, NULL, 1);
INSERT INTO Character VALUES (5, 1, 'Spada', 3);

-- Insertion of items
INSERT INTO Item VALUES (1, 1, 0, NULL, 2);
INSERT INTO Item VALUES (1, 1, 30, NULL, 4);

-- Insert actions
INSERT INTO Action VALUES (1, 'Parla con Steve', 1);
INSERT INTO Action VALUES (2, 'Raccogli la spada', 1);
INSERT INTO Action VALUES (2, 'Uccidi il creeper', 1);
INSERT INTO Action VALUES (3, 'Raccogli la pozione della salute', 1);




