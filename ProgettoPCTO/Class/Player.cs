using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Player : Character
    {
        public Player() : this(null) { }

        public Player(string URL) : base(URL)
        {
            Inventory = new Dictionary<string, Item>();
            Health = 100;
            Strength = 50;
        }


        [DataMember]
        public int Armor { get; set; }

        [DataMember]
        public int Experience { get; set; }

        [DataMember]
        public Power Powers { get; set; }

        public string InventoryToString()
        {
            string s = "";
            foreach (string name in Inventory.Keys)
                s += name + " ";

            return s;
        }

        public void Collect(Item item)
        {
            if (Inventory.Count == 4)
                throw new Exception("L'inventario è pieno! Lascia un oggetto per poter raccogliere " + item.Name + "\n");

            if (Inventory.ContainsKey(item.Name))
                throw new Exception("Non puoi avere due oggetti dello stesso tipo nell'inventario.\n");

            Inventory.Add(item.Name, item);
        }

        public void Drop(string itemName)
        {
            if (Inventory.Count == 0)
                throw new Exception("L'inventario è vuoto, non puoi lasciare niente!\n");

            Inventory.Remove(itemName);
        }

        public string Cure(int value)
        {
            try
            {
                this.Health += value;
            }
            catch
            {

            }
            return "Hai recuperato alcuni punti salute.";
        }

        public string Strengthen(int val)
        {
            int forza = Strength;
            int forzaaumentata = forza + val;
            Strength = forzaaumentata;
            return "Hai aumentato la tua forza.";
        }
    }
}