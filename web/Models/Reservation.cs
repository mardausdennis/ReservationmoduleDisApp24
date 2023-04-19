namespace web.Models
{
    public class Reservation
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Date { get; set; }
        public string TimeSlot { get; set; }
        public string Resource { get; set; }

        public string Name
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string Time
        {
            get
            {
                return TimeSlot;
            }
        }
    }


}
