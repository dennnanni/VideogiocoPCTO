using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

using System.Web.UI.WebControls;

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ProgettoPCTO
{
    [DataContract]
    
    public class Gameplay
    {

        private Dictionary<string, Situation> _situations = new Dictionary<string, Situation>();

        public Gameplay()
        {
        }
        
        public int Count
        {
            get => _situations.Count;
        }

        public void Add(string key, Situation situation)
        {
            _situations.Add(key, situation);
        }

        [DataMember]
        public Dictionary<string, Situation> Situations
        {
            get => _situations;
            set => _situations = value;
        }

        [DataMember]
        public Player PlayerProfile { get; set; }
        
        [DataMember]
        public string CurrentAreaID { get; set; }

        /// <summary>
        /// Indexer of the dictionary
        /// </summary>
        /// <param name="name">name of the situation</param>
        /// <returns>Situation object, null if it doesn't exist</returns>
        public Situation this[string name]
        {
            get
            {
                if (_situations.ContainsKey(name))
                    return _situations[name];

                return null;
            }

            set
            {
                if (_situations.ContainsKey(name))
                    _situations[name] = value;
            }
        }
        
        public Image SetEntityImage(Entity e)
        {
            Image img = new Image();
            img.ImageUrl = e.ImageURL;
            img.Style["position"] = "absolute";
            img.Style["width"] = e.Width + "px";
            img.Style["height"] = e.Height + "px";
            img.Style["left"] = e.X + "px";
            img.Style["top"] = e.Y + "px";
            img.ID = "img" + e.Name;
            return img;
        }

        /// <summary>
        /// Saves the profile to make it possiblo to load it again
        /// </summary>
        /// <param name="server"></param>
        /// <returns>If the saving process goes well returns true</returns>
        public bool Save(HttpServerUtility server)
        {
            try
            {
                XMLHandler.Write(this, server, "~/App_Data/SavedData.XML");
                
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
            
        }


        /// <summary>
        /// Loads the last saving, if there are no parameters in saving file, returns the initial game
        /// </summary>
        /// <param name="server"></param>
        /// <returns>Saved game if there is, basic game if there isn't, null if problems show up</returns>
        public Gameplay Restore(HttpServerUtility server)
        {
            try
            {
                Gameplay game = XMLHandler.Read(server, "~/App_Data/SavedData.XML");

                // If no situation exists, then the file is empty and has nothing to load, it must return null
                if (game.Situations.Count == 0)
                    throw new Exception();

                return game;
                
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// Sets up the entire game
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Gameplay SetUp(HttpServerUtility server)
        {
            try
            {
                return XMLHandler.Read(server, "~/App_Data/Data.XML");
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
