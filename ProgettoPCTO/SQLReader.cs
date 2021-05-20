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
        public SQLReader(Gameplay g, string connectionString)
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

            g.IdGameplay = int.Parse(reader[0].ToString());
            g.CurrentAreaID = reader[1].ToString().TrimEnd(null);

            reader.Close();
        }

        private Dictionary<string, Situation> ReadSituations(int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT IDSituation, Title, Name, Description, ImageURL, UnlockingItem, IDForward, IDRight, IDBackward, IDLeft
                                                 FROM Situation 
                                                 WHERE Situation.IDGameplay = @Gameplay", conn);
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
                        GetTitle(reader["IDForward"].ToString().TrimEnd(null) == "" ? int.Parse(reader[5].ToString().TrimEnd(null)) : -1, conn),
                        GetTitle(reader["IDRight"].ToString().TrimEnd(null) == "" ? int.Parse(reader[5].ToString().TrimEnd(null)) : -1, conn),
                        GetTitle(reader["IDBackward"].ToString().TrimEnd(null) == "" ? int.Parse(reader[5].ToString().TrimEnd(null)) : -1, conn),
                        GetTitle(reader["IDLeft"].ToString().TrimEnd(null) == "" ? int.Parse(reader[5].ToString().TrimEnd(null)) : -1, conn)
                    },
                    UnlockingItem = reader["UnlockingItem"].ToString().TrimEnd(null),
                    Entities = ReadCharacters(id, conn),
                    Items = ReadItems(id, conn),
                    Actions = ReadAction(id, conn)
                });
            }

            reader.Close();

            return dict;
        }

        private string GetTitle(int id, SqlConnection conn)
        {
            if (id == -1)
                return null;

            SqlCommand select = new SqlCommand(@"SELECT Title
                                                 FROM Situation 
                                                 WHERE Situation = @Id", conn);
            select.Parameters.AddWithValue("@Id", id);

            SqlDataReader reader = select.ExecuteReader();

            string value = reader[0].ToString().TrimEnd(null);
            reader.Close();
            return value;
        }

        private Dictionary<string, Item> GetInventoryItems(int idPlayer, SqlConnection conn)
        {
            if (idPlayer == -1)
                return null;

            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, IT.IsCollectable, IT.IsVisible, IT.Effectiveness
                                                 FROM Item AS IT INNER JOIN 
                                                      Image AS I ON IT.IDImage = I.IDImage
                                                 WHERE I.IDPlayer = @Id", conn);
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
                    Effectiveness = int.Parse(reader["IsVisible"].ToString().TrimEnd(null))
                });
            }

            reader.Close();
            return dict;
        }

        #region Select of characters and items
        private Player ReadPlayer(int idGameplay, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT C.Strength, C.IsVisible, P.IDPlayer, P.Armor, P.Health, P.Experience
                                                 FROM Player AS P INNER JOIN 
                                                      Character AS C ON P.IDPlayer = C.IDCharacter
                                                 WHERE I.IDGameplay = @Id", conn);
            select.Parameters.AddWithValue("@Id", idGameplay);

            SqlDataReader reader = select.ExecuteReader();
            reader.Read();
            int id = int.Parse(reader["IDPlayer"].ToString().TrimEnd(null));

            Player p = new Player()
            {
                IdCharacter = id,
                Strength = int.Parse(reader["Strength"].ToString().TrimEnd(null)),
                Health = int.Parse(reader["Health"].ToString().TrimEnd(null)),
                IsVisible = int.Parse(reader["IsVisible"].ToString().TrimEnd(null)) != 0,
                Armor = int.Parse(reader["Armor"].ToString().TrimEnd(null)),
                Experience = int.Parse(reader["Experience"].ToString().TrimEnd(null)),
                Inventory = GetInventoryItems(id, conn)
            };

            reader.Close();

            return p;
        }

        private List<Character> ReadCharacters(int idSituation, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, C.Health, C.Strength, C.IsVisible, C.EffectiveWeapon
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
                    IsVisible = int.Parse(reader["IsVisible"].ToString().TrimEnd(null)) != 0,
                    EffectiveWeapon = reader["EffectiveWeapon"].ToString().TrimEnd(null)
                });
            }

            reader.Close();

            return list;
        }

        private List<Item> ReadItems(int idSituation, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT I.IDImage, I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
                                                        I.Dialogue, IT.IsCollectable, IT.IsVisible, IT.Effectiveness
                                                 FROM Image AS I INNER JOIN 
                                                      Item AS IT ON I.IDImage = IT.IDImage
                                                 WHERE I.IDSituation = @Situation AND I.IsItem = 1 AND IT.IDPlayer = NULL", conn);
            select.Parameters.AddWithValue("@Situation", idSituation);

            SqlDataReader reader = select.ExecuteReader();
            List<Item> list = new List<Item>();

            while (reader.Read())
            {
                list.Add(new Item(reader["ImageURL"].ToString().TrimEnd(null))
                {
                    IdImage = int.Parse(reader["IDImage"].ToString().TrimEnd(null)),
                    IdItem = int.Parse(reader["IDCharacter"].ToString().TrimEnd(null)),
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    X = int.Parse(reader["X"].ToString().TrimEnd(null)),
                    Y = int.Parse(reader["Y"].ToString().TrimEnd(null)),
                    Width = int.Parse(reader["Width"].ToString().TrimEnd(null)),
                    Height = int.Parse(reader["Height"].ToString().TrimEnd(null)),
                    Dialogue = reader["Dialogue"].ToString().TrimEnd(null),
                    IsVisible = int.Parse(reader["IsVisible"].ToString().TrimEnd(null)) != 0,
                    Effectiveness = int.Parse(reader["Effectiveness"].ToString().TrimEnd(null)),
                    IsCollectable = int.Parse(reader["IsVisible"].ToString().TrimEnd(null)) != 0,

                });
            }

            reader.Close();

            return list;
        }

        #endregion

        private List<string> ReadAction(int idSituation, SqlConnection conn)
        {
            SqlCommand select = new SqlCommand(@"SELECT Dialogue
                                                 FROM Action 
                                                 WHERE IdAction = @Id", conn);
            select.Parameters.AddWithValue("@Id", idSituation);

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
                InsertGameplay(username, g, conn);
                InsertSituation(g.IdGameplay, g.Situations, conn);
                foreach(Situation s in g.Situations.Values)
                {
                    List<Entity> entities = new List<Entity>(s.Entities);
                    entities.AddRange(s.Items);
                }
            }
        }

        public void InsertAccount(string username, string password)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand insert = new SqlCommand(@"INSERT INTO Account VALUES (@Username, @Password);", conn);

                insert.Parameters.AddWithValue("@Username", username);
                insert.Parameters.AddWithValue("@Password", Helper.HashPassword(password));

                insert.ExecuteNonQuery();
            }
        }

        private void InsertGameplay(string username, Gameplay g, SqlConnection conn)
        {
            SqlCommand insert = new SqlCommand(@"INSERT INTO Gameplay VALUES (@CurrentArea, @Username);", conn);

            insert.Parameters.AddWithValue("@Username", username);
            insert.Parameters.AddWithValue("@CurrentArea", g.CurrentAreaID);

            insert.ExecuteNonQuery();
        }

        private void InsertPlayer(int idGameplay, Player p, SqlConnection conn)
        {
            SqlCommand insert = new SqlCommand(@"INSERT INTO Player VALUES (@Health, @Armor, @Experience, @IDGameplay);", conn);
        }

        private void InsertSituation(int idGameplay, Dictionary<string, Situation> situations, SqlConnection conn)
        {
            foreach(KeyValuePair<string, Situation> el in situations)
            {
                SqlCommand insert = new SqlCommand(@"INSERT INTO Situation 
                                                     VALUES (@Title, @Name, @Description, @ImageURL, @UnlockingItem, 
                                                     @IdGameplay, @IdForward, @IdRight, @IdBackward, @IdLeft);", conn);

                insert.Parameters.AddWithValue("@Title", el.Key);
                insert.Parameters.AddWithValue("@Name", el.Value.Name);
                insert.Parameters.AddWithValue("@Description", el.Value.Description);
                insert.Parameters.AddWithValue("@ImageURL", el.Value.ImageURL);
                insert.Parameters.AddWithValue("@UnlockingItem", el.Value.UnlockingItem);
                insert.Parameters.AddWithValue("@IdGameplay", idGameplay);
                insert.Parameters.AddWithValue("@IdForward", el.Value.Areas[0]);
                insert.Parameters.AddWithValue("@IdRight", el.Value.Areas[1]);
                insert.Parameters.AddWithValue("@IdBackward", el.Value.Areas[2]);
                insert.Parameters.AddWithValue("@IdLeft", el.Value.Areas[3]);

                insert.ExecuteNonQuery();
                insert.Dispose();
            }
        }

        private void InsertCharacterAndImage(int idSituation, List<Entity> entities, SqlConnection conn)
        {
            foreach(Entity e in entities)
            {
                SqlCommand insertImage = new SqlCommand(@"INSERT INTO Image(Name, Description, X, Y, ImageURL, Width, Height, Dialogue, IDSituation)
                                                          VALUES (@Name, @Description, @X, @Y, @ImageURL, @Width, @Height, @Dialogue, @IdSituation);", conn);

                insertImage.Parameters.AddWithValue("@Name", e.Name);
                insertImage.Parameters.AddWithValue("@Description", e.Description);
                insertImage.Parameters.AddWithValue("@X", e.X);
                insertImage.Parameters.AddWithValue("@Y", e.Y);
                insertImage.Parameters.AddWithValue("@ImageURL", e.ImageURL);
                insertImage.Parameters.AddWithValue("@Width", e.Width);
                insertImage.Parameters.AddWithValue("@Height", e.Height);
                insertImage.Parameters.AddWithValue("@Dialogue", e.Dialogue);
                insertImage.Parameters.AddWithValue("@IdSituation", idSituation);

                insertImage.ExecuteNonQuery();
                insertImage.Dispose();

                if(e.GetType() == typeof(Character))
                {
                    Character c = (Character)e;
                    SqlCommand update = new SqlCommand(@"UPDATE Character SET IsCharacter = 1;", conn);

                    SqlCommand insertChar = new SqlCommand("INSERT INTO Character VALUES (@Strength, @IsVisible, @EffectiveWeapon, @IDImage);", conn);

                    insertChar.Parameters.AddWithValue("@Strength", c.Strength);
                    insertChar.Parameters.AddWithValue("@IsVisbile", c.IsVisible);
                    insertChar.Parameters.AddWithValue("@EffectiveWeapon", c.EffectiveWeapon);
                    insertChar.Parameters.AddWithValue("@IDImage", c.IdCharacter);
                }
                else if (e.GetType() == typeof(Item))
                {
                    SqlCommand update = new SqlCommand(@"UPDATE Character SET IsItem = 1;", conn);

                }
            }
        }


        #endregion


        #region Updater

        #endregion
    }

}