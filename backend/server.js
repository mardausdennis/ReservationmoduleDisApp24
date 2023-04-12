const express = require('express');
const dotenv = require('dotenv');
const { google } = require('googleapis');
const bodyParser = require('body-parser');
const cors = require('cors');
const calendar = require('./routes/calendar');

const result = dotenv.config();
if (result.error) {
  throw result.error;
}

const app = express();

app.use(cors());
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

const OAuth2 = google.auth.OAuth2;

const oauth2Client = new OAuth2(
  process.env.GOOGLE_CLIENT_ID,
  process.env.GOOGLE_CLIENT_SECRET,
  process.env.GOOGLE_REDIRECT_URI
);

// Diese Funktion generiert die Authentifizierungs-URL für Google-Anmeldung
function getAuthUrl() {
  const authUrl = oauth2Client.generateAuthUrl({
    access_type: 'offline',
    scope: [
      'openid',
      'email',
      'profile',
      'https://www.googleapis.com/auth/calendar',
    ],
    prompt: 'consent',
  });
  return authUrl;
}

// Diese Route leitet den Benutzer zur Google-Anmeldeseite weiter
app.get('/auth', (req, res) => {
  const authUrl = getAuthUrl();
  res.redirect(authUrl);
});

// Diese Route wird aufgerufen, wenn Google den Benutzer nach erfolgreicher Anmeldung zurückleitet
app.get('/auth/callback', async (req, res) => {
  const code = req.query.code;

  try {
    const { tokens } = await oauth2Client.getToken(code);
    oauth2Client.setCredentials(tokens);

    // Zeige das Refresh-Token in der Konsole an (nur zu Testzwecken)
    console.log('Refresh-Token:', tokens.refresh_token);

    // Speichern Sie das Refresh-Token in der Umgebung oder in der Datenbank
    process.env.GOOGLE_REFRESH_TOKEN = tokens.refresh_token;

    // Leiten Sie den Benutzer mit dem Zugriffstoken zurück zur App
    res.redirect(`disapp24://?access_token=${tokens.access_token}`);
  } catch (error) {
    res.status(400).send('Authentifizierungsfehler: ' + error.message);
  }
});

app.set('oauth2Client', oauth2Client);

// Eigene Routen definieren
app.use('/calendar', calendar);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
