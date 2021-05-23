using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace ProgettoPCTO
{
    public class SQLReader
    {
        private string _connectionString = "";
        public SQLReader(string connectionString)
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
                                                      SituationVariable AS SV ON S.IDSituation = SV.IDInstance
                                                 WHERE SV.IDGameplay = @Gameplay OR SV.IDInstance IS NULL;", conn);
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
                    UnlockingItem = reader["UnlockingItem"].ToString().TrimEnd(null),
                    IsUnlocked = reader["Unlocked"].GetType() == typeof(DBNull) ? true : (bool)reader["Unlocked"]
                });

            }
            reader.Close();

            foreach(string title in dict.Keys)
            {
                dict[title].Areas[0] = GetTitle(int.TryParse(dict[title].Areas[0], out int n0) ? n0 : -1, conn);
                dict[title].Areas[1] = GetTitle(int.TryParse(dict[title].Areas[1], out int n1) ? n1 : -1, conn);
                dict[title].Areas[2] = GetTitle(int.TryParse(dict[title].Areas[2], out int n2) ? n2 : -1, conn);
                dict[title].Areas[3] = GetTitle(int.TryParse(dict[title].Areas[3], out int n3) ? n3 : -1, conn);
                dict[title].Entities = ReadCharacters(dict[title].IdSituation, conn);
                dict[title].Items = ReadItems(dict[title].IdSituation, conn);
                dict[title].Actions = ReadAction(dict[title].IdSituation, idGameplay, conn);
            }

            return dict;
        }

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
            SqlCommand identity = new SqlCommand($"DBCC CHECKIDENT('{table}', NORESEED);", conn);

            object id = identity.ExecuteScalar();
            if (id is null)
                return -1;
            else
                return (int)id;
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

        private List<Character> ReadCharacters(int idSituation, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, C.IDCharacter, C.Strength, C.IsVisible, C.EffectiveWeapon
                                                 FROM Image AS I INNER JOIN 
                                                      Character AS C ON I.IDImage = C.IDImage
                                                 WHERE I.IDSituation = @Situation AND I.IsCharacter = 1", conn);
            select.Parameters.AddWithValue("@Situation", idSituation);

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

        private List<Item> ReadItems(int idSituation, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, IT.IDItem, IT.IsCollectable, IT.IsVisible, IT.Effectiveness
                                                 FROM Image AS I INNER JOIN 
                                                      Item AS IT ON I.IDImage = IT.IDImage
                                                 WHERE I.IDSituation = @Situation AND I.IsItem = 1 AND IT.IDPlayer IS NULL;", conn);
            select.Parameters.AddWithValue("@Situation", idSituation);

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
                // Write gameplay associated with the account
                g.IdGameplay = InsertGameplay(username, g, conn);
                InsertPlayer(g.IdGameplay, g.PlayerProfile, conn);
                foreach(Situation s in g.Situations.Values)
                {
                    List<Entity> entities = new List<Entity>(s.Entities);
                    entities.AddRange(s.Items);
                    InsertVariations(g.IdGameplay, s.IdSituation, s.IsUnlocked, conn);
                    InsertCharacterAndItem(g.IdGameplay, s.IdSituation, entities, conn);
                    InsertActions(g.IdGameplay, s.IdSituation, s.Actions, conn);
                }
                
            }
        }

        public void InsertAccount(string username, string email, string password)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
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
            SqlCommand insert = new SqlCommand("INSERT INTO SituationVariable VALUES (@IDSituation, @Unlocked, @IDGameplay);", conn);

            insert.Parameters.AddWithValue("@IDSituation", idSituation);
            insert.Parameters.AddWithValue("@Unlocked", value);
            insert.Parameters.AddWithValue("IDGameplay", idGameplay);

            insert.ExecuteNonQuery();
        }

        private void InsertActions(int idGameplay, int idSituation, List<string> actions, SqlConnection conn)
        {
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
            SqlCommand insertCharacter = new SqlCommand("INSERT INTO Character VALUES (@Strength, @IsVisible, @EffectiveWeapon, @IDImage, @IDGameplay);", conn);
            SqlCommand insert = new SqlCommand(@"INSERT INTO Player VALUES (@IDCharacter, @Health, @Armor, @Experience, @IDGameplay);", conn);

            insert.Parameters.AddWithValue("@Strength", p.Strength);
            insert.Parameters.AddWithValue("@IsVisible", p.IsVisible);
            insert.Parameters.AddWithValue("@EffectiveWeapon", p.EffectiveWeapon);
            insert.Parameters.AddWithValue("@IDImage", DBNull.Value);
            insert.Parameters.AddWithValue("@IDGameplay", idGameplay);

            insertCharacter.ExecuteNonQuery();
            int id = GetIdentityValue("Character", conn);

            insert.Parameters.AddWithValue("@IDCharacter", id);
            insert.Parameters.AddWithValue("@Health", p.Health);
            insert.Parameters.AddWithValue("@Armor", p.Armor);
            insert.Parameters.AddWithValue("@Experience", p.Experience);
            insert.Parameters.AddWithValue("@IDGameplay", idGameplay);

            insert.ExecuteNonQuery();

        }

        private void InsertCharacterAndItem(int idGameplay, List<Entity> entities, SqlConnection conn)
        {
            foreach(Entity e in entities)
            {
                if (e.GetType() == typeof(Character))
                {
                    Character c = (Character)e;

                    SqlCommand insertChar = new SqlCommand("INSERT INTO Character VALUES (@Strength, @IsVisible, @EffectiveWeapon, @IDImage, @IDGameplay);", conn);

                    insertChar.Parameters.AddWithValue("@Strength", c.Strength);
                    insertChar.Parameters.AddWithValue("@IsVisible", c.IsVisible);
                    insertChar.Parameters.AddWithValue("@EffectiveWeapon", c.EffectiveWeapon);
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