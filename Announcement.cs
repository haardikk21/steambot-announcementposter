using SteamKit2;
using System.Collections.Generic;
using SteamTrade;
using SteamTrade.TradeWebAPI;
using System.IO;
using System.Timers;

namespace SteamBot
{
    public class Announcement : UserHandler
    {
    	public int hours = 3;
    	public Timer bump = new Timer(10800000);
        public bool firstMsg = false;
        public List<string> urls = new List<string>();
        public string title = "";
        public string body = "";
        public Announcement(Bot bot, SteamID sid) : base(bot, sid) { }

        public override bool OnGroupAdd()
        {
            return false;
        }

        public override bool OnFriendAdd()
        {
            System.DateTime now = System.DateTime.Now;
            File.AppendAllText("friendrequests.txt", now + " | " + OtherSID + " added me" + System.Environment.NewLine);
            return true;
        }

        public override void OnLoginCompleted()
        {
        	/*bump.Interval = hours*60*60*1000;
        	bump.Elapsed += new ElapsedEventHandler(bumpElapsed);
        	bump.Enabled = true;
        	Bump();
        	*/
        }
        
        public void bumpElapsed(object sender, ElapsedEventArgs e)
        {
        	//Bump();
        }

        public override void OnChatRoomMessage(SteamID chatID, SteamID sender, string message)
        {
            Log.Info(Bot.SteamFriends.GetFriendPersonaName(sender) + ": " + message);
            base.OnChatRoomMessage(chatID, sender, message);
        }

        public override void OnFriendRemove() { }

        public override void OnMessage(string message, EChatEntryType type)
        {
        	message = message.ToLower();
            /*if(firstMsg == false)
            {
                SendChatMessage("Hey. Seems like you've reached BOT Bone. Bone is currently asleep/busy right now and not online. He uses me to automate some trades and other functions for him.");
                SendChatMessage("If you want me to leave a message for him, just send me messages and they will be logged for him to read later on when he comes online.");
                SendChatMessage("Have a good day! Bot out.");
                firstMsg = true;
            }
            else
            {
                System.DateTime now = System.DateTime.Now;
                string path = @"chats/" + OtherSID.ConvertToUInt64().ToString() + " _ " + Bot.SteamFriends.GetFriendPersonaName(OtherSID) + ".txt";
                try{
                	File.AppendAllText(path, now + " | " + message + System.Environment.NewLine);
                }
                catch
                {
                	path = @"chats/" + OtherSID.ConvertToUInt64().ToString() + ".txt";
                	File.AppendAllText(path, now + " | " + message + System.Environment.NewLine);
                }
            }*/
            if(IsAdmin)
            {
	            if(message.ToLower() == "bump")
	            {
	            	Bump();
	            }
	            else if(message.StartsWith("send"))
            	{
            		if(message.Length >= 6)
            		{
                        Log.Success("Sending message");
            			string chatmsg = message.Substring(5);
            			int numFriends = Bot.SteamFriends.GetFriendCount();
                        for (int count = 0; count < numFriends; count++)
                        {
                            SteamID friend = Bot.SteamFriends.GetFriendByIndex(count);
                            ulong friendID = friend.ConvertToUInt64();
                            Bot.SteamFriends.SendChatMessage(friendID, type, chatmsg);
                            System.Threading.Thread.Sleep(100);
                        }
            		}
            	}
	       	
            }

        }

        public override bool OnTradeRequest()
        {
            return false;
        }

        public override void OnTradeError(string error)
        {
            SendChatMessage("Oh, there was an error: {0}.", error);
            Log.Warn(error);
        }

        public override void OnTradeTimeout()
        {
            SendChatMessage("Sorry, but you were AFK and the trade was canceled.");
            Log.Info("User was kicked because he was AFK.");
        }

        public override void OnTradeInit()
        {
            SendTradeMessage("Success. Please put up your items.");
        }

        public override void OnTradeAddItem(Schema.Item schemaItem, Inventory.Item inventoryItem) { }

        public override void OnTradeRemoveItem(Schema.Item schemaItem, Inventory.Item inventoryItem) { }

        public override void OnTradeMessage(string message) { }

        public override void OnTradeReady(bool ready)
        {
            
        }

        public override void OnTradeSuccess()
        {
            Log.Success("Trade Complete.");
        }

        public override void OnTradeAwaitingEmailConfirmation(long tradeOfferID)
        {
            Log.Warn("Trade ended awaiting email confirmation");
            SendChatMessage("Please complete the email confirmation to finish the trade");
        }

        public override void OnTradeAccept()
        {
            
        }

        public void UpdateRaffleDetails()
        {
            urls.Clear();
            var txt = File.ReadLines("groups.txt");
            foreach(var line in txt)
            {
                urls.Add(line); 
            }
            title = File.ReadAllText("title.txt");
            body = File.ReadAllText("body.txt");
        }

        public void Bump()
        {
            UpdateRaffleDetails();
            var data = new System.Collections.Specialized.NameValueCollection();
            data.Add("sessionID", Bot.SteamWeb.SessionId);
            data.Add("action", "post");
            data.Add("headline", title);
            data.Add("body", body);
            foreach(string url in urls)
            {
            	try
            	{
                	var response = SteamWeb.Fetch(url, "post", data, true, "");
            	}
            	catch
            	{
            		Bot.Log.Error("Error while posting announcement. " + url);
            	}
            }
            Bot.Log.Success("All announcements bumped");
        }

    }

}

