
using Microsoft.Data.SqlClient;

namespace apbd_kolos1;

public class DBService : IDBService
{
    protected const string GET_BOOKING_BY_ID_SQL_QUERRY = "select booking.booking_id as b_booking_id, booking.guest_id as b_guest_id, booking.employee_id as b_employee_id, booking.date as b_date, guest.first_name as g_first_name, guest.last_name as g_last_name, guest.date_of_birth as g_date_of_birth, employee.first_name as e_first_name, employee.last_name as e_last_name, employee.employee_number as e_employee_number, booking_attraction.attraction_id as ba_attraction_id, booking_attraction.amount as ba_amount, attraction.name as a_name, attraction.price as a_price from booking join guest on guest.guest_id = booking.guest_id join employee on employee.employee_id = booking.employee_id join booking_attraction on booking_attraction.booking_id = booking.booking_id join attraction on attraction.attraction_id = booking_attraction.attraction_id;";

    protected const string INSERT_NEW_BOOKING_QUERRY = "insert into booking values (@booking_id, @guest_id, (select employee_id from employee where employee_number = @employee_number), @booking_date);";
    protected string connectionString;

    public DBService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<BookingDTO> GetBookingByIdAsync(int id)
    {
        BookingDTO booking;

        using (SqlConnection connection = new(this.connectionString))
        using (SqlCommand command = new(GET_BOOKING_BY_ID_SQL_QUERRY, connection))
        {
            await connection.OpenAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                GuestDTO guest;
                EmployeeDTO employee;

                await reader.ReadAsync();

                // var bookingIdOridinal = reader.GetOrdinal("b_booking_id");
                // var guestIdOridinal = reader.GetOrdinal("b_guest_id");
                // var employeeIdOridinal = reader.GetOrdinal("b_employee_id");
                var bookingDateOridinal = reader.GetOrdinal("b_date");
                var guestFirstNameOridinal = reader.GetOrdinal("g_first_name");
                var guestLastNameOridinal = reader.GetOrdinal("g_last_name");
                var guestDateOfBirthOridinal = reader.GetOrdinal("g_date_of_birth");
                var employeeFirstNameOridinal = reader.GetOrdinal("e_first_name");
                var employeeLastNameOridinal = reader.GetOrdinal("e_last_name");
                var employeeNumberOridinal = reader.GetOrdinal("e_employee_number");
                // var attractionIdOridinal = reader.GetOrdinal("ba_attraction_id");
                var bookingAttractionAmountOridinal = reader.GetOrdinal("ba_amount");
                var attractionNameOridinal = reader.GetOrdinal("a_name");
                var attractionPriceOridinal = reader.GetOrdinal("a_price");

                guest = new()
                {
                    // GuestId = reader.GetInt32(guestIdOridinal),
                    FirstName = reader.GetString(guestFirstNameOridinal),
                    LastName = reader.GetString(guestLastNameOridinal),
                    DateOfBirth = reader.GetDateTime(guestDateOfBirthOridinal)
                };

                employee = new()
                {
                    // EmployeeId = reader.GetInt32(employeeIdOridinal),
                    FirstName = reader.GetString(employeeFirstNameOridinal),
                    LastName = reader.GetString(employeeLastNameOridinal),
                    EmployeeNumber = reader.GetString(employeeNumberOridinal)
                };

                booking = new()
                {
                    // BookingId = reader.GetInt32(bookingIdOridinal),
                    // GuestId = guest.GuestId,
                    // EmployeeId = employee.EmployeeId,
                    Date = reader.GetDateTime(bookingDateOridinal),
                    Guest = guest,
                    Employee = employee
                };

                do
                {
                    /* AttractionDTO attraction = new()
                    {
                        // AttractionId = reader.GetInt32(attractionIdOridinal),
                        Name = reader.GetString(attractionNameOridinal),
                        Price = reader.GetDecimal(attractionPriceOridinal)
                    }; */

                    booking.BookingAttractions.Add
                    (
                        new()
                        {
                            // BookingId = booking.BookingId,
                            // AttractionId = attraction.AttractionId,
                            Amount = reader.GetInt32(bookingAttractionAmountOridinal),
                            Name = reader.GetString(attractionNameOridinal),
                            Price = reader.GetDecimal(attractionPriceOridinal)
                        }
                    );
                }
                while (await reader.ReadAsync());
            }
        }

        return booking;
    }

    public async Task AddNewBookingAsync(BookingCreateRequestDTO booking)
    {
        string commandStr = INSERT_NEW_BOOKING_QUERRY;
        for (int i = 0; i < booking.Attractions.Length; i++)
        {
            commandStr += $"insert into booking_attraction values (@booking_id{i}, (select attraction_id from attraction where attraction.name = @attraction_name{i}), @amount{i});";
        }

        using (SqlConnection connection = new(this.connectionString))
        using (SqlCommand command = new(GET_BOOKING_BY_ID_SQL_QUERRY, connection))
        {
            await connection.OpenAsync();

            command.Parameters.AddWithValue("@booking_id", booking.BookingId);
            command.Parameters.AddWithValue("@guest_id", booking.GuestId);
            command.Parameters.AddWithValue("@booking_date", booking.Date);

            for (int i = 0; i < booking.Attractions.Length; i++)
            {
                command.Parameters.AddWithValue($"@booking_id{i}", booking.BookingId);
                command.Parameters.AddWithValue($"@booking_id{i}, @attraction_id{i}, @amount{i}", booking.Attractions[i].Amount);
                command.Parameters.AddWithValue($"@attraction_name{i}", booking.Attractions[i].Name);
            }

            command.ExecuteNonQuery();
        }
    }
}
