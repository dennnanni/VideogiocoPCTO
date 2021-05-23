USE Videogame

SELECT * 
FROM Situation AS S INNER JOIN SituationVariable AS SV ON S.IDSituation = SV.IDInstance;

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

