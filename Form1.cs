
using System.Text.RegularExpressions;
namespace FlightReservation
{
    public partial class Form1 : Form
    {
        
        int dataCheck = 0;

        private List<Reservation> reservations = new List<Reservation>();
        private List<Reservation> waiting = new List<Reservation>();
        public Form1()
        {
            InitializeComponent();
            InitializeSeatButtons();
        }
        int businessClassPrice = 450;
        int economyClassPrice = 200;
        int businessClassSeatAvailable = 12;
        int economyClassSeatAvailable = 40;
        int seatsAvailbl = 52;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (waiting.Count >= 10)
            {
                MessageBox.Show("The Waiting List is full. ", "Waiting List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                waitingBtn.Enabled = false;

            }

            Console.WriteLine($"Reservations Count: {reservations.Count}");

            cancelAllBtn.Enabled = reservations.Any(); // Enable Cancel All Bookings if there is at least one reservation

            cancelBtn.Enabled = seatsAvailbl > 0; // Enable Cancel if seats are booked

            checkBtn.Enabled = seatsAvailbl > 0; // Enable Check if seats are available

            reserveBtn.Enabled = seatsAvailbl > 0 && waiting.Count < 10; // Enable Reserve if seats are available and waiting list is not full

            waitingBtn.Enabled = seatsAvailbl > 0 && waiting.Count < 10; // Enable Waiting if seats are available and waiting list is not full

        }
        private void EnablingCancelAll()
        {
            cancelAllBtn.Enabled = reservations.Any() || waiting.Any(); // Enable Cancel All Bookings if there is at least one reservation
        }
        


        private void InitializeSeatButtons()
        {
            for (char row = 'A'; row <= 'D'; row++)
            {
                for (int col = 1; col <= 13; col++)
                {
                    string seatName = $"{row}{col}";

                    Button seatButton = Controls.Find($"con{seatName}", true).FirstOrDefault() as Button;
                    if (seatButton != null)
                    {
                        seatButton.Click += seatButton_Click;


                    }
                }
            }
        }
        private void UpdateSeatPrices(int businessPrice, int economyPrice, params string[] seatNames)
        {

            foreach (var seatName in seatNames)
            {

                Button seatButton = Controls.Find(seatName, true).FirstOrDefault() as Button;
                if (seatButton != null && !IsSeatAlreadyReserved(seatName))
                {
                    if (!IsSeatAlreadyReserved(seatName))
                    {

                        // Update the text property based on the class of the seat
                        int col = int.Parse(seatName.Substring(4)); // Business class
                        if (col <= 3)
                        {
                            seatButton.Text = $"${businessPrice}"; // Use business class price
                        }
                        else // Economy class
                        {
                            seatButton.Text = $"${economyPrice}"; // Use economy class price
                        }
                    }
                }
            }
        }

        private void seatButton_Click(object sender, EventArgs e)
        {
            Button selectedSeatButton = (Button)sender;
            seatNumberlbl.Text = selectedSeatButton.Name;

            //string seatNumber = selectedSeatButton.Name.Substring(3); // Assuming the button name starts with "con"

            /*if (IsSeatAlreadyReserved(seatNumber))
            {
                selectedSeatButton.BackColor = Color.Red;
            }
            else
            {
                selectedSeatButton.BackColor = Color.LawnGreen;
            }*/
        }

