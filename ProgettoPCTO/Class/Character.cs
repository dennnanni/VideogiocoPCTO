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
        public Character(string URL) : base(URL)
        {

        }

        /// <summary>
        /// Match of a dialogue with a situation, the key is the situation, the value is the dialogue
        /// </summary>
        [DataMember]
        public Dictionary<string, string> Dialogue { get; set; } = new Dictionary<string, string>();

        [DataMember]
        public int Health { get; set; }

    }
}