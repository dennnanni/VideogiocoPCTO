using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Item : Entity
    {
        public Item(string URL) : base(URL)
        {

        }

        [DataMember]
        public bool IsCollectable { get; set; } = true;

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