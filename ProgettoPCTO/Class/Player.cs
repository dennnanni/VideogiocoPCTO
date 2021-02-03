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
        }

        [DataMember]
        public int Armor { get; set; }

        [DataMember]
        public int Experience { get; set; }

        [DataMember]
        public int Strength { get; set; }

        [DataMember]
        public Power Powers { get; set; }

        public void Collect(Item item)
        {
            if (Inventory.Count == 4)
                throw new Exception("L'inventario è pieno! Lascia un oggetto per poter raccogliere " + item.Name + "\n");

            Inventory.Add(item.Name, item);
        }

        public void Drop(Item item)
        {
            if (Inventory.Count == 0)
                throw new Exception("L'inventario è vuoto, non puoi lasciare niente!\n");

            Inventory.Remove(item.Name);
        }
    }
}