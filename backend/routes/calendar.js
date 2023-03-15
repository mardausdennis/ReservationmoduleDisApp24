const express = require('express');
const router = express.Router();

//Kalender-Endpunkte definieren

// Route zum Hinzufügen eines Termins
router.post('/event', async (req, res) => {
    const oauth2Client = req.app.get('oauth2Client');
    const calendar = google.calendar({ version: 'v3', auth: oauth2Client });
  
    const { summary, description, start, end } = req.body;
  
    try {
      const event = {
        summary,
        description,
        start: {
          dateTime: start,
          timeZone: 'Europe/Vienna',
        },
        end: {
          dateTime: end,
          timeZone: 'Europe/Vienna',
        },
      };
  
      const createdEvent = await calendar.events.insert({
        calendarId: process.env.GOOGLE_CALENDAR_ID,
        requestBody: event,
      });
  
      res.json(createdEvent.data);
    } catch (error) {
      res.status(400).send('Fehler beim Hinzufügen des Termins: ' + error.message);
    }
  });

module.exports = router;
