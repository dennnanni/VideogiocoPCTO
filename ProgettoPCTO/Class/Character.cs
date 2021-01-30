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
    [KnownType(typeof(Character))]
    public class Character : Entity
    {
        public Character() : this(null)
        {

        }

        public Character(string URL) : base(URL)
        {
            Inventory = new List<Item>();
        }

       
        [DataMember]
        public int Health { get; set; }

    }
}