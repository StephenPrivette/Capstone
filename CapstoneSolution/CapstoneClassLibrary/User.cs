﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneClassLibrary
{
    // class for convention users that directly corresponds to the Users table in the database except for the itinerary
    public class User
    {
        public int userID { get; set; }
        public string userName { get; set; }
        public string userPass { get; set; }
        public string userTypeName { get; set; }
        public string userEmail { get; set; }
        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userSalt { get; set; }

        // is stored in a seperate table in the database from the rest of the fields
        private List<Event> itinerary;

        // adds passed in event to itinerary
        public bool addEventToItinerary(Event eve)
        {
            foreach(Event i in itinerary)
            {
                if(i.eventID == eve.eventID)
                {
                    return false;
                }
            }

            itinerary.Add(eve);
            return true;
        }

        // deletes passed in event from itinerary
        public bool deletEventFromItinerary(Event eve)
        {
            foreach (Event i in itinerary)
            {
                if (i.eventID == eve.eventID)
                {
                    itinerary.Remove(i);
                    return true;
                }
            }

            return false;
        }

        // getters and setters for itinerary
        public List<Event> getItinerary()
        {
            return itinerary;
        }

        public void setItinerary(List<Event> events)
        {
            itinerary = events;
        }
    }
}
