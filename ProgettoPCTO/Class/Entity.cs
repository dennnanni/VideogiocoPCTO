using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

using System.Runtime.Serialization;

namespace Progetto_PCTO
{
    
    public abstract class Entity : IBaseObject
    {
        public Entity(string URL)
        {
            ImageURL = URL;
        }


        public int X
        {
            get; 
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public string ImageURL { get; private set; }

        public string Description { get; set; }
        public string Name { get; set; }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public Item Inventory
        {
            get => default;
            set
            {
            }
        }
    }
}