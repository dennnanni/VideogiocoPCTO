using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Character : Entity
    {
        public Character(string URL) : base(URL)
        {

        }

        [DataMember]
        public int Health
        {
            get => default;
            set
            {
            }
        }

    }
}