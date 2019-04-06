﻿using CapstoneClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapstoneProject
{
    public partial class EventEditorForm : Form
    {
        public EventEditorForm()
        {
            InitializeComponent();
        }

        private void EventEditorForm_Load(object sender, EventArgs e)
        {
            // on load populate info
            populateEventListView(Apex.i.getAllFromTable(new Event()).Cast<Event>().ToList(), eventsListView);

            loadEventTypes();
            loadLocations();
        }

        // method for populating event type list box
        private void loadEventTypes()
        {
            eventTypeListBox.SelectedItems.Clear();
            eventTypeListBox.Items.Clear();

            foreach (EventType type in Apex.i.getAllFromTable(new EventType()).Cast<EventType>().ToList())
            {
                eventTypeListBox.Items.Add(type.eventTypeName);
            }
        }

        // method for populating location list box
        private void loadLocations()
        {
            locationListBox.SelectedItems.Clear();
            locationListBox.Items.Clear();

            foreach (Location loc in Apex.i.getAllFromTable(new Location()).Cast<Location>().ToList())
            {
                locationListBox.Items.Add(loc.locationName);
            }
        }

        // populates the listviews on this form with data
        private void populateEventListView(List<Event> events, ListView lv)
        {
            lv.SelectedItems.Clear();
            lv.Items.Clear();

            foreach (Event i in events)
            {
                ListViewItem lvItem = new ListViewItem(i.startTime.ToString("HH:mm"));
                lvItem.SubItems.Add(i.startTime.ToString("t")
                    + " - " + (i.startTime + i.eventDuration).ToString("t"));
                lvItem.SubItems.Add(i.eventName);
                lvItem.SubItems.Add(i.eventTypeName);
                lvItem.SubItems.Add(i.locationName);
                lv.Items.Add(lvItem);
            }


        }

        // when changing selection the form will populate with current highlighted event
        private void eventsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(eventsListView.SelectedIndices.Count > 0)
            {
                Event currentEvent = (Event)Apex.i.getObjectFromDbByName(new Event(), eventsListView.SelectedItems[0].SubItems[2].Text);

                nameTextBox.Text = currentEvent.eventName;
                startTimePicker1.Value = currentEvent.startTime;
                startTimePicker2.Value = currentEvent.startTime;
                eventDurationTextBox.Text = currentEvent.eventDuration.ToString();
                setupDurationTextBox.Text = currentEvent.setupDuration.ToString();
                breakdownDurationTextBox.Text = currentEvent.breakdownDuration.ToString();
                descriptionRichTextBox.Text = currentEvent.description;
            }
        }
    }
}
