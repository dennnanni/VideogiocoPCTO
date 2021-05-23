USE Videogame

SELECT IDENT_CURRENT('Situation') AS [Identity];
DBCC CHECKIDENT('Situation', NORESEED);

SELECT * 
FROM Situation AS S LEFT JOIN SituationVariable AS SV ON S.IDSituation = SV.IDInstance
WHERE SV.IDGameplay = 1 OR SV.IDInstance IS NULL;

SELECT * 
FROM Image INNER JOIN Item ON Image.IDImage = Item.IDImage;

SELECT *
FROM Gameplay;

SELECT * 
FROM Situation;

SELECT * 
FROM Image;

SELECT * 
FROM Character;

SELECT * 
FROM Item;

SELECT * 
FROM Player;

SELECT * 
FROM Action;

SELECT * 
FROM Account;

