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
        private int _health = 100;
        
        public Character() : this(null)
        {

        }

        public Character(string URL) : base(URL)
        {
        
        }


        [DataMember]
        public int Strength { get; set; }

        [DataMember]
        public int Health
        {
            get
            {
                return _health;
            }

            set
            {
                _health = value;
                if (_health > 100)
                    _health = 100;
                else if (_health <= 0)
                    throw new Exception("Sei morto.");
                
            }
        }

    }
}