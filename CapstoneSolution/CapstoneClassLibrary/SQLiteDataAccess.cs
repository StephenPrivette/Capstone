﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapstoneClassLibrary
{
    public class SQLiteDataAccess
    {
        // Method for creating the string that allows the program to interact with the database
        public string loadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        // saves the passed in object to its appropriate table in the database via an update query
        public void updateDbFromObjectByName(object obj)
        {
            string objTable = obj.GetType().Name + "s";
            string query = "UPDATE " + objTable + " SET "; // start concatenating string for query

            int count = 1;

            // iterating through the properties of the object and concatenating string with them
            foreach (var i in obj.GetType().GetProperties())
            {
                if(count != 1)
                    query += i.Name + " = '" + i.GetValue(obj) + "', ";
                count++;
            }

            query = query.Substring(0, (query.Length - 2));

            query += " WHERE " + obj.GetType().GetProperties()[1].Name + " = '"
                + obj.GetType().GetProperties()[1].GetValue(obj).ToString() + "'";

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // saves the passed in object to its appropriate table in the database via an update query
        public void updateDbFromObjectById(object obj)
        {
            string objTable = obj.GetType().Name + "s";
            string query = "UPDATE " + objTable + " SET "; // start concatenating string for query

            int count = 1;

            // iterating through the properties of the object and concatenating string with them
            foreach (var i in obj.GetType().GetProperties())
            {
                if (count != 1)
                    query += i.Name + " = '" + i.GetValue(obj) + "', ";
                count++;
            }

            query = query.Substring(0, (query.Length - 2));

            query += " WHERE " + obj.GetType().GetProperties()[0].Name + " = '"
                + obj.GetType().GetProperties()[0].GetValue(obj).ToString() + "'";

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // saves the passed in object to its appropriate table in the database via an insert query
        public void insertObjectIntoDb(object obj)
        {
            string objTable = obj.GetType().Name + "s";
            string query = "INSERT INTO " + objTable + " ("; // start concatenating string for query
            string values = "VALUES (";

            int count = 1;

            // iterating through the properties of the object and concatenating string with them
            foreach (var i in obj.GetType().GetProperties())
            {
                if (count != 1)
                {
                    query += i.Name + ", ";
                    values += "'" + i.GetValue(obj) + "', ";
                }
                count++;
            }

            query = query.Substring(0, (query.Length - 2)) + ")";
            values = values.Substring(0, (values.Length - 2)) + ")";
            query += values;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // deletes passed in object from database
        public void deleteObjectFromDb(object obj, string name)
        {
            string objTable = obj.GetType().Name + "s";
            string query = "DELETE FROM " + objTable + " WHERE " + obj.GetType().GetProperties()[1].Name + " = '" + name + "'";

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // retrieves corresponding object from the database by looking up its name
        public object getObjectFromDbByName(object obj, string name)
        {
            string objTable = obj.GetType().Name + "s";
            // start concatenating string for query
            string query = "SELECT * FROM " + objTable + " WHERE " + obj.GetType().GetProperties()[1].Name + " = '" + name + "'";

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                int numOfColumns = obj.GetType().GetProperties().Length;
                SQLiteDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    for (int i = 0; i < numOfColumns; i++)
                    {
                        string prop = obj.GetType().GetProperties()[i].Name;

                        PropertyInfo pi = obj.GetType().GetProperty(prop);

                        // making sure to set property with correct type
                        if (pi.PropertyType == typeof(int))
                        {
                            pi.SetValue(obj, reader.GetInt32(i));
                        }
                        else if (pi.PropertyType == typeof(DateTime))
                        {
                            pi.SetValue(obj, DateTime.Parse(reader.GetString(i)));
                        }
                        else if (pi.PropertyType == typeof(TimeSpan))
                        {
                            pi.SetValue(obj, TimeSpan.Parse(reader.GetString(i)));
                        }
                        else
                        {
                            pi.SetValue(obj, reader.GetString(i));
                        }
                    }
                }
                reader.Close();
                cnn.Close();
            }

            return obj;
        }

        // retrieves corresponding object from the database by looking up its ID
        public object getObjectFromDbById(object obj, int id)
        {
            string objTable = obj.GetType().Name + "s";
            // start concatenating string for query
            string query = "SELECT * FROM " + objTable + " WHERE " + obj.GetType().GetProperties()[0].Name + " = " + id;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                int numOfColumns = obj.GetType().GetProperties().Length;
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < numOfColumns; i++)
                    {
                        string prop = obj.GetType().GetProperties()[i].Name;

                        PropertyInfo pi = obj.GetType().GetProperty(prop);

                        // making sure to set property with correct type
                        if (pi.PropertyType == typeof(int))
                        {
                            pi.SetValue(obj, reader.GetInt32(i));
                        }
                        else if (pi.PropertyType == typeof(DateTime))
                        {
                            pi.SetValue(obj, DateTime.Parse(reader.GetString(i)));
                        }
                        else if (pi.PropertyType == typeof(TimeSpan))
                        {
                            pi.SetValue(obj, TimeSpan.Parse(reader.GetString(i)));
                        }
                        else
                        {
                            pi.SetValue(obj, reader.GetString(i));
                        }
                    }
                }
                reader.Close();
                cnn.Close();
            }

            return obj;
        }

        // returns true if the passed in object's ID already exists as a row in the database
        public bool isObjectIdInDb(object obj, int id)
        {
            string objTable = obj.GetType().Name + "s";
            // start concatenating string for query
            string query = "SELECT * FROM " + objTable + " WHERE " + obj.GetType().GetProperties()[0].Name + " = " + id;
            bool isInDB;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                SQLiteDataReader reader = command.ExecuteReader();
                isInDB = reader.Read();
                reader.Close();
                cnn.Close();
            }

            return isInDB;
        }

        // returns true if the passed in object's name already exists as a row in the database
        public bool isObjectNameInDb(object obj, string name)
        {
            string objTable = obj.GetType().Name + "s";
            // start concatenating string for query
            string query = "SELECT * FROM " + objTable + " WHERE " + obj.GetType().GetProperties()[1].Name + " = '" + name + "'";
            bool isInDB;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                SQLiteDataReader reader = command.ExecuteReader();
                isInDB = reader.Read();
                reader.Close();
                cnn.Close();
            }

            return isInDB;
        }

        // generic method for seeing if an entry exists in a record via a select query with a where clause
        public bool isSelectWhereInDb(string table, string column, string item)
        {
            string query = "SELECT * FROM " + table + " WHERE " + column + " = '" + item + "'";

            bool isInDB;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                SQLiteDataReader reader = command.ExecuteReader();
                isInDB = reader.Read();
                reader.Close();
                cnn.Close();
            }

            return isInDB;
        }

        // gets all records from table
        public List<object> getAllFromTable(object obj)
        {
            List<object> objects = new List<object>();
            string objTable = obj.GetType().Name + "s";
            string query = "SELECT * FROM " + objTable; // start concatenating string for query

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                int numOfColumns = obj.GetType().GetProperties().Length;
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    object obj1 = Activator.CreateInstance(obj.GetType());

                    for (int i = 0; i < numOfColumns; i++)
                    {
                        string prop = obj.GetType().GetProperties()[i].Name;

                        PropertyInfo pi = obj.GetType().GetProperty(prop);

                        // making sure to set property with correct type
                        if (pi.PropertyType == typeof(int))
                        {
                            pi.SetValue(obj1, reader.GetInt32(i));
                        }
                        else if (pi.PropertyType == typeof(DateTime))
                        {
                            pi.SetValue(obj1, DateTime.Parse(reader.GetString(i)));
                        }
                        else if (pi.PropertyType == typeof(TimeSpan))
                        {
                            pi.SetValue(obj1, TimeSpan.Parse(reader.GetString(i)));
                        }
                        else
                        {
                            pi.SetValue(obj1, reader.GetString(i));
                        }
                    }
                    
                    objects.Add(obj1);
                }

                reader.Close();
                cnn.Close();
            }

            return objects;
        }

        // creates the user's itinerary table in the database
        public void createItinerary(User user1)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();

                // string to execute query
                command.CommandText = "CREATE TABLE Itinerary" + user1.userID.ToString() +
                    " (eventID NUMBER, CONSTRAINT itinerary_fk FOREIGN KEY (eventID) REFERENCES Events (eventID))";
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // saves passed in itinerary to the database
        public void saveItineraryToDb(int userID, List<Event> events)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "delete from Itinerary" + userID.ToString(); // start concatenating string for query
                command.ExecuteNonQuery();

                // each iteration executes a query
                foreach (Event i in events)
                {
                    command.CommandText = "Insert into Itinerary" + userID + " (eventID) VALUES ('" + i.eventID + "')";
                    command.ExecuteNonQuery();
                }

                cnn.Close();
            }
        }

        // gets itinerary from the database
        public List<Event> getItineraryFromDb(int userID)
        {
            List<int> eventIDs = new List<int>();
            List<Event> events = new List<Event>();

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "SELECT * FROM Itinerary" + userID.ToString(); // start concatenating string for query
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // only need to save ids
                    eventIDs.Add(reader.GetInt32(0));
                }

                reader.Close();
                cnn.Close();
            }

            // for every id get the corresponding event
            foreach (int i in eventIDs)
            {
                events.Add((Event)getObjectFromDbById(new Event(), i));
            }

            return events; ;
        }

        // deletes event id from user id's itinerary
        public void deleteEventFromItinerary(int userID, int eventID)
        {
            string query = "DELETE FROM Itinerary" + userID + " WHERE EventID = " + eventID;

            Event eventToRemove = (Event)getObjectFromDbById(new Event(), eventID);
            int numStaffAssigned = eventToRemove.staffAssigned - 1;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                command.CommandText = "UPDATE events SET staffAssigned = " + numStaffAssigned + " WHERE eventID = " + eventID;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // deletes passed in event id from all itineraries
        public void deleteEventFromAllItineraries(int id)
        {
            List<int> userIDs = new List<int>();

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "SELECT UserID FROM Users";
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    userIDs.Add(reader.GetInt32(0));
                }

                reader.Close();
                cnn.Close();
            }

            // for every user id select that itinerary and delete the event from it
            foreach (int i in userIDs)
            {
                string query = "DELETE FROM Itinerary" + i + " WHERE EventID = " + id;

                using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
                {
                    cnn.Open();
                    SQLiteCommand command = cnn.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }

        // adds event to staff user's itinerary
        public void assignStaffToEvent(string eventname, string username)
        {
            User staffer = (User)getObjectFromDbByName(new User(), username);
            Event eventToAdd = (Event)getObjectFromDbByName(new Event(), eventname);
            int numStaffAssigned = eventToAdd.staffAssigned + 1;

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "INSERT INTO Itinerary" + staffer.userID + " (eventID) VALUES ('" + eventToAdd.eventID + "')";
                command.ExecuteNonQuery();
                command.CommandText = "UPDATE events SET staffAssigned = " + numStaffAssigned + " WHERE eventID = " + eventToAdd.eventID;
                command.ExecuteNonQuery();
                cnn.Close();
            }
        }

        // gets all users who have scheduled passed in event id
        public List<User> getStaffAssignedToEvent(int id)
        {
            List<int> userIDs = new List<int>();

            using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = cnn.CreateCommand();
                command.CommandText = "SELECT UserID FROM Users";
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    userIDs.Add(reader.GetInt32(0));
                }

                reader.Close();
                cnn.Close();
            }

            List<int> userIdsWhoScheduledEvent = new List<int>();
            List<User> staffWhoScheduledEvent = new List<User>();

            // for every user id select that itinerary and delete the event from it
            foreach (int i in userIDs)
            {
                string query = "SELECT * FROM Itinerary" + i + " WHERE EventID = " + id;

                bool isInDB;

                using (SQLiteConnection cnn = new SQLiteConnection(loadConnectionString()))
                {
                    cnn.Open();
                    SQLiteCommand command = cnn.CreateCommand();
                    command.CommandText = query;
                    SQLiteDataReader reader = command.ExecuteReader();
                    isInDB = reader.Read();
                    reader.Close();
                    cnn.Close();
                }

                if (isInDB)
                {
                    userIdsWhoScheduledEvent.Add(i);
                }
            }

            foreach(int userId in userIdsWhoScheduledEvent)
            {
                User user1 = (User)getObjectFromDbById(new User(), userId);

                foreach(UserType ut in getAllFromTable(new UserType()).Cast<UserType>().ToList())
                {
                    if(ut.userPermissionsLevel == 1)
                    {
                        if(user1.userTypeName == ut.userTypeName)
                        {
                            staffWhoScheduledEvent.Add(user1);
                        }
                    }
                }
            }

            return staffWhoScheduledEvent;
        }
    }
}