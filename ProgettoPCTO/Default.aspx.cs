using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProgettoPCTO
{
    enum Cardinals : int { North, East, South, West }
    public partial class Default : System.Web.UI.Page
    {
        private Gameplay _game = null;
        private string _currentAreaID = "area1";
        private Situation _currentSituation;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Da togliere, necessario per fase debug
                Gameplay game = new Gameplay();
                XMLHandler.Write(game, Server);
                // Creates the gameplay and loads the first situation
                _game = XMLHandler.Read(Server);
                Session["game"] = _game;
                Session["currentArea"] = _currentAreaID;
                _currentSituation = _game[_currentAreaID];
                this.LoadSituation(_currentAreaID);
            }
            else
            {
                // Restores last changes 
                _currentAreaID = Session["currentArea"].ToString();
                _game = (Gameplay)Session["game"];
                _currentSituation = _game[_currentAreaID];
                this.LoadSituation(_currentAreaID);

            }
        }

        protected void btnCardinal_Click(object sender, EventArgs e)
        {
            // Gets clicked direction
            string btnName = ((Button)sender).ID.Substring(3);
            int index = -1;
            bool exit = false;
            for (int i = 0; i < 4 && !exit; i++)
                if (btnName == ((Cardinals)i).ToString())
                {
                    index = i;
                    exit = true;
                }


            this.LoadSituation(_currentSituation.Areas[index]);

            Session["currentArea"] = _currentAreaID;
            Session["gameplay"] = _game;
        }

        public void LoadSituation(string name) // Loading by name is much more easy to reuse code
        {
            Situation s = _game[name];
            pnlImages.Controls.Clear();
            pnlImages.BackImageUrl = s.ImageURL; // Load background
            txtStory.Text += "Hai raggiunto " + s.Name + "\n";
            txtStory.Text += s.Description;
            _currentAreaID = name; // Sets the global variable

            foreach(string st in s.Actions)
            {
                drpActions.Items.Add(st);
            }

            // Enables and unables direction buttons
            for (int i = 0; i < 4; i++)
            {
                if (s.Areas[i] == null)
                {
                    Button btn = pnlCardinals.FindControl("btn" + (Cardinals)i) as Button;
                    btn.Enabled = false;
                    btn.Text = "";
                }
                else
                {
                    Button btn = pnlCardinals.FindControl("btn" + (Cardinals)i) as Button;
                    btn.Enabled = true;
                    btn.Text = _game[s.Areas[i]].Name;
                }

            }


            // Loading all entities in the situation if there are
            if(s.Entities != null)
                foreach (Character e in s.Entities)
                {
                    Image img = new Image();
                    img.ImageUrl = e.ImageURL;
                    img.Style["position"] = "absolute";
                    img.Style["width"] = e.Width + "px";
                    img.Style["height"] = e.Height + "px";
                    img.Style["left"] = e.X + "px";
                    img.Style["top"] = e.Y + "px";
                    img.ID = "img" + e.Name;

                    pnlImages.Controls.Add(img);
                }

            if(s.Items != null)
                foreach(Item i in s.Items)
                {
                    ImageButton img = new ImageButton();
                    img.ImageUrl = i.ImageURL;
                    img.Style["position"] = "absolute";
                    img.Style["width"] = i.Width + "px";
                    img.Style["height"] = i.Height + "px";
                    img.Style["left"] = i.X + "px";
                    img.Style["top"] = i.Y + "px";
                    img.ID = "img" + i.Name;
                    pnlImages.Controls.Add(img);
                }
        }

        protected ImageClickEventHandler EventDispatcher(Entity e)
        {
            switch (e.Name.ToLower())
            {
                case "steve":
                    return btnSteve_Click;

                default:
                    return null;
            }
        }

        #region Entities events
        protected void btnSteve_Click(object sender, EventArgs e)
        {
            Character c = null;
            for(int i = 0; i < _currentSituation.Entities.Count; i++)
            {
                if(_currentSituation.Entities[i].Name == "Steve")
                {
                    c = _currentSituation.Entities[i];
                    i = _currentSituation.Entities.Count;
                }
            }
            
            //lstStory.Items.Add(c.Dialogue[_currentAreaID]);
        }

        protected void btnCreeper_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Items events

        #endregion
    }
}