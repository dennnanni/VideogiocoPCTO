using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Power : IBaseObject
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Armor { get; set; }

        [DataMember]
        public int Experience { get; set; }

        [DataMember]
        public int Strength { get; set; }

        [DataMember]
        public int Health { get; set; }
    }
}