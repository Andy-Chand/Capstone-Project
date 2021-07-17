/// Purpose: connectionString.js runs on MainPage.aspx and determines what user is accessing the page
/// and determines whether the user wants to rebuild/build the database.
///
/// Vers        Date        Coder       Comments
/// 1.0         2021-03-21  Allan       Initial
///

var modal = document.getElementById("myModal");
document.getElementById("demo").innerHTML =
    '<p>Do you want to rebuild the database?</p>' +
    // the rebuild function brings up one of two modals based on which button is clicked.
    '<button class="button" type="button" onclick="rebuild(1)">Yes</button>' +
    '<button class="button" type="button" onclick="rebuild(0)">No</button>';

modal.style.display = "block";

/// Purpose: The rebuild function determines whether or not the user wants to build/rebuild the database and also
/// sends the funtion userPlusDB the user's id.
/// Vers        Date        Coder       Comments
/// 1.0         2021-03-21  Allan       Initial
function rebuild(i) {
    if (i == 1) {
        var modal = document.getElementById("myModal");
        document.getElementById("demo").innerHTML =
            '<p>Click your name to add your connection string and build the database.</p>' +
            '<p onclick="userPlusDB(1)">Clay</p>' +
            '<p onclick="userPlusDB(2)">Taylor</p>' +
            '<p onclick="userPlusDB(3)">Jill</p>' +
            '<p onclick="userPlusDB(4)">Andy</p>' +
            '<p onclick="userPlusDB(5)">Allan</p>';

        modal.style.display = "block";
    }
    else if (i == 0) {
        var modal = document.getElementById("myModal");
        document.getElementById("demo").innerHTML =
            '<p>Click your name to add your connection string.</p>' +
            '<p onclick="user(1)">Clay</p>' +
            '<p onclick="user(2)">Taylor</p>' +
            '<p onclick="user(3)">Jill</p>' +
            '<p onclick="user(4)">Andy</p>' +
            '<p onclick="user(5)">Allan</p>';

        modal.style.display = "block";
    }
}

///Purpose: takes variable i and sends it to ConnectionPlusDB then closes the modal.
///
/// Vers        Date        Coder       Comments
/// 1.0         2021-03-21  Allan       Initial
function userPlusDB(i) {
    PageMethods.ConnectionPlusDB(i);
    alert("Welcome!");
    modal.style.display = "none";
}

///Purpose: takes variable i and sends it to Connection then closes the modal.
///
/// Vers        Date        Coder       Comments
/// 1.0         2021-03-21  Allan       Initial
function user(i) {
    PageMethods.Connection(i);
    alert("Welcome!");
    modal.style.display = "none";
}