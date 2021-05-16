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

            SqlCommand select = new SqlCommand(@"SELECT I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
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
            SqlCommand select = new SqlCommand(@"SELECT C.Strength, C.Health, C.IsVisible, P.IDPlayer, P.Armor, P.Experience
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
            SqlCommand select = new SqlCommand(@"SELECT I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
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
                    IdCharacter = int.Parse(reader["IDCharacter"].ToString().TrimEnd(null)),
                    Name = reader["Name"].ToString().TrimEnd(null),
                    Description = reader["Description"].ToString().TrimEnd(null),
                    X = int.Parse(reader["X"].ToString().TrimEnd(null)),
                    Y = int.Parse(reader["Y"].ToString().TrimEnd(null)),
                    Width = int.Parse(reader["Width"].ToString().TrimEnd(null)),
                    Height = int.Parse(reader["Height"].ToString().TrimEnd(null)),
                    Dialogue = reader["Dialogue"].ToString().TrimEnd(null),
                    Health = int.Parse(reader["Health"].ToString().TrimEnd(null)),
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
            SqlCommand select = new SqlCommand(@"SELECT I.Name, I.Description, I.X, I.Y, I.ImageURL, I.Width, I.Height,
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
    }

}