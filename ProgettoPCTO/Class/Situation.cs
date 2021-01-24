using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace ProgettoPCTO
{
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

        public string Description { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }

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

        public List<ProgettoPCTO.Entity> Entities
        {
            get;
            set;
        }

        public List<ProgettoPCTO.Item> Items
        {
            get;
            set;
        }
    }
}