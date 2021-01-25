﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace ProgettoPCTO
{
    public static class XMLHandler
    {
        public static Gameplay Read(HttpServerUtility server)
        {
            Dictionary<string, Situation> situations;

            using (FileStream openStream =
                    new FileStream(server.MapPath("~/App_Data/Data.XML"),
                                    FileMode.Open,
                                    FileAccess.Read,
                                    FileShare.None))
            using (XmlReader xmlReader =
                    XmlReader.Create(openStream))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Dictionary<string, Situation>));
                situations = (Dictionary<string, Situation>)dcSerializer.ReadObject(xmlReader);
            }

            Gameplay game = new Gameplay();
            game.Situations = situations;
            return game;
        }

        public static void Write(Gameplay gameplay, HttpServerUtility server)
        {
            Dictionary<string, Situation> s = gameplay.Situations;
            
            // Serializza sul file /App_Data/File.XML.
            // Per avere la cartella con i giusti permessi create
            // un progetto ASP.NET vuoto ma spuntate Web Form.
            // Per avere meno problemi togliete la spunta al check ... https.
            using (FileStream saveStream =
                new FileStream(server.MapPath("~/App_Data/Data.XML"),
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None))
            {
                // Grazie a Indent va anche a capo con i tag.
                XmlWriterSettings xws = new XmlWriterSettings()
                {
                    Indent = true
                };

                using (XmlWriter xmlWriter =
                        XmlWriter.Create(saveStream, xws))
                {
                    DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Dictionary<string, Situation>));
                    dcSerializer.WriteObject(xmlWriter, s);
                }
            }
        }
    }
}