        private void reserveBtn_Click(object sender, EventArgs e)
        {
            string selectedSeat = seatNumberlbl.Text;
            dataCheck = 0;
            string pattern = @"^[A-Za-z']{2,}$";

            Regex regex = new Regex(pattern);
            if (firstName.Text != null && regex.IsMatch(firstName.Text))
            {
                dataCheck++;
            }

            else
            {
                MessageBox.Show("Enter a Valid First Name");
            }
            if (lastName.Text != null && regex.IsMatch(lastName.Text))
            {
                dataCheck++;
            }

            else
            {
                MessageBox.Show("Enter a Valid Last Name");
            }

            string phoneNumberPattern = @"^\d{10}$";

            Regex regex2 = new Regex(phoneNumberPattern);
            if (phoneNo.Text != null && regex2.IsMatch(phoneNo.Text))
            {
                dataCheck++;
            }
            else
            {
                MessageBox.Show("Please enter a valid phone number");
            }

            string emailPattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
            Regex regex3 = new Regex(emailPattern);
            if (emailTb.Text != null && regex3.IsMatch(emailTb.Text))
            {
                dataCheck++;
            }
            else
            {
                MessageBox.Show("Please Enter a Valid Email");
            }

            if (dataCheck != 4)
            {
                MessageBox.Show("Fill out all the details");
            }
            else
            {
                if (IsSeatAlreadyReserved(selectedSeat))
                {
                    MessageBox.Show("This seat is already booked.", "Seat Reservation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (IsSeatInWaitingList(selectedSeat))
                {
                    MessageBox.Show("This seat is already in Waiting List.", "Waiting List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    string fName = firstName.Text;
                    string lName = lastName.Text;
                    string email = emailTb.Text;
                    string phoneNumber = phoneNo.Text;

                    Reservation newreservation = new Reservation
                    {
                        FirstName = fName,
                        LastName = lName,
                        EmailCheck = email,
                        PhoneNumber = phoneNumber,
                        SeatNumber = selectedSeat
                    };


                    reservations.Add(newreservation);
                    UpdateReservationListBox();
                    ClearInputFields();

                    string seatNumber = selectedSeat.Substring(3);
                    Button reservedSeatButton = Controls.Find($"con{seatNumber}", true).FirstOrDefault() as Button;

                    seatsAvailbl--;
                    seatAvailableLbl.Text = seatsAvailbl.ToString();
                    UpdatePercentageTally();
                    EnablingCancelAll();
                    if (reservedSeatButton != null)
                    {
                        reservedSeatButton.BackColor = Color.Red;

                        int col = int.Parse(seatNumber.Substring(1));

                        if (col <= 3) // Business class
                        {
                            businessClassSeatAvailable--;
                            businessClassPrice += 120 / businessClassSeatAvailable; // Adjust the business class price 
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conA1", "conA2", "conA3", "conB1", "conB2", "conB3", "conC1", "conC2", "conC3", "conD1", "conD2", "conD3");
                        }

                        else // Economy class
                        {
                            economyClassSeatAvailable--;
                            economyClassPrice += 60 / economyClassSeatAvailable; // Adjust the economy class price 
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conD4", "conD5", "conD6", "conD7", "conD8", "conD9", "conD10", "conD11", "conD12", "conD13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conA4", "conA5", "conA6", "conA7", "conA8", "conA9", "conA10", "conA11", "conA12", "conA13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conB4", "conB5", "conB6", "conB7", "conB8", "conB9", "conB10", "conB11", "conB12", "conB13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conC4", "conC5", "conC6", "conC7", "conC8", "conC9", "conC10", "conC11", "conC12", "conC13");
                        }

                        
                    }
                }

            }
            
        }


        private bool IsSeatAlreadyReserved(string seat)
        {

            return reservations.Any(r => r.SeatNumber.Equals(seat));
        }

        private bool IsSeatInWaitingList(string seat)
        {

            return waiting.Any(r => r.SeatNumber.Equals(seat));

        }
        private void UpdateReservationListBox()
        {
            reservationList.Items.Clear();
            reservationList.Items.AddRange(reservations.Reverse<Reservation>().ToArray());
        }

        private void UpdateWaitingListBox()
        {
            waitingList.Items.Clear();
            foreach (var entry in waiting.Reverse<Reservation>())
            {
                waitingList.Items.Add(entry.WaitingListDisplay());
            }
        }
        private void ClearInputFields()
        {
            firstName.Text = string.Empty;
            lastName.Text = string.Empty;
            emailTb.Text = string.Empty;
            phoneNo.Text = string.Empty;
            seatNumberlbl.Text = string.Empty;
        }

        private void seatSelector(object sender, EventArgs e)
        {
            Button btnSelectSeat = (Button)sender;
            string seatNumber = btnSelectSeat.Name;
            seatNumberlbl.Text = seatNumber;

        }

        private void waitingBtn_Click(object sender, EventArgs e)
        {

            string selectedSeat = seatNumberlbl.Text;
            dataCheck = 0;
            string pattern = @"^[A-Za-z']{2,}$";

            Regex regex = new Regex(pattern);
            if (firstName.Text != null && regex.IsMatch(firstName.Text))
            {
                dataCheck++;
            }

            else
            {
                MessageBox.Show("Enter a Valid First Name");
            }
            if (lastName.Text != null && regex.IsMatch(lastName.Text))
            {
                dataCheck++;
            }

            else
            {
                MessageBox.Show("Enter a Valid Last Name");
            }

            string phoneNumberPattern = @"^\d{10}$";

            Regex regex2 = new Regex(phoneNumberPattern);
            if (phoneNo.Text != null && regex2.IsMatch(phoneNo.Text))
            {
                dataCheck++;
            }
            else
            {
                MessageBox.Show("Please enter a valid phone number");
            }

            string emailPattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
            Regex regex3 = new Regex(emailPattern);
            if (emailTb.Text != null && regex3.IsMatch(emailTb.Text))
            {
                dataCheck++;
            }
            else
            {
                MessageBox.Show("Please Enter a Valid Email");
            }

            if (dataCheck != 4)
            {
                MessageBox.Show("Fill out all the details");
            }
            else
            {
                if (IsSeatAlreadyReserved(selectedSeat))
                {
                    MessageBox.Show("This seat is already booked.", "Seat Reservation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else if (IsSeatInWaitingList(selectedSeat))
                {
                    MessageBox.Show("This seat is already in Waiting List.", "Waiting List", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string fName = firstName.Text;
                    string lName = lastName.Text;
                    string email = emailTb.Text;
                    string phoneNumber = phoneNo.Text;

                    Reservation newWaiting = new Reservation
                    {
                        FirstName = fName,
                        LastName = lName,
                        EmailCheck = email,
                        PhoneNumber = phoneNumber,
                        SeatNumber = ""
                    };


                    waiting.Add(newWaiting);
                    UpdateWaitingListBox();
                    ClearInputFields();
                    EnablingCancelAll();
                }
            }
        }


        private void cancelBtn_Click(object sender, EventArgs e)
        {
            string selectedSeat = seatNumberTb.Text;
            if (IsSeatAlreadyReserved(selectedSeat))
            {
                Reservation cancelReservation = reservations.FirstOrDefault(r => r.SeatNumber == selectedSeat); //Comparing Seat Number

                if (cancelReservation != null)
                {
                    reservations.Remove(cancelReservation);

                    UpdateReservationListBox();



                    string seatNumber = selectedSeat.Substring(3);
                    Button canceledSeatButton = Controls.Find($"con{seatNumber}", true).FirstOrDefault() as Button;

                    if (canceledSeatButton != null)
                    {

                        int col = int.Parse(selectedSeat.Substring(4));

                        if (col <= 3) // Business class
                        {
                            businessClassSeatAvailable++;
                            canceledSeatButton.BackColor = Color.Yellow;
                            businessClassPrice -= 120 / businessClassSeatAvailable; // Adjust the business class price 
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conA1", "conA2", "conA3", "conB1", "conB2", "conB3", "conC1", "conC2", "conC3", "conD1", "conD2", "conD3");
                        }
                        else // Economy class
                        {
                            economyClassSeatAvailable++;
                            canceledSeatButton.BackColor = Color.LawnGreen;
                            economyClassPrice -= 60 / economyClassSeatAvailable; // Adjust the economy class price 
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conD4", "conD5", "conD6", "conD7", "conD8", "conD9", "conD10", "conD11", "conD12", "conD13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conA4", "conA5", "conA6", "conA7", "conA8", "conA9", "conA10", "conA11", "conA12", "conA13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conB4", "conB5", "conB6", "conB7", "conB8", "conB9", "conB10", "conB11", "conB12", "conB13");
                            UpdateSeatPrices(businessClassPrice, economyClassPrice, "conC4", "conC5", "conC6", "conC7", "conC8", "conC9", "conC10", "conC11", "conC12", "conC13");
                        }

                    }
                    seatsAvailbl++;
                    seatAvailableLbl.Text = seatsAvailbl.ToString();
                    UpdatePercentageTally();
                }
            }
            else
            {
                MessageBox.Show("The selected seat is not reserved.", "Cancellation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }
        
        private void checkBtn_Click(object sender, EventArgs e)
        {
            //Check Status Funtion
            string selectedSeat = seatNumberTb2.Text;
            if (IsSeatAlreadyReserved(selectedSeat))
            {
                msgReserved.Visible = true;
                msgWaiting.Visible = false;
                msgInvalid.Visible = false;

            }
            else if (IsSeatInWaitingList(selectedSeat))
            {

                msgReserved.Visible = false;
                msgWaiting.Visible = true;
                msgInvalid.Visible = false;
            }
            else
            {
                msgReserved.Visible = false;
                msgWaiting.Visible = false;
                msgInvalid.Visible = true;
            }
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            seatNumberTb.Text = "";
            seatNumberTb2.Text = "";
            msgReserved.Visible = false;
            msgWaiting.Visible = false;
            msgInvalid.Visible = false;
        }

        private void cancellAllBtn_Click(object sender, EventArgs e)
        {
            reservations.Clear();
            waiting.Clear();

            UpdateReservationListBox();
            UpdateWaitingListBox();

            businessClassPrice = 450;
            economyClassPrice = 200;

            ResetSeatColors();

            seatsAvailbl = 52;
            seatAvailableLbl.Text = seatsAvailbl.ToString();

            UpdatePercentageTally();
        }
        private void ResetSeatColors()
        {
            // Iterate through all seats and set their colors to default
            for (char row = 'A'; row <= 'D'; row++)
            {
                for (int col = 1; col <= 13; col++)
                {
                    string seatName = $"{row}{col}";

                    Button seatButton = Controls.Find($"con{seatName}", true).FirstOrDefault() as Button;
                    if (seatButton != null)
                    {
                        int seatCol = int.Parse(seatName.Substring(1));

                        if (seatCol <= 3) // Business class
                        {
                            seatButton.BackColor = Color.Yellow;
                            seatButton.Text = "$450";
                        }
                        else // Economy class
                        {
                            seatButton.BackColor = Color.LawnGreen;
                            seatButton.Text = "$200";
                        }

                        

                    }
                }
            }
        }
        private void UpdatePercentageTally()
        {
            int totalSeats = 52;
            int bookedSeats = totalSeats - seatsAvailbl;

            double percentageBooked = (double)seatsAvailbl/ totalSeats * 100;

            // Display the percentage in the label
            percentSeat.Text = $"{percentageBooked:F2}%";

        }

       
    }
     public class Reservation // Reservation Class for Setting up the User Information
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailCheck { get; set; }
            public string PhoneNumber { get; set; }
            public string SeatNumber { get; set; }

            public override string ToString()
            {
                return $"{SeatNumber}- {FirstName}: {LastName}: ({EmailCheck}, {PhoneNumber})";
            }

            // Display format for waiting list entries (without SeatNumber)
            public string WaitingListDisplay()
            {
                return $"{FirstName} {LastName} ({EmailCheck}, {PhoneNumber})";
            }

        }
}