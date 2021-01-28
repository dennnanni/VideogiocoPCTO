using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    public class Situation : IBaseObject
    {
        private string[] _areasID = new string[4];

        public Situation()
        {

        }

        public Situation(string URL) : base()
        {
            ImageURL = URL;
        }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ImageURL { get; set; }

        [DataMember]
        public string[] Areas
        {
            get
            {
                return _areasID;
            }

            set
            {
                _areasID = value;
            }
        }

        [DataMember]
        public List<ProgettoPCTO.Character> Entities
        {
            get;
            set;
        }

        [DataMember]
        public List<ProgettoPCTO.Item> Items
        {
            get;
            set;
        }

        [DataMember]
        public string[] Actions { get; set; }
    }
}