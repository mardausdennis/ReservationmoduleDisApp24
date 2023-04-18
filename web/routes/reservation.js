var express = require('express');
var router = express.Router();
var firebase = require('../firebaseConfig');

// Endpoint to create a new reservation
router.post('/', function (req, res) {
  // Extract reservation data from the request body
  const reservationData = req.body;

  // Save the reservation data to Firebase Realtime Database
  firebase.database().ref('reservations').push(reservationData).then((snapshot) => {
    res.json({ success: true, reservationId: snapshot.key });
  }).catch((error) => {
    res.status(500).json({ success: false, error: error.message });
  });
});

module.exports = router;
