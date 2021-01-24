using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Player : Character
    {
        public Player(string URL) : base(URL)
        {
        }

        [DataMember]
        public int Armor
        {
            get => default;
            set
            {
            }
        }

        [DataMember]
        public int Experience
        {
            get => default;
            set
            {
            }
        }

        [DataMember]
        public int Strength
        {
            get => default;
            set
            {
            }
        }

        [DataMember]
        public Power Powers
        {
            get => default;
            set
            {
            }
        }
    }
}