using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ProgettoPCTO
{
    public static class Helper
    {
        public static string HashPassword(string pwd)
        {
            byte[] salt = new byte[32];
            var csp = new RNGCryptoServiceProvider();
            csp.GetBytes(salt);

            Rfc2898DeriveBytes hashObj = new Rfc2898DeriveBytes(pwd, salt, 10000);
            byte[] hash = hashObj.GetBytes(32);

            // Combine hash with salt
            byte[] crypts = new byte[64];
            Array.Copy(salt, 0, crypts, 0, 32);
            Array.Copy(hash, 0, crypts, 32, 32);

            return Convert.ToBase64String(crypts);
        }

        public static bool Authenticate(string username, string password, string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Recupera le credenziali dal DB.
                    conn.Open();
                    // Comando per leggere la password dell'utente dal DB.
                    SqlCommand test = new SqlCommand(
                    "SELECT Password FROM Account WHERE Username = @User",
                    conn);
                    test.Parameters.AddWithValue("@User", username);
                    // Verifica se i dati di accesso dell'utente sono corretti.
                    // ExecuteScalar restituisce la prima colonna della prima riga
                    // dei risultati o null.
                    var pwd = test.ExecuteScalar();
                    if (pwd != null)
                    {
                        // Ricava un vettore di byte dal codice hash ed ...
                        byte[] hashBytesDB = Convert.FromBase64String(pwd.ToString());
                        // ... estrapola il salt.
                        byte[] saltDB = new byte[32];
                        Array.Copy(hashBytesDB, 0, saltDB, 0, 32);
                        // Usando il salt salvato nel DB calcola l'hash della password
                        // inserita dall'utente e ne ricava il vettore di byte.
                        var pbkdf2User = new Rfc2898DeriveBytes(password, saltDB, 10000);
                        byte[] hashDBUser = pbkdf2User.GetBytes(32);
                        // Compara i due vettori.
                        for (int i = 0; i < 32; i++)
                            if (hashBytesDB[i + 32] != hashDBUser[i])
                                return false;
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
        
    }
}