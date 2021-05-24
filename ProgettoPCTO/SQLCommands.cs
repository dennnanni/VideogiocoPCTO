﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace ProgettoPCTO
{
    public class SQLCommands
    {
        private string _connectionString = "";
        public SQLCommands(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Reader

        public Gameplay ReadData(string username)
        {
            Gameplay g = new Gameplay();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // TODO: Selezione nel database dei valori associati all'account

                conn.Open();
                ReadGameplay(username, g, conn);
                g.PlayerProfile = ReadPlayer(g.IdGameplay, conn);
                g.Situations = ReadSituations(g.IdGameplay, conn);
            }
            
            return g;
        }

        public bool ValidateUsernameAndEmail(string username, string email)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand select = new SqlCommand("SELECT Username, Email FROM Account WHERE Username = @Username OR Email = @Email;", conn);
                select.Parameters.AddWithValue("@Username", username);
                select.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = select.ExecuteReader();
                if(reader.Read())
                {
                    reader.Close();
                    return false;
                }
                else
                {
                    reader.Close();
                    return true;
                }

            }
        }

        private void ReadGameplay(string username, Gameplay g, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT IDGameplay, CurrentAreaID 
                                                 FROM Gameplay 
                                                 WHERE Gameplay.Username = @Username", conn);
            select.Parameters.AddWithValue("@Username", username);

            SqlDataReader reader = select.ExecuteReader();

            reader.Read();

            g.IdGameplay = int.Parse(reader["IDGameplay"].ToString());
            g.CurrentAreaID = reader["CurrentAreaID"].ToString().TrimEnd(null);

            reader.Close();
        }

        private Dictionary<string, Situation> ReadSituations(int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT S.IDSituation, S.Title, S.Name, S.Description, S.ImageURL, S.UnlockingItem, S.IDForward, S.IDRight, S.IDBackward, S.IDLeft, SV.Unlocked
                                                 FROM Situation AS S LEFT JOIN 
                                                      SituationVariable AS SV ON S.IDSituation = SV.IDInstance;", conn);
            select.Parameters.AddWithValue("@Gameplay", idGameplay);

            SqlDataReader reader = select.ExecuteReader();

            Dictionary<string, Situation> dict = new Dictionary<string, Situation>();

            while (reader.Read())
            {
                int id = int.Parse(reader["IDSituation"].ToString().TrimEnd(null));

                dict.Add(reader["Title"].ToString().TrimEnd(null), new Situation(reader["ImageURL"].ToString().TrimEnd(null))
                {
                    IdSituation = id,
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    Areas = new string[]
                    {
                        reader["IDForward"].ToString().TrimEnd(null),
                        reader["IDRight"].ToString().TrimEnd(null),
                        reader["IDBackward"].ToString().TrimEnd(null),
                        reader["IDLeft"].ToString().TrimEnd(null)
                    },
                    UnlockingItem = reader["UnlockingItem"].ToString().TrimEnd(null)
                });

            }
            reader.Close();

            // It values the properties of the object with the data which require to be read from the db by another reader
            foreach(string title in dict.Keys)
            {
                dict[title].Areas[0] = GetTitle(int.TryParse(dict[title].Areas[0], out int n0) ? n0 : -1, conn);
                dict[title].Areas[1] = GetTitle(int.TryParse(dict[title].Areas[1], out int n1) ? n1 : -1, conn);
                dict[title].Areas[2] = GetTitle(int.TryParse(dict[title].Areas[2], out int n2) ? n2 : -1, conn);
                dict[title].Areas[3] = GetTitle(int.TryParse(dict[title].Areas[3], out int n3) ? n3 : -1, conn);
                dict[title].Entities = ReadCharacters(dict[title].IdSituation, idGameplay, conn);
                dict[title].Items = ReadItems(dict[title].IdSituation, idGameplay, conn);
                dict[title].Actions = ReadAction(dict[title].IdSituation, idGameplay, conn);
            }

            // Reads data from the variables table
            SqlCommand selectVariables = new SqlCommand(@"SELECT IDInstance, Unlocked
                                                          FROM SituationVariable
                                                          WHERE IDGameplay = @IDGameplay", conn);

            selectVariables.Parameters.AddWithValue("@IDGameplay", idGameplay);

            reader = selectVariables.ExecuteReader();
            while (reader.Read())
            {
                // Finds the name of the situation using Linq
                string title = (from dictEl in dict
                               where dictEl.Value.IdSituation == int.Parse(reader[0].ToString().TrimEnd(null))
                               select dictEl.Key).First();

                dict[title].IsUnlocked = (bool)reader[1];
                if (dict[title].IsUnlocked)
                {
                    // If the situation is unlocked it needs another image, so the image url changes
                    string[] temp = dict[title].ImageURL.Split('.');
                    dict[title].ImageURL = string.Format("{0}u.{1}", temp[0], temp[1]);
                }
            }

            reader.Close();

            return dict;
        }

        /// <summary>
        /// Gets situation title given the ID
        /// </summary>
        /// <param name="id">Situation ID</param>
        /// <param name="conn">Connection to the db</param>
        /// <returns>The title of the situation</returns>
        private string GetTitle(int id, SqlConnection conn)
        {
            if (id == -1)
                return null;

            SqlCommand select = new SqlCommand(@"SELECT Title
                                                 FROM Situation 
                                                 WHERE IDSituation = @Id", conn);
            select.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = select.ExecuteReader();
            reader.Read();
            string value = reader[0].ToString().TrimEnd(null);
            reader.Close();
            return value;
        }

        private int GetIdentityValue(string table, SqlConnection conn)
        {
            SqlCommand identity = new SqlCommand($"SELECT IDENT_CURRENT('{table}');", conn);

            SqlDataReader reader = identity.ExecuteReader();

            reader.Read();
            object id = reader[0];
            reader.Close();

            if (id is null)
                return -1;
            else
                return int.Parse(id.ToString());
        }

        private Dictionary<string, Item> GetInventoryItems(int idPlayer, SqlConnection conn)
        {
            if (idPlayer == -1)
                return null;

            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, IT.IsCollectable, IT.IsVisible, IT.Effectiveness
                                                 FROM Item AS IT INNER JOIN 
                                                      Image AS I ON IT.IDImage = I.IDImage
                                                 WHERE IT.IDPlayer = @Id", conn);
            select.Parameters.AddWithValue("@Id", idPlayer);

            SqlDataReader reader = select.ExecuteReader();

            Dictionary<string, Item> dict = new Dictionary<string, Item>();

            while (reader.Read())
            {
                dict.Add(reader["Name"].ToString().TrimEnd(null), new Item(reader["ImageURL"].ToString().TrimEnd(null))
                {
                    IdImage = int.Parse(reader["IDImage"].ToString().TrimEnd(null)),
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    X = int.Parse(reader["X"].ToString().TrimEnd(null)),
                    Y = int.Parse(reader["Y"].ToString().TrimEnd(null)),
                    Width = int.Parse(reader["Width"].ToString().TrimEnd(null)),
                    Height = int.Parse(reader["Height"].ToString().TrimEnd(null)),
                    Dialogue = reader["Dialogue"].ToString().TrimEnd(null),
                    IsCollectable = int.Parse(reader["IsCollectable"].ToString().TrimEnd(null)) != 0,
                    IsVisible = int.Parse(reader["IsVisible"].ToString().TrimEnd(null)) != 0,
                    Effectiveness = int.Parse(reader["Effectiveness"].ToString().TrimEnd(null))
                });
            }

            reader.Close();
            return dict;
        }

        #region Select of characters and items
        private Player ReadPlayer(int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT C.Strength, C.IsVisible, P.IDCharacter, P.Armor, P.Health, P.Experience
                                                 FROM Player AS P INNER JOIN 
                                                      Character AS C ON P.IDCharacter = C.IDCharacter
                                                 WHERE P.IDGameplay = @Id", conn);
            select.Parameters.AddWithValue("@Id", idGameplay);

            SqlDataReader reader = select.ExecuteReader();
            reader.Read();
            int id = int.Parse(reader["IDCharacter"].ToString().TrimEnd(null));

            Player p = new Player()
            {
                IdCharacter = id,
                Strength = int.Parse(reader["Strength"].ToString().TrimEnd(null)),
                Health = int.Parse(reader["Health"].ToString().TrimEnd(null)),
                IsVisible = (bool)reader["IsVisible"],
                Armor = int.Parse(reader["Armor"].ToString().TrimEnd(null)),
                Experience = int.Parse(reader["Experience"].ToString().TrimEnd(null))
            };

            reader.Close();
            p.Inventory = GetInventoryItems(id, conn);

            

            return p;
        }

        private List<Character> ReadCharacters(int idSituation, int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, C.IDCharacter, C.Strength, C.IsVisible, C.EffectiveWeapon
                                                 FROM Image AS I INNER JOIN 
                                                      Character AS C ON I.IDImage = C.IDImage
                                                 WHERE I.IDSituation = @Situation AND I.IsCharacter = 1 AND C.IDGameplay = @IDGameplay", conn);
            select.Parameters.AddWithValue("@Situation", idSituation);
            select.Parameters.AddWithValue("@IDGameplay", idGameplay);

            SqlDataReader reader = select.ExecuteReader();
            List<Character> list = new List<Character>();

            while (reader.Read())
            {
                list.Add(new Character(reader["ImageURL"].ToString().TrimEnd(null))
                {
                    IdImage = int.Parse(reader["IDImage"].ToString().TrimEnd(null)),
                    IdCharacter = int.Parse(reader["IDCharacter"].ToString().TrimEnd(null)),
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    X = int.Parse(reader["X"].ToString().TrimEnd(null)),
                    Y = int.Parse(reader["Y"].ToString().TrimEnd(null)),
                    Width = int.Parse(reader["Width"].ToString().TrimEnd(null)),
                    Height = int.Parse(reader["Height"].ToString().TrimEnd(null)),
                    Dialogue = reader["Dialogue"].ToString().TrimEnd(null),
                    Strength = int.Parse(reader["Strength"].ToString().TrimEnd(null)),
                    IsVisible = (bool)reader["IsVisible"],
                    EffectiveWeapon = reader["EffectiveWeapon"].ToString().TrimEnd(null)
                });
            }

            reader.Close();

            return list;
        }

        private List<Item> ReadItems(int idSituation, int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, IT.IDItem, IT.IsCollectable, IT.IsVisible, IT.Effectiveness
                                                 FROM Image AS I INNER JOIN 
                                                      Item AS IT ON I.IDImage = IT.IDImage
                                                 WHERE I.IDSituation = @Situation AND I.IsItem = 1 AND IT.IDPlayer IS NULL AND IT.IDGameplay = @IDGameplay;", conn);
            select.Parameters.AddWithValue("@Situation", idSituation);
            select.Parameters.AddWithValue("@IDGameplay", idGameplay);

            SqlDataReader reader = select.ExecuteReader();
            List<Item> list = new List<Item>();

            while (reader.Read())
            {
                list.Add(new Item(reader["ImageURL"].ToString().TrimEnd(null))
                {
                    IdImage = int.Parse(reader["IDImage"].ToString().TrimEnd(null)),
                    IdItem = int.Parse(reader["IDItem"].ToString().TrimEnd(null)),
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    X = int.Parse(reader["X"].ToString().TrimEnd(null)),
                    Y = int.Parse(reader["Y"].ToString().TrimEnd(null)),
                    Width = int.Parse(reader["Width"].ToString().TrimEnd(null)),
                    Height = int.Parse(reader["Height"].ToString().TrimEnd(null)),
                    Dialogue = reader["Dialogue"].ToString().TrimEnd(null),
                    IsVisible = (bool)reader["IsVisible"],
                    Effectiveness = int.Parse(reader["Effectiveness"].ToString().TrimEnd(null)),
                    IsCollectable = (bool)reader["IsVisible"]

                });
            }

            reader.Close();

            return list;
        }

        #endregion

        private List<string> ReadAction(int idSituation, int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT Dialogue
                                                 FROM Action 
                                                 WHERE IDSituation = @Id AND IDGameplay = @IdGame", conn);
            select.Parameters.AddWithValue("@Id", idSituation);
            select.Parameters.AddWithValue("@IdGame", idGameplay);

            SqlDataReader reader = select.ExecuteReader();

            List<string> list = new List<string>();

            while (reader.Read())
            {
                list.Add(reader[0].ToString().TrimEnd(null));
            }

            reader.Close();
            return list;
        }

        #endregion

        #region Writer
        /* 
         * To Do List:
         * Character, Item, Player, Action, Gameplay, Account, Image
         */
        public void WriteData(string username, Gameplay g)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Write gameplay associated with the account
                g.IdGameplay = InsertGameplay(username, g, conn);
                InsertPlayer(g.IdGameplay, g.PlayerProfile, conn);
                foreach(Situation s in g.Situations.Values)
                {
                    List<Entity> entities = new List<Entity>();
                    if (!(s.Items is null))
                    {
                        entities.AddRange(s.Items);
                    }

                    if (!(s.Entities is null))
                    {
                        entities.AddRange(s.Entities);
                    }

                    InsertVariations(g.IdGameplay, s.IdSituation, s.IsUnlocked, conn);
                    InsertCharacterAndItem(g.IdGameplay, entities, conn);
                    InsertActions(g.IdGameplay, s.IdSituation, s.Actions, conn);
                }
                
            }
        }

        public void InsertAccount(string username, string email, string password)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand insert = new SqlCommand(@"INSERT INTO Account VALUES (@Username, @Email, @Password);", conn);

                insert.Parameters.AddWithValue("@Username", username);
                insert.Parameters.AddWithValue("@Email", email);
                insert.Parameters.AddWithValue("@Password", password);

                insert.ExecuteNonQuery();
            }
        }

        private int InsertGameplay(string username, Gameplay g, SqlConnection conn)
        {
            SqlCommand insert = new SqlCommand(@"INSERT INTO Gameplay VALUES (@CurrentArea, @Username);", conn);

            insert.Parameters.AddWithValue("@Username", username);
            insert.Parameters.AddWithValue("@CurrentArea", g.CurrentAreaID);

            insert.ExecuteNonQuery();

            return GetIdentityValue("Gameplay", conn);
        }

        private void InsertVariations(int idGameplay, int idSituation, bool value, SqlConnection conn)
        {
            // If the situation is already unlocked there's no need to save the variable
            if (value)
                return;

            SqlCommand insert = new SqlCommand("INSERT INTO SituationVariable VALUES (@IDSituation, @Unlocked, @IDGameplay);", conn);

            insert.Parameters.AddWithValue("@IDSituation", idSituation);
            insert.Parameters.AddWithValue("@Unlocked", value);
            insert.Parameters.AddWithValue("IDGameplay", idGameplay);

            insert.ExecuteNonQuery();
        }

        private void InsertActions(int idGameplay, int idSituation, List<string> actions, SqlConnection conn)
        {
            if (actions is null)
                return;

            foreach(string s in actions)
            {
                SqlCommand insert = new SqlCommand("INSERT INTO Action VALUES (@IDSituation, @Dialogue, @IDGameplay);", conn);

                insert.Parameters.AddWithValue("@IDSituation", idSituation);
                insert.Parameters.AddWithValue("@Dialogue", s);
                insert.Parameters.AddWithValue("@IDGameplay", idGameplay);

                insert.ExecuteNonQuery();
                insert.Dispose();
            }
            
        }

        private void InsertPlayer(int idGameplay, Player p, SqlConnection conn)
        {
            SqlCommand insertCharacter = new SqlCommand(@"INSERT INTO Character(Strength, IsVisible, IDImage, IDGameplay)
                                                          VALUES (@Strength, @IsVisible, @IDImage, @IDGameplay);", conn);
            SqlCommand insert = new SqlCommand(@"INSERT INTO Player VALUES (@IDCharacter, @Health, @Armor, @Experience, @IDGameplay);", conn);

            insertCharacter.Parameters.AddWithValue("@Strength", p.Strength);
            insertCharacter.Parameters.AddWithValue("@IsVisible", p.IsVisible);
            insertCharacter.Parameters.AddWithValue("@IDImage", DBNull.Value);
            insertCharacter.Parameters.AddWithValue("@IDGameplay", idGameplay);

            insertCharacter.ExecuteNonQuery();
            int id = GetIdentityValue("Character", conn);

            insert.Parameters.AddWithValue("@IDCharacter", id);
            insert.Parameters.AddWithValue("@Health", p.Health);
            insert.Parameters.AddWithValue("@Armor", p.Armor);
            insert.Parameters.AddWithValue("@Experience", p.Experience);
            insert.Parameters.AddWithValue("@IDGameplay", idGameplay);

            insert.ExecuteNonQuery();

        }

        public void InsertSituation(Dictionary<string, Situation> situations)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                foreach (KeyValuePair<string, Situation> el in situations)
                {
                    SqlCommand insert = new SqlCommand(@"INSERT INTO Situation (Title, Name, Description, ImageURL, UnlockingItem)
                                                     VALUES (@Title, @Name, @Description, @ImageURL, @UnlockingItem);", conn);

                    insert.Parameters.AddWithValue("@Title", el.Key);
                    insert.Parameters.AddWithValue("@Name", el.Value.Name);
                    insert.Parameters.AddWithValue("@Description", el.Value.Description is null ? "" : el.Value.Description);
                    insert.Parameters.AddWithValue("@ImageURL", el.Value.ImageURL);
                    if (el.Value.UnlockingItem is null)
                        insert.Parameters.AddWithValue("@UnlockingItem", DBNull.Value);
                    else
                        insert.Parameters.AddWithValue("@UnlockingItem", el.Value.UnlockingItem);

                    insert.ExecuteNonQuery();
                    insert.Dispose();

                    situations[el.Key].IdSituation = GetIdentityValue("Situation", conn);
                }

                foreach (Situation s in situations.Values)
                {
                    SqlCommand update = new SqlCommand(@"UPDATE Situation SET IDForward = @IdForward, IDRight = @IdRight, 
                    IDBackward = @IdBackward, IDLeft = @IdLeft
                    WHERE IDSituation = @IdSituation;", conn);

                    if (s.Areas[0] is null)
                        update.Parameters.AddWithValue("@IdForward", DBNull.Value);
                    else
                        update.Parameters.AddWithValue("@IdForward", situations[s.Areas[0]].IdSituation);
                    if (s.Areas[1] is null)
                        update.Parameters.AddWithValue("@IdRight", DBNull.Value);
                    else
                        update.Parameters.AddWithValue("@IdRight", situations[s.Areas[1]].IdSituation);
                    if (s.Areas[2] is null)
                        update.Parameters.AddWithValue("@IdBackward", DBNull.Value);
                    else
                        update.Parameters.AddWithValue("@IdBackward", situations[s.Areas[2]].IdSituation);
                    if (s.Areas[3] is null)
                        update.Parameters.AddWithValue("@IdLeft", DBNull.Value);
                    else
                        update.Parameters.AddWithValue("@IdLeft", situations[s.Areas[3]].IdSituation);

                    update.Parameters.AddWithValue("@IdSituation", s.IdSituation);

                    update.ExecuteNonQuery();
                    update.Dispose();
                }
            }
            
        }

        public void InsertImage(int idSituation, List<Entity> entities)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                for(int i = 0; i < entities.Count; i++)
                {
                    Entity e = entities[i];
                    SqlCommand insert = new SqlCommand(@"INSERT INTO Image VALUES(@Name, @Description, @X, @Y, @ImageURL, 
                                                         @Width, @Height, @Dialogue, @IsCharacter, @IsItem, @IDSituation);", conn);

                    insert.Parameters.AddWithValue("@Name", e.Name);
                    insert.Parameters.AddWithValue("@Description", e.Description);
                    insert.Parameters.AddWithValue("@X", e.X);
                    insert.Parameters.AddWithValue("@Y", e.Y);
                    insert.Parameters.AddWithValue("@ImageURL", e.ImageURL);
                    insert.Parameters.AddWithValue("@Width", e.Width);
                    insert.Parameters.AddWithValue("@Height", e.Height);
                    insert.Parameters.AddWithValue("@Dialogue", e.Dialogue);
                    insert.Parameters.AddWithValue("@IsCharacter", e.GetType() == typeof(Character));
                    insert.Parameters.AddWithValue("@IsItem", e.GetType() == typeof(Item));
                    insert.Parameters.AddWithValue("@IDSituation", idSituation);

                    insert.ExecuteNonQuery();
                    insert.Dispose();

                    entities[i].IdImage = GetIdentityValue("Image", conn);
                }
            }
        }

        private void InsertCharacterAndItem(int idGameplay, List<Entity> entities, SqlConnection conn)
        {
            foreach(Entity e in entities)
            {
                if (e.GetType() == typeof(Character))
                {
                    Character c = (Character)e;

                    SqlCommand insertChar = new SqlCommand("INSERT INTO Character VALUES (@Strength, @IsVisible, @EffectiveWeapon, @IDImage, @IDGameplay);", conn);

                    if(c.EffectiveWeapon is null)
                        insertChar.Parameters.AddWithValue("@EffectiveWeapon", DBNull.Value);
                    else
                        insertChar.Parameters.AddWithValue("@EffectiveWeapon", c.EffectiveWeapon);
                    insertChar.Parameters.AddWithValue("@Strength", c.Strength);
                    insertChar.Parameters.AddWithValue("@IsVisible", c.IsVisible);
                    insertChar.Parameters.AddWithValue("@IDImage", c.IdImage);
                    insertChar.Parameters.AddWithValue("@IDGameplay", idGameplay);

                    insertChar.ExecuteNonQuery();
                    insertChar.Dispose();

                    c.IdCharacter = GetIdentityValue("Character", conn);
                }
                else if (e.GetType() == typeof(Item))
                {
                    Item i = (Item)e;

                    SqlCommand insertItem = new SqlCommand("INSERT INTO Item VALUES (@IsCollectable, @IsVisible, @Effectiveness, NULL, @IDImage, @IDGameplay);", conn);

                    insertItem.Parameters.AddWithValue("@IsCollectable", i.IsCollectable);
                    insertItem.Parameters.AddWithValue("@IsVisible", i.IsVisible);
                    insertItem.Parameters.AddWithValue("@Effectiveness", i.Effectiveness);
                    insertItem.Parameters.AddWithValue("@IDImage", i.IdImage);
                    insertItem.Parameters.AddWithValue("@IDGameplay", idGameplay);

                    insertItem.ExecuteNonQuery();
                    insertItem.Dispose();

                    i.IdItem = GetIdentityValue("Item", conn);
                }
            }
        }

        

        #endregion

        #region Updater

        private void UpdateReferences(Gameplay g, SqlConnection conn)
        {

        }

        #endregion
    }

}