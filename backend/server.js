const express = require('express');
const dotenv = require('dotenv');
const { google } = require('googleapis');
const bodyParser = require('body-parser');
const cors = require('cors');
const calendar = require('./routes/calendar');

dotenv.config();
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

function getAuthUrl() {
  const authUrl = oauth2Client.generateAuthUrl({
    access_type: 'offline',
    scope: ['https://www.googleapis.com/auth/calendar'],
  });
  return authUrl;
}

app.get('/auth', (req, res) => {
  const authUrl = getAuthUrl();
  res.redirect(authUrl);
});

app.get('/auth/callback', async (req, res) => {
  const code = req.query.code;

  try {
    const { tokens } = await oauth2Client.getToken(code);
    oauth2Client.setCredentials(tokens);

    process.env.REFRESH_TOKEN = tokens.refresh_token;

    res.send('Authentifizierung erfolgreich. Sie können jetzt die API verwenden.');
  } catch (error) {
    res.status(400).send('Authentifizierungsfehler: ' + error.message);
  }
});

app.set('oauth2Client', oauth2Client);

//eigenen Routen definieren
app.use('/calendar', calendar);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
