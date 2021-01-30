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
        private string _currentAreaID = "area0";
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
                Session["currentArea"] = "area1";
                _currentSituation = _game["area1"];
                this.LoadSituation("area1");
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

            // If a cardinal button has been pressed, the textbox must be empty
            txtStory.Text = "";
            this.LoadSituation(_currentSituation.Areas[index]);

            Session["currentArea"] = _currentAreaID;
            Session["gameplay"] = _game;
        }

        public void LoadSituation(string name) // Loading by name is much more easy to reuse code
        {
            Situation s = _game[name];

            // Clears the images panel from all controls and sets the background
            pnlImages.Controls.Clear();
            pnlImages.BackImageUrl = s.ImageURL; // Load background


            // If the situation is changed
            if (_currentAreaID != name)
            {
                txtStory.Text += "Hai raggiunto " + s.Name + "\n";
                txtStory.Text += s.Description;

                // Update the current area ID
                _currentAreaID = name;
            }

            // Fill the drop down list with actions in the situation, if there are
            if(s.Actions != null)
                foreach (string st in s.Actions)
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

            // Loads all items in the situation
            if(s.Items != null)
                foreach(Item i in s.Items)
                {
                    Image img = new Image();
                    img.ImageUrl = i.ImageURL;
                    img.Style["position"] = "absolute";
                    img.Style["width"] = i.Width + "px";
                    img.Style["height"] = i.Height + "px";
                    img.Style["left"] = i.X + "px";
                    img.Style["top"] = i.Y + "px";
                    img.ID = "img" + i.Name;
                    pnlImages.Controls.Add(img);
                }

            // Save the parameters
            Session["currentArea"] = _currentAreaID;
            Session["gameplay"] = _game;
        }

        protected void btnDo_Click(object sender, EventArgs e)
        {
            string actionText = drpActions.SelectedValue;

            // If nothing is selected or there are no entities to interact with, the method does nothing
            if (actionText == "" || _currentSituation.Entities is null && _currentSituation.Items is null)
                return;

            Entity entity = null;

            if(_currentSituation.Entities != null)
                // Search for the character that fits the action (it is possible to have only one entity of the 
                // same type in a situation so there's no problem of ambiguity)
                foreach(Character c in _currentSituation.Entities)
                    if(actionText.ToLower().Contains(c.Name.ToLower()))
                    {
                        entity = c;
                        break;
                    }

            if(_currentSituation.Items != null)
                // Search for the item that fits the action (same of above)
                foreach(Item i in _currentSituation.Items)
                    if (actionText.ToLower().Contains(i.Name.ToLower()))
                    {
                        entity = i;
                        break;
                    }

            // Shows the dialogue and remove the action from the situation and the drp
            txtStory.Text += entity.Dialogue[_currentAreaID];
            _game[_currentAreaID].Actions.Remove(actionText);

            // I don't know why only one instruction does not work
            drpActions.Items.RemoveAt(drpActions.SelectedIndex);
            drpActions.Items.RemoveAt(drpActions.SelectedIndex);

        }
    }
}